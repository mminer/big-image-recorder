using UnityEditor.Recorder;

namespace UnityEditor.BigImageRecorder
{
    /// <summary>
    /// Recorder to capture image sequences that exceed Unity's maximum texture size.
    /// </summary>
    class BigImageRecorder : GenericRecorder<BigImageRecorderSettings>
    {
        protected override void RecordFrame(RecordingSession session)
        {
            var path = Settings.FileNameGenerator.BuildAbsolutePath(session);
            var input = m_Inputs[0] as BigCameraInput;
            // TODO: save images
        }
    }
}
