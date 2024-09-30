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

    public Grid(GridConfig gridConfig)
    {
        config = gridConfig;
        random = new Random();
        Generate();
    }

    public int[,] GetState() => state;
    public GridConfig GetConfig() => config;

    private bool GetIsInBounds(int row, int column)
    {
        if(row < 0 || row >= config.rows)
            return false;

        if (column < 0 || column >= config.columns)
            return false;

        return true;
    }
    private int? Get(int row, int column)
    {
        return GetIsInBounds(row, column) ? state[row, column]: null;
    }

    private Tuple<int, int> GetIndexInDirection(int row, int column, GridDirection direction, int distance = 1)
    {
        int targetRow = row, targetColumn = column;
        switch (direction)
        {
            case GridDirection.Up:
                targetRow += distance;
                break;
            case GridDirection.Down:
                targetRow -= distance;
                break;
            case GridDirection.Left:
                targetColumn += distance;
                break;
            case GridDirection.Right:
                targetColumn -= distance;
                break;

        }
        return new Tuple<int, int>(targetRow, targetColumn);
    }

    private int? GetInDirection(int row, int column, GridDirection direction, int distance = 1)
    {
        var targetIndex = GetIndexInDirection(row, column, direction, distance);
        return Get(targetIndex.Item1, targetIndex.Item2);
    }

    private bool TrySetupPotentialMatch(int gemIndex, int row, int column)
    {
        bool validateTile(int targetRow, int targetColumn, GridDirection direction)
        {
            var targetIndex = GetIndexInDirection(targetRow, targetColumn, direction);
            if (GetIsInBounds(targetIndex.Item1, targetIndex.Item2) == false)
                return false;

            // We mustn't make a match by placing this one here.
            if (GetSolvingGemsForTile(targetIndex.Item1, targetIndex.Item2).Contains(gemIndex))
                return false;

            return true;
        }

        List<Tuple<int, int>> GetAdjacentTilesThatCanMatch(int targetRow, int targetColumn, GridDirection directionToIgnore)
        {
            var possibleTiles = new List<Tuple<int, int>>();
            foreach (GridDirection direction in Enum.GetValues(typeof(GridDirection)))
            {
                if (direction == directionToIgnore)
                    continue;

                if (validateTile(targetRow, targetColumn, direction) == false)
                    continue;

                possibleTiles.Add(GetIndexInDirection(targetRow, targetColumn, direction));
            }
            return possibleTiles;
        }

        var potentialLayouts = new List<Tuple<int, int>[]>();
        foreach (GridDirection direction in Enum.GetValues(typeof(GridDirection)))
        {
            // If the other direction has a matching tile, we'll do that check separately
            if (gemIndex == GetInDirection(row, column, direction.Opposite()))
                continue;

            if (validateTile(row, column, direction) == false)
                continue;

            // Is the tile empty or matching?
            var gemInDirection = GetInDirection(row, column, direction);
            if (gemInDirection != gemIndex && gemInDirection != null)
                continue;

            // Can either end of the line formed have a matching tile adjacent to it?
            var endA = GetIndexInDirection(row, column, direction, 2);
            var possibleTiles = GetAdjacentTilesThatCanMatch(endA.Item1, endA.Item2, direction.Opposite());

            var endB = GetIndexInDirection(row, column, direction.Opposite());
            possibleTiles.AddRange(GetAdjacentTilesThatCanMatch(endA.Item1, endA.Item2, direction));

            if (possibleTiles.Count == 0)
                continue;

            var currIndex = GetIndexInDirection(row, column, direction);

            foreach(var tile in possibleTiles)
            {
                potentialLayouts.Add(new Tuple<int, int>[] { currIndex, tile });
            }
        }

        if(potentialLayouts.Count == 0)
            return false;

        var chosenLayout = potentialLayouts[random.Next(potentialLayouts.Count)];
        state[chosenLayout[0].Item1, chosenLayout[0].Item2] = gemIndex;
        state[chosenLayout[1].Item1, chosenLayout[1].Item2] = gemIndex;

        return true;
    }

    private List<int> GetSolvingGemsForTile(int row, int column)
    {
        var valuesThatCouldSolve = new List<int>();
        var valuesInDirections = new Dictionary<GridDirection, int?>();
        foreach(GridDirection direction in Enum.GetValues(typeof(GridDirection)))
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
        if (valuesInDirections[GridDirection.Up] != null && valuesInDirections[GridDirection.Up] == valuesInDirections[GridDirection.Down])
        {
            valuesThatCouldSolve.Add((int)valuesInDirections[GridDirection.Up]);
        }
        if (valuesInDirections[GridDirection.Left] != null && valuesInDirections[GridDirection.Left] == valuesInDirections[GridDirection.Right])
        {
            valuesThatCouldSolve.Add((int)valuesInDirections[GridDirection.Left]);
        }

        return valuesThatCouldSolve.Distinct().ToList();
    }

    public void Generate()
    {
        state = new int[config.rows, config.columns];

        void SetupSolve()
        {
            var chosenGem = random.Next(config.gemCount);
            int originalRow = random.Next(config.rows);
            int originalColumn = random.Next(config.columns);

            int currRow = originalRow, currColumn = originalColumn;
            bool foundMatch = TrySetupPotentialMatch(chosenGem, currRow, currColumn);
            while(foundMatch == false)
            {
                // Cycle through the whole grid starting from the random point until we find a match
                currColumn++;
                if(currColumn >= config.columns)
                {
                    currRow++;
                    if (currRow >= config.rows)
                        currRow = 0;

                    currColumn = 0;
                }

                if (currColumn == originalColumn && currRow == originalRow)
                    break;

                foundMatch = TrySetupPotentialMatch(chosenGem, currRow, currColumn);
                // TODO - Consider if we fail to find a match with the given Gem. Unlikely but worth expanding on.
            }
        }

        // Prefil rows of two for existing solves
        for(int i = 0; i < config.startingSolves; i++)
        {
            SetupSolve();
        }

        void fillTile(int row, int column)
        {
            var gemsThatWouldSolve = GetSolvingGemsForTile(row, column);
            var availableGemsForTile = new List<int>();
            for (int i = 0; i < config.gemCount; i++)
            {
                if (gemsThatWouldSolve.Contains(i))
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