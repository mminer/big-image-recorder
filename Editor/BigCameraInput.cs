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

            for (var row = 0; row < InputSettings.RowCount; row++)
            {
                for (var column = 0; column < InputSettings.ColumnCount; column++)
                {
                    camera.projectionMatrix = projectionMatrices[row, column];
                    camera.targetTexture = OutputRenderTextures[row, column];
                    camera.Render();
                }
            }

            camera.targetTexture = originalTargetTexture;
            camera.ResetProjectionMatrix();
        }

        static RenderTexture[,] CreateOutputRenderTextures(BigCameraInputSettings inputSettings)
        {
            var outputRenderTextures = new RenderTexture[inputSettings.ColumnCount, inputSettings.RowCount];
            var width = inputSettings.OutputWidth / inputSettings.ColumnCount;
            var height = inputSettings.OutputHeight / inputSettings.RowCount;

            for (var row = 0; row < inputSettings.RowCount; row++)
            {
                for (var column = 0; column < inputSettings.ColumnCount; column++)
                {
                    var renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
                    renderTexture.Create();
                    outputRenderTextures[row, column] = renderTexture;
                }
            }

            return outputRenderTextures;
        }

        static Matrix4x4[,] CreateProjectionMatrices(BigCameraInputSettings inputSettings)
        {
            var projectionMatrices = new Matrix4x4[inputSettings.ColumnCount, inputSettings.RowCount];
            var camera = GetTargetCamera(inputSettings.CameraTag);

            // Values to create the original projection matrix.
            // We multiply these by a modifier from -1 to 1 to get a partial projection matrix.
            //
            // Say we want to split the projection matrix into vertical thirds. To get the three projection matrices:
            // [ | | ]
            // Left:    left * 1;       right * -1/3
            // Center:  left * 1/3;     right * 1/3
            // Right:   left * -1/3;    right * 1
            var top = camera.nearClipPlane * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            var bottom = -top;
            var left = bottom * camera.aspect;
            var right = top * camera.aspect;

            // How much of the final image each tile accounts for.
            var horizontalTilePercent = 1f / inputSettings.ColumnCount;
            var verticalTilePercent = 1f / inputSettings.RowCount;

            for (var row = 0; row < inputSettings.RowCount; row++)
            {
                var tileTop = top * (1 - 2 * verticalTilePercent * row);
                var tileBottom = bottom * (-1 + 2 * verticalTilePercent * (row + 1));

                for (var column = 0; column < inputSettings.ColumnCount; column++)
                {
                    var tileLeft = left * (1 - 2 * horizontalTilePercent * column);
                    var tileRight = right * (-1 + 2 * horizontalTilePercent * (column + 1));

                    var projectionMatrix = camera.projectionMatrix;
                    projectionMatrix.m00 = 2 * camera.nearClipPlane / (tileRight - tileLeft);
                    projectionMatrix.m02 = (tileRight + tileLeft) / (tileRight - tileLeft);
                    projectionMatrix.m11 = 2 * camera.nearClipPlane / (tileTop - tileBottom);
                    projectionMatrix.m12 = (tileTop + tileBottom) / (tileTop - tileBottom);
                    projectionMatrices[row, column] = projectionMatrix;
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
