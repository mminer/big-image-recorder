using System.Collections.Generic;
using UnityEditor.Recorder;
using UnityEngine;

namespace UnityEditor.BigImageRecorder
{
    [RecorderSettings(typeof(BigImageRecorder), "Big Image Sequence", "imagesequence_16")]
    class BigImageRecorderSettings : RecorderSettings
    {
        static readonly string columnsWildcard = DefaultWildcard.GeneratePattern("Columns");
        static readonly string rowsWildcard = DefaultWildcard.GeneratePattern("Rows");
        static readonly string tileColumnWildcard = DefaultWildcard.GeneratePattern("Tile Column");
        static readonly string tileRowWildcard = DefaultWildcard.GeneratePattern("Tile Row");

        public bool DeleteAfterStitching => deleteAfterStitching;

        [Tooltip("Whether to trash the images after running the stitch command.")]
        [SerializeField] bool deleteAfterStitching = true;

        protected override string Extension => "png";

        [SerializeField] BigImageInputSelector imageInputSelector = new BigImageInputSelector();
        public string StitchCommand => stitchCommand;

        [Tooltip("Command to run after each frame is written to image tiles.")]
        [SerializeField] string stitchCommand = "";

        public string StitchCommandArguments => stitchCommandArguments;

        [Tooltip("Arguments to pass to the stitch command.")]
        [SerializeField] string stitchCommandArguments = "";

        public override IEnumerable<RecorderInputSettings> InputsSettings
        {
            get { yield return imageInputSelector.Selected; }
        }

        public BigImageRecorderSettings()
        {
            FileNameGenerator.AddWildcard(rowsWildcard, session =>
                (session?.recorder as BigImageRecorder)?.Input.InputSettings.Rows.ToString() ?? "0");

            FileNameGenerator.AddWildcard(columnsWildcard, session =>
                (session?.recorder as BigImageRecorder)?.Input.InputSettings.Columns.ToString() ?? "0");

            FileNameGenerator.AddWildcard(tileRowWildcard, session =>
                (session?.recorder as BigImageRecorder)?.RowBeingWritten.ToString() ?? "0");

            FileNameGenerator.AddWildcard(tileColumnWildcard, session =>
                (session?.recorder as BigImageRecorder)?.ColumnBeingWritten.ToString() ?? "0");

            FileNameGenerator.FileName = $"image_{DefaultWildcard.Frame}_{tileRowWildcard}-{tileRowWildcard}";
        }
    }
}
