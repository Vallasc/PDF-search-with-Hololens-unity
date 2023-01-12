using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

public class GrabManager : MonoBehaviour
{
    [SerializeField]
    private GameObject buttonPin = null;
    [SerializeField]
    private BoxCollider box;

    public void UpdateGrab()
    {
        box.enabled = !box.enabled;

        if (box.enabled)
        {
            buttonPin.GetComponent<ButtonConfigHelper>().MainLabelText = "UNPIN";
        }
        else
        {
            buttonPin.GetComponent<ButtonConfigHelper>().MainLabelText = "PIN";
        }

    }
}
