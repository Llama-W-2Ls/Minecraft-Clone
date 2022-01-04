using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshData
{
    public List<Vector3> Vertices;
    public List<int> Triangles;
    public List<Vector2> UVs;

    public MeshData()
    {
        Vertices = new List<Vector3>();
        Triangles = new List<int>();
        UVs = new List<Vector2>();
    }

    public void AddQuad(Quad quad, Vector2Int uvCoords, Vector3Int offset)
    {
        AddQuad(quad.BL + offset, quad.BR + offset, quad.TL + offset, quad.TR + offset, uvCoords, quad.FacesOutward);
    }

    void AddQuad(Vector3 bl, Vector3 br, Vector3 tl, Vector3 tr, Vector2Int uvCoords, bool outward = false)
    {
        var triangles = outward ? new int[] { 0, 1, 2, 1, 3, 2 } : new int[] { 2, 1, 0, 2, 3, 1 };
        AddTriangles(triangles, Vertices.Count);
        AddTexture(uvCoords);

        Vertices.AddRange(new Vector3[] { bl, br, tl, tr });
    }

    void AddTriangles(int[] triangles, int increment)
    {
        foreach (var triangle in triangles)
        {
            Triangles.Add(triangle + increment);
        }
    }

    void AddTexture(Vector2Int coords)
    {
        var uvs = Textures.instance.GetUVs(coords);
        UVs.AddRange(uvs);
    }

    public Mesh ToMesh()
    {
        var mesh = new Mesh()
        {
            vertices = Vertices.ToArray(),
            triangles = Triangles.ToArray(),
            uv = UVs.ToArray()
        };

        mesh.RecalculateNormals();

        return mesh;
    }
}
