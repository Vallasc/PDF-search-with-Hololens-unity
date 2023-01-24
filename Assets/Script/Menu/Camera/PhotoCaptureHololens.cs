using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.Windows.WebCam;

public class PhotoCaptureHololens : MonoBehaviour
{
    private PhotoCapture photoCaptureObject = null;
    private Resolution cameraResolution = default;
    private bool isCapturingPhoto, isReadyToCapturePhoto = false;
    private uint numPhotos = 0;
    private Texture2D targetTexture;

    private float nextActionTime = 0.0f;
    private readonly float period = 6f;
    private bool stopped = true;


    // Use this for initialization
    void Start()
    {
        
    }

    //void Update()
    //{
    //    if (Time.time > nextActionTime)
    //    {
    //        nextActionTime += period;
    //        print(nextActionTime);

    //        if (!stopped)
    //        {
    //            TakePhoto();
    //        }
    //    }
    //}

    public void StartPhotoCapture()
    {
        var resolutions = PhotoCapture.SupportedResolutions;
        if (resolutions == null || resolutions.Count() == 0)
        {
            return;
        }
        cameraResolution = resolutions.OrderByDescending((res) => res.width * res.height).First();
        targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);

        // Create a PhotoCapture object
        PhotoCapture.CreateAsync(false, delegate (PhotoCapture captureObject) {
            photoCaptureObject = captureObject;
            CameraParameters cameraParameters = new CameraParameters
            {
                hologramOpacity = 0.0f,
                cameraResolutionWidth = cameraResolution.width,
                cameraResolutionHeight = cameraResolution.height,
                pixelFormat = CapturePixelFormat.BGRA32
            };

            // Activate the camera
            photoCaptureObject.StartPhotoModeAsync(cameraParameters, delegate (PhotoCapture.PhotoCaptureResult result) {
                // Take a picture
                photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
            });
        });
    }

    public void TakePhoto()
    {
        if (!isReadyToCapturePhoto || isCapturingPhoto)
        {
            return;
        }

        isCapturingPhoto = true;

        photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
    }

    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        // Copy the raw image data into our target texture
        photoCaptureFrame.UploadImageDataToTexture(targetTexture);

        //// Create a gameobject that we can apply our texture to
        //GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        //Renderer quadRenderer = quad.GetComponent<Renderer>() as Renderer;
        //quadRenderer.material = new Material(Shader.Find("Unlit/Texture"));

        //quad.transform.parent = this.transform;
        //quad.transform.localPosition = new Vector3(0.0f, 0.0f, 3.0f);

        //quadRenderer.material.SetTexture("_MainTex", targetTexture);

        // Deactivate our camera
        photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }

    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        photoCaptureObject.Dispose();
        photoCaptureObject = null;
    }

    void OnDestroy()
    {
        isReadyToCapturePhoto = false;

        if (photoCaptureObject != null)
        {
            photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
        }
    }
}
