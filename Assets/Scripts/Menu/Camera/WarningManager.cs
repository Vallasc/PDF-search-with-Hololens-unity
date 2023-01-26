using Microsoft.MixedReality.Toolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //this.transform.position = new Vector3 (CoreServices.InputSystem.EyeGazeProvider.GazeOrigin.x, CoreServices.InputSystem.EyeGazeProvider.GazeOrigin.y, CoreServices.InputSystem.EyeGazeProvider.GazeOrigin.z + 0.5f);
        this.transform.position = CoreServices.InputSystem.EyeGazeProvider.GazeOrigin + CoreServices.InputSystem.EyeGazeProvider.GazeDirection.normalized * 0.5f;
        // this.transform.rotation = Quaternion.Euler(CoreServices.InputSystem.EyeGazeProvider.GazeDirection);
        //this.transform.rotation = new Vector3(this.transform.rotation.x, 0, this.transform.rotation.z);


    }
}
