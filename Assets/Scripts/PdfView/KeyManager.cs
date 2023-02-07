using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static KeywordsManager;

public class KeyManager : MonoBehaviour
{
    public bool selected = false;
    public bool disabled = false;

    public GameObject pdfViewObject;
    private GameObject iconShow;
    private GameObject iconShowDisabled;
    private GameObject iconHide;
    private GameObject text;

    void Start()
    {
        iconShow = this.transform.Find("IconAndText").Find("ShowIcon").gameObject;
        iconHide = this.transform.Find("IconAndText").Find("HideIcon").gameObject;
        iconShowDisabled = this.transform.Find("IconAndText").Find("ShowIconDisabled").gameObject;
        text = this.transform.Find("IconAndText").Find("TextMeshPro").gameObject;
        selected = false;

        disabled = pdfViewObject.GetComponent<PdfManager>().keyword == null || pdfViewObject.GetComponent<PdfManager>().keyword.Equals("");
        Debug.Log(pdfViewObject.GetComponent<PdfManager>().keyword);
        if (disabled)
        {
            this.GetComponent<Interactable>().IsEnabled = false;
            iconShow.SetActive(false);
            iconHide.SetActive(false);
            iconShowDisabled.SetActive(true);
            text.GetComponent<TextMeshPro>().color = new Color32(130, 130, 130, 255);
        } else
        {
            iconShow.SetActive(!selected);
            iconHide.SetActive(selected);
            iconShowDisabled.SetActive(false);
        }
    }

    void Update()
    {

    }

    public void OnClick()
    {
        if(disabled)
            return;
        selected = !selected;
        iconShow.SetActive(!selected);
        iconHide.SetActive(selected);
        if (selected)
        {
            text.GetComponent<TextMeshPro>().text = "Hide Keyword";
        }
        else
        {
            text.GetComponent<TextMeshPro>().text = "Show Keyword";
        }
        pdfViewObject.GetComponent<PdfManager>().showKeyword = selected;
        pdfViewObject.GetComponent<PdfManager>().UpdatePage();
    }
}
