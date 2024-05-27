using System;
using Grids;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Grid = Grids.Grid;
using Object = UnityEngine.Object;

namespace Tests
{
    public class GridTests
    {
        private Grid grid;

        [SetUp]
        public void SetUp()
        {
            var gridGameObject = new GameObject("Grid");
            grid = gridGameObject.AddComponent<Grid>();
            grid.width = 3;
            int height = 3;
            grid.walkableGrid = new GridCell[9];
            int i = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < grid.width; x++)
                {
                    var cellGameObject = new GameObject("GridCell");
                    cellGameObject.transform.SetParent(grid.transform);
                    cellGameObject.transform.position = new Vector3(x, y, 0);
                    cellGameObject.AddComponent<SpriteRenderer>();
                    var cell = cellGameObject.AddComponent<GridCell>();
                    grid.walkableGrid[i++] = cell;
                }
            }
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(grid);
            grid = null;
        }
        
        [Test]
        public void GetCellForPositionReturnsCorrectCell()
        {
            var cell = grid.GetCellForPosition(new Vector3(0, 0, 0));
            Assert.That(cell, Is.EqualTo(grid.walkableGrid[0]));
            
            cell = grid.GetCellForPosition(new Vector3(.4f, .4f, 0));
            Assert.That(cell, Is.EqualTo(grid.walkableGrid[0]));
            
            cell = grid.GetCellForPosition(new Vector3(-.4f, -.4f, 0));
            Assert.That(cell, Is.EqualTo(grid.walkableGrid[0]));
            
            cell = grid.GetCellForPosition(new Vector3(.5f, .5f, 0));
            Assert.That(cell, Is.EqualTo(grid.walkableGrid[4]));
            
            cell = grid.GetCellForPosition(new Vector3(1, 2, 0));
            Assert.That(cell, Is.EqualTo(grid.walkableGrid[7]));
            
            cell = grid.GetCellForPosition(new Vector3(1.4f, .4f, 0));
            Assert.That(cell, Is.EqualTo(grid.walkableGrid[1]));
            
            Assert.Throws<IndexOutOfRangeException>(() => grid.GetCellForPosition(new Vector3(-1, 0, 0)));
        }
    }
}