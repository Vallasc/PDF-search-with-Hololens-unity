using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextPrevHandler : MonoBehaviour
{
    public ScrollingObjectCollection scrollView;
    public GridObjectCollection gridObjectCollection;
    public GameObject nextButton;
    public GameObject prevButton;



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePrevButton();
        UpdateNextButton();
    }

    private void UpdatePrevButton()
    {
        if (!scrollView.IsCellVisible(0) || !scrollView.IsCellVisible(1))
        {
            prevButton.SetActive(true);
        }
        else
        {
            prevButton.SetActive(false);
        }
    }

    private void UpdateNextButton()
    {
        if (!scrollView.IsCellVisible(gridObjectCollection.transform.childCount - 1))
        {
            nextButton.SetActive(true);
        }
        else
        {
            nextButton.SetActive(false);
        }
    }

    public void OnNext()
    {
        int newIndex = scrollView.FirstVisibleCellIndex + 4;
        //newIndex = newIndex > gridObjectCollection.transform.childCount - 1 ? newIndex : gridObjectCollection.transform.childCount - 1;
        scrollView.MoveToIndex(newIndex);
    }

    public void OnPrev()
    {
        int newIndex = scrollView.FirstVisibleCellIndex - 4;
        //newIndex = newIndex < 1 ? newIndex : 1;
        scrollView.MoveToIndex(newIndex);
    }
}
