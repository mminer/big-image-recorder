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

        public int HorizontalTileCount
        {
            get => horizontalTileCount;
            set => horizontalTileCount = value;
        }

        [SerializeField] int horizontalTileCount = 2;

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

        public int VerticalTileCount
        {
            get => verticalTileCount;
            set => verticalTileCount = value;
        }

        [SerializeField] int verticalTileCount= 2;

        protected override bool ValidityCheck(List<string> errors)
        {
            var ok = true;

            if (HorizontalTileCount < 1 || VerticalTileCount < 1)
            {
                errors.Add($"Need a minimum of 1 horizontal and vertical tile.");
                ok = false;
            }

            if (OutputWidth % HorizontalTileCount != 0)
            {
                errors.Add($"Output width must be a multiple of the horizontal tile count.");
                ok = false;
            }

            if (OutputHeight % VerticalTileCount != 0)
            {
                errors.Add($"Output height must be a multiple of the vertical tile count.");
                ok = false;
            }

            if (OutputWidth <= 0 || OutputHeight <= 0)
            {
                errors.Add($"Invalid output resolution: {OutputWidth}x{OutputHeight}");
                ok = false;
            }

            return ok;
        }
    }
}
