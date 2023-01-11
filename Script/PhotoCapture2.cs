using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class PhotoCapture2 : MonoBehaviour
{
	private bool camAvailable;
	private WebCamTexture cameraTexture;
	private Texture defaultBackground;

	//public RawImage background;
	//public AspectRatioFitter fit;
	public bool frontFacing;


    private float nextActionTime = 0.0f;
    private readonly float period = 5f;

    // Use this for initialization
    void Start()
	{	
		//defaultBackground = background.texture;
		WebCamDevice[] devices = WebCamTexture.devices;

		print("Photo1");
		print(devices.Length);
		if (devices.Length == 0)
			return;

		//cameraTexture = new WebCamTexture(devices[0].name, Screen.width, Screen.height);
		for (int i = 0; i < devices.Length; i++)
		{
			var curr = devices[i];

			if (curr.isFrontFacing == frontFacing)
			{
				cameraTexture = new WebCamTexture(curr.name, Screen.width, Screen.height);
				break;
			}
		}

		if (cameraTexture == null)
			return;

		print("Photo2");
		cameraTexture.Play(); // Start the camera
		print("Photo3");
		//background.texture = cameraTexture; // Set the texture

        Texture2D tex = new Texture2D(cameraTexture.width, cameraTexture.height);
		//tex.SetPixels(cameraTexture.GetPixels());
		tex.SetPixels32(cameraTexture.GetPixels32());
        tex.Apply();
        byte[] bytes = tex.EncodeToPNG();

		File.WriteAllBytes("./img.png", bytes);

        camAvailable = true; // Set the camAvailable for future purposes.
	}

    //// Update is called once per frame
    //void Update()
    //{
    //	if (!camAvailable)
    //		return;

    //	float ratio = (float)cameraTexture.width / (float)cameraTexture.height;
    //	fit.aspectRatio = ratio; // Set the aspect ratio

    //	float scaleY = cameraTexture.videoVerticallyMirrored ? -1f : 1f; // Find if the camera is mirrored or not
    //	background.rectTransform.localScale = new Vector3(1f, scaleY, 1f); // Swap the mirrored camera

    //	int orient = -cameraTexture.videoRotationAngle;
    //	background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);
    //}


    // CODICI SOTTO SONO PER FARE GLI SCREENSHOT OGNI TOT TEMPO, O UTILIZZO DI INVOKE REPEATING

    //private float time = 0.0f;
    //public float interpolationPeriod = 0.1f;

    //void Update()
    //{
    //	time += Time.deltaTime;

    //	if (time >= interpolationPeriod)
    //	{
    //		time -= interpolationPeriod;

    //		// execute block of code here
    //	}
    //}

    

    void Update()
    {
        if (Time.time > nextActionTime)
        {
            nextActionTime += period;
            print(nextActionTime);
        }
    }
}