using System.Collections.Generic;
using UnityEditor.Recorder;
using UnityEngine;

namespace UnityEditor.BigImageRecorder
{
    [RecorderSettings(typeof(BigImageRecorder), "Big Image Sequence", "imagesequence_16")]
    class BigImageRecorderSettings : RecorderSettings
    {
        [SerializeField] BigImageInputSelector imageInputSelector = new BigImageInputSelector();

        protected override string Extension => "png";

        public override IEnumerable<RecorderInputSettings> InputsSettings
        {
            get { yield return imageInputSelector.Selected; }
        }

        public BigImageRecorderSettings()
        {
            FileNameGenerator.FileName = $"image_{DefaultWildcard.Frame}";
        }
    }
}
