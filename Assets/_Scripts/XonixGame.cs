using UnityEngine;
using System.Collections.Generic;
using Xonix.Grid;
using Xonix.Entities;



namespace Xonix
{
    public class XonixGame : MonoBehaviour
    {
        private static XonixGame _instance;

        [SerializeField] private XonixGrid _grid;
        [SerializeField] private List<Enemy> _enemies;


        public static IList<Enemy> SeaEnemies => _instance._enemies;


        public static bool TryToGetNodeWithPosition(Vector2 position, out GridNode node)
        {
            return _instance._grid.TryToGetNode(position, out node);
        }



        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;

/*            var enemy = new GameObject("TestEnemy").AddComponent<Enemy>();
            enemy.transform.position = new Vector3(3f, 3f, -1f);
            _enemies.Add(enemy);*/
        }
    }
}
