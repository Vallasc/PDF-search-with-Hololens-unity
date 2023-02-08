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
    private GameObject global;
    

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

    //private string history = "";
    private List<Pdf> his;
    private int maxNew = 3;


    [SerializeField]
    private TextMeshPro tmp;

    private string tmpShow = "Say \"Show History\"";
    private string tmpHide = "Say \"Hide History\"";

    private string historyFileName = "History.txt";
    private string faToken;


    void Start()
    {
        StartCoroutine(OnStartHistory());
        if (this.gameObject.activeSelf)
        {
            ChangeSISIToHide();
        }
        else
        {
            ChangeSISIToShow();
        }
    }

    IEnumerator OnStartHistory()
    {
        his = new List<Pdf>();

        //ReadHistortyFromFile();
        PopulateHistory();
        yield return StartCoroutine(UpdateCollection());

        menu.GetComponent<MenuManager>().OnHistoryUpdated();

        //Debug.Log("HISTORY FILE: " + history);
    }

    void OnDestroy()
    {
        //SaveHistoryToFile2();
        SelectFolder();
        SaveFile();
    }

    private void SelectFolder()
    {
#if ENABLE_WINMD_SUPPORT
        UnityEngine.WSA.Application.InvokeOnUIThread(async () =>  
        {  
            var folderPicker = new Windows.Storage.Pickers.FolderPicker();  
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;  
            folderPicker.FileTypeFilter.Add("*.txt");  
  
            Windows.Storage.StorageFolder folder = await folderPicker.PickSingleFolderAsync();  
            if (folder != null)  
            {  
                // Application now has read/write access to all contents in the picked folder  
                // (including other sub-folder contents)

                //string faToken = Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.AddOrReplace(folder);  
                Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);  
            }  
        }, false);  
