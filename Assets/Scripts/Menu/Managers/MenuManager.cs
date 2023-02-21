using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject menu;

    [SerializeField]
    private GameObject rec;
    [SerializeField]
    private GameObject pdfs;

    [SerializeField]
    private GameObject menuNew;
    [SerializeField]
    private GameObject menuKeys;

    [SerializeField]
    private GameObject backplateHistory = null;
    [SerializeField]
    private GameObject backplatePdfs = null;
    [SerializeField]
    private float textHight = 0.01f;
    [SerializeField]
    private float buttonWidth = 0.032f;
    [SerializeField]
    private float padding = 0.01f;
    [SerializeField]
    private float paddingFinal = 0.02f;
    [SerializeField]
    private float offsetY = -0.02f;
    [SerializeField]
    private float offsetX = 0.02f;
    [SerializeField]
    private float margin = 0.03f;
    [SerializeField]
    private float appBarOffset = 0.032f;
    [SerializeField]
    private float offsetFromTitle = 0.01f;
    [SerializeField]
    private float offsetBoxCollider = 0.01f;
    
    private int maxNew = 3;
    private int maxPdfs = 5;

    private Vector3 oldSize;
    private Vector3 oldCenter;


    void Start()
    {
        oldSize = new Vector3(0, 0, 0);
        oldCenter = new Vector3(0, 0, 0);

        UpdateMenu();
        Vector3 center = menu.transform.GetComponent<BoxCollider>().center;
        menu.transform.GetComponent<BoxCollider>().center = new Vector3(center.x, center.y, offsetBoxCollider);
    }

    public void SetOldBoxCollider(Vector3 oldCenter, Vector3 oldSize)
    {
        this.oldCenter = oldCenter;
        this.oldSize = oldSize;
    }

    void OnEnable()
    {
        if (!((oldSize == new Vector3(0, 0, 0)) && oldCenter == new Vector3(0, 0, 0)))
        {
            this.GetComponent<BoxCollider>().size = oldSize;
            this.GetComponent<BoxCollider>().center = oldCenter;
        }
    }

    public void OnHistoryPressed()
    {
        if (rec.activeSelf)
        {
            HideHistory();
        }
        else
        {
            ShowHistory();
        }
    }
    public void HideHistory()
    {
        rec.SetActive(false);

        UpdateMenu();
    }
    public void ShowHistory()
    {
        rec.SetActive(true);

        UpdateMenu();
    }
    public void OnHistoryUpdated()
    {
        UpdateMenu();
    }
    public void OnKeywordsUpdated()
    {
        UpdateMenu();
    }

    private void UpdateMenu()
    {
        float[] xyNew = UpdateMenuHistory();
        float[] xyPdfs = UpdateMenuPdfs();
        float sizeX = xyPdfs[0];
        float centerX = 0f;
        float sizeY = xyPdfs[1];

        if (xyNew[1] == 0)
        {
            centerX += xyNew[0] + offsetX;
        }
        else
        {
            sizeX += xyNew[0] + offsetX;
            if (xyNew[1] > xyPdfs[1])
            {
                sizeY = xyNew[1];
            }
        }
        centerX += (sizeX / 2);

        UpdateAppBar(sizeX, sizeY, centerX, (sizeY / 2) * -1);
    }

    private void UpdateAppBar(float sizeX, float sizeY, float centerX, float centerY)
    {
        Transform appBar = menu.transform.Find("AppBarVertical");
        Transform back = appBar.Find("BackgroundBar");
        Transform quad = back.Find("Quad");


        if (back.localScale.y > sizeY)
        {
            sizeY = back.localScale.y;
        }

        BoxCollider boxMenu = menu.GetComponent<BoxCollider>();
        boxMenu.size = new Vector3(sizeX + appBarOffset + (quad.localScale.x / 2) + margin, sizeY + margin, boxMenu.size.z);
        boxMenu.center = new Vector3(centerX + ((appBarOffset + (quad.localScale.x / 2)) / 2), (sizeY / 2) * -1, boxMenu.center.z);
    }


    private float[] UpdateMenuHistory()
    {
        Transform quad = menuNew.transform.Find("BackplateHistory").Find("Quad");
        Transform grid = menuNew.transform.Find("History").Find("ScrollingObjectCollection").Find("Container").Find("GridObjectCollection");

        float[] xy = new float[2];
        if (rec.activeSelf)
        {
            float collectionHeight = 0f;

            if (grid.childCount > 0)
            {
                if (grid.childCount >= maxNew)
                {
                    collectionHeight = maxNew * buttonWidth;
                }
                else
                {
                    collectionHeight = grid.childCount * buttonWidth;
                }
            }
            else
            {
                GameObject obj = menuNew.transform.Find("History").Find("NoObjectText").gameObject;
                if (obj.activeSelf)
                {
                    RectTransform rect = obj.GetComponent<RectTransform>();
                    collectionHeight = rect.rect.size.y * rect.localScale.y;
                }
            }

            float quadScaleY = padding + textHight + padding;
            if (collectionHeight != 0)
            {
                quadScaleY += offsetFromTitle + collectionHeight + paddingFinal;
            }


            quad.localScale = new Vector3(quad.localScale.x, quadScaleY, quad.localScale.z);

            backplateHistory.transform.localPosition = new Vector3(backplateHistory.transform.localPosition.x, (quadScaleY / 2f) * -1, backplateHistory.transform.localPosition.z);


            xy[1] = quadScaleY;
        }
        else
        {
            xy[1] = 0;
        }

        xy[0] = quad.localScale.x;
        return xy;
    }

    private float[] UpdateMenuPdfs()
    {
        Transform quad = menuKeys.transform.Find("BackplateKeywords").transform.Find("Quad");
        Transform grid = menuKeys.transform.Find("Keywords").Find("ScrollingObjectCollection").Find("Container").Find("GridObjectCollection");


        float[] xy = new float[2];
        if (pdfs.activeSelf)
        {
            float collectionHeight = 0f;

            if (grid.childCount > 0)
            {
                if (grid.childCount >= maxPdfs)
                {
                    collectionHeight = maxPdfs * buttonWidth;
                }
                else
                {
                    collectionHeight = grid.childCount * buttonWidth;
                }
            }
            else
            {
                GameObject first = menuKeys.transform.Find("Keywords").Find("TakeFirstPhotoText").gameObject;
                GameObject obj = menuKeys.transform.Find("Keywords").Find("NoObjectText").gameObject;
                if (obj.activeSelf)
                {
                    RectTransform rect = obj.GetComponent<RectTransform>();
                    collectionHeight = rect.rect.size.y * rect.localScale.y;
                }
                else
                {
                    RectTransform rect = first.GetComponent<RectTransform>();
                    collectionHeight = rect.rect.size.y * rect.localScale.y;
                }
            }

            float quadScaleY = padding + textHight + padding;
            if (collectionHeight != 0)
            {
                quadScaleY += offsetFromTitle + collectionHeight + paddingFinal;
            }

            quad.localScale = new Vector3(quad.localScale.x, quadScaleY, quad.localScale.z);

            backplatePdfs.transform.localPosition = new Vector3(backplatePdfs.transform.localPosition.x, (quadScaleY / 2f) * -1, backplatePdfs.transform.localPosition.z);
            
            xy[1] = quadScaleY;
        }
        else
        {
            xy[1] = 0;
        }

        xy[0] = quad.localScale.x;
        return xy;
    }   
}
