using UnityEngine;

// TODO: add interface (call this implementation Gpu or ComputeShader)
public class TerrainLandscapeEditor
{
    private readonly TerrainGenerationData generationData;
    private readonly TerrainBrushData brushData;

    // Compute shader data
    private Vector3[] vertices;
    private ComputeBuffer vertexBuffer;
    private float[,] heights;
    private ComputeBuffer heightBuffer;

    private ComputeShader computeShader;
    private int kernelId;


    public TerrainLandscapeEditor(TerrainGenerationData generationData, TerrainBrushData brushData)
    {
        this.generationData = generationData;
        this.brushData = brushData;

        // Set random noise offset to have different terrain landscapes
        this.generationData.Scale = Random.Range(5f, 20f);
        this.generationData.NoiseOffset = Random.Range(0f, 1f) * this.generationData.Scale;
    }

    public void SetRandomHeightForVertices(Mesh terrainMesh)
    {
        terrainMesh.MarkDynamic();

        int heightMapSize = generationData.HeightMapSize;
        float scale = generationData.Scale;
        float noiseOffset = generationData.NoiseOffset;
        float heightMultiplier = generationData.HeightMultiplier;

        Vector3[] vertices = terrainMesh.vertices;

        for (int x = 0; x <= heightMapSize; x++)
        {
            for (int z = 0; z <= heightMapSize; z++)
            {
                float y = PerlinNoiseUtilities.GetHeightForVertex(x, z, heightMapSize, scale, noiseOffset, heightMultiplier);
                vertices[x * (heightMapSize + 1) + z].y = y;

            }
        }

        terrainMesh.vertices = vertices;
        terrainMesh.RecalculateNormals();

        // TMP
        int terrainSize = generationData.TerrainSize;
        heights = new float[terrainSize, terrainSize];
        for (int i = 0; i < vertices.Length; i++)
        {
            float xCoord = ((vertices[i].x + terrainSize / 2) / terrainSize) * 2048;
            float zCoord = ((vertices[i].z + terrainSize / 2) / terrainSize) * 2048;
            heights[(int)xCoord, (int)zCoord] = vertices[i].y;
        }

        InitComputeShader();
    }

    public void SculptTerrain(Mesh terrainMesh, int x, int z, TerrainOperationType operationType)
    {
        float digDirection = 1;

        if (operationType == TerrainOperationType.DIG)
        {
            digDirection = -1;
        }

        DispatchComputeShader(terrainMesh, x, z, digDirection);
    }

    private void InitComputeShader()
    {
        computeShader = (ComputeShader)Resources.Load("TerrainSculptingComputeShader");
        kernelId = computeShader.FindKernel("SculptTerrain");

        vertexBuffer = new ComputeBuffer(vertices.Length, sizeof(float) * 3);
        vertexBuffer.SetData(vertices);

        heightBuffer = new ComputeBuffer(heights.Length, sizeof(float));
        heightBuffer.SetData(heights);
    }

    private void DispatchComputeShader(Mesh terrainMesh, int x, int z, float digDirection)
    {
        float brushSize = brushData.BrushSize;
        float brushStrength = brushData.BrushStrength;
        int terrainSize = generationData.TerrainSize;

        computeShader.SetFloat("brushSize", brushSize);
        computeShader.SetFloat("brushStrength", brushStrength);
        computeShader.SetInt("terrainWidth", terrainSize);
        computeShader.SetInt("terrainLength", terrainSize);
        computeShader.SetInt("x", x);
        computeShader.SetInt("z", z);
        computeShader.SetFloat("direction", digDirection);
        computeShader.SetBuffer(kernelId, "vertices", vertexBuffer);
        computeShader.SetBuffer(kernelId, "heights", heightBuffer);

        int threadsPerGroup = 8;
        int numGroupsX = Mathf.CeilToInt(terrainSize / (float)threadsPerGroup);
        int numGroupsY = Mathf.CeilToInt(terrainSize / (float)threadsPerGroup);
        computeShader.Dispatch(kernelId, numGroupsX, numGroupsY, 1);

        vertexBuffer.GetData(vertices);
        heightBuffer.GetData(heights);

        terrainMesh.vertices = vertices;
        terrainMesh.RecalculateNormals();
    }
}
