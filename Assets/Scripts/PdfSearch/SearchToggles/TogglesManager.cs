using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TogglesManager : MonoBehaviour
{
    public GameObject favouritesToggle;
    public GameObject moreOccToggle;
    public GameObject mostViewedToggle;
    public GameObject pdfSearchView;

    private FetchPdfs pdfSearch;
    private

    void Start()
    {
        pdfSearch = pdfSearchView.GetComponent<FetchPdfs>();
        moreOccToggle.GetComponent<Interactable>().IsToggled = true;
        pdfSearch.orderMoreOccurencies = true;
        moreOccToggle.GetComponent<Interactable>().OnClick.AddListener(OnMoreClick);
        mostViewedToggle.GetComponent<Interactable>().IsToggled = false;
        pdfSearch.orderMostViewed = false;
        mostViewedToggle.GetComponent<Interactable>().OnClick.AddListener(OnMostViewedClick);
        favouritesToggle.GetComponent<Interactable>().IsToggled = false;
        pdfSearch.onlyFavourites = false;
        favouritesToggle.GetComponent<Interactable>().OnClick.AddListener(OnFavClick);
    }

    void Update()
    {
        
    }

    public void OnFavClick()
    {
        bool value = favouritesToggle.GetComponent<Interactable>().IsToggled;
        Debug.Log("Fav filter " + value);
        UpdateFlags();
    }

    public void OnMoreClick()
    {
        bool value = moreOccToggle.GetComponent<Interactable>().IsToggled;
        mostViewedToggle.GetComponent<Interactable>().IsToggled = !value;
        Debug.Log("More filter " + value);
        UpdateFlags();
    }

    public void OnMostViewedClick()
    {
        bool value = mostViewedToggle.GetComponent<Interactable>().IsToggled;
        moreOccToggle.GetComponent<Interactable>().IsToggled = !value;
        Debug.Log("Most filter " + value);
        UpdateFlags();
    }

    void UpdateFlags()
    {
        pdfSearch.orderMostViewed = mostViewedToggle.GetComponent<Interactable>().IsToggled;
        pdfSearch.orderMoreOccurencies = moreOccToggle.GetComponent<Interactable>().IsToggled;
        pdfSearch.onlyFavourites = favouritesToggle.GetComponent<Interactable>().IsToggled;
        pdfSearch.OnSearch();
    }
}
