using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Xonix.Entities
{
    using static StaticData;

    public class Enemy : Entity
    {
        public override Direction MoveDirection => Direction.Zero;


        public override void Init()
        {
            //throw new System.NotImplementedException();
        }
    }
}
