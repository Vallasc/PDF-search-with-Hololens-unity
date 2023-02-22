using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using TMPro;


public class GrabManager : MonoBehaviour
{
    [SerializeField]
    private GameObject menu;
    [SerializeField]
    private GameObject buttonPin = null;
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
        radialViewEn = radialView.enabled;
        menu.GetComponent<RadialView>().enabled = false;
    }

    public void OnManipulationEnded()
    {
        menu.GetComponent<RadialView>().enabled = radialViewEn;
    }

    public void UpdateButtonPin()
    {
        if (buttonPin.GetComponent<ButtonConfigHelper>().MainLabelText == "Pin")
        {
            ChangeButtonToFollowMe();
        }
        else
        {
            ChangeButtonToPin();
        }

    }

    public void ChangeButtonToPin()
    {
        buttonPin.GetComponent<ButtonConfigHelper>().MainLabelText = "Pin";
        buttonPin.GetComponent<ButtonConfigHelper>().SetQuadIcon(pin);
        ChangeSISIToPin();
    }

    public void ChangeButtonToFollowMe()
    {
        buttonPin.GetComponent<ButtonConfigHelper>().MainLabelText = "Follow Me";
        buttonPin.GetComponent<ButtonConfigHelper>().SetQuadIcon(follow);
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
