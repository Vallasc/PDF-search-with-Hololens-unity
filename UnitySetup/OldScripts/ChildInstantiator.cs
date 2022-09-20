using UnityEngine;
using Vuforia;

public class ChildInstantiator : DefaultObserverEventHandler
{
    private ImageTargetBehaviour mImageTarget;
    private static int countButton = 0;
    public GameObject myPrefab;
    //public SideLoadImageTarget slit;

    protected override void OnTrackingFound()
    {
        countButton++;
        InstantiateContent();
        AddVirtualButton();
    }

    protected override void OnTrackingLost()
    {
        Destroy(myPrefab);
        DestroyVirtualButton();
        countButton--;
    }

    void InstantiateContent()
    {
        if (myPrefab != null)
        {
            GameObject myModelTrf = Instantiate(myPrefab);
            myModelTrf.transform.parent = mImageTarget.transform;
            myModelTrf.transform.localPosition = new Vector3(0f, 0f, 0f);
            myModelTrf.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            myModelTrf.transform.gameObject.SetActive(true);
        }
    }

    public void AddVirtualButton()
    {
        var mTargetVirtualButton = mImageTarget.CreateVirtualButton("button " + countButton.ToString(), new Vector2(0f, 0f), new Vector2(0.9f, 0.9f));
        mTargetVirtualButton.gameObject.AddComponent<Utils>();
        mTargetVirtualButton.RegisterOnButtonPressed(pressed => { mTargetVirtualButton.gameObject.GetComponent<Utils>().SetPDF("button " + countButton.ToString()); });
        mTargetVirtualButton.RegisterOnButtonReleased(released => { mTargetVirtualButton.gameObject.GetComponent<Utils>().OpenSelectedPDF(); });
        mTargetVirtualButton.gameObject.AddComponent<KeyRecoSpeech>();
    }

    public void DestroyVirtualButton()
    {
        mImageTarget.DestroyVirtualButton("button " + countButton.ToString());
    }

    public void SetImageTarget(ImageTargetBehaviour imageTarget)
    {
        mImageTarget = imageTarget;
    }

    public int GetCountButton()
    {
        return countButton;
    }
}
