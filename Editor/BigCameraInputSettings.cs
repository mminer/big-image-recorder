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

        public float AspectRatio => (float)OutputWidth / OutputHeight;

        public string CameraTag => cameraTag;

        [SerializeField] string cameraTag = "MainCamera";

        public int Columns => columns;

        [Tooltip("Number of horizontal tiles.")]
        [SerializeField] int columns = 2;

        public override int OutputHeight
        {
            get => outputHeight;
            set => outputHeight = value;
        }

        [Tooltip("Vertical resolution of final image once stitched together.")]
        [SerializeField] int outputHeight = 8192;

        public override int OutputWidth
        {
            get => outputWidth;
            set => outputWidth = value;
        }

        [Tooltip("Horizontal resolution of final image once stitched together.")]
        [SerializeField] int outputWidth = 8192;

        public int Rows => rows;

        [Tooltip("Number of vertical tiles.")]
        [SerializeField] int rows = 2;

        public int TileHeight => OutputHeight / Rows;
        public int TileWidth => OutputWidth / Columns;

        protected override void CheckForErrors(List<string> errors)
        {
            base.CheckForErrors(errors);

            if (Columns < 1 || Rows < 1)
            {
                errors.Add($"Need at least one row and one column.");
            }

            if (OutputWidth % Columns != 0)
            {
                errors.Add($"Output width must be a multiple of the columns.");
            }

            if (OutputHeight % Rows != 0)
            {
                errors.Add($"Output height must be a multiple of the rows.");
            }

            if (OutputWidth <= 0 || OutputHeight <= 0)
            {
                errors.Add($"Invalid output resolution: {OutputWidth}x{OutputHeight}");
            }

            if (TileWidth > SystemInfo.maxTextureSize || TileHeight > SystemInfo.maxTextureSize)
            {
                errors.Add($"Tile size exceeds the maximum texture size ({SystemInfo.maxTextureSize}).");
            }
        }
    }
}
