using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using TMPro;
using System.Threading;
using System.Runtime.InteropServices.ComTypes;
using System.Collections.Generic;

public class FetchPdfs : MonoBehaviour
{
    public GameObject pdfViewObject = null;
    public GameObject NotFoundObject = null;
    public GameObject PagesObject = null;
    public GameObject inputField = null;
    //public GameObject favCollection = null;
    //public GameObject favButton = null;

    public ScrollingObjectCollection firstScrollView;
    public ScrollingObjectCollection secondScrollView;
    public GameObject imageButton;
    public float cellWidth = 0.2f;

    private string serverIp;
    private int serverPort = 8573;
    private string baseUrl;
    private GridObjectCollection firstGridObjectCollection;
    private GridObjectCollection secondGridObjectCollection;
    private PdfsKeywordResponse pdfRes;

    void Start()
    {
        SetServerIp("127.0.0.1"); //TODO REMOVE

        firstScrollView.CellWidth = cellWidth;
        firstGridObjectCollection = firstScrollView.GetComponentInChildren<GridObjectCollection>();
        secondScrollView.CellWidth = cellWidth;
        secondGridObjectCollection = secondScrollView.GetComponentInChildren<GridObjectCollection>();
        PagesObject.SetActive(false);

        //StartCoroutine(GetPdfs(""));
    }

    void Update() {}


