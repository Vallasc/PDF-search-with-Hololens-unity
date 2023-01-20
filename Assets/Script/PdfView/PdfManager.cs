using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class PdfManager : MonoBehaviour
{
    public string pdfId = "2";
    public int currentPageNumber = 0;
    private int totalPages = 0;

    public GameObject imageObject;
    public GameObject windowTitleObject;
    public GameObject pageCounterObject;

    private string serverIp;
    private int serverPort = 8573;
    private string baseUrl;
    private Pdf pdf = null;

    void Start()
    {
        SetServerIp("127.0.0.1"); // TODO REMOVE

        StartCoroutine(GetPdf(pdfId));
        StartCoroutine(GetPage(pdfId, currentPageNumber));
    }

    void Update() {}

    public void SetServerIp(string serverIp)
    {
        this.serverIp = serverIp;
        baseUrl = "https://" + serverIp + ":" + serverPort.ToString() + "/pdfs";
    }

    public void GoPreviousPage()
    {
        int newPageNumber = currentPageNumber - 1;
        if (newPageNumber >= 0)
        {
            currentPageNumber = newPageNumber;
            StartCoroutine(GetPage(pdfId, newPageNumber));
            this.GetComponent<SliderManager>().SlideToPage(newPageNumber, totalPages);
        }
    }

    public void GoNextPage()
    {
        int newPageNumber = currentPageNumber + 1;
        if (newPageNumber < totalPages)
        {
            currentPageNumber = newPageNumber;
            StartCoroutine(GetPage(pdfId, newPageNumber));
            this.GetComponent<SliderManager>().SlideToPage(newPageNumber, totalPages);
        }
    }

    public int getTotalPageCount()
    {
        return totalPages;
    }

    public int getCurrentPageNumber()
    {
        return currentPageNumber;
    }

    public void goToPagePageNumber(int pageNumber)
    {
        currentPageNumber = pageNumber;
        StartCoroutine(GetPage(pdfId, pageNumber));
    }

    private IEnumerator GetPdf(string pdfId)
    {
        Debug.Log("GET pdf");
        string url = baseUrl + "/" + pdfId;
        Debug.Log(url);
        UnityWebRequest webRequest = UnityWebRequest.Get(url);
        webRequest.certificateHandler = new BypassCertificate();
        yield return webRequest.SendWebRequest();
        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            this.pdf = Pdf.CreateFromJSON(webRequest.downloadHandler.text);
            totalPages = this.pdf.numPages;
            SetWindowTitle(this.pdf.name);
            SetPageCounter(currentPageNumber, totalPages);
            this.GetComponent<SliderManager>().SlideToPage(currentPageNumber, totalPages);
        }
        else
        {
            Debug.Log(webRequest.error);
        }

    }

    private IEnumerator GetPage(string pdfId, int pageNumber)
    {
        Debug.Log("GET page");
        UnityWebRequest webRequest = UnityWebRequest.Get(baseUrl + "/" + pdfId + "/" + pageNumber.ToString());
        webRequest.certificateHandler = new BypassCertificate();
        yield return webRequest.SendWebRequest();
        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            PageImage res = PageImage.CreateFromJSON(webRequest.downloadHandler.text);
            PaintImage(res.img, res.width, res.height);
            if (this.pdf != null)
            {
                SetPageCounter(pageNumber + 1, totalPages);
                this.GetComponent<SliderManager>().SlideToPage(currentPageNumber, totalPages);
            }
        }
        else
        {
            Debug.Log(webRequest.error);
        }

    }

    private void SetWindowTitle(string title)
    {
        this.windowTitleObject.GetComponent<TextMeshPro>().text = title;
    }
    private void SetPageCounter(int currentpage, int totalpages)
    {
        this.pageCounterObject.GetComponent<TextMeshPro>().text = currentpage.ToString() + "/" + totalpages.ToString();
    }

    private void PaintImage(string base64Image, int width, int height)
    {
        imageObject.transform.localScale = ChangeScale(imageObject.transform.localScale, width, height);
        Material mat = new Material(imageObject.GetComponent<UnityEngine.UI.Image>().material.shader);
        mat.mainTexture = Base64ToTexture(base64Image, width, height);
        imageObject.GetComponent<UnityEngine.UI.Image>().material = mat;
    }

    private Texture2D Base64ToTexture(string base64Image, int width, int height)
    {
        Byte[] imgBytes = Convert.FromBase64String(base64Image);
        Texture2D tex = new Texture2D(width, height, TextureFormat.BC7, false);
        tex.LoadImage(imgBytes);
        return tex;
    }

    private Vector3 ChangeScale(Vector3 localScale, int width, int height)
    {
        Vector3 initialScale = localScale;

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
        return initialScale;
    }

    [Serializable]
    public class Pdf
    {
        public string _id;
        public string name;
        public string path;
        public int numPages;
        public List<Pages> pages;

        public static Pdf CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<Pdf>(jsonString);
        }
    }

    [Serializable]
    public class Pages
    {
        public int number;
        public string pdfId;
        public string path;
        public string url;
        public static Pages CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<Pages>(jsonString);
        }
    }

    [Serializable]
    public class PageImage
    {
        public string img;
        public int width;
        public int height;
        public static PageImage CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<PageImage>(jsonString);
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
