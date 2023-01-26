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
    public GameObject pdfSearch;
    [SerializeField]
    private GameObject pdfView;

    public void SetActivePdfSearch(string keyword)
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

    public void SetActiveMenu()
    {
        SwitchInterfaceToMenu();
    }

    private void SwitchInterfaceToMenu()
    {
        menu.SetActive(true);
        pdfSearch.SetActive(false);
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
