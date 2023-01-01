using UnityEngine;
using System.Collections.Generic;




namespace Xonix.Grid
{
    using static StaticData;

    public class XonixGrid : MonoBehaviour
    {
        #region [Grid Init Parameters]

        private const int LineCellsCount = 90;
        private const int ColumnCellsCount = 60;

        private const float LineUnitSize = LineCellsCount * CellSize;
        private const float ColumnUnitSize = ColumnCellsCount * CellSize;
        private const int FieldSize = LineCellsCount * ColumnCellsCount;

        private const float EarthInitBorderThickness = 2 * CellSize;
        private static readonly Vector2 EarthInitBorderAligment = new Vector2(EarthInitBorderThickness, EarthInitBorderThickness);

        #endregion

        [SerializeField] private GridNodeSource _seaNodeSource;
        [SerializeField] private GridNodeSource _earthNodeSource;
        [SerializeField] private Camera _mainCamera;

        private readonly Dictionary<Vector2, GridNode> _grid = new Dictionary<Vector2, GridNode>(FieldSize);
        private readonly Dictionary<Vector2, GridNode> _seaGrid = new Dictionary<Vector2, GridNode>();


        private GridNodeFactory _gridNodeFactory;



        public bool TryToGetNode(Vector2 position, out GridNode node)
        {
            return _grid.TryGetValue(position, out node);
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


            float x = 0f;
            float y = 0f;

            for (int i = 0; i < FieldSize; i++, x += CellSize)
            {
                var nodePosition = new Vector2(x, y);
                var currentNode = _gridNodeFactory.CreateGridNode(nodePosition);

                var isInSeaInitArea = seaGridArea.IsPositionInArea(nodePosition);
                var nodeSource = (isInSeaInitArea) ? _seaNodeSource : _earthNodeSource;

                if (isInSeaInitArea)
                    _seaGrid.Add(nodePosition, currentNode);

                currentNode.SetSource(nodeSource);

                _grid.Add(nodePosition, currentNode);

                if (x == LineUnitSize - 1)
                {
                    x = -CellSize;
                    y += CellSize;
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
