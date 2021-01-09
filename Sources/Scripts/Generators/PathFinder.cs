using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class PathFinder : MonoBehaviour
{
    Stack<Tuple<int, int>> pathStack;
    Tuple<int, int> farthestCell;

    public void findPath()
    {
        pathStack = new Stack<Tuple<int, int>>();

        Tuple<int, int> startingCell = new Tuple<int, int>(0, 0);
        Tuple<int, int> endingCell = new Tuple<int, int>(DataHolder.Instance.grid_size - 1, DataHolder.Instance.grid_size - 1);

        pathStack.Push(startingCell);

        int pathLength = 0;

        farthestCell = startingCell;

        processPath(pathLength);

        Debug.Log(farthestCell);
    }

    private void processPath(int pathLength)
    {
        List<Tuple<int, int>> nextCells = new List<Tuple<int, int>>();
        Debug.Log(pathLength);
        if (pathStack.Count != 0)
        {
            // detecte la suite du chemin
            nextCells = getNextCells(processCurrentCell(pathLength));

            addToPathStack(nextCells);

            processPath(pathLength+=1);
        }
    }

    private void addToPathStack(List<Tuple<int, int>> nextCells)
    {
        // ajoute la suite du chemin a la stack
        foreach (Tuple<int, int> cell in nextCells)
        {
            if (pathStack.Contains(cell) == false) // On ajoute pas deux fois
            {
                if (DataHolder.Instance.noiseMap[cell.Item1, cell.Item2] < 0) // On ajoute pas une case deja parcourue
                {
                    Debug.Log("Cell to Add:" + cell.ToString() + '\n');
                    pathStack.Push(cell);
                }
            }
        }
        nextCells.Clear();
    }

    public Tuple<int, int> processCurrentCell(int pathLength)
    {
        // recupere la derniere case ajoutee
        Tuple<int, int> currentCell = pathStack.Pop();

        // donne a la case sa distance par rapport a la case de depart
        DataHolder.Instance.noiseMap[currentCell.Item1, currentCell.Item2] = pathLength;

        // enregistre la case la plus loin
        if (DataHolder.Instance.noiseMap[currentCell.Item1, currentCell.Item2] > DataHolder.Instance.noiseMap[farthestCell.Item1, farthestCell.Item2])
        {
            farthestCell = currentCell;
        }

        return currentCell;
    }

    private List<Tuple<int, int>> getNextCells(Tuple<int, int> originalCell)
    {
        List<Tuple<int, int>> nextCells = new List<Tuple<int, int>>();
        int x = originalCell.Item1;
        int y = originalCell.Item2;

        // UP
        if ((y - 1) >= 0 && DataHolder.Instance.noiseMap[x, y - 1] == DataHolder.Instance.FULL)
        {
            nextCells.Add(new Tuple<int, int>(x, y - 1));
        }

        // Down
        if ((y + 1) < DataHolder.Instance.grid_size && DataHolder.Instance.noiseMap[x, y + 1] == DataHolder.Instance.FULL)
        {
            nextCells.Add(new Tuple<int, int>(x, y + 1));
        }

        // Right
        if ((x + 1) < DataHolder.Instance.grid_size && DataHolder.Instance.noiseMap[x + 1, y] == DataHolder.Instance.FULL)
        {
            nextCells.Add(new Tuple<int, int>(x + 1, y));
        }

        // Left
        if ((x - 1) >= 0 && DataHolder.Instance.noiseMap[x - 1, y] == DataHolder.Instance.FULL)
        {
            nextCells.Add(new Tuple<int, int>(x - 1, y));
        }

        return nextCells;
    }
}
