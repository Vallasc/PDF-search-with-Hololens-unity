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

        ReadHistory();
        PopulateHistory();
        yield return StartCoroutine(UpdateCollection());

        menu.GetComponent<MenuManager>().OnHistoryUpdated();
    }

    void OnDestroy()
    {
        SaveHistory();
    }

    private void ReadHistory()
    {
        string json = PlayerPrefs.GetString("History");
        Debug.Log("READ");
        Debug.Log(json);
        if (json != "")
        {
            History h = JsonUtility.FromJson<History>(json);
            his = h.history;
        }
    }

    private void SaveHistory()
    {
        History toSave = new History
        {
            history = his
        };

        string json = JsonUtility.ToJson(toSave);
        Debug.Log("SAVE");
        Debug.Log(json);
        PlayerPrefs.SetString("History", json);
    } 

    private IEnumerator UpdateCollection()
    {
        Transform scroll = menuNew.transform.Find("History").Find("ScrollingObjectCollection");
        Transform grid = scroll.Find("Container").Find("GridObjectCollection");

        yield return new WaitForEndOfFrame();
        grid.GetComponent<GridObjectCollection>().UpdateCollection();
        yield return new WaitForEndOfFrame();
        scroll.GetComponent<ScrollingObjectCollection>().UpdateContent();

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
    }

    private void PopulateHistory()
    {
        Transform scroll = menuNew.transform.Find("History").Find("ScrollingObjectCollection");
        Transform grid = scroll.Find("Container").Find("GridObjectCollection");

        foreach (Pdf pdf in his)
        {
            GameObject gameObjectButton = Instantiate(buttonPrefab, grid);
            int p = pdf.page + 1;
            gameObjectButton.GetComponent<ButtonConfigHelper>().MainLabelText = pdf.name + " - p. " + p.ToString();
            gameObjectButton.name = pdf.name;

            gameObjectButton.GetComponent<Interactable>().OnClick.AddListener(() => global.GetComponent<InterfaceManager>().OpenNewPdfView(pdf._id, pdf.page));
            gameObjectButton.SetActive(true);
        }
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
    }
}
