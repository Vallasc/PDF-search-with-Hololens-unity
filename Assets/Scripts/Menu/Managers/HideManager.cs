using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideManager : MonoBehaviour
{
    [SerializeField]
    private GameObject menu;

    private bool scrollNew = true;
    private bool scrollFav = true;
    private bool scrollPdfs = true;

    private Vector3 size;
    private Vector3 center;

    private float buttonHeight = 0.032f;

    public void HideMenu()
    {
        size = menu.GetComponent<BoxCollider>().size;
        center = menu.GetComponent<BoxCollider>().center;
        menu.GetComponent<BoxCollider>().enabled = false;
        menu.GetComponent<BoundsControl>().enabled = false;

        scrollNew = menu.transform.Find("MenuHistory").gameObject.activeSelf;
        scrollFav = menu.transform.Find("MenuFavorites").gameObject.activeSelf;
        scrollPdfs = menu.transform.Find("MenuKeywords").gameObject.activeSelf;

        menu.transform.Find("MenuHistory").gameObject.SetActive(false);
        menu.transform.Find("MenuFavorites").gameObject.SetActive(false);
        menu.transform.Find("MenuKeywords").gameObject.SetActive(false);

        StartCoroutine(UpdateAppBarVerticalHide());
    }

    private IEnumerator UpdateAppBarVerticalHide()
    {
        Transform appBar = menu.transform.Find("AppBarVertical");
        Transform grid = appBar.Find("Container").Find("GridObjectCollection");
        Transform hide = grid.Find("Hide");

        Transform back = appBar.Find("BackgroundBar");

        int i = hide.GetSiblingIndex();
        int n = 0;

        for (int c = 0; c < grid.childCount; c++)
        {
            if (c != i + 1)
            {
                grid.GetChild(c).gameObject.SetActive(false);
            }
            else
            {
                grid.GetChild(i + 1).gameObject.SetActive(true);
                n++;
            }
        }

        back.localScale = new Vector3(back.localScale.x, n * buttonHeight, back.localScale.z);
        appBar.localPosition = new Vector3(appBar.localPosition.x, (n * buttonHeight) / -2, appBar.localPosition.z);

        yield return new WaitForEndOfFrame();
        grid.GetComponent<GridObjectCollection>().UpdateCollection();
    }

    public void ShowMenu()
    {
        menu.GetComponent<BoxCollider>().size = size;
        menu.GetComponent<BoxCollider>().center = center;
        menu.GetComponent<BoxCollider>().enabled = true;
        menu.GetComponent<BoundsControl>().enabled = true;
       

        menu.transform.Find("MenuHistory").gameObject.SetActive(scrollNew);
        menu.transform.Find("MenuFavorites").gameObject.SetActive(scrollFav);
        menu.transform.Find("MenuKeywords").gameObject.SetActive(scrollPdfs);

        StartCoroutine(UpdateAppBarVerticalShow());
    }

    private IEnumerator UpdateAppBarVerticalShow()
    {
        Transform appBar = menu.transform.Find("AppBarVertical");
        Transform grid = appBar.Find("Container").Find("GridObjectCollection");
        Transform show = grid.Find("Show");

        Transform back = appBar.Find("BackgroundBar");

        int i = show.GetSiblingIndex();
        int n = 0;

        for (int c = 0; c < grid.childCount; c++)
        {
            if (c == i || c > 4)
            {
                grid.GetChild(c).gameObject.SetActive(false);
            }
            else
            {
                grid.GetChild(c).gameObject.SetActive(true);
                n++;
            }
        }

        back.localScale = new Vector3(back.localScale.x, n * buttonHeight, back.localScale.z);
        appBar.localPosition = new Vector3(appBar.localPosition.x, (n * buttonHeight) / -2, appBar.localPosition.z);
        
        yield return new WaitForEndOfFrame();
        grid.GetComponent<GridObjectCollection>().UpdateCollection();
    }
}
