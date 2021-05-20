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

        public int ColumnCount
        {
            get => columnCount;
            set => columnCount = value;
        }

        [SerializeField] int columnCount = 2;

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

        public int RowCount
        {
            get => rowCount;
            set => rowCount = value;
        }

        [SerializeField] int rowCount = 2;

        protected override bool ValidityCheck(List<string> errors)
        {
            var ok = true;

            if (ColumnCount < 1 || RowCount < 1)
            {
                errors.Add($"Need a minimum of 1 horizontal and vertical tile.");
                ok = false;
            }

            if (OutputWidth % ColumnCount != 0)
            {
                errors.Add($"Output width must be a multiple of the column count.");
                ok = false;
            }

            if (OutputHeight % RowCount != 0)
            {
                errors.Add($"Output height must be a multiple of the row count.");
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
