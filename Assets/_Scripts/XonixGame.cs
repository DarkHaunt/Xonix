using UnityEngine;
using Xonix.Grid;



namespace Xonix
{
    public class XonixGame : MonoBehaviour
    {
        private static XonixGame _instance;

        [SerializeField] private XonixGrid _grid;



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
        }
    }
}
