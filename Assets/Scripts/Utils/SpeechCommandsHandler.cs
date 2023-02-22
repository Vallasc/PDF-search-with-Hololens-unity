using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechCommandsHandler : MonoBehaviour
{
    public HeadGaze headGaze;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CloseLastFocused()
    {
        GameObject last = headGaze.GetLastFocusedObject();
        if (last.name.Equals("PdfView(Clone)"))
            last.SetActive(false);
    }
}
