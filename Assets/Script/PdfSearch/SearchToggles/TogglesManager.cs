using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TogglesManager : MonoBehaviour
{
    public GameObject favouritesToggle;
    public GameObject moreOccToggle;
    public GameObject mostViewedToggle;

    void Start()
    {
        moreOccToggle.GetComponent<Interactable>().IsToggled = true;
        moreOccToggle.GetComponent<Interactable>().OnClick.AddListener(OnMoreClick);
        mostViewedToggle.GetComponent<Interactable>().IsToggled = false;
        mostViewedToggle.GetComponent<Interactable>().OnClick.AddListener(OnMostViewedClick);
        favouritesToggle.GetComponent<Interactable>().IsToggled = false;
        favouritesToggle.GetComponent<Interactable>().OnClick.AddListener(OnFavClick);
    }

    void Update()
    {
        
    }


    void OnMoreClick()
    {

    }

    void OnFavClick()
    {
        Debug.Log(favouritesToggle.GetComponent<Interactable>().IsToggled);
    }

    void OnMostViewedClick()
    {

    }
}
