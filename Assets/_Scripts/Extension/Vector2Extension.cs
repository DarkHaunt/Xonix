using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace UnityEngine
{
    public static class Vector2Extension
    {
        public static Vector2 RotateVector2CounterClockwise(this Vector2 vector2)
        {
/*            float y = vector2.y;

            vector2.y = vector2.x;
            vector2.x = -y;*/

            return new Vector2(-vector2.y, vector2.x);
        }

        public static Vector2 RotateVector2Clockwise(this Vector2 vector2)
        {
/*            float x = vector2.x;

            vector2.x = vector2.y;
            vector2.y = -x;*/


            return new Vector2(vector2.y, -vector2.x);
        }
    }
}
