using UnityEngine;

[System.Serializable]
public class TerrainGenerationData
{
    /// <summary>
    /// Value in vertices, result mesh 256x256
    /// </summary>
    [SerializeField] private int terrainSize = 256;
    /// <summary>
    /// Value in pixels PxP (2048x2048)
    /// </summary>
    [SerializeField] private int heightMapSize = 2048;
    [SerializeField] private float scale = 10f;
    [SerializeField] private float noiseOffset = 75f;
    [SerializeField] private float maxHeight = 75f;


    public int TerrainSize { get => terrainSize; set => terrainSize = value; }
    public int HeightMapSize { get => heightMapSize; set => heightMapSize = value; }
    public float Scale { get => scale; set => scale = value; }
    public float NoiseOffset { get => noiseOffset; set => noiseOffset = value; }
    public float MaxHeight { get => maxHeight; }
}
