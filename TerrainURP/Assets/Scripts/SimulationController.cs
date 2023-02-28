using UnityEngine;
using UnityEngine.UI;

public class SimulationController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private HandleInputMono handleInput;
    [SerializeField] private ComputeShader computeShader;

    [Header("UI")]
    [SerializeField] private Slider brushSizeSlider;
    [SerializeField] private Slider brushStrengthSlider;
    [SerializeField] private Text brushSizeValueText;
    [SerializeField] private Text brushStrengthValueText;

    [Header("Data")]
    [SerializeField] private TerrainGenerationData generationData;
    [SerializeField] private TerrainBrushData brushData;

    private ITerrainLandscapeEditor landscapeEditor;

    public TerrainGenerationData GenerationData { get => generationData; }
    public MeshRenderer MeshRenderer { set => meshRenderer = value; }


    void Start()
    {
        brushSizeSlider.value = brushData.BrushSize;
        brushStrengthSlider.value = brushData.BrushStrength;

        landscapeEditor = new TerrainLandscapeEditor(computeShader, generationData, brushData);
        var heightMapTexture = landscapeEditor.GenerateHeightmapTexture();

        meshRenderer.material.SetTexture("_HeightMapTexture", heightMapTexture);
        meshRenderer.material.SetFloat("_Height", generationData.MaxHeight);

        meshRenderer.gameObject.SetActive(true);
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
        landscapeEditor.UpdateRequest();
    }

    public void OnBrushSizeValueChanged()
    {
        brushData.BrushSize = brushSizeSlider.value;

        brushSizeValueText.text = brushSizeSlider.value.ToString();
    }

    public void OnBrushStrengthValueChanged()
    {
        brushData.BrushStrength = brushStrengthSlider.value;

        brushStrengthValueText.text = brushStrengthSlider.value.ToString();
    }
}
