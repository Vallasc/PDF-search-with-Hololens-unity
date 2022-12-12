using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using TMPro;

public class KeywordList : MonoBehaviour
{
    List<string> keys = new List<string>()
        {
            "John Doe",
            "Jane Doe",
            "Joe Doe",
            "Another Doe",
            "John Doe",
            "Jane Doe",
            "Joe Doe",
            "John Doe",
            "Jane Doe",
            "Joe Doe"
        };

    [SerializeField]
    [Tooltip("The ScrollingObjectCollection to populate, if left empty. the populator will create on your behalf.")]
    private ScrollingObjectCollection scrollView;

    [SerializeField]
    [Tooltip("Object to duplicate in ScrollCollection")]
    private GameObject dynamicItem;

    private GridObjectCollection gridObjectCollection;

    [SerializeField]
    private float cellWidth = 0.1f;

    [SerializeField]
    private float cellHeight = 0.032f;

    [SerializeField]
    private float cellDepth = 0.032f;

    [SerializeField]
    private int cellsPerTier = 1;

    [SerializeField]
    private int tiersPerPage = 5;

    [SerializeField]
    private Transform scrollPositionRef = null;

    // Start is called before the first frame update
    void Start()
    {
        if (scrollView == null)
        {
            GameObject newScroll = new GameObject("Scrolling Object Collection");
            newScroll.transform.parent = scrollPositionRef ? scrollPositionRef : transform;
            newScroll.transform.localPosition = Vector3.zero;
            newScroll.transform.localRotation = Quaternion.identity;
            newScroll.SetActive(false);
            scrollView = newScroll.AddComponent<ScrollingObjectCollection>();

            // Prevent the scrolling collection from running until we're done dynamically populating it.
            scrollView.CellWidth = cellWidth;
            scrollView.CellHeight = cellHeight;
            scrollView.CellDepth = cellDepth;
            scrollView.CellsPerTier = cellsPerTier;
            scrollView.TiersPerPage = tiersPerPage;
        }

        gridObjectCollection = scrollView.GetComponentInChildren<GridObjectCollection>();

        if (gridObjectCollection == null)
        {
            GameObject collectionGameObject = new GameObject("Grid Object Collection");
            collectionGameObject.transform.position = scrollView.transform.position;
            collectionGameObject.transform.rotation = scrollView.transform.rotation;

            gridObjectCollection = collectionGameObject.AddComponent<GridObjectCollection>();
            gridObjectCollection.CellWidth = cellWidth;
            gridObjectCollection.CellHeight = cellHeight;
            gridObjectCollection.SurfaceType = ObjectOrientationSurfaceType.Plane;
            gridObjectCollection.Layout = LayoutOrder.ColumnThenRow;
            gridObjectCollection.Columns = cellsPerTier;
            gridObjectCollection.Anchor = LayoutAnchor.UpperLeft;

            scrollView.AddContent(collectionGameObject);
        }

        foreach (string key in keys)
        {
            MakeItem(key);
        }
        scrollView.gameObject.SetActive(true);
        gridObjectCollection.UpdateCollection();
    }

    private void MakeItem(string item)
    {
        print(item);
        GameObject itemInstance = Instantiate(dynamicItem, gridObjectCollection.transform);
        itemInstance.name = item;
        itemInstance.GetComponentInChildren<TextMeshPro>().text = item;
        itemInstance.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
