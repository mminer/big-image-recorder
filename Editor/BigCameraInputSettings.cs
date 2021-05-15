using System;
using System.Collections.Generic;
using UnityEditor.Recorder.Input;
using UnityEngine;

namespace UnityEditor.BigImageRecorder
{
    [Serializable]
    class BigCameraInputSettings : ImageInputSettings
    {
        protected override Type InputType => typeof(BigCameraInput);

        public string CameraTag
        {
            get => cameraTag;
            set => cameraTag = value;
        }

        [SerializeField] string cameraTag = "MainCamera";

        public override int OutputHeight
        {
            get => outputHeight;
            set => outputHeight = value;
        }

        [SerializeField] int outputHeight = 8096;

        public override int OutputWidth
        {
            get => outputWidth;
            set => outputWidth = value;
        }

        [SerializeField] int outputWidth = 8096;

        protected override bool ValidityCheck(List<string> errors)
        {
            var ok = true;

            if (OutputWidth <= 0 || OutputHeight <= 0)
            {
                errors.Add($"Invalid output resolution: {OutputWidth}x{OutputHeight}");
                ok = false;
            }

            return ok;
        }
    }
}
