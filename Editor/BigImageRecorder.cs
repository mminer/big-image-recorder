using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityEditor.Recorder;
using UnityEngine;

namespace UnityEditor.BigImageRecorder
{
    /// <summary>
    /// Recorder to capture image sequences that exceed Unity's maximum texture size.
    /// </summary>
    class BigImageRecorder : GenericRecorder<BigImageRecorderSettings>
    {
        public BigCameraInput Input => m_Inputs[0] as BigCameraInput;

        // These are exposed so that the filename generator in the settings can read what tile we're currently writing.
        public int ColumnBeingWritten { get; private set; }
        public int RowBeingWritten { get; private set; }

        string[] paths;
        Texture2D tileTexture;

        protected override bool BeginRecording(RecordingSession session)
        {
            if (!base.BeginRecording(session) || !Input.HasCamera)
            {
                return false;
            }

            paths = new string[Input.InputSettings.Rows * Input.InputSettings.Columns];
            tileTexture = new Texture2D(Input.InputSettings.TileWidth, Input.InputSettings.TileHeight, TextureFormat.RGBA32, false);

            Settings.FileNameGenerator.CreateDirectory(session);
            return true;
        }

        protected override void RecordFrame(RecordingSession session)
        {
            WriteImageTiles(session);
            RunStitchCommand(session);
        }

        protected override void EndRecording(RecordingSession session)
        {
            base.EndRecording(session);
            paths = null;

            Destroy(tileTexture);
            tileTexture = null;
        }

        void RunStitchCommand(RecordingSession session)
        {
            if (string.IsNullOrWhiteSpace(Settings.StitchCommand))
            {
                return;
            }

            var arguments = ApplyWildcards(Settings, session, Settings.StitchCommandArguments);

            var processInfo = new ProcessStartInfo(Settings.StitchCommand, arguments)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                WorkingDirectory = Path.GetDirectoryName(paths[0]) ?? string.Empty,
            };

            using var process = Process.Start(processInfo);
            process?.WaitForExit();

            if (Settings.DeleteAfterStitching)
            {
                foreach (var path in paths)
                {
                    File.Delete(path);
                }
            }
        }

        void WriteImageTiles(RecordingSession session)
        {
            for (RowBeingWritten = 0; RowBeingWritten < Input.InputSettings.Rows; RowBeingWritten++)
            {
                for (ColumnBeingWritten = 0; ColumnBeingWritten < Input.InputSettings.Columns; ColumnBeingWritten++)
                {
                    var path = Settings.FileNameGenerator.BuildAbsolutePath(session);
                    paths[RowBeingWritten * Input.InputSettings.Columns + ColumnBeingWritten] = path;

                    // Convert render texture to Texture2D.
                    var renderTexture = Input.OutputRenderTextures[RowBeingWritten, ColumnBeingWritten];
                    RenderTexture.active = renderTexture;
                    tileTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
                    tileTexture.Apply();

                    var bytes = tileTexture.EncodeToPNG();
                    File.WriteAllBytes(path, bytes);
                }
            }
        }

        static string ApplyWildcards(RecorderSettings settings, RecordingSession session, string str)
        {
            // Danger zone: we use reflection to call a private API, which could break with a new version of Recorder.
            var applyWildcardsMethod = settings.FileNameGenerator
                .GetType()
                .GetMethod("ApplyWildcards", BindingFlags.Instance | BindingFlags.NonPublic);

            UnityEngine.Debug.Assert(applyWildcardsMethod != null);
            return applyWildcardsMethod.Invoke(settings.FileNameGenerator, new object[] {str, session}) as string;
        }
    }
}
