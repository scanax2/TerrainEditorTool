using UnityEngine;
using UnityEngine.Rendering;

// TODO: add interface (call this implementation Gpu or ComputeShader)
public class TerrainLandscapeEditor
{
    private readonly TerrainGenerationData generationData;
    private readonly TerrainBrushData brushData;

    private bool isDispatched;

    // Compute shader data
    private Vector3[] vertices;
    private ComputeBuffer vertexBuffer;
    private float[,] heights;

    private ComputeShader computeShader;
    private int kernelId;
    private AsyncGPUReadbackRequest request;
    private float dispatched;


    public TerrainLandscapeEditor(TerrainGenerationData generationData, TerrainBrushData brushData)
    {
        this.generationData = generationData;
        this.brushData = brushData;

        // Set random noise offset to have different terrain landscapes
        this.generationData.Scale = UnityEngine.Random.Range(10f, 20f);
        this.generationData.NoiseOffset = UnityEngine.Random.Range(0.75f, 1.5f) * this.generationData.Scale;
    }

    public void SetRandomHeightForVertices(Mesh terrainMesh)
    {
        int heightMapSize = generationData.HeightMapSize;
        float scale = generationData.Scale;
        float noiseOffset = generationData.NoiseOffset;
        float heightMultiplier = generationData.HeightMultiplier;

        vertices = terrainMesh.vertices;
        heights = new float[heightMapSize + 1, heightMapSize + 1];

        for (int x = 0; x <= heightMapSize; x++)
        {
            for (int z = 0; z <= heightMapSize; z++)
            {
                float y = PerlinNoiseUtilities.GetHeightForVertex(x, z, heightMapSize, scale, noiseOffset, heightMultiplier);
                vertices[x * (heightMapSize + 1) + z].y = y;
                heights[x, z] = y;
            }
        }

        terrainMesh.vertices = vertices;
        terrainMesh.RecalculateNormals();

        InitComputeShader();
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

        Debug.Log($"Took time request: {Time.realtimeSinceStartup - dispatched}");

        var nativeArray = request.GetData<Vector3>();
        terrainMesh.MarkDynamic();

        Debug.Log($"Took time request (readed data): {Time.realtimeSinceStartup - dispatched}");

        vertices = nativeArray.ToArray();

        Debug.Log($"Took time request (copied values): {Time.realtimeSinceStartup - dispatched}");

        terrainMesh.SetVertices(nativeArray);
        //terrainMesh.SetVertexBufferData(nativeArray, 0, 0, nativeArray.Length);
        //terrainMesh.RecalculateNormals();

        Debug.Log($"Took time (whole): {Time.realtimeSinceStartup - dispatched}");
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

        Debug.Log($"Prepare to dispatch");

        SetDataForShader(x, z, digDirection);

        Debug.Log($"Data set");

        computeShader.Dispatch(kernelId, Mathf.CeilToInt(vertices.Length / 1024), 1, 1);

        Debug.Log($"Dispatched !");

        request = AsyncGPUReadback.Request(vertexBuffer);

        Debug.Log($"Request sended");

        dispatched = Time.realtimeSinceStartup;
    }

    private void SetDataForShader(float x, float z, float digDirection)
    {
        computeShader.SetBuffer(kernelId, "vertices", vertexBuffer);
        computeShader.SetFloat("brushRadius", brushData.BrushSize);
        computeShader.SetFloat("brushStrength", brushData.BrushStrength * digDirection);
        computeShader.SetVector("brushPosition", new Vector3(x, 0, z));
    }
}
