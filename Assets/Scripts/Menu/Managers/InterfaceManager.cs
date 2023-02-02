using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using Microsoft.MixedReality.Toolkit.UI;

public class InterfaceManager : MonoBehaviour
{
    public GameObject menu;
    public GameObject pdfSearch;
    public GameObject pdfView;

    public void SwitchToPdfSearch(string keyword)
    {
        StartCoroutine(SwitchInterfaceToPdfSearch(keyword));
    }

    private IEnumerator SwitchInterfaceToPdfSearch(string keyword)
    {
        pdfSearch.SetActive(true);
        yield return new WaitForEndOfFrame();
        pdfSearch.GetComponent<FetchPdfs>().OnSearch(keyword);
       
        menu.SetActive(false);
    }

    public void SwitchToMenu()
    {
        menu.SetActive(true);
        pdfSearch.SetActive(false);
    }

    public void OpenNewPdfView(string pdfId, int pageNumber)
    {
        OpenPdfView(pdfId, pageNumber);
    }

    private void OpenPdfView(string pdfId, int pageNumber)
    {
        PdfManager pdfManager = pdfView.GetComponent<PdfManager>();
        pdfManager.pdfId = pdfId;
        pdfManager.currentPageNumber = pageNumber;

        GameObject gameObjectPdfView = Instantiate(pdfView);
        gameObjectPdfView.SetActive(true);
    }
}
