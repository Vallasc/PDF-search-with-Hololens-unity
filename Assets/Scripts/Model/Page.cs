using System;
using UnityEngine;

[Serializable]
public class Page
{
    public int number;
    public string pdfId;
    public string thumbnail;
    public int thumbnailWidth = 0;
    public int thumbnailHeight = 0;
    public string url;

    public static Page CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<Page>(jsonString);
    }

}
