using System.Collections.Generic;
using UnityEngine;



public static class StaticData
{
    public const float CellSize = 1f;

    public static readonly System.Random Randomizer = new System.Random();

    private static readonly Dictionary<Direction, Vector2> DirectionVector2Values = new Dictionary<Direction, Vector2>(8)
    {
        [Direction.Top] = Vector2.up,
        [Direction.Down] = Vector2.down,
        [Direction.Left] = Vector2.left,
        [Direction.Right] = Vector2.right,

        [Direction.Zero] = Vector2.zero
    };    



    /// <returns>Or 1, or -1</returns>
    public static int GetRandomSign() => (Randomizer.Next(0, 2) * 2) - 1;
    public static Vector2 GetDirectionValue(Direction direction) => DirectionVector2Values[direction];



    // TODO: –азделить направлени€ дл€ врага и игрока
    public enum Direction
    {
        Top,
        Down,
        Left,
        Right,

        Zero
    }
}
