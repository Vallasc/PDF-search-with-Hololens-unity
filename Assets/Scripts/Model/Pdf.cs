using System;
using UnityEngine;

[Serializable]
public class Pdf
{
    public string _id;
    public string name;
    public int numOccKeyword;
    public int numPages;
    public Page[] pages;
    public string thumbnail;
    public int thumbnailWidth = 0;
    public int thumbnailHeight = 0;
    public bool isFav = false;
    public int numVisit = 0;

    public static Pdf CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<Pdf>(jsonString);
    }

}