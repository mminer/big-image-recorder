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
        BigCameraInputSettings InputSettings => settings as BigCameraInputSettings;

        protected override void BeginRecording(RecordingSession session)
        {
            base.BeginRecording(session);
            // TODO: create render textures
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            // TODO: throw out render textures
        }

        protected override void NewFrameReady(RecordingSession session)
        {
            base.NewFrameReady(session);
            var camera = GetTargetCamera(InputSettings.CameraTag);

            // TODO:
            // save current projection matrix
            // save current render target
            // change projection matrix
            // change render target
            // render camera
            // reset render target
            // reset projection matrix
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
