using UnityEngine;

public static class PerlinNoiseUtilities
{
    public static float GetHeightForVertex(int heightMapSize, int x, int y, float scale, float noiseOffset)
    {
        float perlinX = ((float)x / heightMapSize + noiseOffset) * scale;
        float perlinY = ((float)y / heightMapSize + noiseOffset) * scale;

        float perlin = Mathf.PerlinNoise(perlinX, perlinY);

        return perlin;
    }
}
