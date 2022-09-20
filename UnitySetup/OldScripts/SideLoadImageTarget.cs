using UnityEngine;
using Vuforia;
using System;
public class SideLoadImageTarget : MonoBehaviour
{
    public ChildInstantiator child;
    //void Start()
    //{
    //    // Use Vuforia Application to invoke the function after Vuforia Engine is initialized
    //    //VuforiaApplication.Instance.OnVuforiaStarted += CreateImageTargetFromSideloadedTexture;
    //}
    //void CreateImageTargetFromSideloadedTexture(Texture2D texture,  float size, string name)
    //{
    //    textureFile = texture;
    //    printedTargetSize = size;
    //    targetName = name;

    //    var mTarget = VuforiaBehaviour.Instance.ObserverFactory.CreateImageTarget(
    //        textureFile,
    //        printedTargetSize,
    //        targetName);
    //    // add the Default Observer Event Handler to the newly created game object
    //    mTarget.gameObject.AddComponent<ChildInstantiator>();
    //    Debug.Log("Instant Image Target created " + mTarget.TargetName);
    //}

    public ImageTargetBehaviour CreateImageTargetFromTexture(Texture2D texture, float widthInMeters, string targetName)
    {
        if (child.GetCountButton() < 5)
        {
            var mImageTarget = VuforiaBehaviour.Instance.ObserverFactory.CreateImageTarget(texture, widthInMeters, targetName);
            mImageTarget.gameObject.AddComponent<ChildInstantiator>();
            mImageTarget.gameObject.GetComponent<ChildInstantiator>().SetImageTarget(mImageTarget);
            return mImageTarget;
        }
        else
        {
            return null;
        }
    }

    public void AddVirtualButtonToImageTarget(ImageTargetBehaviour mTarget)
    {
        //var mTarget = CreateImageTargetFromTexture(textureFile, printedTargetSize, targetName);
        mTarget.gameObject.AddComponent<ChildInstantiator>();
        mTarget.gameObject.GetComponent<ChildInstantiator>().SetImageTarget(mTarget);
        var mTargetVirtualButton = mTarget.CreateVirtualButton("button" + mTarget.TargetName, new Vector2(0f, 0f), new Vector2(0.9f, 0.9f));
        mTargetVirtualButton.gameObject.AddComponent<Utils>();
        mTargetVirtualButton.RegisterOnButtonPressed(pressed => { mTargetVirtualButton.gameObject.GetComponent<Utils>().SetPDF("button" + mTarget.TargetName); });
        mTargetVirtualButton.RegisterOnButtonReleased(released => { mTargetVirtualButton.gameObject.GetComponent<Utils>().OpenSelectedPDF(); });
        mTargetVirtualButton.gameObject.AddComponent<KeyRecoSpeech>();
    }

    //public void LoadImageTarget(string dataSetPath, string imageTargetName)
    //{
    //    VuforiaBehaviour.Instance.ObserverFactory.CreateImageTarget(dataSetPath, imageTargetName);
    //}
}
