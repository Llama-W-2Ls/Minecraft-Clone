using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct Noise
{
    [Header("Main")]
    public float Scale;
    public float Amplitude;
    public float Height;

    [Header("Detail")]
    public int Octaves;
    public float Persistence;
    public float Lacunarity;

    [Header("Randomness")]
    public string Seed;
    public Vector2 Offset;

    public void Initialize(bool RandomizeSeed = false)
    {
        if (RandomizeSeed)
            Seed = System.DateTime.Now.ToString();

        var rand = new System.Random(Seed.GetHashCode());

        Offset.x += rand.Next(-100000, 100000);
        Offset.y += rand.Next(-100000, 100000);
    }

    public float GetValue2D(int x, int y)
    {
        float amplitude = 1;
        float frequency = 1;
        float noiseHeight = 0;

        for (int i = 0; i < Octaves; i++)
        {
            float sampleX = (x + Offset.x) / Scale * frequency;
            float sampleY = (y + Offset.y) / Scale * frequency;

            float noiseValue = Mathf.PerlinNoise(sampleX, sampleY);

            noiseHeight += noiseValue * amplitude;

            amplitude *= Persistence;
            frequency *= Lacunarity;
        }

        return noiseHeight * Amplitude + Height;
    }
}
