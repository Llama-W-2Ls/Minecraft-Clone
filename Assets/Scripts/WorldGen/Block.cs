using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Block
{
    byte type;

    #region Properties

    public BlockType Type 
    { 
        get { return (BlockType)type; }
        set { type = (byte)value; }
    }

    #endregion

    #region Constructors

    public Block(BlockType type)
    {
        this.type = (byte)type;
    }

    #endregion
}

public enum BlockType
{
    Air,
    Dirt
}
