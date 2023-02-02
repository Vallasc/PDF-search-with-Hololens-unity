using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using static HistoryManager;

public class FavManager : MonoBehaviour
{
    public bool selected = false;
    public string pdfId;
    public string serverIp;

    private int serverPort = 8573;
    private GameObject starIconFilled;
    private GameObject starIconOutline;
    private GameObject text;
    private string url;

    void Start()
    {
        starIconOutline = this.transform.Find("IconAndText").Find("StarIcon").gameObject;
        starIconFilled = this.transform.Find("IconAndText").Find("StarIconFilled").gameObject;
        text = this.transform.Find("IconAndText").Find("TextMeshPro").gameObject;
        SetFav(selected);
    }

    void Update()
    {
        
    }

    public void OnClick()
    {
        selected = !selected;
        SetFav(selected);
    }

    public void SetFav(bool value)
    {
        url = "https://" + serverIp + ":" + serverPort.ToString() + "/favs/" + pdfId;
        starIconFilled.SetActive(value);
        starIconOutline.SetActive(!value);
        if (value)
        {
            text.GetComponent<TextMeshPro>().text = "Remove from favourites";
        } else
        {
            text.GetComponent<TextMeshPro>().text = "Add to favourites";
        }
        selected = value;
        StartCoroutine(PostFav(selected));
    }

    public IEnumerator PostFav(bool selected)
    {
        Debug.Log("Post fav");
        var form = new List<IMultipartFormSection>
        {
            new MultipartFormDataSection("value", selected.ToString()),
        };

        UnityWebRequest webRequest = UnityWebRequest.Post(url, form);
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
