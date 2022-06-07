using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.Windows.WebCam;
using System.IO;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Net.Http.Headers;

public class PhotoCaptureToTexture : MonoBehaviour
{
    private readonly float metersPerPixel = 0.0002645833f;
    private Resolution cameraResolution = default;
    private bool isReadyToCapturePhoto, isCapturingPhoto = false;
    private PhotoCapture photoCaptureObject = null;
    private CameraParameters cameraParameters;
    private Texture2D imageTexture = null;

    private SideLoadImageTarget slit;
    private static readonly HttpClient client = new HttpClient();


    private void Start()
    {
        /*var resolutions = PhotoCapture.SupportedResolutions;
        if (resolutions == null || resolutions.Count() == 0)
        {
            if (text != null)
            {
                text.text = "Resolutions not available. Did you provide web cam access?";
            }
            return;
        }*/
        Debug.Log("START CAMERA");
        Debug.Log( PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).Count() );
        cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
        imageTexture = new Texture2D(cameraResolution.width, cameraResolution.height);
        
        PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
    }

    private void Awake()
    {
        Debug.Log("AWAKE CAMERA");
    }

    private void OnDestroy()
    {
        isReadyToCapturePhoto = false;

        if (photoCaptureObject != null)
        {
            photoCaptureObject.StopPhotoModeAsync(OnPhotoCaptureStopped);
        }
    }

    private void OnPhotoCaptureCreated(PhotoCapture captureObject)
    {
        photoCaptureObject = captureObject;

        cameraParameters = new CameraParameters(WebCamMode.PhotoMode)
        {
            hologramOpacity = 0.0f,
            cameraResolutionWidth = cameraResolution.width,
            cameraResolutionHeight = cameraResolution.height,
            pixelFormat = CapturePixelFormat.BGRA32
        };

        captureObject.StartPhotoModeAsync(cameraParameters, OnPhotoModeStarted);
    }

    /*
    private void OnPhotoCaptureCreated(PhotoCapture captureObject)
    {
        photoCaptureObject = captureObject;

        cameraParameters = new CameraParameters(WebCamMode.PhotoMode)
        {
            hologramOpacity = 0.0f,
            cameraResolutionWidth = cameraResolution.width,
            cameraResolutionHeight = cameraResolution.height,
            pixelFormat = CapturePixelFormat.BGRA32
        };
    }
    */

    private void StartTakePhoto()
    {
        photoCaptureObject.StartPhotoModeAsync(cameraParameters, OnPhotoModeStarted);
    }

    private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            isReadyToCapturePhoto = true;
            InvokeRepeating(nameof(TakePhoto), 10f, 2f);
        }
        else
        {
            isReadyToCapturePhoto = false;
        }
    }

    private void TakePhoto()
    {
        Debug.Log("START TAKE PHOTO");
        if (!isReadyToCapturePhoto || isCapturingPhoto)
        {
            Debug.Log("NOT READY TO TAKE PHOTO");
            return;
        }

        isCapturingPhoto = true;
        //Vuforia.Image image = CameraDevice.Instance.GetCameraImage(mPixelFormat);
        photoCaptureObject.TakePhotoAsync(OnPhotoCaptured);
    }

    private void StopTakePhoto()
    {
        CancelInvoke(nameof(TakePhoto));
    }

    private async Task<string> UploadImage(byte[] img)
    {
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri("http://192.168.43.229:9999/");
        MultipartFormDataContent form = new MultipartFormDataContent();
        HttpContent content = new StringContent("fileToUpload");
        form.Add(content, "fileToUpload");
        var stream = new MemoryStream(img);
        content = new StreamContent(stream);
        content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
            Name = "snapshot",
            FileName = "image_data"
        };
        form.Add(content);
        var response = await client.PostAsync("upload", form);
        return response.Content.ReadAsStringAsync().Result;
    }

    private void OnPhotoCaptured(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        Debug.Log("START PHOTO CAPTURED");
        /*
        if (result.success)
        {
            string targetName = "";
            Texture2D targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);
            photoCaptureFrame.UploadImageDataToTexture(targetTexture);

            byte[] bytes = imageTexture.EncodeToPNG();
            byte[] bytesTarget = targetTexture.EncodeToPNG();

            // CHIAMATA AL SERVER
            UploadImage(bytesTarget);

            // CROP DELL'IMMAGINE
            Texture2D croppedTexture = new Texture2D(cameraResolution.width, cameraResolution.height);

            //croppedTexture.SetPixels(imageTexture.GetPixels(x, y, width, height));
            //croppedTexture.Apply();

            //bytes = croppedTexture.EncodeToPNG();

            //File.WriteAllBytes(Application.dataPath + "/../provaTexture.png", bytes);
            if (slit.CreateImageTargetFromTexture(croppedTexture, cameraResolution.width, targetName) != null)
    
            {

            }
        }*/

        isCapturingPhoto = false;
    }

    private void OnPhotoCaptureStopped(PhotoCapture.PhotoCaptureResult result)
    {
        photoCaptureObject.Dispose();
        photoCaptureObject = null;
    }


    //void Start()
    //{
    //    Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
    //    imageTexture = new Texture2D(cameraResolution.width, cameraResolution.height);

    //    // Create a PhotoCapture object
    //    PhotoCapture.CreateAsync(false, delegate (PhotoCapture captureObject) {
    //        photoCaptureObject = captureObject;
    //        CameraParameters cameraParameters = new CameraParameters
    //        {
    //            hologramOpacity = 0.0f,
    //            cameraResolutionWidth = cameraResolution.width,
    //            cameraResolutionHeight = cameraResolution.height,
    //            pixelFormat = CapturePixelFormat.BGRA32
    //        };

    //        // Activate the camera
    //        photoCaptureObject.StartPhotoModeAsync(cameraParameters, delegate (PhotoCapture.PhotoCaptureResult result) {
    //            // Take a picture
    //            photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
    //        });
    //    });
    //}

    //void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    //{
    //    // Copy the raw image data into our target texture
    //    photoCaptureFrame.UploadImageDataToTexture(imageTexture);

    //    byte[] bytes = imageTexture.EncodeToPNG();

    //    Texture2D croppedTexture = new Texture2D(width, height);

    //    croppedTexture.SetPixels(imageTexture.GetPixels(x, y, width, height));
    //    croppedTexture.Apply();

    //    bytes = croppedTexture.EncodeToPNG();

    //    File.WriteAllBytes(Application.dataPath + "/../provaTexture.png", bytes);

    //    // Deactivate our camera
    //    photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    //}

    //void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    //{
    //    // Shutdown our photo capture resource
    //    photoCaptureObject.Dispose();
    //    photoCaptureObject = null;
    //}
}
