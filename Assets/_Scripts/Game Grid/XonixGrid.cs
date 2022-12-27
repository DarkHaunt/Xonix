using UnityEngine;



namespace Xonix.Grid
{
    using static StaticData;

    public class XonixGrid : MonoBehaviour
    {
        private const int LineCellsCount = 30;
        private const int ColumnCellsCount = 20;

        private const float LineUnitSize = LineCellsCount * CellSize;
        private const float ColumnUnitSize = ColumnCellsCount * CellSize;

        private const float EarthInitBorderThickness = 2 * CellSize;

        private static readonly Vector2 EarthInitBorderAligment = new Vector2(EarthInitBorderThickness, EarthInitBorderThickness);


        [SerializeField] private GridNodeSource _seaNodeSource;
        [SerializeField] private GridNodeSource _earthNodeSource;
        [SerializeField] private Camera _mainCamera;


        private readonly GridNode[,] _grid = new GridNode[LineCellsCount, ColumnCellsCount];

        private GridNodeFactory _gridNodeFactory;




        /*        public GridNode GetNode(Vector2 nodePosition)
                {
                    var xIndex = (nodePosition.x == 0) ? 0 : ((int)nodePosition.x) - 1;
                    var yIndex = (nodePosition.y == 0) ? 0 : ((int)nodePosition.y) - 1;


                    return _grid[xIndex, yIndex];
                }*/

        public bool TryToGetNode(Vector2 nodePosition, out GridNode node)
        {
            var xIndex = (nodePosition.x == 0) ? 0 : ((int)nodePosition.x) - 1;
            var yIndex = (nodePosition.y == 0) ? 0 : ((int)nodePosition.y) - 1;

            var isNodeOutOfIndex = yIndex < 0 || (yIndex > (ColumnCellsCount - 1)) || xIndex < 0 || (xIndex > (LineCellsCount - 1));

            if (isNodeOutOfIndex)
            {
                node = null;
                return false;
            }

            node = _grid[xIndex, yIndex];
            return true;
        }


        private void Init()
        {
            _gridNodeFactory = new GridNodeFactory();

            var firstNodePosition = Vector2.zero;

            // Area of grid, which will be filled by sea tiles
            var seaGridArea = new SquareArea
                (
                    leftBottomCornerPosition:
                        firstNodePosition + EarthInitBorderAligment,

                    rightTopCornerPosition:
                        firstNodePosition + new Vector2(LineUnitSize, ColumnUnitSize) - EarthInitBorderAligment
                );


            for (int x = 0; x < LineCellsCount; x++)
            {
                for (int y = 0; y < ColumnCellsCount; y++)
                {
                    var nodePosition = firstNodePosition + (new Vector2(x, y) * CellSize);

                    _grid[x, y] = _gridNodeFactory.CreateGridNode(nodePosition);
                    _grid[x, y].transform.SetParent(transform);

                    var nodeSource = (seaGridArea.IsPositionInArea(nodePosition)) ? _seaNodeSource : _earthNodeSource;
                    _grid[x, y].SetSource(nodeSource);
                }
            }

            _mainCamera.transform.position = new Vector3(LineUnitSize / 2, ColumnUnitSize / 2, -10f) - new Vector3(CellSize / 2, CellSize / 2);
        }



        private void Awake()
        {
            Init();
        }



        /// <summary>
        /// Sub class to convenience sea init area calculation
        /// </summary>
        private class SquareArea
        {
            public readonly Vector2 LeftBottomCornerPosition;
            public readonly Vector2 RightTopCornerPosition;


            public SquareArea(Vector2 leftBottomCornerPosition, Vector2 rightTopCornerPosition)
            {
                LeftBottomCornerPosition = leftBottomCornerPosition;
                RightTopCornerPosition = rightTopCornerPosition;
            }


            public bool IsPositionInArea(Vector2 position)
            {
                return position.x >= LeftBottomCornerPosition.x &&
                    position.x < RightTopCornerPosition.x &&
                    position.y >= LeftBottomCornerPosition.y &&
                    position.y < RightTopCornerPosition.y;
            }

        }
    }
}
