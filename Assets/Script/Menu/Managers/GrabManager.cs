using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;

public class GrabManager : MonoBehaviour
{
    [SerializeField]
    private GameObject menu;
    [SerializeField]
    private GameObject buttonPin = null;
    [SerializeField]
    private BoxCollider box;

    private bool radialView;

    public void OnManipulationStarted()
    {
        Debug.Log("levato");
        radialView = menu.GetComponent<RadialView>().enabled;
        menu.GetComponent<RadialView>().enabled = false;
        //box.enabled = false;
    }

    public void OnManipulationEnded()
    {
        menu.GetComponent<RadialView>().enabled = radialView;
        //box.enabled = false;
    }

    public void UpdateButtonPin()
    {
        if (buttonPin.GetComponent<ButtonConfigHelper>().MainLabelText == "PIN")
        {
            buttonPin.GetComponent<ButtonConfigHelper>().MainLabelText = "UNPIN";
        }
        else
        {
            buttonPin.GetComponent<ButtonConfigHelper>().MainLabelText = "PIN";
        }

    }
}
