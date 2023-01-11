using System.Collections.Generic;
using System;
using UnityEngine;
using Xonix.LevelHandling;



namespace Xonix.Grid
{
    using static StaticRandomizer;

    /// <summary>
    /// A field of nodes
    /// </summary>
    public class XonixGrid : MonoBehaviour
    {
        #region [Grid Init Parameters]

        public const float CellSize = 1f;

        private const int LineCellsCount = 100;
        private const int ColumnCellsCount = 60;
        private const int InitEarthBorderWigthNodesCount = 4;
        private const int FullFieldSize = LineCellsCount * ColumnCellsCount;

        private const float LineUnitSize = LineCellsCount * CellSize;
        private const float ColumnUnitSize = ColumnCellsCount * CellSize;
        private const float EarthInitBorderThickness = InitEarthBorderWigthNodesCount * CellSize;

        private static readonly Vector2 EarthInitBorderAligment = new Vector2(EarthInitBorderThickness, EarthInitBorderThickness);
        private static readonly Vector2 FirstNodePosition = Vector2.zero;

        #endregion

        public event Action<float> OnSeaNodesPercentChange;

        [SerializeField] private GridNodeSource _seaNodeSource;
        [SerializeField] private GridNodeSource _earthNodeSource;

        private readonly Dictionary<Vector2, GridNode> _grid = new Dictionary<Vector2, GridNode>(FullFieldSize);
        private readonly HashSet<GridNode> _seaNodes = new HashSet<GridNode>();

        private GridNodeFactory _gridNodeFactory;

        private RectangularArea _seaNodesFieldArea;

        private int _initSeaNodesCount = 0; // Count of sea nodes at the intialization



        private void Init()
        {
            _gridNodeFactory = new GridNodeFactory();

            // A rectangular area of grid, which will be filled by sea tiles
            _seaNodesFieldArea = new RectangularArea
                (
                    leftBottomCornerPosition:
                        FirstNodePosition + EarthInitBorderAligment,

                    rightTopCornerPosition:
                        FirstNodePosition + new Vector2(LineUnitSize - 1, ColumnUnitSize - 1) - EarthInitBorderAligment
                );

            int index = 0;
            for (float x = 0f, y = 0f; index < FullFieldSize; index++, x += CellSize)
            {
                var nodePosition = new Vector2(x, y);
                var currentNode = _gridNodeFactory.CreateGridNode(nodePosition);

                var isInSeaInitArea = _seaNodesFieldArea.IsPositionInArea(nodePosition);

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

            _initSeaNodesCount = _seaNodes.Count;
        }

        public void RemoveSeaNodes(IEnumerable<GridNode> seaNodes)
        {
            _seaNodes.ExceptWith(seaNodes);

            OnSeaNodesPercentChange?.Invoke(1f - ((float)_seaNodes.Count / _initSeaNodesCount));
        }

        public Vector2 GetFieldTopCenterPosition() => FirstNodePosition + new Vector2(Mathf.Round(LineUnitSize / 2), ColumnUnitSize - CellSize);

        public Vector2 GetFieldBottomCenterPosition() => FirstNodePosition + new Vector2(Mathf.Round(LineUnitSize / 2), 0f);

        public Vector2 GetGridCenter() => new Vector2(LineUnitSize / 2, ColumnUnitSize / 2) - new Vector2(CellSize / 2, CellSize / 2);

        public Vector2 GetRandomSeaFieldNodePosition()
        {
            var randomX = CellSize * Randomizer.Next(InitEarthBorderWigthNodesCount, LineCellsCount - InitEarthBorderWigthNodesCount - 1);
            var randomY = CellSize * Randomizer.Next(InitEarthBorderWigthNodesCount, ColumnCellsCount - InitEarthBorderWigthNodesCount -1);

            return FirstNodePosition + new Vector2(randomX, randomY);
        }

        public bool TryToGetNodeWithPosition(Vector2 position, out GridNode node)
        {
            return _grid.TryGetValue(position, out node);
        }

        private void ResetSeaField()
        {
            var currentNodePosition = _seaNodesFieldArea.LeftBottomCornerPosition;

            while (_seaNodesFieldArea.IsPositionInArea(currentNodePosition))
            {
                var currentNode = _grid[currentNodePosition];

                if (currentNode.State != _seaNodeSource.State)
                {
                    _seaNodes.Add(currentNode);
                    currentNode.SetSource(_seaNodeSource);
                }

                if (currentNodePosition.x == (LineUnitSize - EarthInitBorderThickness - 1))
                {
                    currentNodePosition.x = EarthInitBorderThickness - CellSize;
                    currentNodePosition.y += CellSize;
                }

                currentNodePosition.x += CellSize;
            }

            OnSeaNodesPercentChange?.Invoke(1f - ((float)_seaNodes.Count / _initSeaNodesCount));
        }



        private void Awake()
        {
            Init();
        }

        private void OnEnable()
        {
            LevelHandler.OnLevelCompleted += ResetSeaField;
        }

        private void OnDisable()
        {
            LevelHandler.OnLevelCompleted -= ResetSeaField;
        }



        /// <summary>
        /// Class, that simplifies border check for rectangles
        /// </summary>
        private class RectangularArea
        {
            public readonly Vector2 LeftBottomCornerPosition;
            public readonly Vector2 RightTopCornerPosition;


            public RectangularArea(Vector2 leftBottomCornerPosition, Vector2 rightTopCornerPosition)
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
