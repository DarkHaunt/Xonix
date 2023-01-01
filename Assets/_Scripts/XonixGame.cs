using System.Collections.Generic;
using System;
using Xonix.Entities;
using Xonix.Grid;
using UnityEngine;



namespace Xonix
{
    public class XonixGame : MonoBehaviour
    {
        private static XonixGame _instance;

        public event Action OnGameEnd;

        [SerializeField] private XonixGrid _grid;
        [SerializeField] private List<Enemy> _enemies;



        public static IList<Enemy> SeaEnemies => _instance._enemies;


        /// <summary>
        /// Tries to get grid node with parameter position
        /// </summary>
        /// <param name="position"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public static bool TryToGetNodeWithPosition(Vector2 position, out GridNode node)
        {
            return _instance._grid.TryToGetNode(position, out node);
        }
        // TODO: Должен быть не конец игры, а проигрыш игрока на текущем уровне
        public static void EndGame() => _instance.OnGameEnd?.Invoke();

        private void PlayerLose()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif

#if UNITY_ANDROID_API
            Application.Quit();
#endif
        }



        private void Awake()
        {
            #region [Singleton]

            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this; 

            #endregion

            _instance.OnGameEnd += PlayerLose;
        }
    }
}
