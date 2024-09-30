using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Grid
{
    private GridConfig config;
    private Random random;
    private int[,] state;

    enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public Grid(GridConfig gridConfig)
    {
        config = gridConfig;
        random = new Random();
        Generate();
    }

    public int[,] GetState() => state;

    private int? Get(int row, int column)
    {
        if(row < 0 || row >= config.rows)
            return null;

        if (column < 0 || column >= config.columns)
            return null;

        return state[row, column];
    }

    private int? GetInDirection(int row, int column, Direction direction, int distance = 1)
    {
        int targetRow = row, targetColumn = column;
        switch (direction)
        {
            case Direction.Up:
                targetRow += distance;
                break;
            case Direction.Down:
                targetRow -= distance;
                break;
            case Direction.Left:
                targetColumn += distance;
                break;
            case Direction.Right:
                targetColumn -= distance;
                break;
        }

        return Get(targetRow, targetColumn);
    }

    private List<int> GetPotentialSolvesForTile(int row, int column)
    {
        var valuesThatCouldSolve = new List<int>();
        var valuesInDirections = new Dictionary<Direction, int?>();
        foreach(Direction direction in Enum.GetValues(typeof(Direction)))
        {
            var closest = GetInDirection(row, column, direction);
            var nextClosest = GetInDirection(row, column, direction, distance: 2);
            valuesInDirections.Add(direction, closest);

            if (closest != nextClosest || closest == null)
                continue;

            // The closest two in this direction already match, so mark this as a gem to avoid
            valuesThatCouldSolve.Add((int)closest);
        }

        // Check the top and bottom, and the left and right to see if they might match
        if (valuesInDirections[Direction.Up] != null && valuesInDirections[Direction.Up] == valuesInDirections[Direction.Down])
        {
            valuesThatCouldSolve.Add((int)valuesInDirections[Direction.Up]);
        }
        if (valuesInDirections[Direction.Left] != null && valuesInDirections[Direction.Left] == valuesInDirections[Direction.Right])
        {
            valuesThatCouldSolve.Add((int)valuesInDirections[Direction.Left]);
        }

        return valuesThatCouldSolve.Distinct().ToList();
    }

    private void Generate()
    {
        state = new int[config.rows, config.columns];

        // TODO - Prefil rows of two for existing solves

        void fillTile(int row, int column)
        {
            var possibleMatches = GetPotentialSolvesForTile(row, column);
            var availableGemsForTile = new List<int>();
            for (int i = 0; i < config.gemCount; i++)
            {
                if (possibleMatches.Contains(i))
                    continue;

                availableGemsForTile.Add(i);
            }

            if(availableGemsForTile.Count == 0)
            {
                // TODO - Handle this properly
                throw new Exception("Couldn't find a gem to place in " + row + ", " + column);
            }

            state[row, column] = availableGemsForTile[random.Next(availableGemsForTile.Count)];
        }

        for(int row = 0; row < config.rows; row++)
        {
            for (int column = 0; column < config.columns; column++)
            {
                fillTile(row, column);
            }
        }
    }
}