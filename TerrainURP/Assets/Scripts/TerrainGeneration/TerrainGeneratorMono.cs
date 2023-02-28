using UnityEngine;

/// <summary>
/// Helper class for mesh generation
/// </summary>
public class TerrainGeneratorMono : MonoBehaviour
{
    [SerializeField] private Material terrainMaterial;
    [SerializeField] private SimulationController simulationController;


    [ContextMenu("TestGenerator")]
    private void TestGenerator()
    {
        if (simulationController == null)
        {
            Debug.LogError("Attach simulation controller");
            return;
        }

        AddNewTerrainObject();
    }

    private void AddNewTerrainObject()
    {
        GameObject gameObject = new GameObject("Terrain");
        gameObject.transform.parent = transform;

        var filter = gameObject.AddComponent<MeshFilter>();
        var renderer = gameObject.AddComponent<MeshRenderer>();
        renderer.material = terrainMaterial;

        gameObject.AddComponent<BoxCollider>();

        GenerateTerrainMesh(filter);

        simulationController.MeshRenderer = renderer;
    }

    private void GenerateTerrainMesh(MeshFilter filter)
    {
        var generationData = simulationController.GenerationData;
        var meshGenerator = new MeshGenerator(generationData.TerrainSize);

        filter.mesh = meshGenerator.GenerateMesh();
    }
}
