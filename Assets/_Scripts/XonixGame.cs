using System.Collections.Generic;
using System;
using Xonix.Entities;
using Xonix.Grid;
using UnityEngine;

namespace Xonix
{
    public class XonixGame : MonoBehaviour
    {
        public event Action OnGameEnd;

        private static XonixGame _instance;

        [SerializeField] private XonixGrid _grid;
        [SerializeField] private List<Enemy> _enemies;


        public static IList<Enemy> SeaEnemies => _instance._enemies;


        public static bool TryToGetNodeWithPosition(Vector2 position, out GridNode node)
        {
            return _instance._grid.TryToGetNode(position, out node);
        }

        public static void EndGame() => _instance.OnGameEnd?.Invoke();



        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
        }
    }
}
