using Grids;
using UnityEditor;
using UnityEngine;
using Grid = Grids.Grid;

namespace Editor
{
    [CustomEditor(typeof(Grid))]
    public class GridInspector : UnityEditor.Editor
    {
        private UnityEngine.Object cellPrefab;


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            cellPrefab = EditorGUILayout.ObjectField("Cell Prefab", cellPrefab, typeof(GridCell), false);
            EditorGUI.BeginDisabledGroup(cellPrefab == null);
            if (GUILayout.Button("Generate Grid"))
            {
                Grid grid = target as Grid;
                Undo.IncrementCurrentGroup();
                Undo.RecordObject(grid, "Update Grid References");
                foreach (var cell in grid.GetComponentsInChildren<GridCell>())
                {
                    if (cell != null)
                    {
                        Undo.DestroyObjectImmediate(cell.gameObject);
                    }
                }

                int i = 0;
                int height = grid.walkableGrid.Length / grid.width;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < grid.width; x++)
                    {
                        var cell = PrefabUtility.InstantiatePrefab(cellPrefab, grid.transform) as GridCell;
                        cell.transform.position = new Vector3(x, y, 0);
                        grid.walkableGrid[i++] = cell;
                        Undo.RegisterCreatedObjectUndo(cell.gameObject, "Create Grid Cell");
                    }
                }

                Undo.SetCurrentGroupName("Generate Grid Cells");
                EditorUtility.SetDirty(grid);
            }

            EditorGUI.EndDisabledGroup();
        }
    }
}