using UnityEngine;

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

        GenerateTerrainMesh(filter);
    }

    private void GenerateTerrainMesh(MeshFilter filter)
    {
        var generationData = simulationController.GenerationData;
        var meshGenerator = new MeshGenerator(generationData);

        filter.mesh = meshGenerator.GenerateMesh();
    }
}
