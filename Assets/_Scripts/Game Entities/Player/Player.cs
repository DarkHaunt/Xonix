using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Xonix.Entities
{
    public class Player : Entity
    {
        public override Vector3 MoveDirection
        {
            get
            {
                #region [TEST INPUT SYSTEM]

                if (Input.GetKey(KeyCode.W))
                    return Vector2.up;

                if (Input.GetKey(KeyCode.S))
                    return Vector2.down;

                if (Input.GetKey(KeyCode.D))
                    return Vector2.right;

                if (Input.GetKey(KeyCode.A))
                    return Vector2.left;

                return Vector2.zero;

                #endregion
            }
        }
    }
}
