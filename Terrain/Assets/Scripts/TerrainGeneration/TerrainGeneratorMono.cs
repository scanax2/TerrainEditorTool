using UnityEngine;

public class TerrainGeneratorMono : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] private int terrainSize = 256;
    [SerializeField] private int heightMapSize = 2048;

    [Header("Secondary")]
    [SerializeField] private float scale = 10f;
    [SerializeField] private float noiseOffset = 75f;
    [SerializeField] private float heightMultiplier = 150f;

    [SerializeField] private Material terrainMaterial;


    void Start()
    {
        // In runtime generation, set noise offset random
        noiseOffset = Random.Range(0f, 1f) * scale;
        AddNewTerrainObject();
    }

    [ContextMenu("TestGenerator")]
    private void TestGenerator()
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

        GenerateTerrainMesh(filter);
    }

    private void GenerateTerrainMesh(MeshFilter filter)
    {
        var meshGenerator = new MeshGenerator(
            heightMapSize, heightMapSize, scale, noiseOffset, heightMultiplier);

        filter.mesh = meshGenerator.GenerateMesh();

        // Generate the heightmap texture using Perlin noise
        var heightmapTexture = GenerateHeightmapTexture();

        // Apply the heightmap texture to the mesh
        terrainMaterial.SetTexture("_Heightmap", heightmapTexture);
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
