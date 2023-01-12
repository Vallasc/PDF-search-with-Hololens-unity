using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;

public class PdfCollectionUpdater : MonoBehaviour
{
    public GameObject menu;
    public GameObject pdfList;
    public ScrollingObjectCollection scrollingCollection;
    public GridObjectCollection objectCollection;
    public GameObject back;
    private bool visibility = false;
    private const float scrollOffsetX = 0.12f;
    private const float backplateOffsetX = 0.06f;
    private const float backplateScale = 2f;

    public void UpdateObjectCollection(int count)
    {
        Debug.Log("DENTRO");
        UpdateGraphic("AppBar", "Backplate");

        //StartCoroutine(UpdateObjectCollection_Coroutine(count));
        visibility = !visibility;
        Debug.Log("FUORI");
    }

    // MODIFICARE IL VALORE DA AGGUNGERE/SOTTRARRE, DIPENDE DALL'ANGOLAZIONE CHE SI HA E FORSE ANCHE DOVE CI SI TROVA RISPETTO ALLA TELECAMERA
    private void UpdateGraphic(string menuBar, string backplate)
    {
        bool enabled = false;
        RadialView comp = menu.GetComponent<RadialView>();
        menu.GetComponent<Billboard>().enabled = false;
        if (comp.enabled)
        {
            comp.enabled = false;
            enabled = true;
        }

        //scrollingCollection.gameObject.SetActive(!visibility);

        Transform menuBarTransform = GameObject.Find(menuBar).transform;
        Transform backplateTransform = GameObject.Find(backplate).transform;


        if (!pdfList.activeSelf)
        {
            if (back.activeSelf)
            {
                backplateTransform.localScale = new Vector3(backplateTransform.localScale.x * backplateScale, backplateTransform.localScale.y, backplateTransform.localScale.z);
                //menuBarTransform.localPosition += new Vector3(buttonOffsetX, 0, 0);
                backplateTransform.localPosition += new Vector3(backplateOffsetX, 0, 0);
            }
            else
            {
                back.SetActive(true);
            }
            pdfList.SetActive(true);
        }
        else
        {
            if (scrollingCollection.transform.Find("Container").Find("GridObjectCollection").childCount == 0)
            {
                if (back.transform.localScale.x == 2f)
                {
                    backplateTransform.localScale = new Vector3(backplateTransform.localScale.x / backplateScale, backplateTransform.localScale.y, backplateTransform.localScale.z);
                    //menuBarTransform.localPosition -= new Vector3(buttonOffsetX, 0, 0);
                    backplateTransform.localPosition -= new Vector3(backplateOffsetX, 0, 0);
                }
                else
                {
                    back.SetActive(false);
                }
                pdfList.SetActive(false);
            }
        }

        menu.GetComponent<Billboard>().enabled = true;
        if (enabled)
        {
            comp.enabled = true;
        }
    }

    public void ShiftLeftPdfCollection()
    {
        GameObject backplateTransform = GameObject.Find("Backplate");

        
        //if (scrollingCollection.gameObject.activeSelf)
        if (pdfList.activeSelf)
        {
            backplateTransform.transform.localScale = new Vector3(backplateTransform.transform.localScale.x / backplateScale, backplateTransform.transform.localScale.y, backplateTransform.transform.localScale.z);
            backplateTransform.transform.localPosition -= new Vector3(backplateOffsetX, 0, 0);
            
        }
        else
        {
            backplateTransform.SetActive(false);
        }

        pdfList.transform.localPosition -= new Vector3(scrollOffsetX, 0, 0);
    }

    public void ShiftRightPdfCollection()
    {
        GameObject backplateTransform = GameObject.Find("Backplate");

        //if (scrollingCollection.gameObject.activeSelf)
        if (pdfList.activeSelf)
        {
            backplateTransform.transform.localScale = new Vector3(backplateTransform.transform.localScale.x * backplateScale, backplateTransform.transform.localScale.y, backplateTransform.transform.localScale.z);
            backplateTransform.transform.localPosition += new Vector3(backplateOffsetX, 0, 0);
        }
        else
        {
            back.SetActive(true);
        }

        pdfList.transform.localPosition += new Vector3(scrollOffsetX, 0, 0);
    }

    public IEnumerator UpdateObjectCollection_Coroutine(int count)
    {
        PressableButtonHoloLens2 button = GameObject.Find("PressableButtonHoloLens2_32x96_NoLabel").GetComponent<PressableButtonHoloLens2>();
        for (int i = 0; i < count; i++)
        {
            Instantiate(button, objectCollection.transform);
        }
        
        yield return new WaitForEndOfFrame();
        objectCollection.UpdateCollection();
        yield return new WaitForEndOfFrame();
        scrollingCollection.UpdateContent();
    }
}
