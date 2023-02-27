using UnityEngine;

public class TerrainGeneratorMono : MonoBehaviour
{
    [SerializeField]
    private int terrainSize = 256;

    [SerializeField]
    private int heightMapSize = 2048;

    [SerializeField]
    private Material terrainMaterial;


    void Start()
    {
        AddNewTerrainObject();
    }

    private void AddNewTerrainObject()
    {
        GameObject gameObject = new GameObject("Terrain");
        gameObject.transform.parent = transform;

        var filter = gameObject.AddComponent<MeshFilter>();
        var renderer = gameObject.AddComponent<MeshRenderer>();
        renderer.material = terrainMaterial;


    }

    private void GenerateTerrainMesh(MeshFilter filter)
    {
        var meshGenerator = new MeshGenerator(heightMapSize, heightMapSize);

        filter.mesh = meshGenerator.GenerateMesh();

        // Generate the heightmap texture using Perlin noise
        // var heightmapTexture = GenerateHeightmapTexture();

        // Apply the heightmap texture to the mesh
        // terrainMaterial.SetTexture("_Heightmap", heightmapTexture);
    }

    private void GenerateTerrain()
    {
        // GenerateTerrainMesh();
    }

    private Texture2D GenerateHeightmapTexture()
    {
        return null;
    }
}
