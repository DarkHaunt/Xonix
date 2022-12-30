


namespace UnityEngine
{
    public static class Vector2Extensions
    {
        // These two methods more simple than other degree rotation, so they should be separated for optimization
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
        
        public static Vector2 Rotate(this Vector2 vector2, float degree)
        {
            float sin = Mathf.Sin(degree * Mathf.Deg2Rad);
            float cos = Mathf.Cos(degree * Mathf.Deg2Rad);

            float tx = vector2.x;
            float ty = vector2.y;

            vector2.x = (cos * tx) - (sin * ty);
            vector2.y = (sin * tx) + (cos * ty);

            return vector2;
        }
    }
}
