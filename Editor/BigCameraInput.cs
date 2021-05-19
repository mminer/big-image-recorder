using System.Linq;
using UnityEditor.Recorder;
using UnityEngine;

namespace UnityEditor.BigImageRecorder
{
    /// <summary>
    /// Input that divides a tagged camera's projection matrix and renders the tiles to individual render textures.
    /// </summary>
    class BigCameraInput : RecorderInput
    {
        public BigCameraInputSettings InputSettings => settings as BigCameraInputSettings;
        public RenderTexture[,] OutputRenderTextures { get; private set; }

        Matrix4x4[,] projectionMatrices;

        protected override void BeginRecording(RecordingSession session)
        {
            base.BeginRecording(session);
            OutputRenderTextures = CreateOutputRenderTextures(InputSettings);
            projectionMatrices = CreateProjectionMatrices(InputSettings);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            foreach (var renderTexture in OutputRenderTextures)
            {
                renderTexture.Release();
            }

            OutputRenderTextures = null;
        }

        protected override void NewFrameReady(RecordingSession session)
        {
            base.NewFrameReady(session);
            var camera = GetTargetCamera(InputSettings.CameraTag);
            var originalTargetTexture = camera.targetTexture;

            for (var i = 0; i < InputSettings.HorizontalTileCount; i++)
            {
                for (var j = 0; j < InputSettings.VerticalTileCount; j++)
                {
                    camera.projectionMatrix = projectionMatrices[i, j];
                    camera.targetTexture = OutputRenderTextures[i, j];
                    camera.Render();
                }
            }

            camera.targetTexture = originalTargetTexture;
            camera.ResetProjectionMatrix();
        }

        static RenderTexture[,] CreateOutputRenderTextures(BigCameraInputSettings inputSettings)
        {
            var outputRenderTextures = new RenderTexture[inputSettings.HorizontalTileCount, inputSettings.VerticalTileCount];
            var width = inputSettings.OutputWidth / inputSettings.HorizontalTileCount;
            var height = inputSettings.OutputHeight / inputSettings.VerticalTileCount;

            for (var i = 0; i < inputSettings.HorizontalTileCount; i++)
            {
                for (var j = 0; j < inputSettings.VerticalTileCount; j++)
                {
                    var renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
                    renderTexture.Create();
                    outputRenderTextures[i, j] = renderTexture;
                }
            }

            return outputRenderTextures;
        }

        static Matrix4x4[,] CreateProjectionMatrices(BigCameraInputSettings inputSettings)
        {
            var projectionMatrices = new Matrix4x4[inputSettings.HorizontalTileCount, inputSettings.VerticalTileCount];
            var camera = GetTargetCamera(inputSettings.CameraTag);

            var top = camera.nearClipPlane * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            var bottom = -top;
            var left = bottom * camera.aspect;
            var right = top * camera.aspect;

            // How much of the final image each tile accounts for.
            var horizontalTilePercent = 1f / inputSettings.HorizontalTileCount;
            var verticalTilePercent = 1f / inputSettings.VerticalTileCount;

            for (var i = 0; i < inputSettings.HorizontalTileCount; i++)
            {
                var tileLeft = left * (1 - 2 * horizontalTilePercent * i);
                var tileRight = right * (-1 + 2 * horizontalTilePercent * (i + 1));

                for (var j = 0; j < inputSettings.VerticalTileCount; j++)
                {
                    var tileTop = top * (1 - 2 * verticalTilePercent * j);
                    var tileBottom = bottom * (-1 + 2 * verticalTilePercent * (j + 1));

                    var projectionMatrix = camera.projectionMatrix;
                    projectionMatrix.m00 = 2 * camera.nearClipPlane / (tileRight - tileLeft);
                    projectionMatrix.m02 = (tileRight + tileLeft) / (tileRight - tileLeft);
                    projectionMatrix.m11 = 2 * camera.nearClipPlane / (tileTop - tileBottom);
                    projectionMatrix.m12 = (tileTop + tileBottom) / (tileTop - tileBottom);
                    projectionMatrices[i, j] = projectionMatrix;
                }
            }

            return projectionMatrices;
        }

        static Camera GetTargetCamera(string cameraTag)
        {
            try
            {
                var cameras = GameObject
                    .FindGameObjectsWithTag(cameraTag)
                    .Select(gameObject => gameObject.GetComponent<Camera>())
                    .Where(camera => camera != null)
                    .ToList();

                if (cameras.Count > 1)
                {
                    Debug.LogWarning($"Found more than one camera with tag '{cameraTag}'.");
                }

                return cameras.FirstOrDefault();
            }
            catch (UnityException)
            {
                Debug.LogWarning($"Tag '{cameraTag}' does not exist.");
                return null;
            }
        }
    }
}
