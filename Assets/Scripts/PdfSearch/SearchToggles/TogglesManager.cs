using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TogglesManager : MonoBehaviour
{
    public Interactable favouritesToggle;
    public Interactable moreOccToggle;
    public Interactable mostViewedToggle;
    public FetchPdfs pdfSearchView;

    void Start()
    {
        moreOccToggle.IsToggled = true;
        pdfSearchView.orderMoreOccurencies = true;
        moreOccToggle.OnClick.AddListener(OnMoreClick);
        mostViewedToggle.IsToggled = false;
        pdfSearchView.orderMostViewed = false;
        mostViewedToggle.OnClick.AddListener(OnMostViewedClick);
        favouritesToggle.GetComponent<Interactable>().IsToggled = false;
        pdfSearchView.onlyFavourites = false;
        favouritesToggle.GetComponent<Interactable>().OnClick.AddListener(OnFavClick);
    }

    void Update()
    {
        
    }

    public void OnFavClick()
    {
        bool value = favouritesToggle.IsToggled;
        //Debug.Log("Fav filter " + value);
        UpdateFlags();
    }

    public void OnMoreClick()
    {
        bool value = moreOccToggle.IsToggled;
        if (!moreOccToggle.IsToggled)
        {
            moreOccToggle.IsToggled = !value;
            mostViewedToggle.IsToggled = value;
        }
        else
        {
            moreOccToggle.IsToggled = value;
            mostViewedToggle.IsToggled = !value;
        }
        //Debug.Log("More filter " + value);
        UpdateFlags();
    }

    public void OnMostViewedClick()
    {
        bool value = mostViewedToggle.IsToggled;
        if (!mostViewedToggle.IsToggled)
        {
            mostViewedToggle.IsToggled = !value;
            moreOccToggle.IsToggled = value;
        }
        else
        {
            mostViewedToggle.IsToggled = value;
            moreOccToggle.IsToggled = !value;
        }
        //Debug.Log("Most filter " + value);
        UpdateFlags();
    }

    void UpdateFlags()
    {
        pdfSearchView.orderMostViewed = mostViewedToggle.IsToggled;
        pdfSearchView.orderMoreOccurencies = moreOccToggle.IsToggled;
        pdfSearchView.onlyFavourites = favouritesToggle.IsToggled;
        pdfSearchView.OnSearch();
    }
}
