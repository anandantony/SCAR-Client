using UnityEngine;
using NatSuite.ML;
using NatSuite.ML.Features;
using NatSuite.ML.Vision;
using NatSuite.ML.Visualizers;
using Vuforia;

public class Detection : MonoBehaviour
{

    [Header(@"NatML Hub")]
    public string accessKey;

    [Header(@"Visualization")]
    public NanoDetVisualizer visualizer;

    byte[] pixelBuffer;
    MLModelData modelData;
    MLModel model;
    NanoDetPredictor predictor;

    public Camera renderTextureCamera;
    public PlaceObject placeObject;
    [HideInInspector] public bool targetFound;
    Texture2D texture;

    LayerMask mask;
    public bool render
    {
        get;
        set;
    } = true;

    public bool perform
    {
        get;
        set;
    } = true;

    Transform mainCam;


    async void Start()
    {
        mask = LayerMask.GetMask("Obstacle");
        VuforiaApplication.Instance.OnVuforiaStarted += VuforiaStarted;
        // Fetch the model data
        Debug.Log("Fetching model from NatML Hub");
        modelData = await MLModelData.FromHub("@natsuite/nanodet", accessKey);
        // Deserialize the model
        model = modelData.Deserialize();
        // Create the SSD Lite predictor
        predictor = new NanoDetPredictor(model, modelData.labels);
        mainCam = Camera.main.transform;
    }

    void VuforiaStarted()
    {
        renderTextureCamera.CopyFrom(Camera.main);
        renderTextureCamera.targetTexture = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.Default);
    }

    void Update()
    {
        // Check that the model has been downloaded
        if (predictor == null)
            return;

        if (!perform)
            return;

        if (renderTextureCamera.targetTexture == null)
            return;

        if (renderTextureCamera.fieldOfView != Camera.main.fieldOfView)
        {
            renderTextureCamera.fieldOfView = Camera.main.fieldOfView;
        }

        texture = renderTextureCamera.activeTexture.toTexture2D();
        texture.Compress(false);

        var inputFeature = new MLImageFeature(texture);
        (inputFeature.mean, inputFeature.std) = modelData.normalization;
        inputFeature.aspectMode = modelData.aspectMode;
        // Predict
        var detections = predictor.Predict(inputFeature);
        // Visualize
        if (render)
        {
            visualizer.Render(texture, detections);
        }

        foreach (var detection in detections)
        {
            if (detection.score < 0.5)
                continue;

            var _rect = detection.rect;
            var _center = _rect.position + new Vector2(_rect.width / 2, _rect.height / 2);
            var _width = _rect.width;
            var _length = _rect.height;

            Ray ray = renderTextureCamera.ViewportPointToRay(_center);
            placeObject.Place(ray, _width, _length, new Vector3(0, mainCam.rotation.eulerAngles.y, 0));
        }

        Resources.UnloadUnusedAssets();
        // Destroy(texture);
    }

    void OnDisable()
    {
        // Dispose the predictor and model
        model?.Dispose();
    }
}

