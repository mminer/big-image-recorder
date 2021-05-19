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
        protected override void RecordFrame(RecordingSession session)
        {
            var path = Settings.FileNameGenerator.BuildAbsolutePath(session);
            var input = m_Inputs[0] as BigCameraInput;

            for (var i = 0; i < input.InputSettings.HorizontalTileCount; i++)
            {
                for (var j = 0; j < input.InputSettings.VerticalTileCount; j++)
                {
                    var sectionPath = AddPathSuffix(path, $"_{i}-{j}");
                    var texture = RenderTextureToTexture2D(input.OutputRenderTextures[i, j]);
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
