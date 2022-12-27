using UnityEngine;



namespace Xonix.Grid
{
    public class Grid : MonoBehaviour
    {
        private const float CellSize = 1f;
        private const int ColumnCellsCount = 20;
        private const int LineCellsCount = 30;

        private const float LineUnitSize = LineCellsCount * CellSize;
        private const float ColumnUnitSize = ColumnCellsCount * CellSize;

        private const float EarthInitBorderThickness = 2 * CellSize;

        private static readonly Vector2 EarthInitBorderAligment = new Vector2(EarthInitBorderThickness, EarthInitBorderThickness);


        [SerializeField] private GridNodeSource _seaNodeSource;
        [SerializeField] private GridNodeSource _earthNodeSource;
        [SerializeField] private Camera _mainCamera;


        private readonly GridNode[,] _grid = new GridNode[LineCellsCount, ColumnCellsCount];

        private GridNodeFactory _gridNodeFactory;



        private void Init()
        {
            _gridNodeFactory = new GridNodeFactory();

            var firstNodePosition = new Vector2(transform.position.x - LineUnitSize / 2,
             transform.position.y - ColumnUnitSize / 2);

            var seaGridSpawnArea = new Area
                (
                    firstNodePosition + EarthInitBorderAligment,
                    firstNodePosition + new Vector2(LineUnitSize, ColumnUnitSize) - EarthInitBorderAligment
                ); 


            for (int x = 0; x < LineCellsCount; x++)
            {
                for (int y = 0; y < ColumnCellsCount; y++)
                {
                    var nodePosition = firstNodePosition + (new Vector2(x, y) * CellSize);

                    _grid[x, y] = _gridNodeFactory.CreateGridNode(nodePosition);
                    _grid[x, y].transform.SetParent(transform);

                    var nodeSource = (seaGridSpawnArea.IsNodeInSeaSpawnDiapasone(nodePosition)) ? _seaNodeSource : _earthNodeSource;
                    _grid[x, y].SetSource(nodeSource);
                }
            }

            _mainCamera.transform.position -= new Vector3(CellSize / 2, CellSize / 2);
        }



        private void Awake()
        {
            Init();
        }



        /// <summary>
        /// Sub class to convenience sea init area calculation
        /// </summary>
        private class Area
        {
            public readonly Vector2 LeftBottomCornerPosition;
            public readonly Vector2 RightTopCornerPosition;


            public Area(Vector2 leftBottomCornerPosition, Vector2 rightTopCornerPosition)
            {
                LeftBottomCornerPosition = leftBottomCornerPosition;
                RightTopCornerPosition = rightTopCornerPosition;
            }


            public bool IsNodeInSeaSpawnDiapasone(Vector2 position) => position.x >= LeftBottomCornerPosition.x && position.x < RightTopCornerPosition.x && position.y >= LeftBottomCornerPosition.y && position.y < RightTopCornerPosition.y;
        }
    }
}
