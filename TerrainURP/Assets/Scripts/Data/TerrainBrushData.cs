
using UnityEngine;

[System.Serializable]
public class TerrainBrushData
{
    [SerializeField] private float brushSize;
    [SerializeField] private float brushStrength;


    public float BrushSize { get => brushSize; }
    public float BrushStrength { get => brushStrength; }
}
