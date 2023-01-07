


namespace UnityEngine
{
    public static class Vector2Extensions
    {
        // These two methods more simple than other degree rotation, so they separated for optimization
        public static Vector2 RotateFor90DegreeCounterClockwise(this Vector2 vector2)
        {
            float ty = vector2.y;

            vector2.y = vector2.x;
            vector2.x = -ty;

            return vector2;
        }

        public static Vector2 RotateFor90DegreeClockwise(this Vector2 vector2)
        {
            float tx = vector2.x;

            vector2.x = vector2.y;
            vector2.y = -tx;

            return vector2;
        }
    }
}
