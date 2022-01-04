using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VoxelData
{
    public static Dictionary<FaceSide, Quad> Faces = new Dictionary<FaceSide, Quad>()
    {
        { 
            FaceSide.Left,
            new Quad
            (
                new Vector3(0, 0, 1),
                new Vector3(0, 0, 0),
                new Vector3(0, 1, 1),
                new Vector3(0, 1, 0),
                false
            )
        },
        {
            FaceSide.Right,
            new Quad
            (
                new Vector3(1, 0, 1),
                new Vector3(1, 0, 0),
                new Vector3(1, 1, 1),
                new Vector3(1, 1, 0),
                true
            )
        },
        {
            FaceSide.Top,
            new Quad
            (
                new Vector3(0, 1, 1),
                new Vector3(1, 1, 1),
                new Vector3(0, 1, 0),
                new Vector3(1, 1, 0),
                true
            )
        },
        {
            FaceSide.Bottom,
            new Quad
            (
                new Vector3(0, 0, 1),
                new Vector3(1, 0, 1),
                new Vector3(0, 0, 0),
                new Vector3(1, 0, 0),
                false
            )
        },
        {
            FaceSide.Front,
            new Quad
            (
                new Vector3(0, 0, 1),
                new Vector3(1, 0, 1),
                new Vector3(0, 1, 1),
                new Vector3(1, 1, 1),
                true
            )
        },
        {
            FaceSide.Back,
            new Quad
            (
                new Vector3(0, 0, 0),
                new Vector3(1, 0, 0),
                new Vector3(0, 1, 0),
                new Vector3(1, 1, 0),
                false
            )
        }
    };
}

public struct Quad
{
    public Vector3 BL;
    public Vector3 BR;
    public Vector3 TL;
    public Vector3 TR;

    public bool FacesOutward;

    public Quad(Vector3 bl, Vector3 br, Vector3 tl, Vector3 tr, bool outward)
    {
        BL = bl;
        BR = br;
        TL = tl;
        TR = tr;
        FacesOutward = outward;
    }
}

public enum FaceSide
{
    Left,
    Right,
    Top,
    Bottom,
    Front,
    Back
}
