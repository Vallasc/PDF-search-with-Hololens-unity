using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;
using UnityEditor;
using UnityEngine.Windows.Speech;

public class SpeechKeyword : MonoBehaviour
{
    private string baseKeyword = "Open ";

    private KeywordRecognizer kr = null;
    private List<string> keywords;

    void Start()
    {
        keywords = new List<string>();
    }

    public void UpdateKeywordRecognizer(int num)
    {
        Debug.Log("update speech");
        if (keywords.Count != num)
        {
            if (keywords.Count > num)
            {
                int count = keywords.Count;
                for (int i = (count - num); i > 0; i--)
                {
                    keywords.Remove(keywords[keywords.Count - 1]);
                }
            }
            else
            {
                int count = keywords.Count;
                for (int i = (num - count); i > 0; i--)
                {
                    int index = keywords.Count + 1;
                    string newKey = baseKeyword + Converter.NumToString.NumIntToWords(index.ToString());
                    keywords.Add(newKey);
                }
            }

            if (kr != null)
            {
                kr.Dispose();
            }

            if (keywords.Count > 0)
            {
                kr = new KeywordRecognizer(keywords.ToArray(), ConfidenceLevel.Low);
                kr.OnPhraseRecognized += OnPhraseRecognized;
                kr.Start();
            }
        }
    }

    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        int index = keywords.IndexOf(args.text) + 1;
        
        this.gameObject.GetComponent<KeywordsManager>().OnKeywordRecognized(index);
    }

    void OnEnable()
    {
        if (kr != null)
        {
            kr.Start();
        }
    }

    void OnDisable()
    {
        if (kr != null)
        {
            kr.Stop();
        }
    }

    void OnDestroy()
    {
        if (kr != null)
        {
            kr.Dispose();
        }
    }
}