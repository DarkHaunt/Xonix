using System.Collections.Generic;
using UnityEngine;



public static class StaticData
{
    public const float CellSize = 1f;

    private static readonly Dictionary<Direction, Vector2> DirectionVector2Values = new Dictionary<Direction, Vector2>(5)
    {
        [Direction.Top] = Vector2.up,
        [Direction.Down] = Vector2.down,
        [Direction.Left] = Vector2.left,
        [Direction.Right] = Vector2.right,
        [Direction.Zero] = Vector2.zero
    };

    public static readonly Dictionary<Direction, DirectionPerpendiculalDirection> DirectionPerpendiculars = new Dictionary<Direction, DirectionPerpendiculalDirection>(4)
    {
        [Direction.Top] = new DirectionPerpendiculalDirection(GetDirectionValue(Direction.Left), GetDirectionValue(Direction.Right)),
        [Direction.Down] = new DirectionPerpendiculalDirection(GetDirectionValue(Direction.Left), GetDirectionValue(Direction.Right)),
        [Direction.Left] = new DirectionPerpendiculalDirection(GetDirectionValue(Direction.Down), GetDirectionValue(Direction.Top)),
        [Direction.Right] = new DirectionPerpendiculalDirection(GetDirectionValue(Direction.Down), GetDirectionValue(Direction.Top)),
    };


    public static Vector2 GetDirectionValue(Direction direction) => DirectionVector2Values[direction];

    public static DirectionPerpendiculalDirection GetDirectionPerpendicularDirections(Direction direction) => DirectionPerpendiculars[direction];



    public struct DirectionPerpendiculalDirection
    {
        public readonly Vector2 FirstDirectionPerpendicular;
        public readonly Vector2 SecondDirectionPerpendicular;


        public DirectionPerpendiculalDirection(Vector2 firstPerpendicularDirection, Vector2 secondPerpendicularDirection)
        {
            FirstDirectionPerpendicular = firstPerpendicularDirection;
            SecondDirectionPerpendicular = secondPerpendicularDirection;
        }


        public override string ToString() => $"\nFirst Direction - {FirstDirectionPerpendicular}\nSecond Direction - {SecondDirectionPerpendicular}";
    }

    public enum Direction
    {
        Top,
        Down,
        Left,
        Right,
        Zero
    }
}
