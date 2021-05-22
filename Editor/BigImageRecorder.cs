using System.Collections.Generic;
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
            var paths = WriteImageTiles(session);
            RunStitchCommand(session, paths);
        }

        void RunStitchCommand(RecordingSession session, IReadOnlyList<string> paths)
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

        IReadOnlyList<string> WriteImageTiles(RecordingSession session)
        {
            var paths = new List<string>();

            for (RowBeingWritten = 0; RowBeingWritten < Input.InputSettings.Rows; RowBeingWritten++)
            {
                for (ColumnBeingWritten = 0; ColumnBeingWritten < Input.InputSettings.Columns; ColumnBeingWritten++)
                {
                    var path = Settings.FileNameGenerator.BuildAbsolutePath(session);
                    paths.Add(path);

                    var renderTexture = Input.OutputRenderTextures[RowBeingWritten, ColumnBeingWritten];
                    var texture = ConvertToTexture2D(renderTexture);
                    var bytes = texture.EncodeToPNG();

                    File.WriteAllBytes(path, bytes);
                }
            }

            return paths;
        }

        static string ApplyWildcards(RecorderSettings settings, RecordingSession session, string str)
        {
            // Danger zone: we use reflection to call a private API, which could break with a new version of Recorder.
            var applyWildcardsMethod = settings.FileNameGenerator
                .GetType()
                .GetMethod("ApplyWildcards", BindingFlags.Instance | BindingFlags.NonPublic);

            return applyWildcardsMethod.Invoke(settings.FileNameGenerator, new object[] {str, session}) as string;
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
