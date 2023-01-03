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

        private const float EarthInitBorderThickness = 4 * CellSize;
        private static readonly Vector2 EarthInitBorderAligment = new Vector2(EarthInitBorderThickness, EarthInitBorderThickness);

        private static readonly Vector2 FirstNodePosition = Vector2.zero;

        #endregion

        [SerializeField] private GridNodeSource _seaNodeSource;
        [SerializeField] private GridNodeSource _earthNodeSource;
        [SerializeField] private Camera _mainCamera;

        private readonly Dictionary<Vector2, GridNode> _grid = new Dictionary<Vector2, GridNode>(FieldSize);
        private readonly HashSet<GridNode> _seaNodes = new HashSet<GridNode>();
        

        private GridNodeFactory _gridNodeFactory;
        private SquareArea _seaFieldInitArea;



        public bool TryToGetNode(Vector2 position, out GridNode node)
        {
            return _grid.TryGetValue(position, out node);
        }

        public Vector2 GetRandomSeaFieldNodePosition()
        {
            var nodes = new GridNode[_seaNodes.Count];
            _seaNodes.CopyTo(nodes);

            return nodes[Randomizer.Next(nodes.Length)].Position;
        }

        public Vector2 GetFieldTopCenterPosition() => FirstNodePosition + new Vector2(Mathf.Round(LineUnitSize / 2), ColumnUnitSize - CellSize);

        public Vector2 GetFieldBottomCenterPosition() => FirstNodePosition + new Vector2(Mathf.Round(LineUnitSize / 2), 0f);

        public void RemoveSeaNodes(IEnumerable<GridNode> nodes)
        {
            _seaNodes.ExceptWith(nodes);
        }

        private void Init()
        {
            _gridNodeFactory = new GridNodeFactory();

            // Area of grid, which will be filled by sea tiles
            _seaFieldInitArea = new SquareArea
                (
                    leftBottomCornerPosition:
                        FirstNodePosition + EarthInitBorderAligment,

                    rightTopCornerPosition:
                        FirstNodePosition + new Vector2(LineUnitSize - 1, ColumnUnitSize - 1) - EarthInitBorderAligment
                );


            float x = 0f;
            float y = 0f;

            for (int i = 0; i < FieldSize; i++, x += CellSize)
            {
                var nodePosition = new Vector2(x, y);
                var currentNode = _gridNodeFactory.CreateGridNode(nodePosition);

                var isInSeaInitArea = _seaFieldInitArea.IsPositionInArea(nodePosition);

                var nodeSource = (isInSeaInitArea) ? _seaNodeSource : _earthNodeSource;
                currentNode.SetSource(nodeSource);

                if (isInSeaInitArea)
                    _seaNodes.Add(currentNode);

                _grid.Add(nodePosition, currentNode);

                currentNode.transform.SetParent(transform);

                if (x == LineUnitSize - 1)
                {
                    x = -CellSize;
                    y += CellSize;
                }
            }
            
            // TODO: ”брать это от сюда
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
                    position.x <= RightTopCornerPosition.x &&
                    position.y >= LeftBottomCornerPosition.y &&
                    position.y <= RightTopCornerPosition.y;
            }

        }
    }
}
