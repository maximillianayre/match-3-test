using System;

public enum GridDirection
{
    Up,
    Down,
    Left,
    Right
}

public static class GridDirectionExtensions
{
    public static GridDirection Opposite(this GridDirection direction)
    {
        switch (direction)
        {
            case GridDirection.Up:
                return GridDirection.Down;
            case GridDirection.Down:
                return GridDirection.Up;
            case GridDirection.Left:
                return GridDirection.Right;
            case GridDirection.Right:
                return GridDirection.Left;
        }
        throw new MissingFieldException("Invalid direction");
    }
}