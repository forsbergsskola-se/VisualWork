using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Grids
{
    public class Grid : MonoBehaviour
    {
        public GridCell[] walkableGrid = new GridCell[100];
        public int width = 10;

        public int Height => walkableGrid.Length / width;
        
        public bool IsWalkable(int x, int y)
        {
            return walkableGrid[y*width+x].Walkable;
        }

        private static Vector2Int GetCellIndexForPosition(Vector3 position)
        {
            return new Vector2Int(
                Mathf.FloorToInt(position.x + 0.5f),
                Mathf.FloorToInt(position.y + 0.5f)
            );
        }

        public GridCell GetCellForPosition(Vector3 position)
        {
            var index = GetCellIndexForPosition(position);
            return walkableGrid[index.x + index.y * width];
        }

        public GridCell GetCellForIndex(Vector2Int index)
        {
            return walkableGrid[index.y * width + index.x];
        }

        bool IsValidAndWalkable(Vector2Int index)
        {
            if (index.x < 0) return false;
            if (index.x >= width) return false;
            if (index.y < 0) return false;
            if (index.y >= Height) return false;
            return IsWalkable(index.x, index.y);
        }

        public IEnumerable<Vector2Int> GetWalkDrections()
        {
            yield return Vector2Int.up; // prio 1
            yield return Vector2Int.right; // prio 2
            yield return Vector2Int.down; // prio 3
            yield return Vector2Int.left; // prio 4
        }

        public IEnumerable<GridCell> GetWalkableNeighborsForCell(GridCell cell)
        {
            var cellIndex = GetCellIndexForPosition(cell.transform.position);
            foreach (var direction in GetWalkDrections())
            {
                if (IsValidAndWalkable(cellIndex + direction))
                    yield return GetCellForIndex(cellIndex + direction);
            }
        }
    }
}
