using System;
using UnityEngine;

[Serializable]
public class PageImageResponse
{
    public string img;
    public int width;
    public int height;

    public static PageImageResponse CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<PageImageResponse>(jsonString);
    }

}
