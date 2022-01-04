using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public Array3D<Block> Blocks;

    public Chunk(Vector2Int pos, Vector3Int size, Noise noise)
    {
        InitializeBlocks(pos, size, noise);
    }

    void InitializeBlocks(Vector2Int pos, Vector3Int size, Noise noise)
    {
        Blocks = new Array3D<Block>(size);

        for (int x = 0; x < Blocks.Size.x; x++)
        {
            for (int z = 0; z < Blocks.Size.z; z++)
            {
                var noiseValue = noise.GetValue2D(x + pos.x, z + pos.y);

                for (int y = 0; y < Blocks.Size.y; y++)
                {
                    var blockType = y > noiseValue ? BlockType.Air : BlockType.Dirt;
                    Blocks[x, y, z] = new Block(blockType);
                }
            }
        }
    }

    public MeshData GenerateMesh(Vector2Int chunkPos)
    {
        var meshData = new MeshData();

        for (int x = 0; x < Blocks.Size.x; x++)
        {
            for (int y = 0; y < Blocks.Size.y; y++)
            {
                for (int z = 0; z < Blocks.Size.z; z++)
                {
                    var block = Blocks[x, y, z];

                    if (block.Type == BlockType.Air)
                        continue;

                    var blockUVs = Textures.instance.UVCoords[block.Type];

                    var neighbours = new Tuple<Vector3Int, Vector2Int, FaceSide>[]
                    {
                        new Tuple<Vector3Int, Vector2Int, FaceSide>(new Vector3Int(x - 1, y, z), blockUVs.LeftCoords, FaceSide.Left),
                        new Tuple<Vector3Int, Vector2Int, FaceSide>(new Vector3Int(x + 1, y, z), blockUVs.RightCoords, FaceSide.Right),
                        new Tuple<Vector3Int, Vector2Int, FaceSide>(new Vector3Int(x, y + 1, z), blockUVs.TopCoords, FaceSide.Top),
                        new Tuple<Vector3Int, Vector2Int, FaceSide>(new Vector3Int(x, y - 1, z), blockUVs.BottomCoords, FaceSide.Bottom),
                        new Tuple<Vector3Int, Vector2Int, FaceSide>(new Vector3Int(x, y, z - 1), blockUVs.FrontCoords, FaceSide.Back),
                        new Tuple<Vector3Int, Vector2Int, FaceSide>(new Vector3Int(x, y, z + 1), blockUVs.BackCoords, FaceSide.Front)
                    };

                    foreach (var neighbour in neighbours)
                    {
                        var offset = new Vector3Int(x, y, z);
                        var neighbourPos = neighbour.Item1;

                        if (IsEmptyAt(neighbourPos, offset, chunkPos))
                        {
                            var quad = VoxelData.Faces[neighbour.Item3];
                            meshData.AddQuad(quad, neighbour.Item2, offset);
                        }
                    }
                }
            }
        }

        return meshData;
    }

    bool IsEmptyAt(Vector3Int neighbourPos, Vector3Int blockPos, Vector2Int chunkPos)
    {
        // Locate neighbour in chunk
        if (!Blocks.OutOfBounds(neighbourPos))
            return Blocks[neighbourPos].Type == BlockType.Air;

        var chunkOffset = (neighbourPos - blockPos) * World.instance.ChunkSize;

        // Block isn't in any chunk (above the sky and below the ground)
        if (chunkOffset.y != 0)
            return true;

        // Neighbour chunk = current chunk + offset
        var neighbourChunkPos = chunkPos + new Vector2Int(chunkOffset.x, chunkOffset.z);

        // Block doesn't exist
        if (!World.instance.Chunks.ContainsKey(neighbourChunkPos))
            return true;

        var neighbourChunk = World.instance.Chunks[neighbourChunkPos].chunk;

        var neighbourBlockPos = neighbourPos + new Vector3Int(chunkPos.x, 0, chunkPos.y) - new Vector3Int(neighbourChunkPos.x, 0, neighbourChunkPos.y);
        var neighbourBlock = neighbourChunk.Blocks[neighbourBlockPos];

        return neighbourBlock.Type == BlockType.Air;
    }
}
