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

    private GridObjectCollection firstGridObjectCollection;
    private GridObjectCollection secondGridObjectCollection;

    public string serverIp;
    private int serverPort = 8573;
    private string baseUrl;

    private PdfsResponse pdfRes;
    private List<VisiblePdf> visiblePdfs = new List<VisiblePdf>();

    private bool orderMostViewed;
    private bool onlyFavourites;
    private bool orderMoreOccurencies;

    void Start()
    {
        //SetServerIp("127.0.0.1"); //TODO REMOVE

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
        if (webRequest.isHttpError || webRequest.isNetworkError)
        {
            NotFoundObject.SetActive(true);
            Debug.Log(webRequest.error);
        }
        else
        {
            pdfRes = PdfsResponse.CreateFromJSON(webRequest.downloadHandler.text);
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
    }

    public void OnSearch()
    {
        OnSearch(inputField.GetComponent<TMP_InputField>().text);
    }

    public void OnSearch(string text)
    {
        inputField.GetComponent<TMP_InputField>().text = text;
        ClearFirstGrid();
        ClearSecondGrid();
        Debug.Log("CHAIAMTo");
        StartCoroutine(GetPdfs(text));
    }

    public void UpdateBaseUrl()
    {
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

    private void OnPdfClick(Pdf pdf)
    {
        ClearSecondGrid();
        foreach (VisiblePdf p in visiblePdfs)
        {
            if(p.pdfId == pdf._id)
                p.selected.GetComponent<MeshRenderer>().enabled = true;
            else
                p.selected.GetComponent<MeshRenderer>().enabled = false;
        }

        MakePageList(pdf);
        secondGridObjectCollection.UpdateCollection();
        secondScrollView.UpdateContent();
    }

    private void OnPageClick(Page page)
    {
        if(pdfViewObject != null)
        {
            PdfManager pdfObj = pdfViewObject.GetComponent<PdfManager>();
            pdfObj.pdfId = page.pdfId;
            pdfObj.currentPageNumber = page.number;
            pdfObj.serverIp = serverIp;
            GameObject newPdfObj = Instantiate(pdfViewObject);
            newPdfObj.SetActive(true);
            //transform.gameObject.SetActive(false); Uncomment to hide pdf search

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

    private void MakePdfList(Pdf[] pdfs)
    {
        ClearFirstGrid();
        visiblePdfs.Clear();
        foreach (Pdf pdf in pdfs)
        {
            GameObject itemInstance = Instantiate(imageButton, firstGridObjectCollection.transform);

            itemInstance.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            itemInstance.name = pdf.name;
            itemInstance.GetComponentInChildren<TextMeshPro>().text = pdf.name;
            itemInstance.SetActive(true);

            GameObject favButton = itemInstance.transform.Find("FavButton").gameObject;
            favButton.SetActive(true);
            FavButtonManager favButtonManager = favButton.GetComponent<FavButtonManager>();
            favButtonManager.serverIp = serverIp;
            favButtonManager.pdfId = pdf._id;
            favButtonManager.selected = pdf.isFav;

            GameObject selected = itemInstance.transform.Find("Selected").gameObject;
            selected.GetComponent<MeshRenderer>().enabled = false;

            visiblePdfs.Add(new VisiblePdf(pdf._id, selected, itemInstance));

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


    private void MakePageList(Pdf pdf)
    {
        ClearSecondGrid();
        if (pdf.pages.Length > 0)
            PagesObject.SetActive(true);
        else
            PagesObject.SetActive(false);

        foreach (Page page in pdf.pages)
        {
            GameObject itemInstance = Instantiate(imageButton, secondGridObjectCollection.transform);
            itemInstance.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            itemInstance.name = pdf.name + "_" + page.number.ToString();
            itemInstance.GetComponentInChildren<TextMeshPro>().text = "Page " + page.number.ToString();
            itemInstance.SetActive(true);

            GameObject favButton = itemInstance.transform.Find("FavButton").gameObject;
            favButton.SetActive(false);

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

    private class VisiblePdf
    {
        public string pdfId;
        public GameObject selected;
        public GameObject button;

        public VisiblePdf(string pdfId, GameObject selected, GameObject button)
        {
            this.pdfId = pdfId;
            this.selected = selected; 
            this.button = button;
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