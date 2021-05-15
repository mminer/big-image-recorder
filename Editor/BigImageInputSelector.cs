using System;
using UnityEditor.Recorder;
using UnityEngine;

namespace UnityEditor.BigImageRecorder
{
    /// <summary>
    /// Input settings specialization with a camera input as the only option.
    /// </summary>
    [Serializable]
    class BigImageInputSelector : InputSettingsSelector
    {
        [SerializeField] BigCameraInputSettings cameraInputSettings = new BigCameraInputSettings();
    }
}
