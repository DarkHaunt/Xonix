using System.Collections.Generic;
using UnityEngine;



public static class StaticData
{
    public const float CellSize = 1f;

    private static readonly Dictionary<Direction, Vector2> DirectionVector2Values = new Dictionary<Direction, Vector2>(8)
    {
        [Direction.Top] = Vector2.up,
        [Direction.Down] = Vector2.down,
        [Direction.Left] = Vector2.left,
        [Direction.Right] = Vector2.right,

        [Direction.TopLeft] = Vector2.up + Vector2.left,
        [Direction.TopRight] = Vector2.up + Vector2.right,
        [Direction.BottomLeft] = Vector2.down + Vector2.left,
        [Direction.BottomRight] = Vector2.down + Vector2.right,

        [Direction.Zero] = Vector2.zero
    };    



    public static Vector2 GetDirectionValue(Direction direction) => DirectionVector2Values[direction];



    // TODO: –азделить направлени€ дл€ врага и игрока
    public enum Direction
    {
        Top,
        Down,
        Left,
        Right,
        
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,

        Zero
    }
}
