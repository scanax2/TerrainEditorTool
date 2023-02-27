using UnityEngine;

public static class PerlinNoiseUtilities
{
    public static float GetYForVertex(int x, int z, float heightMapSize, 
        float scale, float noiseOffset, float heightMultiplier)
    {
        float perlinX = (float)x / heightMapSize * scale + noiseOffset;
        float perlinZ = (float)z / heightMapSize * scale + noiseOffset;

        float y = Mathf.PerlinNoise(perlinX, perlinZ) * heightMultiplier;

        return y;
    }
}
