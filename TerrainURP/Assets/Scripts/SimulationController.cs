using UnityEngine;

public class SimulationController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private MeshFilter terrainMeshFilter;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private HandleInputMono handleInput;

    [Header("Data")]
    [SerializeField] private TerrainGenerationData generationData;
    [SerializeField] private TerrainBrushData brushData;

    private RenderTexture heightMapTexture;
    private TerrainLandscapeEditor landscapeEditor;

    public TerrainGenerationData GenerationData { get => generationData; }


    void Start()
    {
        landscapeEditor = new TerrainLandscapeEditor(generationData, brushData);
        heightMapTexture = landscapeEditor.GenerateHeightmapTexture();
        meshRenderer.material.SetTexture("_HeightMapTexture", heightMapTexture);
        meshRenderer.material.SetFloat("_Height", generationData.MaxHeight);

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
        landscapeEditor.TryGetResult(heightMapTexture);
    }
}
