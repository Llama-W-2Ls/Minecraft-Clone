using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Array3D<T> : IEnumerable
{
    readonly T[] Data;

    #region Properties

    /// <summary>
    /// Dimensions of array
    /// </summary>
    public readonly Vector3Int Size;

    /// <summary>
    /// Total length of all data in the array
    /// </summary>
    public int Length { get { return Data.Length; } }

    #endregion

    #region Constructors

    public Array3D(int xSize, int ySize, int zSize)
    {
        Size = new Vector3Int(xSize, ySize, zSize);
        Data = new T[xSize * ySize * zSize];
    }

    public Array3D(Vector3Int size)
    {
        Size = size;
        Data = new T[size.x * size.y * size.z];
    }

    #endregion

    #region Indexers

    public T this[int index]
    {
        get { return Data[index]; }
        set { Data[index] = value; }
    }

    public T this[int x, int y, int z]
    {
        get { return Data[To1DIndex(x, y, z)]; }
        set { Data[To1DIndex(x, y, z)] = value; }
    }

    public T this[Vector3Int pos]
    {
        get { return this[pos.x, pos.y, pos.z]; }
        set { this[pos.x, pos.y, pos.z] = value; }
    }

    #endregion

    #region Index Conversion Methods

    public int To1DIndex(int x, int y, int z)
    {
        if (OutOfBounds(x, y, z))
            return -1;

        return z * Size.x * Size.y + y * Size.x + x;
    }

    public Vector3Int To3DIndex(int index)
    {
        if (OutOfBounds(index))
            return new Vector3Int(-1, -1, -1);

        int z = index / (Size.x * Size.y);

        index -= (z * Size.x * Size.y);
        int y = index / Size.x;
        int x = index % Size.x;

        return new Vector3Int(x, y, z);
    }

    #endregion

    #region Bound Checks

    public bool OutOfBounds(int index)
    {
        return index < 0 || index >= Data.Length;
    }

    public bool OutOfBounds(int x, int y, int z)
    {
        return x < 0 || x >= Size.x ||
               y < 0 || y >= Size.y ||
               z < 0 || z >= Size.z;
    }

    public bool OutOfBounds(Vector3Int pos)
    {
        return OutOfBounds(pos.x, pos.y, pos.z);
    }

    #endregion

    #region Enumerators

    public IEnumerator GetEnumerator()
    {
        return new Array3DEnumerator<T>(Data);
    }

    #endregion
}

class Array3DEnumerator<T> : IEnumerator<T>
{
    readonly T[] Data;
    int counter = -1;

    #region Properties

    public T Current => Data[counter];

    object IEnumerator.Current => Data[counter];

    #endregion

    #region Constructors

    public Array3DEnumerator(T[] data)
    {
        Data = data;
    }

    #endregion

    #region Enumeration Methods

    public void Dispose() { }

    public bool MoveNext()
    {
        counter++;
        return counter != Data.Length;
    }

    public void Reset()
    {
        counter = -1;
    }

    #endregion
}
