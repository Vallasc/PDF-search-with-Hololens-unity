using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;

using UnityEngine.Networking;
using TMPro;

public class NewFavManager : MonoBehaviour
{
    [SerializeField]
    private GameObject menuNew;

    [SerializeField]
    private GameObject menuFav;

    [SerializeField]
    private GameObject PdfCollection;

    public GameObject buttonPrefab;

    private bool shifted = false;

    [SerializeField]
    private GameObject backplateHistory = null;
    [SerializeField]
    private GameObject backplateFavorites = null;
    [SerializeField]
    private float textHight = 0.01f;
    [SerializeField]
    private float buttonWidth = 0.032f;
    [SerializeField]
    private float padding = 0.01f;
    [SerializeField]
    private float offsetY = -0.05f;


    [SerializeField]
    private GameObject fav;

    private string url = "https://127.0.0.1:8573/favorites";

    //void Start()
    //{
    //    StartCoroutine(GetFavorites());

    //    // CHIAMARE METODO PER POPOLARE LA LISTA DEI PREFERITI
    //    // PASSARE LA LISTA, O USARE UN CAMPO GLOBALE, FORSE MEGLIO GLOBALE
    //    StartCoroutine(PopulateFavorites());
    //}

    IEnumerator GetFavorites()
    {
        Debug.Log("GET favorites list");
        

        using UnityWebRequest uwr = UnityWebRequest.Get(url);
        uwr.certificateHandler = new BypassCertificate();
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(uwr.error);
        }
        else
        {
            var serverResponse = uwr.downloadHandler.text;

            Debug.Log(serverResponse);
        }
    }

    public IEnumerator PopulateFavorites()
    {
        Transform scroll = fav.transform.Find("ScrollingObjectCollection");
        Transform grid = scroll.Find("Container").Find("GridObjectCollection");


        PressableButtonHoloLens2 button = GameObject.Find("PressableButtonHoloLens2_32x96_NoLabel").GetComponent<PressableButtonHoloLens2>();
        
        // FOR SULLA LISTA RITORNATA

        //for (int i = 0; i < count; i++)
        //{
        //    Instantiate(button, grid.transform);
        //}

        //button.GetComponent<ButtonConfigHelper>().MainLabelText = "";
        //button.GetComponent<Interactable>().OnClick.AddListener(() => PdfCollection.GetComponent<NewFavManager>().CallUpdateHistory(""));

        // DA INSERIRE ANCHE IL COLLEGAMENTO ALLA SCHERMATA SUCCESSIVA

        yield return new WaitForEndOfFrame();
        grid.GetComponent<GridObjectCollection>().UpdateCollection();
        yield return new WaitForEndOfFrame();
        scroll.GetComponent<ScrollingObjectCollection>().UpdateContent();
    }



    public float UpdateBackplateHistory()
    {
        Transform quad = backplateHistory.transform.Find("Quad");

        float collectionHeight = menuNew.transform.childCount * buttonWidth;
        float quadScaleY = padding + textHight + padding + collectionHeight;
        if (menuNew.transform.childCount > 0)
        {
            quadScaleY += collectionHeight + padding;
        }

        quad.localScale = new Vector3(quad.localScale.x, quadScaleY, quad.localScale.z);

        backplateHistory.transform.localPosition = new Vector3(backplateHistory.transform.localPosition.x, quadScaleY / 2f, backplateHistory.transform.localPosition.z);

        return quadScaleY * -1;
    }

    public void UpdateBackplateFavorites(float offset)
    {
        Transform quad = backplateFavorites.transform.Find("Quad");

        float collectionHeight = menuFav.transform.childCount * buttonWidth;
        float quadScaleY = padding + textHight + padding + collectionHeight;
        if (menuNew.transform.childCount > 0)
        {
            quadScaleY += collectionHeight + padding;
        }

        quad.localScale = new Vector3(quad.localScale.x, quadScaleY, quad.localScale.z);

        backplateFavorites.transform.localPosition = new Vector3(backplateFavorites.transform.localPosition.x, quadScaleY / 2f, backplateFavorites.transform.localPosition.z);

        float finalOffset = offset + offsetY;
    }



    public void ShiftUpMenuFav()
    {
        menuFav.transform.localPosition += new Vector3(0, 0.11225f, 0); //0.032f * 3
    }

    public void ShiftDownMenuFav()
    {
        menuFav.transform.localPosition -= new Vector3(0, 0.11225f, 0);
    }

    public void UpdateHistoryGraphic()
    {
        menuNew.SetActive(!menuNew.activeSelf);

        if (menuNew.activeSelf)
        {
            ShiftDownMenuFav();
        }
        else
        {
            ShiftUpMenuFav();
        }

        UpdateMenuGraphic();
    }

    public void UpdateFavoritesGraphic()
    {
        menuFav.SetActive(!menuFav.activeSelf);

        UpdateMenuGraphic();
    }

    private void UpdateMenuGraphic()
    {
        if (!menuNew.activeSelf && !menuFav.activeSelf)
        {
            shifted = true;
            PdfCollection.GetComponent<PdfCollectionUpdater>().ShiftLeftPdfCollection();
        }
        else if (shifted)
        {
            shifted = false;
            PdfCollection.GetComponent<PdfCollectionUpdater>().ShiftRightPdfCollection();
        }
    }

    public void CallUpdateHistory(string newPdf)
    {
        StartCoroutine(UpdateHistory(newPdf));
    }

    private IEnumerator UpdateHistory(string newPdf)
    {
        Debug.Log("HISTORY");
        bool found = false;
        Transform grid = menuNew.transform.Find("ScrollingObjectCollection").Find("Container").Find("GridObjectCollection");

        Debug.Log(grid.childCount);
        int i = 0;
        while (!found && i < grid.childCount)
        {
            Debug.Log(i);
            if (!found && grid.GetChild(i).GetComponent<ButtonConfigHelper>().MainLabelText == newPdf)
            {
                if (i != 2)
                {
                    grid.GetChild(i).SetAsLastSibling();
                }

                found = true;
            }

            i++;
        }


        if (!found)
        {
            if (grid.childCount == 3)
            {
                Destroy(grid.GetChild(0));
            }

            PressableButtonHoloLens2 button = GameObject.Find("PressableButtonHoloLens2_32x96_NoLabel").GetComponent<PressableButtonHoloLens2>();
            Instantiate(button, grid);
            button.GetComponent<ButtonConfigHelper>().MainLabelText = newPdf;
            button.GetComponent<Interactable>().OnClick.AddListener(() => PdfCollection.GetComponent<NewFavManager>().CallUpdateHistory(newPdf));

            // DA INSERIRE ANCHE IL COLLEGAMENTO ALLA SCHERMATA SUCCESSIVA
        }


        yield return new WaitForEndOfFrame();
        grid.GetComponent<GridObjectCollection>().UpdateCollection();
        yield return new WaitForEndOfFrame();
        menuNew.transform.Find("ScrollingObjectCollection").GetComponent<ScrollingObjectCollection>().UpdateContent();
        Debug.Log("HISTORY");
    }
}
