using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class FollowMeButton : MonoBehaviour
{
    public FollowMeToggle followMeToggle;
    public bool followMeEnabled = true;
    public GameObject followMeButtonObject;
    private GameObject pinObject;
    private GameObject followObject;

    void Start()
    {
        pinObject = followMeButtonObject.transform.Find("Pin").gameObject;
        followObject = followMeButtonObject.transform.Find("FollowMe").gameObject;
        pinObject.SetActive(followMeEnabled);
        followObject.SetActive(!followMeEnabled);
    }

    void Update() {}

    public void OnClick()
    {
        followMeEnabled = !followMeEnabled;
        followMeToggle.ToggleFollowMeBehavior();
        pinObject.SetActive(followMeEnabled);
        followObject.SetActive(!followMeEnabled);
    }

    public void SetPin()
    {
        if (followMeEnabled)
            OnClick();
    }

    public void SetFollowMe()
    {
        if (!followMeEnabled)
            OnClick();
    }
}
