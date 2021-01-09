using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NoiseMapGenerator : MonoBehaviour
{
    [Range(0f, 0.5f)]
    public float PerlinScale;

    [SerializeField]
    private float noiseMapMeanValue;

    public Renderer textureRenderer;
   
    public bool shouldRender_NoiseMap;
    
    public void generateNoiseMap()
    {
        if (PerlinScale <= 0)
        {
            PerlinScale = 0.0001f;
        }


        for (int y = 0; y < DataHolder.Instance.grid_size; y++)
        {
            for (int x = 0; x < DataHolder.Instance.grid_size; x++)
            {
                float sampleX = x / (DateTime.Now.Second * PerlinScale);
                float sampleY = y / (DateTime.Now.Second * PerlinScale);

                float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);

                DataHolder.Instance.noiseMap[x, y] = perlinValue;
            }
        }

        computeNoiseMapMeanValue();

        binarizeNoiseMap();

        DataHolder.Instance.noiseMap[0, 0] = DataHolder.Instance.FULL;
        DataHolder.Instance.noiseMap[DataHolder.Instance.grid_size - 1, DataHolder.Instance.grid_size - 1] = DataHolder.Instance.FULL;

        if (shouldRender_NoiseMap)
        {
            textureRenderer.enabled = true;
            renderNoiseMap();
        }
        else
        {
            textureRenderer.enabled = false;
        }
    }

    public void generateDebugNoiseMap1()
    {
        float _in = DataHolder.Instance.FULL;
        float _out = DataHolder.Instance.EMPTY;
        float[,] temp = new float[,]{{  _in,
                                        _in,
                                        _in,
                                        _out,
                                        _in,
                                        _in,
                                        _out,
                                        _in,
                                        _out,
                                        _in},{  _out,
                                                _in,
                                                _in,
                                                _in,
                                                _in,
                                                _out,
                                                _in,
                                                _in,
                                                _out,
                                                _out},{ _out,
                                                        _in,
                                                        _out,
                                                        _out,
                                                        _out,
                                                        _in,
                                                        _in,
                                                        _in,
                                                        _out,
                                                        _in},{  _in,
                                                                _in,
                                                                _out,
                                                                _in,
                                                                _out,
                                                                _in,
                                                                _out,
                                                                _out,
                                                                _in,
                                                                _out},{ _out,
                                                                        _out,
                                                                        _out,
                                                                        _in,
                                                                        _out,
                                                                        _out,
                                                                        _out,
                                                                        _out,
                                                                        _out,
                                                                        _in},{  _in,
                                                                                _in,
                                                                                _in,
                                                                                _in,
                                                                                _out,
                                                                                _out,
                                                                                _in,
                                                                                _out,
                                                                                _in,
                                                                                _out},{ _in,
                                                                                        _out,
                                                                                        _in,
                                                                                        _in,
                                                                                        _in,
                                                                                        _out,
                                                                                        _out,
                                                                                        _in,
                                                                                        _out,
                                                                                        _out},{ _out,
                                                                                                _out,
                                                                                                _out,
                                                                                                _out,
                                                                                                _in,
                                                                                                _out,
                                                                                                _out,
                                                                                                _in,
                                                                                                _in,
                                                                                                _in}, { _in,
                                                                                                        _in,
                                                                                                        _in,
                                                                                                        _out,
                                                                                                        _in,
                                                                                                        _out,
                                                                                                        _in,
                                                                                                        _in,
                                                                                                        _out,
                                                                                                        _out},{ _out,
                                                                                                                _in,
                                                                                                                _out,
                                                                                                                _out,
                                                                                                                _out,
                                                                                                                _in,
                                                                                                                _out,
                                                                                                                _in,
                                                                                                                _out,
                                                                                                                _in}};
        DataHolder.Instance.noiseMap = temp;
        renderNoiseMap();
        DataHolder.Instance.printGrid();
    }
    private void renderNoiseMap()
    {

        Texture2D texture = new Texture2D(DataHolder.Instance.grid_size, DataHolder.Instance.grid_size);

        Color[] colorMap = new Color[DataHolder.Instance.grid_size * DataHolder.Instance.grid_size];
        for (int y = 0; y < DataHolder.Instance.grid_size; y++)
        {
            for (int x = 0; x < DataHolder.Instance.grid_size; x++)
            {
                if (DataHolder.Instance.noiseMap[x, y] == DataHolder.Instance.EMPTY)
                {
                    colorMap[y * DataHolder.Instance.grid_size + x] = Color.black;
                }
                else
                {
                    colorMap[y * DataHolder.Instance.grid_size + x] = Color.white;
                }
                
            }
        }
        texture.SetPixels(colorMap);
        texture.Apply();

        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(DataHolder.Instance.grid_size, 1, DataHolder.Instance.grid_size);
    }

    private void computeNoiseMapMeanValue()
    {
        float mean = 0;
        int nb_cells = DataHolder.Instance.grid_size * DataHolder.Instance.grid_size;

        for (int y = 0; y < DataHolder.Instance.grid_size; y++)
        {
            for (int x = 0; x < DataHolder.Instance.grid_size; x++)
            {
                mean += DataHolder.Instance.noiseMap[x, y];
            }
        }

        noiseMapMeanValue = mean / nb_cells;
    }

    public float retrieveNoiseMapMeanValue()
    {
        return noiseMapMeanValue;
    }

    private void binarizeNoiseMap()
    {
        for (int y = 0; y < DataHolder.Instance.grid_size; y++)
        {
            for (int x = 0; x < DataHolder.Instance.grid_size; x++)
            {
                if (DataHolder.Instance.noiseMap[x, y] < noiseMapMeanValue)
                {
                    DataHolder.Instance.noiseMap[x, y] = DataHolder.Instance.EMPTY;
                }
                else
                {
                    DataHolder.Instance.noiseMap[x, y] = DataHolder.Instance.FULL;
                }
            }
        }
    }
}
