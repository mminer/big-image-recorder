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

        public string CameraTag => cameraTag;

        [SerializeField] string cameraTag = "MainCamera";

        public int ColumnCount => columnCount;

        [Tooltip("Number of horizontal tiles.")]
        [SerializeField] int columnCount = 2;

        public override int OutputHeight
        {
            get => outputHeight;
            set => outputHeight = value;
        }

        [Tooltip("Vertical resolution of final image once stitched together.")]
        [SerializeField] int outputHeight = 8096;

        public override int OutputWidth
        {
            get => outputWidth;
            set => outputWidth = value;
        }

        [Tooltip("Horizontal resolution of final image once stitched together.")]
        [SerializeField] int outputWidth = 8096;

        public int RowCount => rowCount;

        [Tooltip("Number of vertical tiles.")]
        [SerializeField] int rowCount = 2;

        public int TileWidth => OutputWidth / ColumnCount;
        public int TileHeight => OutputWidth / ColumnCount;

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

            if (TileWidth > SystemInfo.maxTextureSize || TileHeight > SystemInfo.maxTextureSize)
            {
                errors.Add($"Tile size exceeds the maximum texture size ({SystemInfo.maxTextureSize}).");
                ok = false;
            }

            return ok;
        }
    }
}
