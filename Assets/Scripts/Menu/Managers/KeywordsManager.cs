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
        this.gameObject.GetComponent<SpeechKeyword>().UpdateKeywordRecognizer(2);
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
        Debug.Log(Time.realtimeSinceStartup + "UPDATE " + grid.childCount);
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
            //gameObjectButton.transform.Find("IconAndText").Find("UIButtonCharIcon").GetComponent<TextMeshPro>().text = .ToString();
            //gameObjectButton.transform.Find("IconAndText").Find("UIButtonCharIcon").gameObject.SetActive(true);
        }

        //Debug.Log(Time.realtimeSinceStartup + "PRE YIELD GRID " + grid.childCount);
        //Debug.Log(Time.realtimeSinceStartup + "PRE YIELD SCROLL " + scroll.GetComponent<ScrollingObjectCollection>().TiersPerPage);

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
        //Debug.Log(Time.realtimeSinceStartup + "FINE YIELD GRID " + grid.childCount);
        //Debug.Log(Time.realtimeSinceStartup + "FINE YIELD SCROLL " + scroll.GetComponent<ScrollingObjectCollection>().TiersPerPage);
    }

    private void InsertKeywords(List<Keyword> newKey)
    {
        Transform scroll = menuKeys.transform.Find("Keywords").Find("ScrollingObjectCollection");
        Transform grid = scroll.Find("Container").Find("GridObjectCollection");

        //GameObject buttonObject = new GameObject();
        //PressableButtonHoloLens2 button = buttonObject.AddComponent<PressableButtonHoloLens2>();
        //PressableButtonHoloLens2 button = buttonPrefab.GetComponent<PressableButtonHoloLens2>();
        //buttonPrefab.GetComponent<ButtonConfigHelper>().IconStyle = ButtonIconStyle.None;

        if (keywords.Count == 0)
        {
            keywords = newKey;

            foreach (Keyword k in keywords)
            {
                //Debug.Log("button: " + buttonPrefab.GetComponent<ButtonConfigHelper>().IconStyle);
                //Debug.Log("ui: " + buttonPrefab.transform.Find("IconAndText").Find("UIButtonSquareIcon").gameObject.activeSelf);
                //button.gameObject.tag = k.GetKeyword(); 
                buttonPrefab.GetComponent<ButtonConfigHelper>().MainLabelText = k.GetKeyword();
                //button.GetComponent<Interactable>().OnClick.AddListener(() => keys.GetComponent<NewFavManager>().CallUpdateHistory(k.GetKeyword()));
                GameObject gameObjectButton = Instantiate(buttonPrefab, grid);
                gameObjectButton.GetComponent<ButtonConfigHelper>().IconStyle = ButtonIconStyle.Char;
                gameObjectButton.transform.Find("IconAndText").Find("UIButtonCharIcon").gameObject.SetActive(true);
                
                    
                //gameObjectButton.transform.Find("IconAndText").Find("UIButtonSquareIcon").gameObject.SetActive(false);
                gameObjectButton.name = k.GetKeyword();

                gameObjectButton.GetComponent<Interactable>().OnClick.AddListener(() => global.GetComponent<InterfaceManager>().SwitchToPdfSearch(k.GetKeyword()));
                gameObjectButton.SetActive(true);

                //gameObjectButton.GetComponent<ButtonConfigHelper>().ForceRefresh();
                //Debug.Log("button: " + gameObjectButton.GetComponent<ButtonConfigHelper>().IconStyle);
                //Debug.Log("ui: " + gameObjectButton.transform.Find("IconAndText").Find("UIButtonSquareIcon").gameObject.activeSelf);

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
                        Debug.Log(Time.realtimeSinceStartup + " trovata");
                        Destroy(grid.Find(old.GetKeyword()).gameObject);
                        toRemove.Add(old);
                        //keywords.Remove(old);
                    }
                    
                    //button.gameObject.tag = k.GetKeyword();
                    //button.GetComponent<ButtonConfigHelper>().MainLabelText = k.GetKeyword();
                    ////button.GetComponent<Interactable>().OnClick.AddListener(() => keys.GetComponent<NewFavManager>().CallUpdateHistoryk.GetKeyword()));
                    //Instantiate(obj, grid);
                    //keywords.Add(k);
                }
            }

            foreach (Keyword k in toRemove)
            {
                keywords.Remove(k);
            }

            foreach (Keyword k in newKey)
            {
                GameObject gameObjectButton = Instantiate(buttonPrefab, grid);
                gameObjectButton.GetComponent<ButtonConfigHelper>().IconStyle = ButtonIconStyle.Char;
                //gameObjectButton.transform.Find("IconAndText").Find("UIButtonCharIcon").GetComponent<TextMeshPro>().text = k.GetKeyword();
                gameObjectButton.transform.Find("IconAndText").Find("UIButtonCharIcon").gameObject.SetActive(true);
                //gameObjectButton.transform.Find("IconAndText").Find("UIButtonSquareIcon").gameObject.SetActive(false);
                gameObjectButton.GetComponent<ButtonConfigHelper>().MainLabelText = k.GetKeyword();
                gameObjectButton.name = k.GetKeyword();

                gameObjectButton.GetComponent<Interactable>().OnClick.AddListener(() => global.GetComponent<InterfaceManager>().SwitchToPdfSearch(k.GetKeyword()));
                gameObjectButton.SetActive(true);

                //k.SetTimeLeft(10);
                keywords.Add(k);
            }
        }
        

        //yield return new WaitForEndOfFrame();
        //grid.GetComponent<GridObjectCollection>().UpdateCollection();
        //yield return new WaitForEndOfFrame();
        //scroll.GetComponent<ScrollingObjectCollection>().UpdateContent();
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

        //yield return new WaitForEndOfFrame();
        //grid.GetComponent<GridObjectCollection>().UpdateCollection();
        //yield return new WaitForEndOfFrame();
        //scroll.GetComponent<ScrollingObjectCollection>().UpdateContent();
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
        Debug.Log(grid.childCount - index);

        //Debug.Log(grid.GetChild(index).gameObject.name);
        global.GetComponent<InterfaceManager>().SwitchToPdfSearch(grid.GetChild(grid.childCount - index).gameObject.name);
    }

    public void OnKeywordRecognized_Prova(int index)
    {
        Transform scroll = menuKeys.transform.Find("Keywords").Find("ScrollingObjectCollection");
        Transform grid = scroll.Find("Container").Find("GridObjectCollection");

        Debug.Log(index);

        GameObject gameObjectButton = Instantiate(buttonPrefab, grid);
        gameObjectButton.name = index.ToString();
        gameObjectButton.GetComponent<ButtonConfigHelper>().MainLabelText = index.ToString();
        //gameObjectButton.GetComponent<ButtonConfigHelper>().IconStyle = ButtonIconStyle.Char;
        //gameObjectButton.transform.Find("IconAndText").Find("UIButtonCharIcon").gameObject.SetActive(true);
        //gameObjectButton.transform.Find("IconAndText").Find("UIButtonSquareIcon").gameObject.SetActive(false);
        gameObjectButton.SetActive(true);

        StartCoroutine(UpdateCollection());

        this.gameObject.GetComponent<SpeechKeyword>().UpdateKeywordRecognizer(4);
    }
}
