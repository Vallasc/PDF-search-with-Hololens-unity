using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadGaze : MonoBehaviour
{
    private GameObject lastFocusedObject = null;

    void Start()
    {
        
    }

    void Update()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(
                Camera.main.transform.position,
                Camera.main.transform.forward,
                out hitInfo,
                20.0f,
                Physics.DefaultRaycastLayers))
        {
            // If the Raycast has succeeded and hit a hologram
            // hitInfo's point represents the position being gazed at
            // hitInfo's collider GameObject represents the hologram being gazed at
            lastFocusedObject = hitInfo.transform.gameObject;
        }
    }

    public GameObject GetLastFocusedObject()
    {
        GameObject gameObject = lastFocusedObject;
        while (gameObject.transform.parent != null && !gameObject.name.Equals("PdfSearch") && !gameObject.name.Equals("PdfView(Clone)") && !gameObject.name.Equals("Menu"))
            gameObject = gameObject.transform.parent.gameObject;
        return gameObject;
    }
}
