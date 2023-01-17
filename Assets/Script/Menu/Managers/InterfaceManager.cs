using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using Microsoft.MixedReality.Toolkit.UI;

public class InterfaceManager : MonoBehaviour
{
    [SerializeField]
    private GameObject menu;

    [SerializeField]
    private GameObject pdfSearch;


    public void SetActivePdfSearch()
    {
        SwitchInterfaceToPdfSearch();
    }

    public void SetActivePdfSearch(string keyword)
    {
        TMP_InputField tmPro = pdfSearch.transform.Find("TitleBar").Find("TextField").Find("InputField (TMP)").gameObject.GetComponent<TMP_InputField>();
        tmPro.text = keyword;

        // FAR PARTIRE QUI LA CHIAMATA AL SERVER

        SwitchInterfaceToPdfSearch();
    }

    private void SwitchInterfaceToPdfSearch()
    {
        menu.SetActive(false);
        pdfSearch.SetActive(true);
    }

    private void SwitchInterfaceToPdfView()
    {
        //menu.SetActive(false);
        //pdfView.SetActive(true);
    }
}
