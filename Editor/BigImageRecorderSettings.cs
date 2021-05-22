using System.Collections.Generic;
using UnityEditor.Recorder;
using UnityEngine;

namespace UnityEditor.BigImageRecorder
{
    [RecorderSettings(typeof(BigImageRecorder), "Big Image Sequence", "imagesequence_16")]
    class BigImageRecorderSettings : RecorderSettings
    {
        static readonly string columnCountWildcard = DefaultWildcard.GeneratePattern("Column Count");
        static readonly string columnWildcard = DefaultWildcard.GeneratePattern("Column");
        static readonly string rowCountWildcard = DefaultWildcard.GeneratePattern("Row Count");
        static readonly string rowWildcard = DefaultWildcard.GeneratePattern("Row");

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
            FileNameGenerator.AddWildcard(rowCountWildcard, session =>
                (session?.recorder as BigImageRecorder)?.Input.InputSettings.RowCount.ToString() ?? "0");

            FileNameGenerator.AddWildcard(columnCountWildcard, session =>
                (session?.recorder as BigImageRecorder)?.Input.InputSettings.ColumnCount.ToString() ?? "0");

            FileNameGenerator.AddWildcard(rowWildcard, session =>
                (session?.recorder as BigImageRecorder)?.RowBeingWritten.ToString() ?? "0");

            FileNameGenerator.AddWildcard(columnWildcard, session =>
                (session?.recorder as BigImageRecorder)?.ColumnBeingWritten.ToString() ?? "0");

            FileNameGenerator.FileName = $"image_{DefaultWildcard.Frame}_{rowWildcard}-{rowWildcard}";
        }
    }
}
