using UnityEngine;

public interface ITerrainLandscapeEditor
{
    public RenderTexture GenerateHeightmapTexture();
    public void SculptTerrain(float x, float z, TerrainOperationType operationType);
    public void UpdateRequest();
}
