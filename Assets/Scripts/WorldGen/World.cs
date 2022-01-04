using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public static World instance;

    [Header("World Properties")]
    public int ViewDistanceInChunks;
    public Transform Player;
    public Dictionary<Vector2Int, ChunkObj> Chunks;

    [Header("Chunk Properties")]
    public Vector3Int ChunkSize;
    public Noise Noise;
    public Material ChunkMat;

    Vector2Int PrevPlayerPos;

    void Awake()
    {
        instance = this;
        Chunks = new Dictionary<Vector2Int, ChunkObj>();
        Noise.Initialize();
    }

    async void Start()
    {
        await GenerateWorld(Vector2Int.zero);

        while (Application.isPlaying)
        {
            var playerPos = GetPlayerPos();

            if (playerPos != PrevPlayerPos)
            {
                await GenerateWorld(playerPos);
                PrevPlayerPos = playerPos;
            }

            await Task.Delay((int)(Time.deltaTime * 1000f));
        }
    }

    void Update()
    {
        DestroyOutOfViewChunks();
    }

    async Task GenerateWorld(Vector2Int playerPos)
    {
        var chunks = new List<Tuple<Chunk, Vector2Int>>();
        var meshes = new List<Tuple<MeshData, Vector2Int>>();

        for (int x = -ViewDistanceInChunks - 1; x <= ViewDistanceInChunks + 1; x++)
        {
            for (int z = -ViewDistanceInChunks - 1; z <= ViewDistanceInChunks + 1; z++)
            {
                var chunkPos = new Vector2Int(x * ChunkSize.x, z * ChunkSize.z) + playerPos;

                var chunk = !Chunks.ContainsKey(chunkPos)
                    ? new Chunk(chunkPos, ChunkSize, Noise)
                    : Chunks[chunkPos].chunk;

                if (!Chunks.ContainsKey(chunkPos))
                    Chunks.Add(chunkPos, new ChunkObj(chunk, null));

                if (x > -ViewDistanceInChunks - 1 && x < ViewDistanceInChunks + 1 &&
                    z > -ViewDistanceInChunks - 1 && z < ViewDistanceInChunks + 1)
                {
                    chunks.Add(new Tuple<Chunk, Vector2Int>(chunk, chunkPos));
                }
            }
        }

        await Task.Run(() =>
        {
            foreach (var chunk in chunks)
            {
                // Chunk is already generated
                if (Chunks[chunk.Item2].IsActive)
                    continue;

                meshes.Add(new Tuple<MeshData, Vector2Int>(chunk.Item1.GenerateMesh(chunk.Item2), chunk.Item2));
            }
        });

        foreach (var mesh in meshes)
        {
            Chunks[mesh.Item2].obj = SpawnChunk(mesh.Item1, mesh.Item2);
            Chunks[mesh.Item2].IsActive = true;
        }
    }

    void DestroyOutOfViewChunks()
    {
        var playerPos = GetPlayerPos();

        foreach (var chunk in Chunks)
        {
            if (Vector2.Distance(chunk.Key, playerPos) > (ViewDistanceInChunks + 1) * ChunkSize.x)
            {
                Destroy(chunk.Value.obj);
                chunk.Value.IsActive = false;
            }
        }
    }

    GameObject SpawnChunk(MeshData meshData, Vector2Int pos)
    {
        var obj = new GameObject(pos.ToString());
        obj.transform.position = new Vector3(pos.x - ChunkSize.x / 2, 0, pos.y - ChunkSize.z / 2);
        obj.transform.parent = transform;

        obj.AddComponent<MeshFilter>().mesh = meshData.ToMesh();
        obj.AddComponent<MeshRenderer>().material = ChunkMat;
        obj.AddComponent<MeshCollider>();

        return obj;
    }

    Vector2Int GetPlayerPos()
    {
        return new Vector2Int
        (
            Mathf.RoundToInt(Player.position.x / ChunkSize.x) * ChunkSize.x,
            Mathf.RoundToInt(Player.position.z / ChunkSize.z) * ChunkSize.z
        );
    }
}

public class ChunkObj
{
    public Chunk chunk;
    public GameObject obj;
    public bool IsActive;

    public ChunkObj(Chunk chunk, GameObject obj)
    {
        this.chunk = chunk;
        this.obj = obj;
    }
}
