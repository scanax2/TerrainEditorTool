using UnityEngine;

public class MeshGenerator
{
    private readonly int terrainSize;


    public MeshGenerator(int terrainSize)
    {
        this.terrainSize = terrainSize;
    }

    public Mesh GenerateMesh()
    {
        Mesh terrainMesh = new Mesh();
        terrainMesh.name = "TerrainMesh";
        terrainMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        Vector2[] uvs;
        terrainMesh.vertices = GenerateVertices(out uvs);
        terrainMesh.uv = uvs;
        terrainMesh.triangles = GenerateTriangles();
        terrainMesh.RecalculateBounds();
        terrainMesh.RecalculateNormals();

        return terrainMesh;
    }

    private Vector3[] GenerateVertices(out Vector2[] uvs)
    {
        Vector3[] vertices = new Vector3[(terrainSize + 1) * (terrainSize + 1)];
        uvs = new Vector2[vertices.Length];

        for (int x = 0; x <= terrainSize; x++)
        {
            for (int z = 0; z <= terrainSize; z++)
            {                
                vertices[x * (terrainSize + 1) + z] = new Vector3(x, 0, z);
                uvs[x * (terrainSize + 1) + z] = new Vector2((float)x / terrainSize, (float)z / terrainSize);
            }
        }

        return vertices;
    }

    private int[] GenerateTriangles()
    {
        int[] triangles = new int[terrainSize * terrainSize * 6];

        for (int x = 0; x < terrainSize; x++)
        {
            for (int z = 0; z < terrainSize; z++)
            {
                int vertexIndex = x * (terrainSize + 1) + z;
                int triangleIndex = (x * terrainSize + z) * 6;

                triangles[triangleIndex] = vertexIndex;
                triangles[triangleIndex + 1] = vertexIndex + 1;
                triangles[triangleIndex + 2] = vertexIndex + terrainSize + 1;
                triangles[triangleIndex + 3] = vertexIndex + 1;
                triangles[triangleIndex + 4] = vertexIndex + terrainSize + 2;
                triangles[triangleIndex + 5] = vertexIndex + terrainSize + 1;
            }
        }

        return triangles;
    }
}
