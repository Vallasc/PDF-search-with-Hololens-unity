using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using Microsoft.MixedReality.Toolkit.UI;

public class SearchUpdater : MonoBehaviour
{
    [SerializeField]
    private GameObject menu;

    [SerializeField]
    private GameObject pdfSearch;

    //public GameObject searchBar;
    //public GameObject buttonMenu;
    //public GameObject buttonSearch;
    //private int numKeys = 0;
    //private readonly string url = "https://127.0.0.1:8574/upload";
    //private string iconSearchString = "IconSearch";
    //private string iconCloseString = "IconClose";
    //public MeshRenderer searchRenderer;
    //public MeshRenderer menuRenderer;
    //public Material whiteMaterial;
    //public Material blackMaterial;


    public void SetActivePdfSearch()
    {
        menu.SetActive(false);
        pdfSearch.SetActive(true);
    }

//    public void UpdateSearchBarGraphic()
//    {
//        if (searchBar.activeSelf)
//        {
//            buttonMenu.GetComponent<ButtonConfigHelper>().SetQuadIconByName(iconSearchString);
//            searchRenderer.material = whiteMaterial;
//            menuRenderer.material = whiteMaterial;
//        }
//        else
//        {
//            buttonMenu.GetComponent<ButtonConfigHelper>().SetQuadIconByName(iconCloseString);
//            searchRenderer.material = blackMaterial;
//            menuRenderer.material = blackMaterial;
//        }

//        searchBar.SetActive(!searchBar.activeSelf);
//        Transform transform = buttonMenu.GetComponent<Transform>();
//        transform.localPosition = new Vector3(transform.localPosition.x * -1, transform.localPosition.y, transform.localPosition.z);
//        buttonSearch.SetActive(!buttonSearch.activeSelf);

//        if (buttonSearch.activeSelf)
//        {
//            buttonSearch.GetComponent<ButtonConfigHelper>().SetQuadIconByName(iconSearchString);
//        }
//    }

//    IEnumerator PostSearchKeyword(string keyword)
//    {
//        Debug.Log("POST keyword inserted");

//        numKeys++;

//        var sections = new List<IMultipartFormSection>
//        {
//            new MultipartFormDataSection("keyword", keyword),
//            new MultipartFormDataSection("request_index", numKeys.ToString())
//        };

//        using UnityWebRequest uwr = UnityWebRequest.Post(url, sections);
//        uwr.certificateHandler = new BypassCertificate();
//        yield return uwr.SendWebRequest();

//        if (uwr.result != UnityWebRequest.Result.Success)
//        {
//            Debug.Log(uwr.error);
//        }
//        else
//        {
//            Debug.Log("RICHIESTA POST ESEGUITA!");
//            var serverResponse = uwr.downloadHandler.text;

//            Debug.Log(serverResponse);
//        }
//    }

//    public void OnKeywordInserted()
//    {
//        string text;

//        if (!searchBar.activeSelf)
//        {
//            return;
//        }

//        text = searchBar.GetComponent<TMP_InputField>().text;

//        if (text == "" || text == null)
//        {
//            return;
//        }

//        Debug.Log(text);
//        StartCoroutine(PostSearchKeyword(text));
//    }
}
