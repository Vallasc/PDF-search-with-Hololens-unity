using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

public class DisableButtonOnScroll : MonoBehaviour
{

    public void DisableButtonBoxCollider()
    {
        Transform grid = this.transform.Find("Container").Find("GridObjectCollection");
        for (int i = 0; i < grid.childCount; i++)
        {
            grid.GetChild(i).GetComponent<BoxCollider>().enabled = false;
        }
    }
    public void EnableButtonBoxCollider()
    {
        Transform grid = this.transform.Find("Container").Find("GridObjectCollection");
        for (int i = 0; i < grid.childCount; i++)
        {
            grid.GetChild(i).GetComponent<BoxCollider>().enabled = true;
        }
    }
}
