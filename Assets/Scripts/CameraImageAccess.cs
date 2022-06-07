using UnityEngine;
using Vuforia;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Net.Http.Headers;

public class CameraImageAccess : MonoBehaviour
{
    //#if UNITY_EDITOR
    //    PixelFormat mPixelFormat = PixelFormat.RGBA8888; // Editor passes in a RGBA8888 texture instead of RGB888
    //#else
    //    PixelFormat mPixelFormat = PIXEL_FORMAT.RGB888; // Use RGB888 for mobile
    //#endif
    PixelFormat mPixelFormat = PixelFormat.RGB888;
    private bool mAccessCameraImage = true;
    private bool mFormatRegistered = false;
    private Texture2D texture;

    void Start()
    {
        // Register Vuforia life-cycle callbacks:
        VuforiaApplication.Instance.OnVuforiaStarted += RegisterFormat;
        //VuforiaApplication.Instance.OnVuforiaStarted += FocusMode;
        VuforiaApplication.Instance.OnVuforiaPaused += OnPause;
        InvokeRepeating(nameof(TakePhoto), 10f, 2f);

    }

    void OnDestroy()
    {
        // Unregister Vuforia life-cycle callbacks:
        VuforiaApplication.Instance.OnVuforiaStarted -= RegisterFormat;
        //VuforiaApplication.Instance.OnVuforiaStarted -= FocusMode;
        VuforiaApplication.Instance.OnVuforiaPaused -= OnPause;
    }

    /// 
    /// Take photo of the world
    /// 
    void TakePhoto()
    {
        if (mFormatRegistered)
        {
            Image image = VuforiaBehaviour.Instance.CameraDevice.GetCameraImage(mPixelFormat);
            if(texture is null)
                texture = new Texture2D(image.BufferWidth, image.BufferHeight);
            image.CopyBufferToTexture(texture);

            Debug.Log(
                "\nImage Format: " + image.PixelFormat +
                "\nImage Size: " + image.Width + " x " + image.Height +
                "\nBuffer Size: " + image.BufferWidth + " x " + image.BufferHeight +
                "\nImage Stride: " + image.Stride + "\n"
            );
            
            
        }
    }


    //private async Task<string> UploadImage(byte[] img)
    //{
    //    HttpClient client = new HttpClient();
    //    client.BaseAddress = new Uri("http://192.168.43.229:9999/");
    //    MultipartFormDataContent form = new MultipartFormDataContent();
    //    HttpContent content = new StringContent("fileToUpload");
    //    form.Add(content, "fileToUpload");
    //    var stream = new MemoryStream(img);
    //    content = new StreamContent(stream);
    //    content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
    //    {
    //        Name = "snapshot",
    //        FileName = "image_data"
    //    };
    //    form.Add(content);
    //    var response = await client.PostAsync("upload", form);
    //    return response.Content.ReadAsStringAsync().Result;
    //}


    /// 
    /// Called when app is paused / resumed
    /// 
    void OnPause(bool paused)
    {
        if (paused)
        {
            Debug.Log("App was paused");
            UnregisterFormat();
        }
        else
        {
            Debug.Log("App was resumed");
            RegisterFormat();
        }
    }

    /// 
    /// Register the camera pixel format
    /// 
    void RegisterFormat()
    {
        // Vuforia has started, now register camera image format
        bool success = VuforiaBehaviour.Instance.CameraDevice.SetFrameFormat(mPixelFormat, true);
        if (success)

        {
            Debug.Log("Successfully registered pixel format " + mPixelFormat.ToString());
            mFormatRegistered = true;
        }
        else
        {
            Debug.LogError(
                "Failed to register pixel format " + mPixelFormat.ToString() +
                "\n the format may be unsupported by your device;" +
                "\n consider using a different pixel format.");
            mFormatRegistered = false;
        }
    }

    //void FocusMode()
    //{
    //    VuforiaBehaviour.Instance.CameraDevice.SetFocusMode(FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
    //}


    /// 
    /// Unregister the camera pixel format (e.g. call this when app is paused)
    /// 
    void UnregisterFormat()
    {
        Debug.Log("Unregistering camera pixel format " + mPixelFormat.ToString());
        VuforiaBehaviour.Instance.CameraDevice.SetFrameFormat(mPixelFormat, false);
        mFormatRegistered = false;
    }
}
