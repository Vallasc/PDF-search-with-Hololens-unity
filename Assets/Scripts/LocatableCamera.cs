// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Windows.WebCam;
using System;
using UnityEngine.Networking;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;

public class LocatableCamera : MonoBehaviour
{
    [SerializeField]
    private GameObject textObject = null;
    private TextMeshPro text = null;

    [SerializeField]
    private GameObject buttonPhoto = null;

    [SerializeField]
    private GameObject keys = null;

    public string serverIp;
    private int serverPort = 8574;
    private string urlOcr = "https://127.0.0.1:8574/upload";

    private PhotoCapture photoCaptureObject = null;
    private Resolution cameraResolution;
    private bool isCapturingPhoto, isReadyToCapturePhoto = false;
    private uint numPhotos = 0;

    private float nextActionTime = 0.0f;
    private readonly float period = 6f;

    private bool stopped = true;


    void Start()
    {
        if(textObject != null)
            text = textObject.GetComponent<TextMeshPro>();
        var resolutions = PhotoCapture.SupportedResolutions;
        if (resolutions == null || resolutions.Count() == 0)
        {
            if (text != null)
            {
                text.text = "Resolutions not available. Did you provide web cam access?";
            }
            return;
        }

        cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
        print(cameraResolution);
        PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);

        
        if (text != null)
        {
            text.text = "Starting camera...";
        }
    }
    void Update()
    {
        /*if (Time.time > nextActionTime)
        {
            nextActionTime += period;
            print(nextActionTime);

            if (!stopped)
            {
                TakePhoto();
            }
        }*/
    }
    public void UpdateBaseUrl()
    {
        urlOcr = "https://" + serverIp + ":" + serverPort.ToString() + "/upload";
    }

    public void UpdateMode()
    {
        stopped = !stopped;

        if (stopped)
        {
            buttonPhoto.GetComponent<ButtonConfigHelper>().MainLabelText = "Off";
        }
        else
        {
            buttonPhoto.GetComponent<ButtonConfigHelper>().MainLabelText = "On";
        }
    }

    private void OnPhotoCaptureCreated(PhotoCapture captureObject)
    {
        if (text != null)
        {
            text.text += "\nPhotoCapture created...";
        }

        photoCaptureObject = captureObject;

        CameraParameters cameraParameters = new CameraParameters()
        {
            hologramOpacity = 0.0f,
            cameraResolutionWidth = cameraResolution.width,
            cameraResolutionHeight = cameraResolution.height,
            pixelFormat = CapturePixelFormat.BGRA32
        };

        captureObject.StartPhotoModeAsync(cameraParameters, OnPhotoModeStarted);
    }

    private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            isReadyToCapturePhoto = true;

            if (text != null)
            {
                text.text = "Ready!\nPress the above button to take a picture.";
            }
        }
        else
        {
            isReadyToCapturePhoto = false;

            if (text != null)
            {
                text.text = "Unable to start photo mode!";
            }
        }
    }

    /// <summary>
    /// Takes a photo and attempts to load it into the scene using its location data.
    /// </summary>
    public void TakePhoto()
    {
        try
        {
            if (!isReadyToCapturePhoto || isCapturingPhoto)
            {
                return;
            }

            isCapturingPhoto = true;

            if (text != null)
            {
                text.text = "Taking picture...";
            }

            photoCaptureObject.TakePhotoAsync(OnPhotoCaptured);
        }
        catch (Exception ex)
        {
            text.text = ex.Message;
        }
    }

    IEnumerator PostPhotoCapture(string base64image)
    {
        Debug.Log("POST photo capture");

        var sections = new List<IMultipartFormSection>
        {
            new MultipartFormDataSection("snapshot", base64image),
            new MultipartFormDataSection("request_index", numPhotos.ToString())
        };

        UnityWebRequest uwr = UnityWebRequest.Post(urlOcr, sections);
        uwr.certificateHandler = new BypassCertificate();
        yield return uwr.SendWebRequest();

        if (uwr.isHttpError || uwr.isNetworkError)
        {
            Debug.Log("ERRORE POST");
            Debug.Log(uwr.error);
        }
        else
        {
            Debug.Log("RICHIESTA POST ESEGUITA!");
            var serverResponse = uwr.downloadHandler.text;

            Debug.Log(serverResponse);
            ResponseKeywords res = ResponseKeywords.CreateFromJSON(serverResponse);
            Debug.Log(res.GetKeywords());

            keys.GetComponent<KeywordsManager>().UpdateKeywordsCollection(res.GetKeywords());
        }
    }

    private void OnPhotoCaptured(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        string textureString = null;
        
        try
        {
            if (result.success)
            {
                if (text != null)
                {
                    text.text += "\nTook picture!";
                }

                Texture2D targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);
                photoCaptureFrame.UploadImageDataToTexture(targetTexture);


                numPhotos++;

                //File.WriteAllBytes("./img.png", targetTexture.EncodeToPNG());
                textureString = Convert.ToBase64String(targetTexture.EncodeToPNG());
                text.text = textureString;
                //Debug.Log(textureString);



                //// UTILE PER IL DECODE, SE NECESSARIO
                //byte[] Bytes = System.Convert.FromBase64String(base64Img);
                //Texture2D tex = new Texture2D(500, 700);
                //tex.LoadImage(Bytes);


            }
            else
            {
                if (text != null)
                {
                    text.text += "\nPicture taking failed: " + result.hResult;
                }
            }
        } catch (Exception ex)
        {
            text.text = ex.Message;
        }

        photoCaptureObject.StopPhotoModeAsync(OnPhotoCaptureStopped);
        isCapturingPhoto = false;

        /*if (textureString != null)
        {
            StartCoroutine(PostPhotoCapture(textureString));
        }*/
    }

    private void OnPhotoCaptureStopped(PhotoCapture.PhotoCaptureResult result)
    {
        Debug.Log("STOP");
        if (text != null)
        {
            text.text = result.success ? "Photo mode stopped." : "Unable to stop photo mode.";
        }

        photoCaptureObject.Dispose();
        photoCaptureObject = null;
    }

    private void OnDestroy()
    {
        Debug.Log("DISTRUZIONE");
        isReadyToCapturePhoto = false;

        if (photoCaptureObject != null)
        {
            photoCaptureObject.StopPhotoModeAsync(OnPhotoCaptureStopped);

            if (text != null)
            {
                text.text = "Stopping camera...";
            }
        }
    }

    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        // Shutdown our photo capture resource
        photoCaptureObject.Dispose();
        photoCaptureObject = null;
    }



    [Serializable]
    public class ResponseKeywords
    {
        public string[] keywords;
        public int request_index;

        public ResponseKeywords(string[] list)
        {
            this.keywords = list;
            this.request_index = -1;
        }

        public static ResponseKeywords CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<ResponseKeywords>(jsonString);
        }

        public string[] GetKeywords()
        {
            return this.keywords;
        }
        public int GetRequestIndex()
        {
            return this.request_index;
        }
    }
}
