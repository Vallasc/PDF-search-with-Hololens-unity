using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SearchKeyword : MonoBehaviour
{
    public GameObject inputField = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnSearchButtonClick()
    {
        Debug.Log(inputField);
        Debug.Log(inputField.GetComponent<TMP_InputField>().text);
    }
}
