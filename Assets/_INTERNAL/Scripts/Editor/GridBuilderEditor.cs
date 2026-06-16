#if UNITY_EDITOR
using Field;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace EditorUtils
{
    [CustomEditor(typeof(GridBuilder))]
    public class GridBuilderEditor : Editor
    {
        private GridBuilder _gridBuilder;
        private int _selectedX = -1;
        private int _selectedY = -1;

        private void OnEnable()
        {
            _gridBuilder = (GridBuilder)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Grid Info", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Total Cells", (_gridBuilder.Columns * _gridBuilder.Rows).ToString());

            EditorGUILayout.Space();

            if (GUILayout.Button("Rebuild Grid", GUILayout.Height(30)))
            {
                Undo.RecordObject(_gridBuilder, "Rebuild Grid");
                _gridBuilder.GetType()
                    .GetMethod("RebuildGrid",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.Invoke(_gridBuilder, null);
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

            if (GUILayout.Button("Clear Grid", GUILayout.Height(30)))
            {
                Undo.RecordObject(_gridBuilder, "Clear Grid");
                _gridBuilder.GetType()
                    .GetMethod("ClearGridMenu",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.Invoke(_gridBuilder, null);
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

            if (_selectedX >= 0 && _selectedY >= 0)
            {
                if (_gridBuilder.TryGetCell(_selectedX, _selectedY, out CustomGridCell cell))
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField($"Selected Cell: [{_selectedX}, {_selectedY}]", EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("Occupied", cell.IsOccupied.ToString());
                    EditorGUILayout.LabelField("Position", cell.WorldPosition.ToString());
                }
            }
        }

        private void OnSceneGUI()
        {
            if (_gridBuilder.Cells == null)
                return;

            Handles.BeginGUI();

            for (int x = 0; x < _gridBuilder.Columns; x++)
            {
                for (int y = 0; y < _gridBuilder.Rows; y++)
                {
                    if (_gridBuilder.TryGetCell(x, y, out var cell))
                    {
                        Vector3 screenPos = HandleUtility.WorldToGUIPoint(cell.WorldPosition);

                        Handles.Label(screenPos, $"{x},{y}");

                        Handles.color = cell.IsOccupied ? Color.red : Color.green;
                        Handles.SphereHandleCap(0, cell.WorldPosition, Quaternion.identity, 0.1f, EventType.Repaint);
                    }
                }
            }

            Handles.EndGUI();
        }
    }
}
#endif