    public IEnumerator GetPdfs(string searchKey)
    {
        Debug.Log("Get pdfs");
        string newUrl = baseUrl;
        if (!searchKey.Equals(""))
            newUrl = baseUrl + "?keyword=" + searchKey;
        Debug.Log(baseUrl);
        PagesObject.SetActive(false);

        UnityWebRequest webRequest = UnityWebRequest.Get(newUrl);
        webRequest.certificateHandler = new BypassCertificate();
        yield return webRequest.SendWebRequest();
        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            pdfRes = PdfsKeywordResponse.CreateFromJSON(webRequest.downloadHandler.text);
            Debug.Log(pdfRes);
            MakePdfList(pdfRes.pdfs);

            if (pdfRes.pdfs.Length > 0)
            {
                NotFoundObject.SetActive(false);
            }
            else
            {
                NotFoundObject.SetActive(true);
            }

            firstGridObjectCollection.UpdateCollection();
            firstScrollView.UpdateContent();
        }
        else
        {
            NotFoundObject.SetActive(true);
            Debug.Log(webRequest.error);
        }
    }

    public void OnSearch()
    {
        Debug.Log(inputField);
        Debug.Log(inputField.GetComponent<TMP_InputField>().text);
        ClearFirstGrid();
        ClearSecondGrid();
        StartCoroutine(GetPdfs(inputField.GetComponent<TMP_InputField>().text));
    }

    public void SetServerIp(string serverIp)
    {
        this.serverIp = serverIp;
        baseUrl = "https://" + serverIp + ":" + serverPort.ToString() + "/pdfs";
    }

    private void ClearFirstGrid()
    {
        while (firstGridObjectCollection.transform.childCount > 0)
            DestroyImmediate(firstGridObjectCollection.transform.GetChild(0).gameObject);
    }

    private void ClearSecondGrid()
    {
        while (secondGridObjectCollection.transform.childCount > 0)
            DestroyImmediate(secondGridObjectCollection.transform.GetChild(0).gameObject);
    }

    private void OnPdfClick(PdfResponse pdf)
    {
        ClearSecondGrid();
        foreach (PdfResponse p in pdfRes.pdfs)
            p.selected.GetComponent<MeshRenderer>().enabled = false;

        MakePageList(pdf);
        pdf.selected.GetComponent<MeshRenderer>().enabled = true;
        secondGridObjectCollection.UpdateCollection();
        secondScrollView.UpdateContent();
    }

    private void OnPageClick(PageResponse page)
    {
        if(pdfViewObject != null)
        {
            PdfManager pdfObj = pdfViewObject.GetComponent<PdfManager>();
            pdfObj.pdfId = page.pdfId;
            pdfObj.currentPageNumber = page.number;
            GameObject newPdfObj = Instantiate(pdfViewObject);
            newPdfObj.SetActive(true);
            //transform.gameObject.SetActive(false);

        } 
        else
        {
            Debug.Log("No pdfViewObject found!");
        }
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

    private void MakePdfList(PdfResponse[] pdfs)
    {
        ClearFirstGrid();
        List<GameObject> objs = new List<GameObject>();
        foreach (PdfResponse pdf in pdfs)
        {
            GameObject itemInstance = Instantiate(imageButton, firstGridObjectCollection.transform);
            objs.Add(itemInstance);

            itemInstance.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            itemInstance.name = pdf.name;
            itemInstance.GetComponentInChildren<TextMeshPro>().text = pdf.name;
            itemInstance.SetActive(true);

            GameObject selected = itemInstance.transform.Find("Selected").gameObject;
            selected.GetComponent<MeshRenderer>().enabled = false;

            pdf.selected = selected;
            pdf.button = itemInstance;

            ButtonConfigHelper button = itemInstance.GetComponent<ButtonConfigHelper>();
            button.OnClick.AddListener(() => { OnPdfClick(pdf); });

            GameObject container = itemInstance.transform.Find("ImageContainer").gameObject;
            GameObject image = container.transform.Find("Image").gameObject;
            container.transform.localScale = ChangeScale(image.transform.localScale, pdf.thumbnailWidth, pdf.thumbnailHeight);

            Material mat = new Material(image.GetComponent<UnityEngine.UI.Image>().material.shader);
            mat.mainTexture = Base64ToTexture(pdf.thumbnail, pdf.thumbnailWidth, pdf.thumbnailHeight);
            image.GetComponent<UnityEngine.UI.Image>().material = mat;

        }
        firstGridObjectCollection.UpdateCollection();
        firstScrollView.UpdateContent();
        firstScrollView.MoveToIndex(0);
    }


    private void MakePageList(PdfResponse pdf)
    {
        ClearSecondGrid();
        if (pdf.pages.Length > 0)
            PagesObject.SetActive(true);
        else
            PagesObject.SetActive(false);

        foreach (PageResponse page in pdf.pages)
        {
            GameObject itemInstance = Instantiate(imageButton, secondGridObjectCollection.transform);
            itemInstance.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            itemInstance.name = pdf.name + "_" + page.number.ToString();
            itemInstance.GetComponentInChildren<TextMeshPro>().text = "Page " + page.number.ToString();
            itemInstance.SetActive(true);

            GameObject selected = itemInstance.transform.Find("Selected").gameObject;
            selected.GetComponent<MeshRenderer>().enabled = false;

            ButtonConfigHelper button = itemInstance.GetComponent<ButtonConfigHelper>();
            button.OnClick.AddListener(() => { OnPageClick(page); });

            GameObject container = itemInstance.transform.Find("ImageContainer").gameObject;
            GameObject image = container.transform.Find("Image").gameObject;
            container.transform.localScale = ChangeScale(image.transform.localScale, page.thumbnailWidth, page.thumbnailHeight);

            Material mat = new Material(image.GetComponent<UnityEngine.UI.Image>().material.shader);
            mat.mainTexture = Base64ToTexture(page.thumbnail, page.thumbnailWidth, page.thumbnailHeight);
            image.GetComponent<UnityEngine.UI.Image>().material = mat;
        }
        secondGridObjectCollection.UpdateCollection();
        secondScrollView.UpdateContent();
        secondScrollView.MoveToIndex(0);
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

    [Serializable]
    public class PdfsKeywordResponse
    {
        public PdfResponse[] pdfs;

        public static PdfsKeywordResponse CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<PdfsKeywordResponse>(jsonString);
        }

    }

    [Serializable]
    public class PdfResponse
    {
        public string _id;
        public string name;
        public int numOccKeyword;
        public PageResponse[] pages;
        public string thumbnail;
        public int thumbnailWidth = 0;
        public int thumbnailHeight = 0;

        public GameObject selected;
        public GameObject button;

        public static PdfResponse CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<PdfResponse>(jsonString);
        }

    }

    [Serializable]
    public class PageResponse
    {
        public int number;
        public string pdfId;
        public string thumbnail;
        public int thumbnailWidth = 0;
        public int thumbnailHeight = 0;
        public string url;
        
        public static PageResponse CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<PageResponse>(jsonString);
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