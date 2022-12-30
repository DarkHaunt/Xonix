using System.Collections.Generic;
using System;
using Xonix.Grid;
using Xonix.Entities.Enemies;
using UnityEngine;



namespace Xonix.Entities
{
    using static StaticData;
    using static GridNodeSource;

    public class Enemy : Entity
    {
        private const int DirectionsCount = 4;
        private static readonly System.Random _randomizer = new System.Random();


        [SerializeField] private EnemyBehaviour _enemyBehaviour;
        //private EnemyWalkDirection _currentDirection;

        private Vector2 _currentDirection;




        public override void Init()
        {
            // _currentDirection = (EnemyWalkDirection)_randomizer.Next(1, DirectionsCount);

            _currentDirection = new Vector2(-1f, -1f);
        }

        protected override void Move()
        {
            var nextNodePosition = Position + _currentDirection * CellSize;

            if (!XonixGame.TryToGetNodeWithPosition(nextNodePosition, out GridNode nextNode) || _enemyBehaviour.IsNodeHasBorderState(nextNode))
                _enemyBehaviour.TryToBounceFromNode(nextNodePosition, _currentDirection, out _currentDirection);

            transform.position += (Vector3)_currentDirection * CellSize;
            //transform.Translate(newDirection * CellSize);
        }



        public enum EnemyWalkDirection
        {
            TopLeft = 1,
            TopRight = 2,
            BottomLeft = 3,
            BottomRight = 4
        }
    }
}
