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
            var columnCount = property.FindPropertyRelative("columnCount");
            var outputHeight = property.FindPropertyRelative("outputHeight");
            var outputWidth = property.FindPropertyRelative("outputWidth");
            var rowCount = property.FindPropertyRelative("rowCount");

            var tileWidth = outputWidth.intValue / columnCount.intValue;
            var tileHeight = outputHeight.intValue / rowCount.intValue;

            using (new EditorGUI.IndentLevelScope(-1))
            {
                EditorGUILayout.PropertyField(cameraTag);
                EditorGUILayout.PropertyField(outputWidth);
                EditorGUILayout.PropertyField(outputHeight);
                EditorGUILayout.PropertyField(rowCount);
                EditorGUILayout.PropertyField(columnCount);
                EditorGUILayout.LabelField("Tile Size", $"{tileWidth} × {tileHeight}");
            }
        }
    }
}
