using UnityEditor.Recorder;
using UnityEngine;

namespace UnityEditor.BigImageRecorder
{
    [CustomEditor(typeof(BigImageRecorderSettings))]
    class BigImageRecorderEditor : RecorderEditor
    {
        static class Styles
        {
            public static readonly GUIContent ArgumentsLabel = new GUIContent("Arguments");
        }

        protected override void FileTypeAndFormatGUI()
        {
            base.FileTypeAndFormatGUI();
            GUILayout.Label("PNG is the only supported image format.");
        }

        protected override void NameAndPathGUI()
        {
            base.NameAndPathGUI();
            var stitchCommand = serializedObject.FindProperty("stitchCommand");
            var stitchCommandArguments = serializedObject.FindProperty("stitchCommandArguments");
            var deleteAfterStitching = serializedObject.FindProperty("deleteAfterStitching");

            EditorGUILayout.PropertyField(stitchCommand);

            using (new EditorGUI.DisabledScope(string.IsNullOrWhiteSpace(stitchCommand.stringValue)))
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(stitchCommandArguments, Styles.ArgumentsLabel);
                }

                EditorGUILayout.PropertyField(deleteAfterStitching);
            }
        }
    }
}
