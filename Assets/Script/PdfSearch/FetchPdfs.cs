using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using TMPro;
using UnityEngine.UI;
using static PdfManager;

public class FetchPdfs : MonoBehaviour
{
    public GameObject inputField = null;
    public string url = "https://127.0.0.1:8573/pdfs";
    public ScrollingObjectCollection firstScrollView;
    public ScrollingObjectCollection secondScrollView;
    public GameObject imageButton;
    public float cellWidth = 0.2f;

    private GridObjectCollection firstGridObjectCollection;
    private GridObjectCollection secondGridObjectCollection;
    private PdfsKeywordResponse pdfRes;

    // Start is called before the first frame update
    void Start()
    {

        firstScrollView.CellWidth = cellWidth;
        firstGridObjectCollection = firstScrollView.GetComponentInChildren<GridObjectCollection>();
        secondScrollView.CellWidth = cellWidth;
        secondGridObjectCollection = secondScrollView.GetComponentInChildren<GridObjectCollection>();

        //StartCoroutine(GetImage());
        StartCoroutine(GetPdfs("ikea"));
    }

    // Update is called once per frame
    void Update()
    {

    }


    IEnumerator GetPdfs(string searchKey)
    {
        Debug.Log("Get pdfs");
        if (!searchKey.Equals(""))
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(url + "?keyword=" + searchKey);
            webRequest.certificateHandler = new BypassCertificate();
            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                pdfRes = PdfsKeywordResponse.CreateFromJSON(webRequest.downloadHandler.text);
                Debug.Log(pdfRes);
                MakePdfList(pdfRes.pdfs);

                firstGridObjectCollection.UpdateCollection();
                firstScrollView.UpdateContent();
            }
            else
            {
                Debug.Log(webRequest.error);
            }
        }
        else
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(url);
            webRequest.certificateHandler = new BypassCertificate();
            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                pdfRes = PdfsKeywordResponse.CreateFromJSON(webRequest.downloadHandler.text);
                Debug.Log(pdfRes);
                MakePdfList(pdfRes.pdfs);

                firstGridObjectCollection.UpdateCollection();
                firstScrollView.UpdateContent();
            }
            else
            {
                Debug.Log(webRequest.error);
            }
        }
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
            // MakeItem("Provola1", res.img, res.width, res.height);
            firstGridObjectCollection.UpdateCollection();
            firstScrollView.UpdateContent();
        }
        else
        {
            Debug.Log(webRequest.error);
        }
    }

    private void ClearFirstGrid()
    {
        while (firstGridObjectCollection.transform.childCount > 0)
            DestroyImmediate(firstGridObjectCollection.transform.GetChild(0).gameObject);
        firstGridObjectCollection.UpdateCollection();
        firstScrollView.UpdateContent();
        //firstScrollView.MoveToIndex(0);
    }

    private void ClearSecondGrid()
    {
        while (secondGridObjectCollection.transform.childCount > 0)
            DestroyImmediate(secondGridObjectCollection.transform.GetChild(0).gameObject);
        secondGridObjectCollection.UpdateCollection();
        secondScrollView.UpdateContent();
        //secondScrollView.MoveToIndex(0);
    }

    void OnPdfClick(PdfResponse pdf)
    {
        ClearSecondGrid();
        foreach (PdfResponse p in pdfRes.pdfs)
            p.selected.GetComponent<MeshRenderer>().enabled = false;

        MakePageList(pdf);
        pdf.selected.GetComponent<MeshRenderer>().enabled = true;
        secondGridObjectCollection.UpdateCollection();
        secondScrollView.UpdateContent();
    }

    Texture2D Base64ToTexture(string base64Image, int width, int height)
    {
        Byte[] imgBytes = Convert.FromBase64String(base64Image);
        Texture2D tex = new Texture2D(width, height, TextureFormat.BC7, false);
        tex.LoadImage(imgBytes);
        return tex;
    }

    Vector3 ChangeScale(Vector3 localScale, int width, int height)
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
        foreach(PdfResponse pdf in pdfs)
        {
            GameObject itemInstance = Instantiate(imageButton, firstGridObjectCollection.transform);
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
    }

    private void MakePageList(PdfResponse pdf)
    {
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
            button.OnClick.AddListener(() => { Debug.Log("Click on " + itemInstance.name); });

            GameObject container = itemInstance.transform.Find("ImageContainer").gameObject;
            GameObject image = container.transform.Find("Image").gameObject;
            container.transform.localScale = ChangeScale(image.transform.localScale, page.thumbnailWidth, page.thumbnailHeight);

            Material mat = new Material(image.GetComponent<UnityEngine.UI.Image>().material.shader);
            mat.mainTexture = Base64ToTexture(page.thumbnail, page.thumbnailWidth, page.thumbnailHeight);
            image.GetComponent<UnityEngine.UI.Image>().material = mat;
        }
    }

    public void OnSearchButtonClick()
    {
        Debug.Log(inputField);
        Debug.Log(inputField.GetComponent<TMP_InputField>().text);
        ClearFirstGrid();
        ClearSecondGrid();
        StartCoroutine(GetPdfs(inputField.GetComponent<TMP_InputField>().text));
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