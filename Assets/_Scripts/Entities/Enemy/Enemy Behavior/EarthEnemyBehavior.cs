using UnityEngine;
using Xonix.Grid;



namespace Xonix.Entities.EnemyComponents
{
    using static GridNodeSource;

    public class EarthEnemyBehavior : EnemyBehaviour
    {
        public EarthEnemyBehavior(XonixGrid grid) : base(NodeState.Sea, grid) { }



        public override Vector2 GetInitPosition() => Grid.GetFieldBottomCenterPosition();

        public override string ToString() => "Earth Enemy";
    } 
}
