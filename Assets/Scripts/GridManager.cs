using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GridConfig defaultConfig;

    private Grid activeGrid;

    // Start is called before the first frame update
    void Start()
    {
        activeGrid = new Grid(defaultConfig);
        var state = activeGrid.GetState();

        for (int i = 0; i < state.GetLength(0); i++)
        {
            string currRow = "";
            for (int j = 0; j < state.GetLength(1); j++)
            {
                currRow += state[i, j] + " ";
            }
            Debug.Log(currRow + "\t");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
