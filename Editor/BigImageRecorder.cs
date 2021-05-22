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
            var path = Settings.FileNameGenerator.BuildAbsolutePath(session);
            var input = m_Inputs[0] as BigCameraInput;

            for (var row = 0; row < input.InputSettings.RowCount; row++)
            {
                for (var column = 0; column < input.InputSettings.ColumnCount; column++)
                {
                    var sectionPath = AddPathSuffix(path, $"_{row}-{column}");
                    var renderTexture = input.OutputRenderTextures[row, column];
                    var texture = RenderTextureToTexture2D(renderTexture);
                    var bytes = texture.EncodeToPNG();
                    File.WriteAllBytes(sectionPath, bytes);
                }
            }
        }

        static string AddPathSuffix(string path, string suffix)
        {
            var directoryName = Path.GetDirectoryName(path);
            var fileName = Path.GetFileNameWithoutExtension(path);
            var extension = Path.GetExtension(path);
            return Path.Combine(directoryName, fileName + suffix + extension);
        }

        static Texture2D RenderTextureToTexture2D(RenderTexture renderTexture)
        {
            var texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
            RenderTexture.active = renderTexture;
            texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture.Apply();
            return texture;
        }
    }
}
