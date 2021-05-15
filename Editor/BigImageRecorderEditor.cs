using UnityEditor.Recorder;
using UnityEngine;

namespace UnityEditor.BigImageRecorder
{
    [CustomEditor(typeof(BigImageRecorderSettings))]
    class BigImageRecorderEditor : RecorderEditor
    {
        protected override void FileTypeAndFormatGUI()
        {
            base.FileTypeAndFormatGUI();
            GUILayout.Label("PNG is the only supported image format.");
        }
    }
}
