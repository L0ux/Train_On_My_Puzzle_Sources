using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

public class LevelGenerator : MonoBehaviour
{
    private NoiseMapGenerator noiseGenerator;
    private PathFinder pathFinder;

    private GameObject map;

    private void Start()
    {
        DataHolder.Instance.Init();
        //DataHolder.Instance.printInfo();
        noiseGenerator = gameObject.GetComponent<NoiseMapGenerator>();
        pathFinder = gameObject.GetComponent<PathFinder>();
        generate();
    }


    public void generate()
    {
        if (map == null)
        {
            map = new GameObject();
            map.name = "Map";
        }
        if (map.transform.childCount != 0)
        {
            Destroy();
            map = new GameObject();
            map.name = "Map";
        }
        //noiseGenerator.generateNoiseMap();
        noiseGenerator.generateDebugNoiseMap1();
        pathFinder.findPath();
    }

    

    private void generateSimpleMap()
    {
        for (int y = 0; y < DataHolder.Instance.grid_size; y++)
        {
            for (int x = 0; x < DataHolder.Instance.grid_size; x++)
            {
                if (DataHolder.Instance.noiseMap[x, y] == DataHolder.Instance.EMPTY)
                {
                    //float real_x = x * DataHolder.Instance.block_size;
                    //float real_z = y * DataHolder.Instance.block_size;
                    //Instantiate(blocks_template[2], new Vector3(real_x, 10, real_z), Quaternion.identity).transform.parent = map.transform;
                }
                if (DataHolder.Instance.noiseMap[x,y] == DataHolder.Instance.FULL) { 
                    float real_x = x * DataHolder.Instance.block_size;
                    float real_z = y * DataHolder.Instance.block_size;
                    GameObject newProp = Instantiate(DataHolder.Instance.blocks_template[1], new Vector3(real_x, 10, real_z), Quaternion.identity);
                    newProp.transform.parent = map.transform;
                }

            }
        }
        map.transform.Rotate(new Vector3(0f, 1f, 0f), -90f);
    }




    private void breakPath()
    {

    }

    public void Destroy()
    {
        DestroyImmediate(map);
    }
}