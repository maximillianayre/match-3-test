using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public float gridXSpacing = 5f;
    public float gridYSpacing = 5f;

    public GridConfig defaultConfig;
    public GemSkin defaultSkin;

    public GameObject tilePrefab;
    public GameObject gemPrefab;

    private Grid activeGrid;
    private GemSkin activeSkin;

    private GameObject[,] tileObjects;
    private GameObject[,] gemObjects;

    // Start is called before the first frame update
    void Start()
    {
        activeSkin = defaultSkin;
        activeGrid = new Grid(defaultConfig);
        var state = activeGrid.GetState();
        CreateGrid();
        UpdateGems();
    }

    void CreateGrid()
    {
        var rows = activeGrid.GetConfig().rows;
        var columns = activeGrid.GetConfig().columns;
        tileObjects = new GameObject[rows, columns];
        gemObjects = new GameObject[rows, columns];

        var totalWidth = gridXSpacing * columns;
        var totalHeight = gridYSpacing * rows;
        for(int row = 0; row < rows; row++)
        {
            for(int column = 0; column < columns; column++)
            {
                var tile = GameObject.Instantiate(tilePrefab, new Vector3((column * gridXSpacing) - 0.5f * totalWidth, (row * gridYSpacing) - 0.5f * totalHeight, 0f), Quaternion.identity);
                tileObjects[row, column] = tile;
            }
        }

    }

    void UpdateGems()
    {
        var rows = activeGrid.GetConfig().rows;
        var columns = activeGrid.GetConfig().columns;
        var state = activeGrid.GetState();

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                if (gemObjects[row, column] != null)
                    GameObject.Destroy(gemObjects[row, column]);

                var gem = GameObject.Instantiate(gemPrefab, tileObjects[row, column].transform);
                gem.GetComponent<SpriteRenderer>().sprite = activeSkin.sprites[state[row, column]];
                gemObjects[row, column] = gem;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
