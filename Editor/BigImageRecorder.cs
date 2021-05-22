using System.Collections.Generic;
using System.IO;
using UnityEditor.Recorder;
using UnityEngine;

namespace UnityEditor.BigImageRecorder
{
    /// <summary>
    /// Recorder to capture image sequences that exceed Unity's maximum texture size.
    /// </summary>
    class BigImageRecorder : GenericRecorder<BigImageRecorderSettings>
    {
        // These are exposed so that the filename generator in the settings can read what tile we're currently writing.
        public int columnBeingWritten { get; private set; }
        public int rowBeingWritten { get; private set; }

        protected override bool BeginRecording(RecordingSession session)
        {
            if (!base.BeginRecording(session))
            {
                return false;
            }

            Settings.FileNameGenerator.CreateDirectory(session);
            return true;
        }

        protected override void RecordFrame(RecordingSession session)
        {
            WriteImageTiles(session);
        }

        IReadOnlyList<string> WriteImageTiles(RecordingSession session)
        {
            var paths = new List<string>();
            var input = m_Inputs[0] as BigCameraInput;

            for (rowBeingWritten = 0; rowBeingWritten < input.InputSettings.RowCount; rowBeingWritten++)
            {
                for (columnBeingWritten = 0; columnBeingWritten < input.InputSettings.ColumnCount; columnBeingWritten++)
                {
                    var path = Settings.FileNameGenerator.BuildAbsolutePath(session);
                    paths.Add(path);

                    var renderTexture = input.OutputRenderTextures[rowBeingWritten, columnBeingWritten];
                    var texture = ConvertToTexture2D(renderTexture);
                    var bytes = texture.EncodeToPNG();

                    File.WriteAllBytes(path, bytes);
                }
            }

            return paths;
        }

        static Texture2D ConvertToTexture2D(RenderTexture renderTexture)
        {
            var texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
            RenderTexture.active = renderTexture;
            texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture.Apply();
            return texture;
        }
    }
}
