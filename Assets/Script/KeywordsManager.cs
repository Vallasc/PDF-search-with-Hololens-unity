using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;

using UnityEngine.Networking;

public class KeywordsManager : MonoBehaviour
{
    public class Keyword
    {
        private string keyword;
        private int timeLeft = 10;

        public Keyword(string keyword)
        {
            this.keyword = keyword;
        }

        public string GetKeyword()
        {
            return this.keyword;
        }
        public int GetTimeLeft()
        {
            return this.timeLeft;
        }
        public int DecreaseTimeLeft()
        {
            this.timeLeft--;
            return this.timeLeft;
        }
    }

    [SerializeField]
    private GameObject keys;

    private List<Keyword> keywords = new List<Keyword>();

    public void UpdateKeywordsCollection(string[] response)
    {
        Debug.Log("KEYYYYYYYYYYYYYYYYYYYYYY");
        List<Keyword> tmp = new List<Keyword>();

        foreach (string k in response)
        {
            tmp.Add(new Keyword(k));
        }

        DecreaseTimeLeftCollection();

        tmp.Reverse();
        StartCoroutine(InsertKeywords(tmp));
        Debug.Log("KEYYYYYYYYYYYYYYYYYYYYYY FINE");
    }

    private IEnumerator InsertKeywords(List<Keyword> newKey)
    {
        Transform scroll = keys.transform.Find("ScrollingObjectCollection");
        Transform grid = scroll.Find("Container").Find("GridObjectCollection");

        //GameObject buttonObject = new GameObject();
        //PressableButtonHoloLens2 button = buttonObject.AddComponent<PressableButtonHoloLens2>();
        PressableButtonHoloLens2 button = GameObject.Find("PressableButtonHoloLens2_32x96_NoLabel").GetComponent<PressableButtonHoloLens2>();

        if (keywords.Count == 0)
        {
            keywords = newKey;

            foreach (Keyword k in keywords)
            {
                //button.gameObject.tag = k.GetKeyword(); 
                button.GetComponent<ButtonConfigHelper>().MainLabelText = k.GetKeyword();
                //button.GetComponent<Interactable>().OnClick.AddListener(() => keys.GetComponent<NewFavManager>().CallUpdateHistory(k.GetKeyword()));
                Instantiate(button, grid.transform);
            }
        }
        else
        {
            foreach (Keyword k in newKey)
            {
                if (keywords.Contains(k))
                {
                    keywords.Remove(k);
                    Destroy(grid.Find(k.GetKeyword()));
                }

                //button.gameObject.tag = k.GetKeyword();
                button.GetComponent<ButtonConfigHelper>().MainLabelText = k.GetKeyword();
                //button.GetComponent<Interactable>().OnClick.AddListener(() => keys.GetComponent<NewFavManager>().CallUpdateHistoryk.GetKeyword()));
                Instantiate(button, grid.transform);
                keywords.Add(k);
            }
        }
        

        yield return new WaitForEndOfFrame();
        grid.GetComponent<GridObjectCollection>().UpdateCollection();
        yield return new WaitForEndOfFrame();
        scroll.GetComponent<ScrollingObjectCollection>().UpdateContent();
    }

    private void DecreaseTimeLeftCollection()
    {
        Transform scroll = keys.transform.Find("ScrollingObjectCollection");
        Transform grid = scroll.Find("Container").Find("GridObjectCollection");

        foreach (Keyword k in keywords)
        {
            if(k.DecreaseTimeLeft() == 0)
            {
                keywords.Remove(k);
                Destroy(grid.Find(k.GetKeyword()));
            }
        }
    }
}
