using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class TerrainLandscapeEditor
{
    private readonly TerrainGenerationData generationData;
    private readonly TerrainBrushData brushData;

    private bool isDispatched;

    // Compute shader data
    private Vector3[] vertices;
    private ComputeBuffer vertexBuffer;

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

    public Texture2D GenerateHeightmapTexture()
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

        return heightmapTexture;
    }

    public void SculptTerrain(Texture2D heightMapTexture, float x, float z, TerrainOperationType operationType)
    {
        float digDirection = 1;

        if (operationType == TerrainOperationType.DIG)
        {
            digDirection = -1;
        }

        float brushSize = brushData.BrushSize;
        float brushStrength = brushData.BrushStrength;
        int heightMapSize = generationData.HeightMapSize;
        int terrainSize = generationData.TerrainSize;
        for (int hX = 0; hX < heightMapSize; hX++)
        {
            float xWorld = (float)hX / heightMapSize * terrainSize;
            for (int hY = 0; hY < heightMapSize; hY++)
            {
                float yWorld = (float)hY / heightMapSize * terrainSize;

                var dist = Vector2.Distance(new Vector2(x, z), new Vector2(xWorld, yWorld));

                if (dist < brushSize)
                {
                    var prevColor = heightMapTexture.GetPixel(hX, hY);
                    float value = prevColor.grayscale;

                    float falloff = 1f - dist / brushSize;
                    float strength = brushStrength * falloff;

                    value += strength * Time.deltaTime * digDirection;
                    value = Mathf.Clamp(value, 0f, 1f);

                    prevColor.r = value;
                    prevColor.g = value;
                    prevColor.b = value;

                    heightMapTexture.SetPixel(hX, hY, prevColor);
                }
            }
        }

        heightMapTexture.Apply();

        // Request(x, z, digDirection);
    }

    public void TryGetResult(Mesh terrainMesh)
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

        var nativeArray = request.GetData<Vector3>();
        terrainMesh.MarkDynamic();

        vertices = nativeArray.ToArray();

        terrainMesh.SetVertices(nativeArray);
        terrainMesh.RecalculateNormals();
    }

    private void InitComputeShader()
    {
        computeShader = (ComputeShader)Resources.Load("TerrainSculptingComputeShader");
        kernelId = computeShader.FindKernel("SculptTerrain");

        vertexBuffer = new ComputeBuffer(vertices.Length, sizeof(float) * 3);
        vertexBuffer.SetData(vertices);
    }

    private void Request(float x, float z, float digDirection)
    {
        if (isDispatched)
        {
            return;
        }

        isDispatched = true;

        SetDataForShader(x, z, digDirection);

        computeShader.Dispatch(kernelId, Mathf.CeilToInt(vertices.Length / 64), 1, 1);

        request = AsyncGPUReadback.Request(vertexBuffer);
    }

    private void SetDataForShader(float x, float z, float digDirection)
    {
        computeShader.SetBuffer(kernelId, "vertices", vertexBuffer);
        computeShader.SetFloat("brushRadius", brushData.BrushSize);
        computeShader.SetFloat("brushStrength", brushData.BrushStrength * digDirection);
        computeShader.SetVector("brushPosition", new Vector3(x, 0, z));
    }
}