#endif
    }

    private async void SaveFile()
    {
#if ENABLE_WINMD_SUPPORT
        if (Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.ContainsItem("PickedFolderToken"))   
        {  
            Windows.Storage.StorageFolder storageFolder = await Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.GetFolderAsync("PickedFolderToken");  
            Windows.Storage.StorageFile historyFile = await storageFolder.CreateFileAsync(historyFileName, Windows.Storage.CreationCollisionOption.OpenIfExists);
            
            History toSave = new History
            {
                history = his
            };
            string json = JsonUtility.ToJson(toSave);

            await Windows.Storage.FileIO.WriteTextAsync(historyFile, json);  
        }  
#endif
    }

    private void ReadFile()
    {
#if ENABLE_WINMD_SUPPORT
        if (Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.ContainsItem("PickedFolderToken"))   
        {
            //Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFolder storageFolder = await Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.GetFolderAsync("PickedFolderToken");  
            Windows.Storage.StorageFile historyFile = await storageFolder.GetFileAsync(historyFileName);

            string json = await Windows.Storage.FileIO.ReadTextAsync(historyFile);

            History h = JsonUtility.FromJson<History>(json);
            his = h.history;
        }
#endif
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

        Pdf p = new Pdf();
        p._id = "ikea";
        p.name = "ikea";
        p.path = "ikea";
        p.page = 1;
        his.Add(p);


        History toSave = new History
        {
            history = his
        };

        string json = JsonUtility.ToJson(toSave);

        byte[] historyByteArray = System.Text.Encoding.UTF8.GetBytes(json);
        File.WriteAllBytes("./History.txt", historyByteArray);
    }

    private async void ReadHistortyFromFile()
    {
        /*
#if ENABLE_WINMD_SUPPORT
        Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        Windows.Storage.StorageFile historyFile = await storageFolder.GetFileAsync("History.txt");
        history = await Windows.Storage.FileIO.ReadTextAsync(historyFile);
#endif
        */
        string jsonHistory = System.Text.Encoding.UTF8.GetString(File.ReadAllBytes("./History.txt"));
        History h = JsonUtility.FromJson<History>(jsonHistory);
        his = h.history;

        //Pdf[] tmp = Pdf.CreateFromJSON(jsonHistory);
        //if(tmp.Length != 0)
        //{
        //    history = new List<Pdf>(tmp)
        //    {
        //        Capacity = maxNew
        //    };
        //}
        //else
        //{
        //    history = new List<Pdf>(maxNew);
        //}

        //Debug.Log(history);
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
            menuNew.transform.Find("History").Find("NoObjectText").gameObject.SetActive(true);
        }
        else
        {
            menuNew.transform.Find("History").Find("NoObjectText").gameObject.SetActive(false);
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

        foreach (Pdf pdf in his)
        {
            //buttonPrefab.GetComponent<ButtonConfigHelper>().IconStyle = ButtonIconStyle.None;
            //buttonPrefab.transform.Find("IconAndText").Find("UIButtonSquareIcon").gameObject.SetActive(false);
            //buttonPrefab.GetComponent<ButtonConfigHelper>().MainLabelText = history;
            GameObject gameObjectButton = Instantiate(buttonPrefab, grid);
            //gameObjectButton.GetComponent<ButtonConfigHelper>().IconStyle = ButtonIconStyle.None;
            //gameObjectButton.transform.Find("IconAndText").Find("UIButtonSquareIcon").gameObject.SetActive(false);

            // NON VERRANNO RITORNATE DELLE STRINGHE, MA JSON DEI PDF
            int p = pdf.page + 1;
            gameObjectButton.GetComponent<ButtonConfigHelper>().MainLabelText = pdf.name + " - p. " + p.ToString();
            gameObjectButton.name = pdf.name;

            //gameObjectButton.GetComponent<Interactable>().OnClick.AddListener(() => StartCoroutine(menuNew.GetComponent<HistoryManager>().CallUpdateHistory(pdf.name)));
            gameObjectButton.GetComponent<Interactable>().OnClick.AddListener(() => global.GetComponent<InterfaceManager>().OpenNewPdfView(pdf._id, pdf.page));
            gameObjectButton.SetActive(true);
        }

        //Debug.Log(Time.realtimeSinceStartup + " ui: " + grid.Find("matteo").Find("IconAndText").Find("UIButtonSquareIcon").gameObject.activeSelf);


        //yield return new WaitForEndOfFrame();
        //grid.GetComponent<GridObjectCollection>().UpdateCollection();
        //yield return new WaitForEndOfFrame();
        //scroll.GetComponent<ScrollingObjectCollection>().UpdateContent();
    }

    public void CallUpdateHistory(string id, string name, int page)
    {
        StartCoroutine(UpdateHistory(id, name, page));
    }


    private IEnumerator UpdateHistory(string id, string name, int page)
    {
        InsertPdfHistory(id, name, page);
        yield return StartCoroutine(UpdateCollection());
        menu.GetComponent<MenuManager>().OnHistoryUpdated();
    }

    private void InsertPdfHistory(string id, string name, int page)
    {
        Transform scroll = menuNew.transform.Find("History").Find("ScrollingObjectCollection");
        Transform grid = scroll.Find("Container").Find("GridObjectCollection");
        bool found = false;
        Pdf newPdf;

        int i = 0;
        while (!found && i < his.Count)
        {
            if (string.Equals(his[i]._id, id))
            {
                found = true;
            }
            else
            {
                i++;
            } 
        }

        if (found)
        {
            int p = page + 1;
            grid.GetChild(i).GetComponent<ButtonConfigHelper>().MainLabelText = name + " - p. " + p.ToString();
            grid.GetChild(i).gameObject.GetComponent<Interactable>().OnClick.RemoveAllListeners();
            grid.GetChild(i).gameObject.GetComponent<Interactable>().OnClick.AddListener(() => global.GetComponent<InterfaceManager>().OpenNewPdfView(id, page));
            grid.GetChild(i).SetAsLastSibling();

            his.RemoveAt(i);
            newPdf = new Pdf
            {
                _id = id,
                name = name,
                page = page
            };
            his.Add(newPdf);
        }
        else 
        {
            if (his.Count == maxNew)
            {
                Destroy(grid.GetChild(0).gameObject);
                his.RemoveAt(0);
            }


            GameObject gameObjectButton = Instantiate(buttonPrefab, grid);

            int p = page + 1;
            gameObjectButton.GetComponent<ButtonConfigHelper>().MainLabelText = name + " - p. " + p.ToString();
            gameObjectButton.name = name;
            gameObjectButton.GetComponent<Interactable>().OnClick.AddListener(() => global.GetComponent<InterfaceManager>().OpenNewPdfView(id, page));
            gameObjectButton.SetActive(true);

            newPdf = new Pdf
            {
                _id = id,
                name = name,
                page = page
            };

            his.Add(newPdf);
        }


        //yield return new WaitForEndOfFrame();
        //grid.GetComponent<GridObjectCollection>().UpdateCollection();
        //yield return new WaitForEndOfFrame();
        //scroll.GetComponent<ScrollingObjectCollection>().UpdateContent();
    }

    public void ResetHistory()
    {
        Transform scroll = menuNew.transform.Find("History").Find("ScrollingObjectCollection");
        Transform grid = scroll.Find("Container").Find("GridObjectCollection");

        if (grid.childCount > 0)
        {
            StartCoroutine(ClearAndUpdateHistory());
        }
    }

    private IEnumerator ClearAndUpdateHistory()
    {
        ClearHistory();

        //if (this.gameObject.activeSelf == true)
        yield return StartCoroutine(UpdateCollection());
        menu.GetComponent<MenuManager>().OnHistoryUpdated();
    }

    private void ClearHistory()
    {
        Transform scroll = menuNew.transform.Find("History").Find("ScrollingObjectCollection");
        Transform grid = scroll.Find("Container").Find("GridObjectCollection");

        foreach (Transform child in grid)
        {
            Destroy(child.gameObject);
        }

        his.Clear();
    }

    public void OpenLastOnePdf()
    {
        if (his.Count > 0) //  && !menuNew.activeSelf
        {
            Pdf last = his[his.Count - 1];
            global.GetComponent<InterfaceManager>().OpenNewPdfView(last._id, last.page);
        }
    }

    public void ChangeSISIToShow()
    {
        tmp.text = tmpShow;
    }

    public void ChangeSISIToHide()
    {
        tmp.text = tmpHide;
    }

    [Serializable]
    public class History
    {
        public List<Pdf> history;
    }

    [Serializable]
    public class Pdf
    {
        public string _id;
        public string name;
        public string path;
        public int page;

        //public static Pdf[] CreateFromJSON(string jsonString)
        //{
        //    return JsonUtility.FromJson<Pdf[]>(jsonString);
        //}

        //public static string HistoryToJSON(Pdf history)
        //{
        //    return JsonUtility.ToJson(history);
        //}
    }
}
