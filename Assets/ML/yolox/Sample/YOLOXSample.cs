/* 
*   YOLOX
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatSuite.Examples {

    using UnityEngine;
    using NatSuite.ML;
    using NatSuite.ML.Features;
    using NatSuite.ML.Vision;
    using NatSuite.ML.Visualizers;
    using Stopwatch = System.Diagnostics.Stopwatch;

    public sealed class YOLOXSample : MonoBehaviour {

        [Header(@"NatML")]
        public string accessKey;

        [Header(@"Prediction")]
        public Texture2D image;

        [Header(@"UI")]
        public YOLOXVisualizer visualizer;

        async void Start () {
            Debug.Log("Fetching model data from NatML...");
            // Fetch model data from NatML Hub
            var modelData = await MLModelData.FromHub("@natsuite/yolox", accessKey);
            // Deserialize the model
            using var model = modelData.Deserialize();
            // Create the YOLOX predictor
            using var predictor = new YOLOXPredictor(model, modelData.labels);
            // Create input feature
            var inputFeature = new MLImageFeature(image);
            (inputFeature.mean, inputFeature.std) = modelData.normalization;
            inputFeature.aspectMode = modelData.aspectMode;
            // Detect
            var watch = Stopwatch.StartNew();
            var detections = predictor.Predict(inputFeature);
            watch.Stop();
            // Visualize
            Debug.Log($"Detected {detections.Length} objects after {watch.Elapsed.TotalMilliseconds}ms");
            visualizer.Render(image, detections);
        }
    }
}