using UnityEngine;

namespace UnityEditor.BigImageRecorder
{
    [CustomPropertyDrawer(typeof(BigCameraInputSettings))]
    class BigCameraInputSettingsPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Removes gap above controls.
            return 0;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var cameraTag = property.FindPropertyRelative("cameraTag");
            var columns = property.FindPropertyRelative("columns");
            var outputHeight = property.FindPropertyRelative("outputHeight");
            var outputWidth = property.FindPropertyRelative("outputWidth");
            var rows = property.FindPropertyRelative("rows");


            using (new EditorGUI.IndentLevelScope(-1))
            {
                EditorGUILayout.PropertyField(cameraTag);
                EditorGUILayout.PropertyField(outputWidth);
                EditorGUILayout.PropertyField(outputHeight);
                EditorGUILayout.PropertyField(rows);
                EditorGUILayout.PropertyField(columns);

                if (rows.intValue > 0 && columns.intValue > 0)
                {
                    var tileWidth = outputWidth.intValue / columns.intValue;
                    var tileHeight = outputHeight.intValue / rows.intValue;
                    EditorGUILayout.LabelField("Tile Size", $"{tileWidth} × {tileHeight}");
                }
                else
                {
                    EditorGUILayout.LabelField("Tile Size", $"");
                }
            }
        }
    }
}
