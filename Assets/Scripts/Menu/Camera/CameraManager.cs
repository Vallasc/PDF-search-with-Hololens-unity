using Microsoft.MixedReality.Toolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Windows.WebCam;
using static LocatableCamera;

public class CameraManager : MonoBehaviour
{
    public GameObject keysListObject = null;
    public GameObject warningBox = null;

    private PhotoCapture photoCaptureObject;
    private Texture2D targetTexture;

    private CameraParameters cameraParameters;

    public string serverIp;
    private int serverPort = 8574;
    private string urlOcr;
    private uint numPhotos = 0;
    private string textureCaptured = null;
    private Resolution cameraResolution;

    void Start()
    {


    }

    void Update()
    {
        if (textureCaptured != null)
        {
            string tmp = textureCaptured;
            Debug.Log(urlOcr);
            StartCoroutine(PostPhotoCapture(tmp));
            textureCaptured = null;
        }

    }

    public void UpdateBaseUrl()
    {
        urlOcr = "https://" + serverIp + ":" + serverPort.ToString() + "/upload";
    }

    public void TakePicture()
    {
        keysListObject.GetComponent<KeywordsManager>().SetFirstPhotoTaken();
        warningBox.SetActive(true);
        StartCoroutine(TakePictureDelayed());

    }

    private IEnumerator TakePictureDelayed()
    {
        yield return new WaitForSeconds(3);
        cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
        targetTexture = new Texture2D(cameraResolution.width/2, cameraResolution.height/2);

        PhotoCapture.CreateAsync(false, delegate (PhotoCapture captureObject)
        {
            photoCaptureObject = captureObject;
            cameraParameters = new CameraParameters
            {
                hologramOpacity = 0.0f,
                cameraResolutionWidth = cameraResolution.width/2,
                cameraResolutionHeight = cameraResolution.height/2,
                pixelFormat = CapturePixelFormat.BGRA32
            };

            //Activate the camera
            photoCaptureObject.StartPhotoModeAsync(cameraParameters, delegate (PhotoCapture.PhotoCaptureResult result)
            {
                // Take a picture
                photoCaptureObject.TakePhotoAsync(OnCapturedPhotoCaptured);
            });
        });

    }

    void OnCapturedPhotoCaptured(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        // Copy the raw image data into our target texture
        photoCaptureFrame.UploadImageDataToTexture(targetTexture);
        warningBox.SetActive(false);
        textureCaptured = Convert.ToBase64String(targetTexture.EncodeToPNG());
        //StartCoroutine(PostPhotoCapture(textureString));
        // Deactivate our camera
        photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }

    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        // Shutdown our photo capture resource
        photoCaptureObject.Dispose();
        photoCaptureObject = null;
    }

    public string GetBase64Image()
    {
        return Convert.ToBase64String(targetTexture.EncodeToPNG());
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

            keysListObject.GetComponent<KeywordsManager>().UpdateKeywordsCollection(res.GetKeywords());
        }
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
