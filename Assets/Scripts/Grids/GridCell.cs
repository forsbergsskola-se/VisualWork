using System;
using UnityEngine;

namespace Grids
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class GridCell : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;
        [SerializeField] private CellType cellType;
        public enum CellType
        {
            Ground,
            Wall,
            Water,
        }

        public bool Walkable => cellType != CellType.Wall;

        public int Costs
        {
            get
            {
                return cellType switch
                {
                    CellType.Ground => 1,
                    CellType.Wall => int.MaxValue,
                    CellType.Water => 2,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        void Start()
        {
            OnValidate();
        }

        public override string ToString()
        {
            return $"{nameof(GridCell)} ({Mathf.FloorToInt(transform.position.x)}|{Mathf.FloorToInt(transform.position.y)})";
        }

        private void OnValidate()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.color = cellType switch
            {
                CellType.Ground => Color.white,
                CellType.Wall => Color.black,
                CellType.Water => Color.blue,
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
    }
}