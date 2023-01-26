using System;
using UnityEngine;

[Serializable]
public class PdfsResponse
{
    public Pdf[] pdfs;

    public static PdfsResponse CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<PdfsResponse>(jsonString);
    }

}