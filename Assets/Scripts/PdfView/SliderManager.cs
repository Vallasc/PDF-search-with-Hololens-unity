using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SliderManager : MonoBehaviour
{
    public GameObject sliderObject;
    public GameObject overlayObject;
    public GameObject counterTMPObject;

    private long counter = 0;
    private int externalChange = 1;

    void Start()
    {
        overlayObject.SetActive(false);
    }

    void Update()
    {

    }

    public void SlideToPage(int pageNumber, int pagesTotal)
    {
        externalChange++;
        float sliderPos = ((float) pageNumber) / ((float)(pagesTotal -1));
        this.sliderObject.GetComponent<PinchSlider>().SliderValue = sliderPos;
    }

    public void OnValueUpdated(SliderEventData data)
    {
        if (externalChange == 0)
        {
            PdfManager pdfManager = this.GetComponent<PdfManager>();
            int pageCount = (int)((pdfManager.GetTotalPageCount() - 1) * data.NewValue);
            this.counterTMPObject.GetComponent<TextMeshPro>().text = "Page " + (pageCount + 1);
            overlayObject.SetActive(true);
            StartCoroutine(disableOverlay(++counter, pageCount));
        }
        else if (externalChange > 0)
        {
            externalChange--;
        }
    }


    private IEnumerator disableOverlay(long overlayId, int pageNumber)
    {
        yield return new WaitForSeconds(1);
        if (overlayId == counter)
        {
            PdfManager pdfManager = this.GetComponent<PdfManager>();
            pdfManager.GoToPagePageNumber(pageNumber);
            overlayObject.SetActive(false);
        }
    }
}
