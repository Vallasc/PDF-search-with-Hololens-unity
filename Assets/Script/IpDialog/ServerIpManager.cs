using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ServerIpManager : MonoBehaviour
{
    public GameObject pdfSearchObject = null;
    public GameObject pdfViewObject = null;
    public GameObject inputField = null;

    void Start() {}

    void Update() {}

    public void OnButtonEnterClick()
    {
        string ipText = inputField.GetComponent<TMP_InputField>().text;
        pdfSearchObject.SetActive(true);
        pdfSearchObject.GetComponent<FetchPdfs>().SetServerIp(ipText);
        StartCoroutine(runDelayed());
        pdfViewObject.GetComponent<PdfManager>().SetServerIp(ipText);
    }

    private IEnumerator runDelayed()
    {
        yield return new WaitForEndOfFrame();
        pdfSearchObject.GetComponent<FetchPdfs>().OnSearch();
        transform.gameObject.SetActive(false);

    }

}
