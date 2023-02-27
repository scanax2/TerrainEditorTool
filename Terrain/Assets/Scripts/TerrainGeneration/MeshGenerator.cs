using UnityEngine;

public class MeshGenerator
{
    private readonly int heightMapSize;
    private readonly int terrainSize;

    private readonly float scale;
    private readonly float noiseOffset;
    private readonly float heightMultiplier;


    public MeshGenerator(int heightMapSize, int terrainSize, 
        float scale, float noiseOffset, float heightMultiplier)
    {
        this.heightMapSize = heightMapSize;
        this.terrainSize = terrainSize;

        this.scale = scale;
        this.noiseOffset = noiseOffset;
        this.heightMultiplier = heightMultiplier;
    }

    public Mesh GenerateMesh()
    {
        Mesh terrainMesh = new Mesh();
        terrainMesh.name = "TerrainMesh";
        terrainMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        terrainMesh.vertices = GenerateVertices();
        terrainMesh.triangles = GenerateTriangles();
        terrainMesh.RecalculateBounds();
        terrainMesh.RecalculateNormals();

        return terrainMesh;
    }

    private Vector3[] GenerateVertices()
    {
        Vector3[] vertices = new Vector3[(heightMapSize + 1) * (heightMapSize + 1)];

        for (int x = 0; x <= heightMapSize; x++)
        {
            for (int z = 0; z <= heightMapSize; z++)
            {
                float y = Mathf.PerlinNoise((float)x / heightMapSize * scale + noiseOffset, (float)z / heightMapSize * scale + noiseOffset) * heightMultiplier;
                vertices[x * (heightMapSize + 1) + z] = new Vector3(x / (float)heightMapSize * terrainSize, y, z / (float)heightMapSize * terrainSize);
            }
        }

        return vertices;
    } 

    private int[] GenerateTriangles()
    {
        int[] triangles = new int[heightMapSize * heightMapSize * 6];

        for (int x = 0; x < heightMapSize; x++)
        {
            for (int z = 0; z < heightMapSize; z++)
            {
                int vertexIndex = x * (heightMapSize + 1) + z;
                int triangleIndex = (x * heightMapSize + z) * 6;

                triangles[triangleIndex] = vertexIndex;
                triangles[triangleIndex + 1] = vertexIndex + 1;
                triangles[triangleIndex + 2] = vertexIndex + heightMapSize + 1;
                triangles[triangleIndex + 3] = vertexIndex + 1;
                triangles[triangleIndex + 4] = vertexIndex + heightMapSize + 2;
                triangles[triangleIndex + 5] = vertexIndex + heightMapSize + 1;
            }
        }

        return triangles;
    }
}
