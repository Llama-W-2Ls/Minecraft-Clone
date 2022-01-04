using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Textures : MonoBehaviour
{
    public static Textures instance;

    public Texture2D TextureAtlas;
    public Vector2Int Dimensions;

    [SerializeField] BlockUV[] UVs;
    public Dictionary<BlockType, BlockUV> UVCoords;

    void Awake()
    {
        instance = this;
        InitializeUVs();
    }

    void InitializeUVs()
    {
        UVCoords = new Dictionary<BlockType, BlockUV>();

        foreach (var item in UVs)
        {
            UVCoords.Add(item.Type, item);
        }
    }

    public Vector2[] GetUVs(Vector2Int coords)
    {
        float xOffset = 1f / Dimensions.x;
        float yOffset = 1f / Dimensions.y;

        return new Vector2[4]
        {
            new Vector2(coords.x * xOffset, coords.y * yOffset),
            new Vector2((coords.x + 1) * xOffset, coords.y * yOffset),
            new Vector2(coords.x * xOffset, (coords.y + 1) * yOffset),
            new Vector2((coords.x + 1) * xOffset, (coords.y + 1) * yOffset)
        };
    }
}

[System.Serializable]
public struct BlockUV
{
    public BlockType Type;

    public Vector2Int LeftCoords;
    public Vector2Int RightCoords;
    public Vector2Int TopCoords;
    public Vector2Int BottomCoords;
    public Vector2Int FrontCoords;
    public Vector2Int BackCoords;
}
