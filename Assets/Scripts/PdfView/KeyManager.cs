using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using UnityEngine;

public class KeyManager : MonoBehaviour
{
    public bool selected = false;
    public bool disabled = false;

    public PdfManager pdfViewObject;
    public GameObject showKeywordButtonObject;
    private GameObject iconShow;
    private GameObject iconShowDisabled;
    private GameObject iconHide;
    private GameObject text;

    void Start()
    {
        iconShow = showKeywordButtonObject.transform.Find("IconAndText").Find("ShowIcon").gameObject;
        iconHide = showKeywordButtonObject.transform.Find("IconAndText").Find("HideIcon").gameObject;
        iconShowDisabled = showKeywordButtonObject.transform.Find("IconAndText").Find("ShowIconDisabled").gameObject;
        text = showKeywordButtonObject.transform.Find("IconAndText").Find("TextMeshPro").gameObject;
        selected = false;

        disabled = pdfViewObject.GetComponent<PdfManager>().keyword == null || pdfViewObject.GetComponent<PdfManager>().keyword.Equals("");
        Debug.Log(pdfViewObject.GetComponent<PdfManager>().keyword);
        if (disabled)
        {
            showKeywordButtonObject.GetComponent<Interactable>().IsEnabled = false;
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

    public void ShowKeyword()
    {
        if (disabled)
            return;
        if (!selected)
            OnClick();
    }

    public void HideKeyword()
    {
        if (disabled)
            return;
        if (selected)
            OnClick();
    }
}
