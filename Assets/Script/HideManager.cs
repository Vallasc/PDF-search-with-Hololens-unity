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
    //private bool back = true;

    public void HideMenu()
    {
        scrollNew = menu.transform.Find("MenuHistory").gameObject.activeSelf;
        scrollFav = menu.transform.Find("MenuFavorites").gameObject.activeSelf;
        scrollPdfs = menu.transform.Find("MenuPdfs").gameObject.activeSelf;
        //back = menu.transform.Find("Backplate").gameObject.activeSelf;

        menu.transform.Find("MenuHistory").gameObject.SetActive(false);
        menu.transform.Find("MenuFavorites").gameObject.SetActive(false);
        menu.transform.Find("MenuPdfs").gameObject.SetActive(false);
        //menu.transform.Find("Backplate").gameObject.SetActive(false);
    }

    public void ShowMenu()
    {
        menu.transform.Find("MenuHistory").gameObject.SetActive(scrollNew);
        menu.transform.Find("MenuFavorites").gameObject.SetActive(scrollFav);
        menu.transform.Find("MenuPdfs").gameObject.SetActive(scrollPdfs);
        //menu.transform.Find("Backplate").gameObject.SetActive(back);
    }
}
