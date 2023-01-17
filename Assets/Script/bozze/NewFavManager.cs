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

public class NewFavManager : MonoBehaviour
{
    [SerializeField]
    private GameObject menu;
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
    private GameObject rec;
    [SerializeField]
    private GameObject fav;
    [SerializeField]
    private string url = "https://127.0.0.1:8573/favorites";

    private string history = "";
    private int maxNew = 3;

    void Start()
    {
        ReadHistortyFromFile();
        StartCoroutine(PopulateHistory());
        StartCoroutine(UpdateCollection());
        menu.GetComponent<MenuManager>().OnHistoryReaded();

        Debug.Log("HISTORY FILE: " + history);
        

        //StartCoroutine(GetFavorites());

        //// CHIAMARE METODO PER POPOLARE LA LISTA DEI PREFERITI
        //// PASSARE LA LISTA, O USARE UN CAMPO GLOBALE, FORSE MEGLIO GLOBALE
        //StartCoroutine(PopulateFavorites());
    }

    void OnDestroy()
    {
        SaveHistoryToFile2();
    }

    async void SaveHistoryToFile()
    {
#if WINDOWS_UWP
        Debug.Log("salvato");
        if (Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.ContainsItem("PickedFolderToken"))   
        {  
            Windows.Storage.StorageFolder storageFolder = await Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.GetFolderAsync("PickedFolderToken");  
            Windows.Storage.StorageFile historyFile = await storageFolder.CreateFileAsync("History.txt", Windows.Storage.CreationCollisionOption.OpenIfExists);  
            await Windows.Storage.FileIO.WriteTextAsync(historyFile, "ikea");
        }
#endif
    }

    async void SaveHistoryToFile2()
    {
#if ENABLE_WINMD_SUPPORT
        Debug.Log("creato");
        // Create sample file; replace if exists.
        Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        Windows.Storage.StorageFile historyFile = await storageFolder.CreateFileAsync("History.txt", Windows.Storage.CreationCollisionOption.ReplaceExisting);
#endif
        byte[] byteArray = System.Text.Encoding.UTF8.GetBytes("matteo");
        File.WriteAllBytes("./History.txt", byteArray);
    }

    async void ReadHistortyFromFile()
    {
#if ENABLE_WINMD_SUPPORT
        Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        Windows.Storage.StorageFile historyFile = await storageFolder.GetFileAsync("History.txt");
        history = await Windows.Storage.FileIO.ReadTextAsync(historyFile);
#endif
        history = System.Text.Encoding.UTF8.GetString(File.ReadAllBytes("./History.txt"));
        Debug.Log(history);
    }
    

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

    private IEnumerator UpdateCollection()
    {
        Transform scroll = rec.transform.Find("Hisotry").Find("ScrollingObjectCollection");
        Transform grid = scroll.Find("Container").Find("GridObjectCollection");

        if (grid.childCount == 0)
        {
            scroll.gameObject.SetActive(false);
        }
        else
        {
            scroll.gameObject.SetActive(true);
            if (grid.childCount <= maxNew)
            {
                scroll.GetComponent<ScrollingObjectCollection>().CellsPerTier = grid.childCount;
            }
            else
            {
                scroll.GetComponent<ScrollingObjectCollection>().CellsPerTier = maxNew;
            }
        }

        yield return new WaitForEndOfFrame();
        grid.GetComponent<GridObjectCollection>().UpdateCollection();
        yield return new WaitForEndOfFrame();
        scroll.GetComponent<ScrollingObjectCollection>().UpdateContent();
    }

    private IEnumerator PopulateHistory()
    {
        Transform scroll = rec.transform.Find("Hisotry").Find("ScrollingObjectCollection");
        Transform grid = scroll.Find("Container").Find("GridObjectCollection");

        PressableButtonHoloLens2 button = buttonPrefab.GetComponent<PressableButtonHoloLens2>();
        button.GetComponent<ButtonConfigHelper>().IconStyle = ButtonIconStyle.None;


        // DA MODIFICARE, NON VERRANNO RITORNATE DELLE STRINGHE, MA JSON DEI PDF
        button.GetComponent<ButtonConfigHelper>().MainLabelText = history;
        button.GetComponent<Interactable>().OnClick.AddListener(() => rec.GetComponent<NewFavManager>().CallUpdateHistory(history));
        GameObject gameObjectButton = Instantiate(buttonPrefab, grid);
        gameObjectButton.name = history;


        yield return new WaitForEndOfFrame();
        grid.GetComponent<GridObjectCollection>().UpdateCollection();
        yield return new WaitForEndOfFrame();
        scroll.GetComponent<ScrollingObjectCollection>().UpdateContent();
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
