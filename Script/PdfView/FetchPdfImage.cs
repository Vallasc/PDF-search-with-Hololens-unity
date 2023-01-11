using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

public class FetchPdfImage : MonoBehaviour
{
    public string url = "https://127.0.0.1/pdf";
    public GameObject canvas;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start");
        StartCoroutine(GetImage());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator GetImage()
    {
        Debug.Log("GET image");
        UnityWebRequest webRequest = UnityWebRequest.Get(url);
        webRequest.certificateHandler = new BypassCertificate();
        yield return webRequest.SendWebRequest();
        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            ImageResponse res = ImageResponse.CreateFromJSON(webRequest.downloadHandler.text);
            PaintImage(res.img, res.width, res.height);
        }
        else
        {
            Debug.Log(webRequest.error);
        }

    }

    void PaintImage(string base64Image, int width, int height)
    {
        Byte[] imgBytes = Convert.FromBase64String(base64Image);
        Texture2D tex = new Texture2D(width, height, TextureFormat.BC7, false);
        tex.LoadImage(imgBytes);
        this.canvas.GetComponent<Renderer>().material.mainTexture = tex;

        Vector3 initialScale = this.canvas.transform.localScale;

        if (width < height)
        {
            float aspectRatio = (float) width / (float) height;
            initialScale.x = aspectRatio;
            initialScale.y = 1;
        }
        else
        {
            float aspectRatio = (float) width / (float) height;
            initialScale.x = 1;
            initialScale.y = aspectRatio;
        }
        this.canvas.transform.localScale = initialScale;
    }    

    /*void post_image()
    {
        List<IMultipartFormSection> form = new List<IMultipartFormSection>();

        form.Add(new MultipartFormDataSection("username", username));
        form.Add(new MultipartFormDataSection("password", password));
        form.Add(new MultipartFormDataSection("age", age != null ? age : "0"));
        form.Add(new MultipartFormDataSection("email", email));

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
        }
    }*/

    [Serializable]
    public class ImageResponse
    {
        public string img;
        public int width;
        public int height;

        public static ImageResponse CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<ImageResponse>(jsonString);
        }

    }

    public class BypassCertificate: CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }
}
