using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ServerIpManager : MonoBehaviour
{
    public GameObject pdfSearchObject = null;
    public GameObject pdfViewObject = null;
    public GameObject inputField = null;
    public GameObject CameraCapture = null;

    void Start() {}

    void Update() {}

    public void OnButtonEnterClick()
    {
        string ipText = inputField.GetComponent<TMP_InputField>().text;
        //pdfSearchObject.SetActive(true);
        pdfSearchObject.GetComponent<FetchPdfs>().serverIp = ipText;
        pdfViewObject.GetComponent<PdfManager>().serverIp = ipText;
        pdfSearchObject.GetComponent<FetchPdfs>().UpdateBaseUrl();
        //StartCoroutine(runDelayed());
        CameraCapture.GetComponent<CameraManager>().serverIp = ipText;
        CameraCapture.GetComponent<CameraManager>().UpdateBaseUrl();
        transform.gameObject.SetActive(false);
    }

    private IEnumerator runDelayed()
    {
        yield return new WaitForEndOfFrame();
        pdfSearchObject.GetComponent<FetchPdfs>().UpdateBaseUrl();
        pdfSearchObject.GetComponent<FetchPdfs>().OnSearch();
        transform.gameObject.SetActive(false);

    }

}
