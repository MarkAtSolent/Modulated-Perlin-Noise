using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.Profiling;
using System.Diagnostics;
using System;

public class GenerateHeights : MonoBehaviour
{
    private float Amplitude = 10.0f;
    private float TileSize = 10.0f;
    private Terrain TerrainComponent;
    private int TerrainSize;

    private int Octaves = 8;
    private float Lacunarity = 2.0f;
    private float Persistance = 1.8f;

    private float ModulatorFrequency = 1.0f;
    private float ModulatorPhase = 0.0f;

    private float MaxTotal = 0.0f;
    private float MinTotal = 1.0f;
    private float TotalTotal = 0.0f;
    private float AvTotal = 0.0f;

    // Use this for initialization
    void Start()
    {
        TerrainComponent = GetComponent<Terrain>();
        TerrainSize = TerrainComponent.terrainData.heightmapWidth * TerrainComponent.terrainData.heightmapHeight;

        float[,] heights = new float[TerrainComponent.terrainData.heightmapWidth, TerrainComponent.terrainData.heightmapHeight];
        for (int z = 0; z < TerrainComponent.terrainData.heightmapWidth; z++)
        {
            for (int x = 0; x < TerrainComponent.terrainData.heightmapHeight; x++)
            {
                heights[z, x] = 0.0f;
            }
        }

        float phaseChange = ModulatorPhase;

        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        for (int z = 0; z < TerrainComponent.terrainData.heightmapWidth; z++)
        {
            float frequencyChange = ModulatorFrequency;

            for (int x = 0; x < TerrainComponent.terrainData.heightmapHeight; x++)
            {
                //heights[z, x] = ScaledUnModulatedNoise(z, x);
                heights[z, x] = ScaledModulatedNoise(z, x, frequencyChange, phaseChange);
                //heights[z, x] = ScaledPerlinModulatedNoise(z, x);
                //heights[z, x] = UnScaledModulatedNoise(z, x, frequencyChange, phaseChange);

                // Increases frequency along the x axis
                frequencyChange += 4 * Mathf.PI / TerrainComponent.terrainData.heightmapHeight;
            }
            // Changes phase along the z axis by 180 degrees
            phaseChange -= Mathf.PI / TerrainComponent.terrainData.heightmapWidth;
        }

        stopWatch.Stop();
        TimeSpan ts = stopWatch.Elapsed;
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds);
        UnityEngine.Debug.Log("RunTime " + elapsedTime);
        //UnityEngine.Debug.Log("MaxPertrb " + MaxTotal);
        //UnityEngine.Debug.Log("MinPerturb " + MinTotal);
        //UnityEngine.Debug.Log("TotalPerturb " + TotalTotal);
        //UnityEngine.Debug.Log("TerrainSize " + TerrainSize);
        //AvTotal = TotalTotal / TerrainSize / 10;
        //UnityEngine.Debug.Log("AvPerturb " + TotalTotal / TerrainSize / 10);

        TerrainComponent.terrainData.SetHeights(0, 0, heights);
    }

    private float ScaledPerlinModulatedNoise(int z, int x)
    {
        float total = 0.0f;
        float scaledAmplitude = 5.0f;
        float sizeModifier = 2.0f;
        float position = (float)(z + x);
        //float size = (float)(TerrainComponent.terrainData.heightmapWidth * TerrainComponent.terrainData.heightmapHeight);

        for (int octave = 0; octave < Octaves; octave++)
        {
            float noise = Mathf.PerlinNoise((z / (float)TerrainComponent.terrainData.heightmapWidth) * sizeModifier, (x / (float)TerrainComponent.terrainData.heightmapHeight) * sizeModifier);
            float modulation = Mathf.PerlinNoise((octave / (float)Octaves) * sizeModifier, 0.0f); // position / TerrainSize * sizeModifier);

            noise *= modulation;
            noise *= 1 / scaledAmplitude;
            total += noise;

            //MaxTotal = Mathf.Max(MaxTotal, total);
            //MinTotal = Mathf.Min(MinTotal, total);
            //TotalTotal += total;

            sizeModifier *= Lacunarity;
            scaledAmplitude *= Persistance;
        }

        return total;
    }

    private float OffsetCos(float x, float frequency, float phase)
    {
        return (Mathf.Cos(x * Mathf.PI * frequency - phase) + 1) / 2;
    }

    private float ScaledUnModulatedNoise(int z, int x)
    {
        float total = 0.0f;
        float scaledAmplitude = Amplitude;
        float tileSizeModifier = TileSize;

        for (int octave = 0; octave < Octaves; octave++)
        {
            float noise = Mathf.PerlinNoise((z / (float)TerrainComponent.terrainData.heightmapWidth) * tileSizeModifier, (x / (float)TerrainComponent.terrainData.heightmapHeight) * tileSizeModifier);
            noise *= 1 / scaledAmplitude;
            total += noise;

            tileSizeModifier *= Lacunarity;
            scaledAmplitude *= Persistance;
        }

        return total;
    }

    private float ScaledModulatedNoise(int z, int x, float frequency, float phase)
    {
        float total = 0.0f;
        float scaledAmplitude = Amplitude;
        float tileSizeModifier = TileSize;

        for (int octave = 0; octave < Octaves; octave++)
        {
            float noise = Mathf.PerlinNoise((z / (float)TerrainComponent.terrainData.heightmapWidth) * tileSizeModifier, (x / (float)TerrainComponent.terrainData.heightmapHeight) * tileSizeModifier);
            float modulation = OffsetCos((octave + 1) / (float)Octaves, frequency, phase);
            noise *= modulation;
            noise *= 1 / scaledAmplitude;
            total += noise;

            tileSizeModifier *= Lacunarity;
            scaledAmplitude *= Persistance;
        }

        return total;
    }

    private float UnScaledModulatedNoise(int z, int x, float frequency, float phase)
    {
        float total = 0.0f;
        float tileSizeModifier = TileSize;

        for (int octave = 0; octave < Octaves; octave++)
        {
            float noise = Mathf.PerlinNoise((z / (float)TerrainComponent.terrainData.heightmapWidth) * tileSizeModifier, (x / (float)TerrainComponent.terrainData.heightmapHeight) * tileSizeModifier);
            float modulation = OffsetCos((octave + 1) / (float)Octaves, frequency, phase);
            //noise *= modulation;
            noise /= (Amplitude / modulation);
            total += noise;

            tileSizeModifier *= Lacunarity;
        }

        return total;
    }
}

