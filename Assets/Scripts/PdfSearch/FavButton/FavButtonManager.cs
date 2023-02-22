using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FavButtonManager : MonoBehaviour
{
    public bool selected = false;
    public string pdfId = "";

    public string serverIp;
    private int serverPort = 8573;
    private string baseUrl;

    void Start()
    {
        baseUrl = "https://" + serverIp + ":" + serverPort.ToString() + "/favs/" + pdfId;
        this.gameObject.transform.Find("Star").GetComponent<RawImage>().enabled = !selected;
        this.gameObject.transform.Find("StarFilled").GetComponent<RawImage>().enabled = selected;
    }

    void Update()
    {
        
    }

    public void OnClick()
    {
        selected = !selected;
        this.gameObject.transform.Find("Star").GetComponent<RawImage>().enabled = !selected;
        this.gameObject.transform.Find("StarFilled").GetComponent<RawImage>().enabled = selected;
        StartCoroutine(PostFav(selected));
    }


    public IEnumerator PostFav(bool selected)
    {
        Debug.Log("Post fav");
        var form = new List<IMultipartFormSection>
        {
            new MultipartFormDataSection("value", selected.ToString()),
        };

        UnityWebRequest webRequest = UnityWebRequest.Post(baseUrl, form);
        webRequest.certificateHandler = new BypassCertificate();
        yield return webRequest.SendWebRequest();

        if (webRequest.isHttpError || webRequest.isNetworkError)
        {
            Debug.Log(webRequest.error);
        }
        else
        {
            Debug.Log("Fav updated");
        }
    }
}
