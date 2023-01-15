// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Windows.WebCam;
using System.IO;
using System;
using UnityEngine.Networking;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;

public class LocatableCamera : MonoBehaviour
{
    [SerializeField]
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

    [SerializeField]
    private Shader textureShader = null;

    [SerializeField]
    private TextMesh text = null;

    [SerializeField]
    private GameObject buttonPhoto = null;

    [SerializeField]
    private GameObject keys = null;

    [SerializeField]
    private GameObject pdfList = null;

    private PhotoCapture photoCaptureObject = null;
    private Resolution cameraResolution = default(Resolution);
    private bool isCapturingPhoto, isReadyToCapturePhoto = false;
    private uint numPhotos = 0;

    private float nextActionTime = 0.0f;
    private readonly float period = 6f;

    private string url = "https://127.0.0.1:8574/upload";
    private string urlKeyword = "https://127.0.0.1:8573/pdfs";

    private bool stopped = true;

    [SerializeField]
    Renderer quadRenderer;
    [SerializeField]
    GameObject quad;

    private void Start()
    {
        var resolutions = PhotoCapture.SupportedResolutions;
        if (resolutions == null || resolutions.Count() == 0)
        {
            if (text != null)
            {
                text.text = "Resolutions not available. Did you provide web cam access?";
            }
            return;
        }

        cameraResolution = resolutions.OrderByDescending((res) => res.width * res.height).First();
        print(cameraResolution);
        PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);

        if (text != null)
        {
            text.text = "Starting camera...";
        }
    }
    void Update()
    {
        if (Time.time > nextActionTime)
        {
            nextActionTime += period;
            print(nextActionTime);

            if (!stopped)
            {
                TakePhoto();
            }
        }
    }

    public void UpdateMode()
    {
        stopped = !stopped;

        if (stopped)
        {
            buttonPhoto.GetComponent<ButtonConfigHelper>().MainLabelText = "START";
            //pdfList.transform.Find("Text").GetComponent<TMP_Text>().text = "PHOTO MODE: OFF";
        }
        else
        {
            buttonPhoto.GetComponent<ButtonConfigHelper>().MainLabelText = "STOP";
            //pdfList.transform.Find("Text").GetComponent<TMP_Text>().text = "PHOTO MODE: ON";
        }
        
    }

    private void OnPhotoCaptureCreated(PhotoCapture captureObject)
    {
        if (text != null)
        {
            text.text += "\nPhotoCapture created...";
        }

        photoCaptureObject = captureObject;

        CameraParameters cameraParameters = new CameraParameters(WebCamMode.PhotoMode)
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

    IEnumerator PostPhotoCapture(string base64image)
    {
        Debug.Log("POST photo capture");

        var sections = new List<IMultipartFormSection>
        {
            new MultipartFormDataSection("snapshot", base64image),
            new MultipartFormDataSection("request_index", numPhotos.ToString())
        };

        using UnityWebRequest uwr = UnityWebRequest.Post(url, sections);
        uwr.certificateHandler = new BypassCertificate();
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
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

            Transform scroll = keys.transform.Find("Keywords").Find("ScrollingObjectCollection");
            Transform grid = scroll.Find("Container").Find("GridObjectCollection");
            if (grid.Find("ikea") != null)
            {
                Debug.Log(Time.realtimeSinceStartup + " ui: " + grid.Find("ikea").Find("IconAndText").Find("UIButtonSquareIcon").gameObject.activeSelf);
            }
        }
        //string[] list = new string[2];
        //list[0] = "ikea";
        //list[1] = "vediamo";
        //keys.GetComponent<KeywordsManager>().UpdateKeywordsCollection(list);

    }

    IEnumerator GetPdfList(string keyword)
    {
        Debug.Log("GET pdf list");

        using UnityWebRequest uwr = UnityWebRequest.Get(urlKeyword + "?keyword=" + keyword);
        uwr.certificateHandler = new BypassCertificate();
        yield return uwr.SendWebRequest();

        Debug.Log("FINE GET");
        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("ERRORE GET");
            Debug.Log(uwr.error);
        }
        else
        {
            Debug.Log("RICHIESTA GET ESEGUITA!");
            var serverResponse = uwr.downloadHandler.text;

            Debug.Log(serverResponse);
        }

        
    }

    private void OnPhotoCaptured(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        string textureString = null;

        if (result.success)
        {
            if (text != null)
            {
                text.text += "\nTook picture!";
            }

            Texture2D targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);
            photoCaptureFrame.UploadImageDataToTexture(targetTexture);

            numPhotos++;
            quadRenderer.material.SetTexture("_MainTex", targetTexture);
            
            File.WriteAllBytes("./img.png", targetTexture.EncodeToPNG());
            textureString = Convert.ToBase64String(targetTexture.EncodeToPNG());

            Debug.Log(textureString);



            //// UTILE PER IL DECODE, SE NECESSARIO
            //byte[] Bytes = System.Convert.FromBase64String(base64Img);
            //Texture2D tex = new Texture2D(500, 700);
            //tex.LoadImage(Bytes);

            

            //Debug.Log(photoCaptureFrame.hasLocationData);
            //if (photoCaptureFrame.hasLocationData)
            //{

            //photoCaptureFrame.TryGetCameraToWorldMatrix(out Matrix4x4 cameraToWorldMatrix);

            //Vector3 position = cameraToWorldMatrix.GetColumn(3) - cameraToWorldMatrix.GetColumn(2);
            //Quaternion rotation = Quaternion.LookRotation(-cameraToWorldMatrix.GetColumn(2), cameraToWorldMatrix.GetColumn(1));

            //photoCaptureFrame.TryGetProjectionMatrix(Camera.main.nearClipPlane, Camera.main.farClipPlane, out Matrix4x4 projectionMatrix);

            //targetTexture.wrapMode = TextureWrapMode.Clamp;


            //quadRenderer.sharedMaterial.SetMatrix("_WorldToCameraMatrix", cameraToWorldMatrix.inverse);
            //quadRenderer.sharedMaterial.SetMatrix("_CameraProjectionMatrix", projectionMatrix);

            //quad.transform.position = position;
            //quad.transform.rotation = rotation;

            //if (text != null)
            //{
            //    text.text += $"\nPosition: ({position.x}, {position.y}, {position.z})";
            //    text.text += $"\nRotation: ({rotation.x}, {rotation.y}, {rotation.z}, {rotation.w})";
            //}
            //}
            //else
            //{
            //    if (text != null)
            //    {
            //        text.text += "\nNo location data :(";
            //    }
            //}
        }
        else
        {
            if (text != null)
            {
                text.text += "\nPicture taking failed: " + result.hResult;
            }
        }

        isCapturingPhoto = false;

        if (textureString != null)
        {
            StartCoroutine(PostPhotoCapture(textureString));

            //StartCoroutine(GetPdfList("ikea"));
        }
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
}