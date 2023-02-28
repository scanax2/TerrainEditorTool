
using UnityEngine;

[System.Serializable]
public class TerrainBrushData
{
    [SerializeField] private float brushSize;
    [Range(0f, 1f)]
    [SerializeField] private float brushStrength;


    public float BrushSize { get => brushSize; }
    public float BrushStrength { get => brushStrength; }
}
