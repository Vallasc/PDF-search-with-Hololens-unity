using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;

using UnityEngine.Networking;
using TMPro;
using System;
using System.IO;

public class FavoritesManager : MonoBehaviour
{
    [SerializeField]
    private GameObject menu;
    [SerializeField]
    private GameObject menuFav;

    [SerializeField]
    private GameObject buttonPrefab;

    [SerializeField]
    private float textHight = 0.01f;
    [SerializeField]
    private float buttonWidth = 0.032f;
    [SerializeField]
    private float padding = 0.01f;
    [SerializeField]
    private float offsetY = -0.05f;

    [SerializeField]
    private string url = "https://127.0.0.1:8573/favorites";

    private int maxFav = 3;

    void Start()
    {
        //StartCoroutine(GetFavorites());

        //// CHIAMARE METODO PER POPOLARE LA LISTA DEI PREFERITI
        //// PASSARE LA LISTA, O USARE UN CAMPO GLOBALE, FORSE MEGLIO GLOBALE
        //StartCoroutine(PopulateFavorites());
    }

    private IEnumerator GetFavorites()
    {
        Debug.Log("GET favorites list");


        UnityWebRequest uwr = UnityWebRequest.Get(url);
        uwr.certificateHandler = new BypassCertificate();
        yield return uwr.SendWebRequest();

        if (uwr.isHttpError || uwr.isNetworkError)
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
        Transform scroll = menuFav.transform.Find("Favorites").Find("ScrollingObjectCollection");
        Transform grid = scroll.Find("Container").Find("GridObjectCollection");


        //PressableButtonHoloLens2 button = buttonPrefab.GetComponent<PressableButtonHoloLens2>();
        //button.GetComponent<ButtonConfigHelper>().IconStyle = ButtonIconStyle.None;

        //// DA MODIFICARE, NON VERRANNO RITORNATE DELLE STRINGHE, MA JSON DEI PDF
        //button.GetComponent<ButtonConfigHelper>().MainLabelText = ;
        //GameObject gameObjectButton = Instantiate(buttonPrefab, grid);
        //gameObjectButton.name = ;

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

    private IEnumerator UpdateCollection()
    {
        Transform scroll = menuFav.transform.Find("Favorites").Find("ScrollingObjectCollection");
        Transform grid = scroll.Find("Container").Find("GridObjectCollection");

        if (grid.childCount == 0)
        {
            scroll.gameObject.SetActive(false);
        }
        else
        {
            scroll.gameObject.SetActive(true);
            if (grid.childCount <= maxFav)
            {
                scroll.GetComponent<ScrollingObjectCollection>().CellsPerTier = grid.childCount;
            }
            else
            {
                scroll.GetComponent<ScrollingObjectCollection>().CellsPerTier = maxFav;
            }
        }

        yield return new WaitForEndOfFrame();
        grid.GetComponent<GridObjectCollection>().UpdateCollection();
        yield return new WaitForEndOfFrame();
        scroll.GetComponent<ScrollingObjectCollection>().UpdateContent();
    }
}
