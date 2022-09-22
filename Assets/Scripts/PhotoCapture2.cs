using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PhotoCapture2 : MonoBehaviour
{

	private bool camAvailable;
	private WebCamTexture cameraTexture;
	private Texture defaultBackground;

	public RawImage background;
	public AspectRatioFitter fit;
	public bool frontFacing;

	// Use this for initialization
	void Start()
	{	
		defaultBackground = background.texture;
		WebCamDevice[] devices = WebCamTexture.devices;

		print("Photo1");
		print(devices.Length);
		if (devices.Length == 0)
			return;

		cameraTexture = new WebCamTexture(devices[0].name, Screen.width, Screen.height);

		print("Photo2");
		cameraTexture.Play(); // Start the camera
		print("Photo3");
		background.texture = cameraTexture; // Set the texture

		camAvailable = true; // Set the camAvailable for future purposes.
	}

	// Update is called once per frame
	void Update()
	{
		if (!camAvailable)
			return;

		float ratio = (float)cameraTexture.width / (float)cameraTexture.height;
		fit.aspectRatio = ratio; // Set the aspect ratio

		float scaleY = cameraTexture.videoVerticallyMirrored ? -1f : 1f; // Find if the camera is mirrored or not
		background.rectTransform.localScale = new Vector3(1f, scaleY, 1f); // Swap the mirrored camera

		int orient = -cameraTexture.videoRotationAngle;
		background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);
	}
}