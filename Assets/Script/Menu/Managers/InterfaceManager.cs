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
    [SerializeField]
    private GameObject pdfView;

    public void SetActivePdfSearch()
    {
        SwitchInterfaceToPdfSearch();
    }

    public void SetActivePdfSearch(string keyword)
    {
        // FAR PARTIRE QUI LA CHIAMATA AL SERVER
        StartCoroutine(pdfSearch.GetComponent<FetchPdfs>().GetPdfs(keyword));

        TMP_InputField tmPro = pdfSearch.transform.Find("TitleBar").Find("TextField").Find("InputField (TMP)").gameObject.GetComponent<TMP_InputField>();
        tmPro.text = keyword;

        SwitchInterfaceToPdfSearch();
    }

    private void SwitchInterfaceToPdfSearch()
    {
        menu.SetActive(false);
        pdfSearch.SetActive(true);
    }

    public void SetActivePdfView(string pdfId, int pageNumber)
    {
        SwitchInterfaceToPdfView(pdfId, pageNumber);
    }

    private void SwitchInterfaceToPdfView(string pdfId, int pageNumber)
    {
        PdfManager pdfManager = pdfView.GetComponent<PdfManager>();
        pdfManager.pdfId = pdfId;
        pdfManager.currentPageNumber = pageNumber;

        GameObject gameObjectPdfView = Instantiate(pdfView);

        menu.SetActive(false);
        gameObjectPdfView.SetActive(true);
    }
}
