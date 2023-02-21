using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;

using UnityEngine.Networking;
using Microsoft.MixedReality.Toolkit.Input;
using TMPro;

public class KeywordsManager : MonoBehaviour
{
    [SerializeField]
    public class Keyword
    {
        public string keyword;
        public int timeLeft = 10;

        public Keyword(string keyword)
        {
            this.keyword = keyword;
        }

        public string GetKeyword()
        {
            return this.keyword;
        }
        public int SetTimeLeft(int newTimeLeft)
        {
            return this.timeLeft = newTimeLeft;
        }
        public int DecreaseTimeLeft()
        {
            this.timeLeft--;
            return this.timeLeft;
        }
    }

    [SerializeField]
    public GameObject global;
    [SerializeField]
    public GameObject buttonPrefab;
    [SerializeField]
    private GameObject menu;
    [SerializeField]
    private GameObject menuKeys;

    private List<Keyword> keywords = new List<Keyword>();
    private int maxPdfs = 5;
    private bool firstPhotoTaken = false;

    private string baseSISI = "Say ";
    private string escape = "\"";

    void Start()
    {
        //this.gameObject.GetComponent<SpeechKeyword>().UpdateKeywordRecognizer(0);
    }

    public void UpdateKeywordsCollection(string[] response)
    {
        StartCoroutine(UpdateKeywordsCollection2(response));
    }

    public IEnumerator UpdateKeywordsCollection2(string[] response)
    {
        Transform scroll = menuKeys.transform.Find("Keywords").Find("ScrollingObjectCollection");
        Transform grid = scroll.Find("Container").Find("GridObjectCollection");

        List<Keyword> tmp = new List<Keyword>();

        foreach (string k in response)
        {
            tmp.Add(new Keyword(k));
        }

        if (keywords.Count > 0)
        {
            DecreaseTimeLeftCollection();
        }

        tmp.Reverse();
        InsertKeywords(tmp);
        yield return StartCoroutine(UpdateCollection());

        this.gameObject.GetComponent<SpeechKeyword>().UpdateKeywordRecognizer(grid.childCount);
        menu.GetComponent<MenuManager>().OnKeywordsUpdated();
    }

    private IEnumerator UpdateCollection()
    {
        Transform scroll = menuKeys.transform.Find("Keywords").Find("ScrollingObjectCollection");
        Transform grid = scroll.Find("Container").Find("GridObjectCollection");

        for (int i = (grid.childCount - 1); i >= 0; i--)
        {
            int n = grid.childCount - i;
            grid.GetChild(i).Find("IconAndText").Find("UIButtonCharIcon").GetComponent<TextMeshPro>().text = n.ToString();
            grid.GetChild(i).Find("SeeItSayItLabel").Find("TextMeshPro").GetComponent<TextMeshPro>().text = baseSISI + escape + Converter.NumToString.NumIntToWords(n.ToString()) + escape;
        }

        yield return new WaitForEndOfFrame();
        grid.GetComponent<GridObjectCollection>().UpdateCollection();
        yield return new WaitForEndOfFrame();
        scroll.GetComponent<ScrollingObjectCollection>().UpdateContent();

        if (grid.childCount == 0)
        {
            scroll.gameObject.SetActive(false);
            if (firstPhotoTaken)
            {
                menuKeys.transform.Find("Keywords").Find("TakeFirstPhotoText").gameObject.SetActive(false);
                menuKeys.transform.Find("Keywords").Find("NoObjectText").gameObject.SetActive(true);
                
            }
            else
            {
                menuKeys.transform.Find("Keywords").Find("NoObjectText").gameObject.SetActive(false);
                menuKeys.transform.Find("Keywords").Find("TakeFirstPhotoText").gameObject.SetActive(true);
            }
        }
        else
        {
            menuKeys.transform.Find("Keywords").Find("TakeFirstPhotoText").gameObject.SetActive(false);
            menuKeys.transform.Find("Keywords").Find("NoObjectText").gameObject.SetActive(false);
            scroll.gameObject.SetActive(true);
            if (grid.childCount < maxPdfs)
            {
                scroll.GetComponent<ScrollingObjectCollection>().TiersPerPage = grid.childCount;
            }
            else
            {
                scroll.GetComponent<ScrollingObjectCollection>().TiersPerPage = maxPdfs;
            }
        }

        yield return new WaitForEndOfFrame();
        grid.GetComponent<GridObjectCollection>().UpdateCollection();
        yield return new WaitForEndOfFrame();
        scroll.GetComponent<ScrollingObjectCollection>().UpdateContent();
    }

