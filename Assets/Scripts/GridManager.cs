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
        Debug.Log(String.Join(" ", activeGrid.GetState().Cast<int>()));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
