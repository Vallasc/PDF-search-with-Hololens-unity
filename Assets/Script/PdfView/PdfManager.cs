using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class PdfManager : MonoBehaviour
{
    public string pdfId = "2";
    public int currentPageNumber = 2;

    public GameObject canvasObject;
    public GameObject windowTitleObject;
    public GameObject pageCounterObject;

    private string host = "https://127.0.0.1:8573";
    private readonly string basePath = "/pdfs";
    private Pdf pdf = null;


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start");
        StartCoroutine(GetPdf(pdfId));
        StartCoroutine(GetPage(pdfId, currentPageNumber));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator GetPdf(string pdfId)
    {
        Debug.Log("GET pdf");
        string url = host + basePath + "/" + pdfId;
        Debug.Log(url);
        UnityWebRequest webRequest = UnityWebRequest.Get(url);
        webRequest.certificateHandler = new BypassCertificate();
        yield return webRequest.SendWebRequest();
        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            this.pdf = Pdf.CreateFromJSON(webRequest.downloadHandler.text);
            SetWindowTitle(this.pdf.name);
            SetPageCounter(currentPageNumber, this.pdf.numPages);
        }
        else
        {
            Debug.Log(webRequest.error);
        }

    }

    private IEnumerator GetPage(string pdfId, int pageNumber)
    {
        Debug.Log("GET page");
        UnityWebRequest webRequest = UnityWebRequest.Get(host + basePath + "/" + pdfId + "/" + pageNumber.ToString());
        webRequest.certificateHandler = new BypassCertificate();
        yield return webRequest.SendWebRequest();
        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            PageImage res = PageImage.CreateFromJSON(webRequest.downloadHandler.text);
            PaintImage(res.img, res.width, res.height);
            if(this.pdf != null)
                SetPageCounter(pageNumber + 1, this.pdf.numPages);
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
        Byte[] imgBytes = Convert.FromBase64String(base64Image);
        Texture2D tex = new Texture2D(width, height, TextureFormat.BC7, false);
        tex.LoadImage(imgBytes);
        this.canvasObject.GetComponent<Renderer>().material.mainTexture = tex;

        Vector3 initialScale = this.canvasObject.transform.localScale;

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
        this.canvasObject.transform.localScale = initialScale;
    }

    public void GoPreviousPage()
    {
        int newPageNumber = currentPageNumber - 1;
        if (newPageNumber >= 0)
        {
            currentPageNumber = newPageNumber;
            StartCoroutine(GetPage(pdfId, newPageNumber));
        }
    }

    public void GoNextPage()
    {
        int newPageNumber = currentPageNumber + 1;
        if (newPageNumber < pdf.numPages)
        {
            currentPageNumber = newPageNumber;
            StartCoroutine(GetPage(pdfId, newPageNumber));
        }
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


    /*
    Pdf = {
        _id: string
        name: string
        path: string
        numPages: number
        pages: list<Page>
    }
    Page = {
        number: int
        pdfId: string
        path: string
        url: string
    }

    PageImage = {
        img: string
        width: int
        height: int
    }
    */

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
