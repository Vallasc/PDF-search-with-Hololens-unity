using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;
using UnityEditor;
using UnityEngine.Windows.Speech;

public class SpeechKeyword : MonoBehaviour
{
    //[SerializeField]
    private SpeechCommands speechCommand;
    private string keyword = "";
    private string baseKeyword = "Open ";


    private KeywordRecognizer kr = null;
    [SerializeField]
    //private List<string> keywords;
    private List<string> keywords;



    //public void Init(MixedRealitySpeechCommandsProfile profile, int num)
    //{

    //}

    void Start()
    {
        List<string> keywords = new List<string>();
        //kr = new KeywordRecognizer(keywords.ToArray(), ConfidenceLevel.Low);
        //kr.OnPhraseRecognized += OnPhraseRecognized;
        //kr.Start();
        //Debug.Log(kr.IsRunning);
    }

    public void UpdateKeywordRecognizer(int num)
    {
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
                    Debug.Log(newKey);
                }
            }

            if (kr != null)
            {
                kr.Dispose();
                Debug.Log(kr.IsRunning);
            }

            if (keywords.Count > 0)
            {
                kr = new KeywordRecognizer(keywords.ToArray(), ConfidenceLevel.Low);
                kr.OnPhraseRecognized += OnPhraseRecognized;
                kr.Start();
            }
            
            Debug.Log(kr.IsRunning);
        }
    }

    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        Debug.Log(args.text);
        int index = keywords.IndexOf(args.text) + 1;
        Debug.Log(index);
        index = 0;
        string[] array = keywords.ToArray();
        bool found = false;
        while (!found && index < array.Length)
        {
            if (string.Equals(args.text, array[index]))
            {
                found = true;
            }
            index++;
        }
        
        Debug.Log(index);
        //this.gameObject.GetComponent<KeywordsManager>().OnKeywordRecognized(index);
        this.gameObject.GetComponent<KeywordsManager>().OnKeywordRecognized_Prova(index);
    }

    //void IMixedRealitySpeechHandler.OnSpeechKeywordRecognized(SpeechEventData eventData)
    //{
    //    Debug.Log("entrato: " + eventData.Command.Keyword);
    //    if (string.Equals(eventData.Command.Keyword, this.keyword))
    //    {
    //        Debug.Log("sentitaaaaaa");
    //    }
    //}

    void OnEnable()
    {
        if (kr != null)
        {
            kr.Start();
        }
    }

    void OnDisable()
    {
        kr.Stop();
    }

    void OnDestroy()
    {
        kr.Dispose();
    }
}

// POSSIBILE WORKAROUND IN CASO DI ESECUZIONE LENTE SU UNITY

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.Windows.Speech;
 
//public class testscript : MonoBehaviour
//{
//    bool initialized = false;
//    bool directInitComplete = false;

//    public Text text;
//    // Use this for initialization
//    void Start()
//    {
//        InitializeWindowsSpeechDirectly();
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (!initialized && directInitComplete)
//        {
//            Debug.Log("Creating KeywordRecognizer!");

//            float timeStart = Time.realtimeSinceStartup;
//            var r = new KeywordRecognizer(new string[] { "test-keyword" }, ConfidenceLevel.Low);
//            float totalTime = Time.realtimeSinceStartup - timeStart;
//            Debug.Log("new KeywordRecognizer took " + totalTime + "s");

//            if (text != null) text.text = "new KeywordRecognizer took " + totalTime + "s";

//            r.OnPhraseRecognized += (args) =>
//            {
//                Debug.Log("Phrase recognized!");
//            };

//            Debug.Log("Starting KeywordRecognizer!");
//            r.Start();

//            initialized = true;

//            Debug.Log("Finished initialization!");
//        }
//    }

//#if ENABLE_WINMD_SUPPORT
//    Windows.Media.SpeechRecognition.SpeechRecognizer _recognizer;
//#endif

//    private void InitializeWindowsSpeechDirectly()
//    {
//#if ENABLE_WINMD_SUPPORT
 
//        Windows.System.Threading.ThreadPool.RunAsync((workItem) =>
//        {
//            // WinRT APIs will throw exceptions
//            try
//            {
//                var recogonizer = new Windows.Media.SpeechRecognition.SpeechRecognizer();
 
//                _recognizer = recogonizer;
//                directInitComplete = true;
//            }
//            catch { };
//        });
//#else
//        directInitComplete = true;
//#endif

//    }
//}