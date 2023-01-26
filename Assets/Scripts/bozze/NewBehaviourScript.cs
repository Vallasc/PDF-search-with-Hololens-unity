using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject SecondaryMenu;
    public GameObject windows;
    public GameObject favoritesList;
    public TMP_Text windowsTitle;

    private void Awake()
    {
        MainMenu.SetActive(true);
        SecondaryMenu.SetActive(false);
        windows.SetActive(false);
        favoritesList.SetActive(false);
    }
    public void ButtonClickedWithGameObject(GameObject selButton)
    {
        Debug.Log(selButton.name);
        MainMenu.SetActive(!MainMenu.activeSelf);
        SecondaryMenu.SetActive(!SecondaryMenu.activeSelf);
        switch (selButton.name)
        {
            case "PdfImageButton":
                LaunchPdfImage();
                break;
            case "FavoritesButton":
                LaunchFavorites();
                break;
            case "SearchButton":
                LaunchSearchButton();
                break;
            case "HelpButton":
                LaunchHelp();
                break;
            case "btnHome":
                ReturnHome();
                break;
            default:
                ReturnHome();
                break;
        }

    }
    #region Launch Tutorials
    private void LaunchPdfImage()
    {
        EnableSecondaryMenu();
        windows.SetActive(true);
        //favoritesList.SetActive(false);
        windowsTitle.text = "Pdf Image";
    }
    private void LaunchFavorites()
    {
  
        EnableSecondaryMenu();
        favoritesList.SetActive(true);
        
        //windows.SetActive(true);
        /* EnableSecondaryMenu();
            windows.SetActive(true);
            windowsTitle.text = "Tutorial2";*/


    }
    private void LaunchSearchButton()
    {
        EnableSecondaryMenu();
        windows.SetActive(true);
        windowsTitle.text = "Search";
    }
    #endregion
    private void LaunchHelp()
    {
        if (MainMenu.activeSelf == true)
        {
            EnableSecondaryMenu();
            windows.SetActive(true);
            //favoritesList.SetActive(false);
            windowsTitle.text = "Help";
        }
        else
        {
            windows.SetActive(true);
            favoritesList.SetActive(false);
            windowsTitle.text = "Help";
        }

    }
    #region Enable menues
    private void ReturnHome()
    {
        EnableMainMenu();
        favoritesList.SetActive(false);
        windows.SetActive(false);
    }
    private void EnableMainMenu()
    {
        MainMenu.SetActive(true);
        SecondaryMenu.SetActive(false);
        favoritesList.SetActive(false);
        windows.SetActive(false);
    }
    private void EnableSecondaryMenu()
    {
        MainMenu.SetActive(false);
        SecondaryMenu.SetActive(true);
    }
    #endregion
}
