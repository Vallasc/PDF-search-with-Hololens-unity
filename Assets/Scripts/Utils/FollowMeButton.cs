using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class FollowMeButton : MonoBehaviour
{
    public GameObject followMeToggleObject;
    public bool followMeEnabled = true;
    private GameObject pinObject;
    private GameObject followObject;

    void Start()
    {
        pinObject = transform.Find("Pin").gameObject;
        followObject = transform.Find("FollowMe").gameObject;
        pinObject.SetActive(followMeEnabled);
        followObject.SetActive(!followMeEnabled);
    }

    void Update() {}

    public void OnClick()
    {
        followMeEnabled = !followMeEnabled;
        followMeToggleObject.GetComponent<FollowMeToggle>().ToggleFollowMeBehavior();
        pinObject.SetActive(followMeEnabled);
        followObject.SetActive(!followMeEnabled);
    }
}
