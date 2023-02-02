using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using TMPro;

// CAMBIARE NOME
public class GrabManager : MonoBehaviour
{
    [SerializeField]
    private GameObject menu;
    [SerializeField]
    private GameObject buttonPin = null;
    [SerializeField]
    private BoxCollider box;
    [SerializeField]
    private Texture2D pin;
    [SerializeField]
    private Texture2D follow;

    private string tmpPin = "Say \"Pin\"";
    private string tmpFollow = "Say \"Follow Me\"";

    private RadialView radialView;
    private bool radialViewEn;
    [SerializeField]
    private TextMeshPro tmp;



    void Start()
    {
        radialView = menu.GetComponent<RadialView>();
        if (radialView.enabled)
        {
            ChangeButtonToPin();
        }
        else
        {
            ChangeButtonToFollowMe();
        }
    }

    public void OnManipulationStarted()
    {
        Debug.Log("levato");
        radialViewEn = radialView.enabled;
        menu.GetComponent<RadialView>().enabled = false;
        //box.enabled = false;
    }

    public void OnManipulationEnded()
    {
        menu.GetComponent<RadialView>().enabled = radialViewEn;
        //box.enabled = false;
    }

    public void UpdateButtonPin()
    {
        if (buttonPin.GetComponent<ButtonConfigHelper>().MainLabelText == "Pin")
        {
            buttonPin.GetComponent<ButtonConfigHelper>().MainLabelText = "Follow me";
            buttonPin.GetComponent<ButtonConfigHelper>().SetQuadIcon(follow);
            //buttonPin.GetComponent<ButtonConfigHelper>().SetQuadIconByName("IconFollowMe");
        }
        else
        {
            buttonPin.GetComponent<ButtonConfigHelper>().MainLabelText = "Pin";
            buttonPin.GetComponent<ButtonConfigHelper>().SetQuadIcon(pin);
            //buttonPin.GetComponent<ButtonConfigHelper>().SetQuadIconByName("IconPin");
        }

    }

    public void ChangeButtonToPin()
    {
        buttonPin.GetComponent<ButtonConfigHelper>().MainLabelText = "Pin";
        buttonPin.GetComponent<ButtonConfigHelper>().SetQuadIcon(pin);
        //buttonPin.GetComponent<ButtonConfigHelper>().SetQuadIconByName("IconPin");
        ChangeSISIToPin();
    }

    public void ChangeButtonToFollowMe()
    {
        buttonPin.GetComponent<ButtonConfigHelper>().MainLabelText = "Follow Me";
        buttonPin.GetComponent<ButtonConfigHelper>().SetQuadIcon(follow);
        //buttonPin.GetComponent<ButtonConfigHelper>().SetQuadIconByName("IconFollowMe");
        ChangeSISIToFollowMe();
    }

    public void ChangeSISIToPin()
    {
        tmp.text = tmpPin;
    }

    public void ChangeSISIToFollowMe()
    {
        tmp.text = tmpFollow;
    }
}
