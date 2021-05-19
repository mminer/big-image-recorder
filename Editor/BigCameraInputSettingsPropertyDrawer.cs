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
            var horizontalTileCount = property.FindPropertyRelative("horizontalTileCount");
            var outputHeight = property.FindPropertyRelative("outputHeight");
            var outputWidth = property.FindPropertyRelative("outputWidth");
            var verticalTileCount = property.FindPropertyRelative("verticalTileCount");

            var tileWidth = outputWidth.intValue / horizontalTileCount.intValue;
            var tileHeight = outputHeight.intValue / verticalTileCount.intValue;

            using (new EditorGUI.IndentLevelScope(-1))
            {
                EditorGUILayout.PropertyField(cameraTag);
                EditorGUILayout.PropertyField(outputWidth);
                EditorGUILayout.PropertyField(outputHeight);
                EditorGUILayout.PropertyField(horizontalTileCount);
                EditorGUILayout.PropertyField(verticalTileCount);
                EditorGUILayout.LabelField("Tile Size", $"{tileWidth} × {tileHeight}");
            }
        }
    }
}
