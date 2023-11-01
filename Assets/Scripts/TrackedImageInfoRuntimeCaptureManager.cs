using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Random = UnityEngine.Random;

//newlines begin
public enum ImageObjectType
{
    FlatObject,

    UpwardObject
}

public class ImageObject
{
    public ImageObjectType imageObjectType;

    public GameObject prefabObject;
}

//newlines end

public class TrackedImageInfoRuntimeCaptureManager : MonoBehaviour
{
    [SerializeField]
    private Text debugLog;

    [SerializeField]
    private Text jobLog;

    [SerializeField]
    private Text currentImageText;

    [SerializeField]
    private Button captureImageButton;

    //New code
    [SerializeField]
    private ImageObject[] imageObjects;
    //end new code

    [SerializeField]
    private GameObject placedObject;

    [SerializeField]
    private Vector3 scaleFactor = new Vector3(0.1f, 0.1f, 0.1f);

    [SerializeField]
    private XRReferenceImageLibrary runtimeImageLibrary;

    private ARTrackedImageManager trackImageManager;

    // Start is called before the first frame update
    void Start()
    {
        debugLog.text += "Creating Runtime Mutable Image Library\n";

        trackImageManager = gameObject.AddComponent<ARTrackedImageManager>();
        trackImageManager.referenceLibrary = trackImageManager.CreateRuntimeLibrary(runtimeImageLibrary);
        trackImageManager.requestedMaxNumberOfMovingImages = 3;
        trackImageManager.trackedImagePrefab = placedObject; //imageObjects[Random.Range(0, imageObjects.Length - 1)].prefabObject;

        trackImageManager.enabled = true;

        trackImageManager.trackedImagesChanged += OnTrackedImagesChanged;

        ShowTrackerInfo();

        //captureImageButton.onClick.AddListener(() => StartCoroutine(CaptureImage()));

    }

    // Update is called once per frame
    /*void Update()
    {
        
    }*/

    private IEnumerator CaptureImage()
    {
        debugLog.text += "Button Clicked!";

        yield return new WaitForEndOfFrame();

        jobLog.text = "Capturing Image... ";

        var texture = ScreenCapture.CaptureScreenshotAsTexture();

        debugLog.text += $"Captured Texture Dimensions: {texture.width}x{texture.height}";

        StartCoroutine(AddImageJob(texture));

    }

    public void ShowTrackerInfo()
    {
        var runtimeReferenceImageLibrary = trackImageManager.referenceLibrary as MutableRuntimeReferenceImageLibrary;
        debugLog.text += $"TextureFormat.RGBA32 supported: ({runtimeReferenceImageLibrary.IsTextureFormatSupported(TextureFormat.RGBA32)}\n";
        debugLog.text += $"Supported Texture Count ({runtimeReferenceImageLibrary.supportedTextureFormatCount})\n";
        debugLog.text += $"trackImageManager.trackables.count ({trackImageManager.trackables.count})\n";
        debugLog.text += $"trackImageManager.trackedImagePrefab.name ({trackImageManager.trackedImagePrefab.name})\n";
        debugLog.text += $"trackImageManager.maxNumberOfMovingImages ({trackImageManager.requestedMaxNumberOfMovingImages})\n";
        debugLog.text += $"trackImageManager.supportsMutableLibrary ({trackImageManager.subsystem.subsystemDescriptor.supportsMutableLibrary})\n";
        debugLog.text += $"trackImageManager.requiresPhysicalImageDimensions ({trackImageManager.subsystem.subsystemDescriptor.requiresPhysicalImageDimensions})\n";

    }

    private void OnDisable()
    {
        trackImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    public IEnumerator AddImageJob(Texture2D texture2D)
    {
        if (texture2D == null)
        {
            debugLog.text += "texture2D is null";
        }
        if (trackImageManager == null)
        {
            debugLog.text += "trackImageManager is null";
        }
        if (trackImageManager.referenceLibrary == null)
        {
            debugLog.text += "trackImageManager.referenceLibrary is null";
        }
        else 
        {
            yield return null;

            debugLog.text = string.Empty;

            debugLog.text += "Adding image \n";

            jobLog.text = "Job Starting...";

            var firstGuid = new SerializableGuid(0, 0);
            var secondGuid = new SerializableGuid(0, 0);

            XRReferenceImage newImage = new XRReferenceImage(firstGuid, secondGuid, new Vector2(0.1f, 0.1f), Guid.NewGuid().ToString(), texture2D);

            try
            {
                MutableRuntimeReferenceImageLibrary mutableRuntimeRefrenceImageLibrary = trackImageManager.referenceLibrary as MutableRuntimeReferenceImageLibrary;
                debugLog.text += $"TextureFormat.RGBA32 supported: {mutableRuntimeRefrenceImageLibrary.IsTextureFormatSupported(TextureFormat.RGBA32)})\n";

                debugLog.text += $"TextureFormat size: {texture2D.width}px width {texture2D.height}px height\n";

                var jobHandle = mutableRuntimeRefrenceImageLibrary.ScheduleAddImageWithValidationJob(texture2D, Guid.NewGuid().ToString(), 0.1f);

                while (!jobHandle.jobHandle.IsCompleted)
                {
                    jobLog.text = "Job Running...";
                }

                jobLog.text = "Job Completed...";
                debugLog.text += $"Job Completed ({mutableRuntimeRefrenceImageLibrary.count})\n";
                debugLog.text += $"Supported Texture Count ({mutableRuntimeRefrenceImageLibrary.supportedTextureFormatCount})\n";
                debugLog.text += $"trackImageManager.trackables.count ({trackImageManager.trackables.count})\n";
                debugLog.text += $"trackImageManager.trackedImagePrefab.name ({trackImageManager.trackedImagePrefab.name})\n";
                debugLog.text += $"trackImageManager.maxNumberOfMovingImages ({trackImageManager.requestedMaxNumberOfMovingImages})\n";
                debugLog.text += $"trackImageManager.supportsMutableLibrary ({trackImageManager.subsystem.subsystemDescriptor.supportsMutableLibrary})\n";
                debugLog.text += $"trackImageManager.requiresPhysicalImageDimensions ({trackImageManager.subsystem.subsystemDescriptor.requiresPhysicalImageDimensions})\n";
            }
            catch (Exception e)
            {
                if (texture2D == null)
                {
                    debugLog.text += "texture2D is null";
                }
                debugLog.text += $"Error: {e.ToString()}";
            }
        }

        
    }
    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            //Display the name of the tracked image in the canvas
            currentImageText.text = trackedImage.referenceImage.name;
            trackedImage.transform.Rotate(Vector3.up, 180);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            //Display the name of the tracked image in the canvas
            currentImageText.text = trackedImage.referenceImage.name;
            trackedImage.transform.Rotate(Vector3.up, 180);
        }
    }

    public void OnButtonClicked()
    {
        debugLog.text += "Button Clicked from Direct Method!";
        StartCoroutine(CaptureImage());
    }
}
