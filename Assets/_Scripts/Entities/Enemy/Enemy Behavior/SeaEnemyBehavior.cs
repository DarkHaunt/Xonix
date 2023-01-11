using UnityEngine;
using Xonix.Grid;



namespace Xonix.Entities.EnemyComponents
{
    using static GridNodeSource;

    public class SeaEnemyBehavior : EnemyBehaviour
    {
        public SeaEnemyBehavior(XonixGrid grid) : base(NodeState.Sea, grid) { }



        public override Vector2 GetInitPosition() => Grid.GetRandomSeaFieldNodePosition();
        public override string ToString() => "Sea Enemy";
    }
}
