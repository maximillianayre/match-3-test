using UnityEngine;

[CreateAssetMenu(fileName = "GridConfig", menuName = "Config/Grid", order = 1)]
public class GridConfig : ScriptableObject
{
    public int columns = 8;
    public int rows = 8;

    public int startingSolves = 3;

    public int gemCount;
}
