using UnityEngine;

[System.Serializable]
public class TerrainBrushData
{
    [SerializeField] private float brushSize = 45f;
    [Range(0f, 1f)]
    [SerializeField] private float brushStrength = 0.009f;


    public float BrushSize { get => brushSize; set => brushSize = value; }
    public float BrushStrength { get => brushStrength; set => brushStrength = value; }
}
