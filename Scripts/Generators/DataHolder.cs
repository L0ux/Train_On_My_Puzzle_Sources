using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

public class DataHolder : Singleton<DataHolder>
{
    public int grid_size;
    public float block_size;

    public float[,] noiseMap;

    public float EMPTY = -5f;
    public float FULL = -10f;

    [Range(0, 100)]
    public int train_speed;

    public List<GameObject> blocks_template;

    protected DataHolder() { }

    public void Init()
    {
        noiseMap = new float[grid_size, grid_size];
    }

    public void printInfo()
    {
        String str = "DataHolder Info\nGrid Size: " + grid_size.ToString() + "x" + grid_size.ToString();
        str += "\nBlock Size: " + block_size.ToString() + "x" + block_size.ToString();
        str += "\nTrain Speed: " + train_speed.ToString();
        str += "\nEmpty cell value: " + EMPTY.ToString();
        str += "\nCell to be Filled value: " + FULL.ToString();
        str += "\nBlocks at our Disposition: \n";
        foreach(GameObject block in blocks_template)
        {
            str += block.name + "\n";
        }
        Debug.Log(str);
    }

    public void printGrid()
    {
        String str = "Current State Of The Grid:\n";
        for (int i = 0; i < grid_size; i++)
        {
            for (int j = 0; j < grid_size; j++)
            {
                str += noiseMap[i, j].ToString() + " | ";
            }
            str += "\n";
        }
        Debug.Log(str);
    }
}
