using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.WebCam;
using System;
using Vuforia;

public class KeyRecoSpeech : MonoBehaviour, IMixedRealitySpeechHandler
{
    public ChildInstantiator child;
    public Utils utils;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("START SPEECH DEBUG");
        CoreServices.InputSystem?.RegisterHandler<IMixedRealitySpeechHandler>(this);
    }

    public void OnSpeechKeywordRecognized(SpeechEventData eventData)
    {
        Debug.Log("KEYWORD SPEECH DEBUG");
        switch (eventData.Command.Keyword.ToLower())
        {
            case "button one":
                if (child.GetCountButton() >= 1)
                {
                    utils.SetPDF("button one");
                    Debug.Log("button one DEBUG");
                }
                break;

            case "button two":
                if (child.GetCountButton() >= 2)
                {
                    utils.SetPDF("button two");
                    Debug.Log("button two DEBUG");
                }
                break;

            case "button three":
                if (child.GetCountButton() >= 3)
                {
                    utils.SetPDF("button three");
                    Debug.Log("button three DEBUG");
                }
                break;

            case "button four":
                if (child.GetCountButton() >= 4)
                {
                    utils.SetPDF("button four");
                    Debug.Log("button four DEBUG");
                }
                break;

            case "button five":
                if (child.GetCountButton() >= 5)
                {
                    utils.SetPDF("button five");
                    Debug.Log("button five DEBUG");
                }
                break;

            default:
                Debug.Log($"Unknown option { eventData.Command.Keyword}");
                break;
        }
    }
}
