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

public class HistoryManager : MonoBehaviour
{
    [SerializeField]
    private GameObject menu;
    [SerializeField]
    private GameObject menuNew;

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

    private string history = "";
    private int maxNew = 3;


    void Start()
    {
        ReadHistortyFromFile();
        PopulateHistory();
        //UpdateHistory(history);
        StartCoroutine(UpdateCollection());

        //yield return new WaitForEndOfFrame();
        menu.GetComponent<MenuManager>().OnHistoryUpdated();

        //Debug.Log("HISTORY FILE: " + history);
    }

    void OnDestroy()
    {
        SaveHistoryToFile2();
    }

    private async void SaveHistoryToFile()
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

    private async void SaveHistoryToFile2()
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

    private async void ReadHistortyFromFile()
    {
#if ENABLE_WINMD_SUPPORT
        Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        Windows.Storage.StorageFile historyFile = await storageFolder.GetFileAsync("History.txt");
        history = await Windows.Storage.FileIO.ReadTextAsync(historyFile);
#endif
        history = System.Text.Encoding.UTF8.GetString(File.ReadAllBytes("./History.txt"));
        Debug.Log(history);
    }

    private IEnumerator UpdateCollection()
    {
        Transform scroll = menuNew.transform.Find("History").Find("ScrollingObjectCollection");
        Transform grid = scroll.Find("Container").Find("GridObjectCollection");

        //if (grid.Find("matteo") != null)
        //{
        //    Debug.Log(Time.realtimeSinceStartup + " ui: " + grid.Find("matteo").Find("IconAndText").Find("UIButtonSquareIcon").gameObject.activeSelf);
        //}

        yield return new WaitForEndOfFrame();
        grid.GetComponent<GridObjectCollection>().UpdateCollection();
        yield return new WaitForEndOfFrame();
        scroll.GetComponent<ScrollingObjectCollection>().UpdateContent();

        //if (grid.Find("matteo") != null)
        //{
        //    Debug.Log(Time.realtimeSinceStartup + " ui: " + grid.Find("matteo").Find("IconAndText").Find("UIButtonSquareIcon").gameObject.activeSelf);
        //}

        if (grid.childCount == 0)
        {
            scroll.gameObject.SetActive(false);
        }
        else
        {
            
            scroll.gameObject.SetActive(true);
            if (grid.childCount < maxNew)
            {
                Debug.Log("active: " + grid.childCount);
                scroll.GetComponent<ScrollingObjectCollection>().TiersPerPage = grid.childCount;
            }
            else
            {
                scroll.GetComponent<ScrollingObjectCollection>().TiersPerPage = maxNew;
            }
        }

        yield return new WaitForEndOfFrame();
        grid.GetComponent<GridObjectCollection>().UpdateCollection();
        yield return new WaitForEndOfFrame();
        scroll.GetComponent<ScrollingObjectCollection>().UpdateContent();

        //if (grid.Find("matteo") != null)
        //{
        //    Debug.Log(Time.realtimeSinceStartup + " ui: " + grid.Find("matteo").Find("IconAndText").Find("UIButtonSquareIcon").gameObject.activeSelf);
        //}
    }

    private void PopulateHistory()
    {
        Transform scroll = menuNew.transform.Find("History").Find("ScrollingObjectCollection");
        Transform grid = scroll.Find("Container").Find("GridObjectCollection");

        //buttonPrefab.GetComponent<ButtonConfigHelper>().IconStyle = ButtonIconStyle.None;
        //buttonPrefab.transform.Find("IconAndText").Find("UIButtonSquareIcon").gameObject.SetActive(false);
        //buttonPrefab.GetComponent<ButtonConfigHelper>().MainLabelText = history;
        GameObject gameObjectButton = Instantiate(buttonPrefab, grid);
        gameObjectButton.GetComponent<ButtonConfigHelper>().IconStyle = ButtonIconStyle.None;
        
        gameObjectButton.transform.Find("IconAndText").Find("UIButtonSquareIcon").gameObject.SetActive(false);

        // DA MODIFICARE, NON VERRANNO RITORNATE DELLE STRINGHE, MA JSON DEI PDF
        gameObjectButton.GetComponent<ButtonConfigHelper>().MainLabelText = history;
        gameObjectButton.name = history;

        //gameObjectButton.GetComponent<Interactable>().OnClick.AddListener(() => menu.GetComponent<InterfaceManager>().SetActivePdfSearch(history));
        gameObjectButton.GetComponent<Interactable>().OnClick.AddListener(() => menuNew.GetComponent<HistoryManager>().CallUpdateHistory(history));


        //Debug.Log(Time.realtimeSinceStartup + " ui: " + grid.Find("matteo").Find("IconAndText").Find("UIButtonSquareIcon").gameObject.activeSelf);


        //yield return new WaitForEndOfFrame();
        //grid.GetComponent<GridObjectCollection>().UpdateCollection();
        //yield return new WaitForEndOfFrame();
        //scroll.GetComponent<ScrollingObjectCollection>().UpdateContent();
    }

    public void CallUpdateHistory(string newPdf)
    {
        UpdateHistory(newPdf);
        StartCoroutine(UpdateCollection());
    }

    private void UpdateHistory(string newPdf)
    {
        Transform scroll = menuNew.transform.Find("History").Find("ScrollingObjectCollection");
        Transform grid = scroll.Find("Container").Find("GridObjectCollection");
        bool found = false;

        Debug.Log(grid.childCount);
        int i = 0;
        while (!found && i < grid.childCount)
        {
            Debug.Log(i);
            if (!found && string.Equals(grid.GetChild(i).GetComponent<ButtonConfigHelper>().MainLabelText, newPdf))
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


            GameObject gameObjectButton = Instantiate(buttonPrefab, grid);
            gameObjectButton.GetComponent<ButtonConfigHelper>().IconStyle = ButtonIconStyle.None;

            gameObjectButton.GetComponent<ButtonConfigHelper>().MainLabelText = newPdf;
            gameObjectButton.GetComponent<Interactable>().OnClick.AddListener(() => menuNew.GetComponent<HistoryManager>().CallUpdateHistory(newPdf));

            gameObjectButton.name = history;        


            // DA INSERIRE ANCHE IL COLLEGAMENTO ALLA SCHERMATA SUCCESSIVA
        }

        
        //yield return new WaitForEndOfFrame();
        //grid.GetComponent<GridObjectCollection>().UpdateCollection();
        //yield return new WaitForEndOfFrame();
        //scroll.GetComponent<ScrollingObjectCollection>().UpdateContent();
    }
}
