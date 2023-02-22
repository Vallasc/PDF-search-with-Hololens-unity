using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechCommandsHandler : MonoBehaviour
{
    public HeadGaze headGaze;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void CloseLastFocused()
    {
        GameObject last = headGaze.GetLastFocusedObject();
        if (last.name.Equals("PdfView(Clone)"))
            last.SetActive(false);
        if (last.name.Equals("PdfSearch"))
            this.GetComponent<InterfaceManager>().SwitchToMenu();
    }

    public void FollowMeLastFocused()
    {
        GameObject last = headGaze.GetLastFocusedObject();
        if (last.name.Equals("PdfView(Clone)") || last.name.Equals("PdfSearch"))
            last.GetComponent<FollowMeButton>().SetFollowMe();
    }

    public void PinLastFocused()
    {
        GameObject last = headGaze.GetLastFocusedObject();
        if (last.name.Equals("PdfView(Clone)") || last.name.Equals("PdfSearch"))
            last.GetComponent<FollowMeButton>().SetPin();
    }

    public void NextPage()
    {
        GameObject last = headGaze.GetLastFocusedObject();
        if (last.name.Equals("PdfView(Clone)"))
            last.GetComponent<PdfManager>().GoNextPage();
    }

    public void PrevPage()
    {
        GameObject last = headGaze.GetLastFocusedObject();
        if (last.name.Equals("PdfView(Clone)"))
            last.GetComponent<PdfManager>().GoPreviousPage();
    }

    public void AddToFavourites()
    {
        GameObject last = headGaze.GetLastFocusedObject();
        if (last.name.Equals("PdfView(Clone)"))
            last.GetComponent<FavManager>().SetFav(true);
    }

    public void RemoveFromFavourites()
    {
        GameObject last = headGaze.GetLastFocusedObject();
        if (last.name.Equals("PdfView(Clone)"))
            last.GetComponent<FavManager>().SetFav(false);
    }

    public void ShowKeyword()
    {
        GameObject last = headGaze.GetLastFocusedObject();
        if (last.name.Equals("PdfView(Clone)"))
            last.GetComponent<KeyManager>().ShowKeyword();
    }

    public void HideKeyword()
    {
        GameObject last = headGaze.GetLastFocusedObject();
        if (last.name.Equals("PdfView(Clone)"))
            last.GetComponent<KeyManager>().HideKeyword();
    }
}
