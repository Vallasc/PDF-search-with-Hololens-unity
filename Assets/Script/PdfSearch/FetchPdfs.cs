using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FetchPdfImage;
using UnityEngine.Networking;
using System;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using TMPro;
using UnityEngine.UI;

public class FetchPdfs : MonoBehaviour
{
    public string url = "https://127.0.0.1/pdf";
    public ScrollingObjectCollection scrollView;
    public GameObject imageButton;
    public float cellWidth = 0.2f;

    private GridObjectCollection gridObjectCollection;

    // Start is called before the first frame update
    void Start()
    {
        
        scrollView.CellWidth = cellWidth;
        gridObjectCollection = scrollView.GetComponentInChildren<GridObjectCollection>();

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
            //PaintImage(res.img, res.width, res.height);


            // Make items
            MakeItem("Provola1", res.img, res.width, res.height);
            MakeItem("Provola2", res.img, res.width, res.height);
            MakeItem("Provola3", res.img, res.width, res.height);
            MakeItem("Provola4", res.img, res.width, res.height);
            MakeItem("Provola5", res.img, res.width, res.height);
            MakeItem("Provola6", res.img, res.width, res.height);
            MakeItem("Provola7", res.img, res.width, res.height);
            gridObjectCollection.UpdateCollection();
            scrollView.UpdateContent();
        }
        else
        {
            Debug.Log(webRequest.error);
        }
    }

    private void MakeItem(string title, string base64Image, int width, int height)
    {
        Debug.Log(title);
        GameObject itemInstance = Instantiate(imageButton, gridObjectCollection.transform);
        itemInstance.transform.localScale = new Vector3(1, 1, 1);
        itemInstance.name = title;
        itemInstance.GetComponentInChildren<TextMeshPro>().text = title;
        itemInstance.SetActive(true);


        Byte[] imgBytes = Convert.FromBase64String(base64Image);
        Texture2D tex = new Texture2D(width, height, TextureFormat.BC7, false);
        tex.LoadImage(imgBytes);
        itemInstance.transform.Find("Image").GetComponent<Image>().material.mainTexture = tex;


        Vector3 initialScale = itemInstance.transform.localScale;

        if (width < height)
        {
            float aspectRatio = (float)width / (float)height;
            initialScale.x = aspectRatio;
            initialScale.y = 1;
        }
        else
        {
            float aspectRatio = (float)width / (float)height;
            initialScale.x = 1;
            initialScale.y = aspectRatio;
        }
        itemInstance.transform.localScale = initialScale;
    }

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

    public class BypassCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }
}