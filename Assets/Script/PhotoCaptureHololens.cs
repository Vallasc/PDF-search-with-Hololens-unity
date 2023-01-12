using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.Windows.WebCam;
using System.IO;

public class PhotoCaptureHololens : MonoBehaviour
{
    private PhotoCapture photoCaptureObject = null;
    private Texture2D targetTexture = null;

    // Use this for initialization
    void Start()
    {
        print("Start photo capture");
        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
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
            print("Provola0");
            // Activate the camera
            photoCaptureObject.StartPhotoModeAsync(cameraParameters, delegate (PhotoCapture.PhotoCaptureResult result) {
                // Take a picture
                print("Provola1");
                photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
            });
            //captureObject.StartPhotoModeAsync(c, false, OnPhotoModeStarted);
        });
    }

    private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
        }
        else
        {
            Debug.LogError("Unable to start photo mode!");
        }
    }

    // SEMBRA MIGLIORE
    //void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    //{
    //    if (result.success)
    //    {
    ////        // Create our Texture2D for use and set the correct resolution
    ////        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
    ////        Texture2D targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);
    //        // Copy the raw image data into our target texture
    //        photoCaptureFrame.UploadImageDataToTexture(targetTexture);
    //        // Do as we wish with the texture such as apply it to a material, etc.
    //    }
    //    // Clean up
    //    photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    //}

    // MODO DA VERIFICARE PER VEDERE SE SI PUO INTERAGIRE CON I RAW BYTES
    //void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    //{
    //    if (result.success)
    //    {
    //        List<byte> imageBufferList = new List<byte>();
    //        // Copy the raw IMFMediaBuffer data into our empty byte list.
    //        photoCaptureFrame.CopyRawImageDataIntoBuffer(imageBufferList);

    //        // In this example, we captured the image using the BGRA32 format.
    //        // So our stride will be 4 since we have a byte for each rgba channel.
    //        // The raw image data will also be flipped so we access our pixel data
    //        // in the reverse order.
    //        int stride = 4;
    //        float denominator = 1.0f / 255.0f;
    //        List<Color> colorArray = new List<Color>();
    //        for (int i = imageBufferList.Count - 1; i >= 0; i -= stride)
    //        {
    //            float a = (int)(imageBufferList[i - 0]) * denominator;
    //            float r = (int)(imageBufferList[i - 1]) * denominator;
    //            float g = (int)(imageBufferList[i - 2]) * denominator;
    //            float b = (int)(imageBufferList[i - 3]) * denominator;

    //            colorArray.Add(new Color(r, g, b, a));
    //        }
    //        // Now we could do something with the array such as texture.SetPixels() or run image processing on the list
            //targetTexture.SetPixels(colorArray.ToArray());
            //targetTexture.Apply();
    //    }
    //    photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    //}

    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        // Copy the raw image data into our target texture
        photoCaptureFrame.UploadImageDataToTexture(targetTexture);
        File.WriteAllBytes("./img.png", targetTexture.EncodeToPNG());

        //// Create a gameobject that we can apply our texture to
        //GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        //Renderer quadRenderer = quad.GetComponent<Renderer>() as Renderer;
        //quadRenderer.material = new Material(Shader.Find("Unlit/Texture"));

        //quad.transform.parent = this.transform;
        //quad.transform.localPosition = new Vector3(0.0f, 0.0f, 3.0f);

        //quadRenderer.material.SetTexture("_MainTex", targetTexture);

        // Deactivate our camera
        //photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }

    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        // Shutdown our photo capture resource
        photoCaptureObject.Dispose();
        photoCaptureObject = null;
    }
}