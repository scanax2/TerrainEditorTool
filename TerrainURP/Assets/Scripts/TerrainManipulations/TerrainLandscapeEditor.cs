using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class TerrainLandscapeEditor
{
    private readonly TerrainGenerationData generationData;
    private readonly TerrainBrushData brushData;

    private bool isDispatched;

    // Render texture to hold the updated texture
    RenderTexture renderTexture;
    private ComputeShader computeShader;
    private int kernelId;
    private AsyncGPUReadbackRequest request;


    public TerrainLandscapeEditor(TerrainGenerationData generationData, TerrainBrushData brushData)
    {
        this.generationData = generationData;
        this.brushData = brushData;

        // Set random noise offset to have different terrain landscapes
        this.generationData.Scale = Random.Range(5f, 10f);
        this.generationData.NoiseOffset = Random.Range(0f, 1f);
    }

    public RenderTexture GenerateHeightmapTexture()
    {
        int heightMapSize = generationData.HeightMapSize;
        float scale = generationData.Scale;
        float noiseOffset = generationData.NoiseOffset;

        Texture2D heightmapTexture = new Texture2D(heightMapSize, heightMapSize);

        for (int x = 0; x < heightMapSize; x++)
        {
            for (int y = 0; y < heightMapSize; y++)
            {
                float value = PerlinNoiseUtilities.GetHeightForVertex(heightMapSize, x, y, scale, noiseOffset);
                Color color = new Color(value, value, value);
                heightmapTexture.SetPixel(x, y, color);
            }
        }

        heightmapTexture.Apply();

        renderTexture = new RenderTexture(heightMapSize, heightMapSize, 0);
        renderTexture.wrapMode = TextureWrapMode.Clamp;
        renderTexture.enableRandomWrite = true;

        Graphics.Blit(heightmapTexture, renderTexture);

        InitComputeShader();

        return renderTexture;
    }

    public void SculptTerrain(float x, float z, TerrainOperationType operationType)
    {
        float digDirection = 1;

        if (operationType == TerrainOperationType.DIG)
        {
            digDirection = -1;
        }

        Request(x, z, digDirection);
    }

    public void TryGetResult(RenderTexture texture)
    {
        if (!isDispatched || !request.done)
        {
            return;
        }

        isDispatched = false;

        if (request.hasError)
        {
            return;
        }
    }

    private void InitComputeShader()
    {
        computeShader = (ComputeShader)Resources.Load("TerrainSculptingComputeShader");
        kernelId = computeShader.FindKernel("SculptTerrain");
    }

    private void Request(float x, float z, float digDirection)
    {
        if (isDispatched)
        {
            return;
        }

        isDispatched = true;

        SetDataForShader(x, z, digDirection);

        int heightMapSize = generationData.HeightMapSize;
        computeShader.Dispatch(kernelId, Mathf.CeilToInt(heightMapSize / 8), Mathf.CeilToInt(heightMapSize / 8), 1);
        request = AsyncGPUReadback.Request(renderTexture);
    }

    private void SetDataForShader(float x, float z, float digDirection)
    {
        computeShader.SetTexture(0, "heightMapTexture", renderTexture);
        computeShader.SetFloat("terrainSize", generationData.TerrainSize);
        computeShader.SetVector("heightMapSize", new Vector2(generationData.HeightMapSize, generationData.HeightMapSize));
        computeShader.SetVector("brushPosition", new Vector2(x, z));
        computeShader.SetFloat("brushRadius", brushData.BrushSize);
        computeShader.SetFloat("brushStrength", brushData.BrushStrength * digDirection);
    }
}
