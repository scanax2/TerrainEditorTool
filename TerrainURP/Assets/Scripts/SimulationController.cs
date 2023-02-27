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
            landscapeEditor.SculptTerrain(hit.point.x, hit.point.z, operationType);
        }
    }

    private void LateUpdate()
    {
        landscapeEditor.TryGetResult(terrainMeshFilter.mesh);
    }
}