    private void InsertKeywords(List<Keyword> newKey)
    {
        Transform scroll = menuKeys.transform.Find("Keywords").Find("ScrollingObjectCollection");
        Transform grid = scroll.Find("Container").Find("GridObjectCollection");

        if (keywords.Count == 0)
        {
            keywords = newKey;

            foreach (Keyword k in keywords)
            {
                GameObject gameObjectButton = Instantiate(buttonPrefab, grid);
                gameObjectButton.GetComponent<ButtonConfigHelper>().MainLabelText = k.GetKeyword();
                gameObjectButton.name = k.GetKeyword();

                gameObjectButton.GetComponent<Interactable>().OnClick.AddListener(() => global.GetComponent<InterfaceManager>().SwitchToPdfSearch(k.GetKeyword()));
                gameObjectButton.SetActive(true);
            }
        }
        else
        {
            List<Keyword> toRemove = new List<Keyword>();

            foreach (Keyword k in newKey)
            {
                foreach (Keyword old in keywords)
                {
                    if (string.Equals(old.GetKeyword(), k.GetKeyword()))
                    {
                        Destroy(grid.Find(old.GetKeyword()).gameObject);
                        toRemove.Add(old);
                    }
                }
            }

            foreach (Keyword k in toRemove)
            {
                keywords.Remove(k);
            }

            foreach (Keyword k in newKey)
            {
                GameObject gameObjectButton = Instantiate(buttonPrefab, grid);
                gameObjectButton.transform.Find("IconAndText").Find("UIButtonCharIcon").gameObject.SetActive(true);
                gameObjectButton.GetComponent<ButtonConfigHelper>().MainLabelText = k.GetKeyword();
                gameObjectButton.name = k.GetKeyword();

                gameObjectButton.GetComponent<Interactable>().OnClick.AddListener(() => global.GetComponent<InterfaceManager>().SwitchToPdfSearch(k.GetKeyword()));
                gameObjectButton.SetActive(true);

                //k.SetTimeLeft(10);
                keywords.Add(k);
            }
        }
    }

    private void DecreaseTimeLeftCollection()
    {
        Transform scroll = menuKeys.transform.Find("Keywords").Find("ScrollingObjectCollection");
        Transform grid = scroll.Find("Container").Find("GridObjectCollection");

        List<Keyword> toRemove = new List<Keyword>();

        foreach (Keyword k in keywords)
        {
            if(k.DecreaseTimeLeft() == 0)
            {
                Destroy(grid.Find(k.GetKeyword()).gameObject);
                toRemove.Add(k);
            }
        }

        foreach (Keyword k in toRemove)
        {
            keywords.Remove(k);
        }
    }

    public void SetFirstPhotoTaken()
    {
        if (!firstPhotoTaken)
        {
            firstPhotoTaken = true;
        }
    }

    public void OnKeywordRecognized(int index)
    {
        Transform scroll = menuKeys.transform.Find("Keywords").Find("ScrollingObjectCollection");
        Transform grid = scroll.Find("Container").Find("GridObjectCollection");

        global.GetComponent<InterfaceManager>().SwitchToPdfSearch(grid.GetChild(grid.childCount - index).gameObject.name);
    }
}
