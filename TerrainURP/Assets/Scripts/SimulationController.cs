using UnityEngine;

public class SimulationController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private MeshFilter terrainMeshFilter;
    [SerializeField] private HandleInputMono handleInput;

    [Header("Data")]
    [SerializeField] private TerrainGenerationData generationData;
    [SerializeField] private TerrainBrushData brushData;

    private TerrainLandscapeEditor landscapeEditor;

    public TerrainGenerationData GenerationData { get => generationData; }


    void Start()
    {
        landscapeEditor = new TerrainLandscapeEditor(generationData, brushData);
        landscapeEditor.SetRandomHeightForVertices(terrainMeshFilter.mesh);

        terrainMeshFilter.gameObject.SetActive(true);
    }

    void Update()
    {
        if (!handleInput.IsHoldLeftMouse)
        {
            return;
        }

        var operationType = TerrainOperationType.DIG;

        if (handleInput.IsHoldLeftShift)
        {
            operationType = TerrainOperationType.RAISE;
        }

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            int terrainSize = generationData.TerrainSize;

            int x = (int)(hit.point.x / terrainSize * 2048);
            int z = (int)(hit.point.z / terrainSize * 2048);

            if (x >= 0 && x < 2048 && z >= 0 && z < 2048)
            {
                landscapeEditor.SculptTerrain(terrainMeshFilter.mesh, x, z, operationType);
            }
        }
    }
}
