using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GridManager : MonoBehaviour
{
    public float gridXSpacing = 5f;
    public float gridYSpacing = 5f;

    public GridConfig[] configs;
    public GemSkin[] skins;

    public GameObject tilePrefab;
    public GameObject gemPrefab;

    private Grid activeGrid;
    private int activeSkinIndex;
    private int activeConfigIndex;

    private GameObject[,] tileObjects;
    private GameObject[,] gemObjects;

    public Button skinButton;
    public Button gridButton;
    public Button difficultyButton;

    public TextMeshProUGUI difficultyLabel;

    // Start is called before the first frame update
    void Start()
    {
        skinButton.onClick.AddListener(CycleSkin);
        gridButton.onClick.AddListener(RefreshGrid);
        difficultyButton.onClick.AddListener(CycleConfig);

        activeSkinIndex = 0;
        activeConfigIndex = 0;

        Teardown();
        SetupGrid();
    }

    void CycleSkin()
    {
        activeSkinIndex++;
        if (activeSkinIndex >= skins.Length)
            activeSkinIndex = 0;

        UpdateGems();
    }

    void RefreshGrid()
    {
        activeGrid.Generate();
        UpdateGems();
    }

    void SetupGrid()
    {
        difficultyLabel.text = configs[activeConfigIndex].name;
        activeGrid = new Grid(configs[activeConfigIndex]);
        CreateGrid();
        UpdateGems();
    }

    void CycleConfig()
    {
        activeConfigIndex++;
        if (activeConfigIndex >= configs.Length)
            activeConfigIndex = 0;

        Teardown();
        SetupGrid();
    }

    void Teardown()
    {
        if(tileObjects != null)
        {
            foreach(var tile in tileObjects)
            {
                GameObject.Destroy(tile);
            }
            tileObjects = null;
        }

        if(gemObjects != null)
        {
            foreach (var gem in gemObjects)
            {
                GameObject.Destroy(gem);
            }
            gemObjects = null;

        }
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
        var activeSkin = skins[activeSkinIndex];

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
