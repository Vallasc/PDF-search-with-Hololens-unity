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
    private GameObject fav;
    [SerializeField]
    private GameObject pdfs;

    [SerializeField]
    private GameObject menuNew;
    [SerializeField]
    private GameObject menuFav;
    [SerializeField]
    private GameObject menuKeys;

    [SerializeField]
    private GameObject backplateHistory = null;
    [SerializeField]
    private GameObject backplateFavorites = null;
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
    private int maxFav = 3;
    private int maxPdfs = 5;

    private Vector3 oldSize;
    private Vector3 oldCenter;

    private bool firstPhotoTaken = false;

    void Start()
    {
        //Transform appBar = menu.transform.Find("AppBarVertical");
        //quad = menu.transform.Find("AppBarVertical").Find("BackgroundBar").Find("Quad");

        UpdateMenu();
        //appBar.localPosition = new Vector3(x + appBarOffset, appBar.localPosition.y, appBar.localPosition.z);
        Vector3 center = menu.transform.GetComponent<BoxCollider>().center;
        menu.transform.GetComponent<BoxCollider>().center = new Vector3(center.x, center.y, offsetBoxCollider);
    }

    void OnDisable()
    {
        oldSize = this.GetComponent<BoxCollider>().size;
        oldCenter = this.GetComponent<BoxCollider>().center;
    }

    void OnEnable()
    {
        this.GetComponent<BoxCollider>().size = oldSize;
        this.GetComponent<BoxCollider>().center = oldCenter;
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

    private void UpdateAppBar(float sizeX, float sizeY, float centerX, float centerY)
    {
        Transform appBar = menu.transform.Find("AppBarVertical");
        Transform back = appBar.Find("BackgroundBar");
        Transform quad = back.Find("Quad");

        //appBar.localPosition = new Vector3(sizeX + appBarOffset, appBar.localPosition.y, appBar.localPosition.z);


        if (back.localScale.y > sizeY)
        {
            sizeY = back.localScale.y;
        }

        BoxCollider boxMenu = menu.GetComponent<BoxCollider>();
        boxMenu.size = new Vector3(sizeX + appBarOffset + (quad.localScale.x / 2) + margin, sizeY + margin, boxMenu.size.z);
        boxMenu.center = new Vector3(centerX + ((appBarOffset + (quad.localScale.x / 2)) / 2), (sizeY / 2) * -1, boxMenu.center.z);
    }

    private void UpdateMenu()
    {
        //Transform scroll = menuKeys.transform.Find("Keywords").Find("ScrollingObjectCollection");
        //Transform grid = scroll.Find("Container").Find("GridObjectCollection");

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

        //BoxCollider boxMenu = menu.GetComponent<BoxCollider>();
        //boxMenu.size = new Vector3(sizeX + margin, sizeY + margin, boxMenu.size.z);
        //boxMenu.center = new Vector3(centerX, (sizeY / 2) * -1, boxMenu.center.z);

        //return sizeX;
    }


    private float[] UpdateMenuHistory()
    {
        //Transform quad = backplateHistory.transform.Find("Quad");
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
        //Debug.Log("new ret:" + xy[1]);
        return xy;
    }

    private float[] UpdateMenuPdfs()
    {
        //Transform quad = backplatePdfs.transform.Find("Quad");
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

            //BoxCollider boxMenu = menu.GetComponent<BoxCollider>();
            //boxMenu.size = new Vector3(finalOffset + 0.12f, boxMenu.size.y, boxMenu.size.z);
            //boxMenu.center = new Vector3(centerOffset + ((finalOffset + 0.12f) / 2), boxMenu.center.y, boxMenu.center.z);
            
            xy[1] = quadScaleY;
        }
        else
        {
            xy[1] = 0;
        }

        xy[0] = quad.localScale.x;
        //Debug.Log("new ret:" + xy[1]);
        return xy;
    }




    public void OnHistoryReaded()
    {
        UpdateMenu2();
    }

    public void OnFavoritesDownloaded()
    {
        OnFavoritesPressed();
    }

    public void OnKeywordsUpdated2()
    {
        OnHistoryPressed2();
    }

    public void OnHistoryPressed2()
    {
        rec.SetActive(!rec.activeSelf);

        UpdateMenu2();
    }

    public void UpdateMenu2()
    {
        float[] xyNew = UpdateBackplateHistory();
        float[] xyFav = UpdateBackplateFavorites(xyNew[1] * -1);
        float x = xyNew[0];
        float y;

        if (xyFav[0] > xyNew[0])
        {
            x = xyFav[0];
        }
        y = UpdateBackplatePdfs(x);

        if ((xyNew[1] + xyFav[1] > 0) && (xyNew[1] + offsetY + xyFav[1] > y))
        {
            if (xyNew[1] == 0)
            {
                y = xyFav[1];
            }
            else if (xyFav[1] == 0)
            {
                y = xyNew[1];
            }
            else
            {
                y = xyNew[1] + (offsetY * -1) + xyFav[1];
            }
        }


        BoxCollider boxMenu = menu.GetComponent<BoxCollider>();
        boxMenu.size = new Vector3(boxMenu.size.x, y, boxMenu.size.z);
        boxMenu.center = new Vector3(boxMenu.center.x, (y / 2) * -1, boxMenu.center.z);
    }

    public void OnFavoritesPressed()
    {
        fav.SetActive(!fav.activeSelf);

        float[] xyNew = UpdateBackplateHistory();
        float[] xyFav = UpdateBackplateFavorites(xyNew[1] * -1);
        float x = xyNew[0];
        float y;

        if (xyFav[0] > xyNew[0])
        {
            x = xyFav[0];
        }
        y = UpdateBackplatePdfs(x);

        if ((xyNew[1] + xyFav[1] > 0) && (xyNew[1] + offsetY + xyFav[1] > y))
        {
            if (xyNew[1] == 0)
            {
                y = xyFav[1];
            }
            else if (xyFav[1] == 0)
            {
                y = xyNew[1];
            }
            else
            {
                y = xyNew[1] + (offsetY * -1) + xyFav[1];
            }
        }


        BoxCollider boxMenu = menu.GetComponent<BoxCollider>();
        boxMenu.size = new Vector3(boxMenu.size.x, y, boxMenu.size.z);
        boxMenu.center = new Vector3(boxMenu.center.x, (y / 2) * -1, boxMenu.center.z);
    }


    public float[] UpdateBackplateHistory()
    {
        Transform quad = backplateHistory.transform.Find("Quad");
        //Transform quad = menuNew.transform.Find("BackplateHistory").transform.Find("Quad");

        float collectionHeight;
        if (menuNew.transform.childCount >= maxNew)
        {
            collectionHeight = maxNew * buttonWidth;
        }
        else
        {
            collectionHeight = menuNew.transform.childCount * buttonWidth;
        }
        
        
        float quadScaleY = padding + textHight + padding + collectionHeight;
        if (menuNew.transform.childCount > 0)
        {
            quadScaleY += padding;
        }
        

        quad.localScale = new Vector3(quad.localScale.x, quadScaleY, quad.localScale.z);

        backplateHistory.transform.localPosition = new Vector3(backplateHistory.transform.localPosition.x, (quadScaleY / 2f) * -1, backplateHistory.transform.localPosition.z);

        float[] toRet = new float[2];
        if (rec.activeSelf)
        {
            toRet[0] = quad.localScale.x;
            toRet[1] = quadScaleY;
        }
        else
        {
            toRet[0] = 0;
            toRet[1] = 0;
        }
        Debug.Log("new ret:" + toRet[1]);
        return toRet;
    }


    
    public float[] UpdateBackplateFavorites(float offset)
    {
        Transform quad = backplateFavorites.transform.Find("Quad");

        //float collectionHeight = menuFav.transform.childCount * buttonWidth;

        float collectionHeight;
        if (menuFav.transform.childCount >= maxFav)
        {
            collectionHeight = maxFav * buttonWidth;
        }
        else
        {
            collectionHeight = menuFav.transform.childCount * buttonWidth;
        }


        float quadScaleY = padding + textHight + padding + collectionHeight;
        if (menuFav.transform.childCount > 0)
        {
            quadScaleY += padding;
        }

        Debug.Log("quad " + quadScaleY);
        quad.localScale = new Vector3(quad.localScale.x, quadScaleY, quad.localScale.z);

        backplateFavorites.transform.localPosition = new Vector3(backplateFavorites.transform.localPosition.x, (quadScaleY / 2f) * -1, backplateFavorites.transform.localPosition.z);

        float finalOffset = 0;
        if (offset != 0)
        {
            finalOffset = offset + offsetY;
        }
        Debug.Log("final:" + finalOffset);

        fav.transform.localPosition = new Vector3(fav.transform.localPosition.x, finalOffset, fav.transform.localPosition.z);

        float[] toRet = new float[2];
        if (fav.activeSelf)
        {
            toRet[0] = quad.localScale.x;
            toRet[1] = quadScaleY;
        }
        else
        {
            toRet[0] = 0;
            toRet[1] = 0;
        }
        return toRet;
    }


    
    public float UpdateBackplatePdfs(float offset)
    {
        Transform quad = backplatePdfs.transform.Find("Quad");

        //float collectionHeight = menuPdfs.transform.childCount * buttonWidth;

        float collectionHeight;
        if (menuKeys.transform.childCount >= maxPdfs)
        {
            collectionHeight = maxPdfs * buttonWidth;
        }
        else
        {
            collectionHeight = menuKeys.transform.childCount * buttonWidth;
        }
        

        float quadScaleY = padding + textHight + padding + collectionHeight;
        if (menuKeys.transform.childCount > 0)
        {
            //pdfs.transform.Find("ScrollingObjectCollection").gameObject.SetActive(true);
            quadScaleY += padding;
        }
        //else
        //{
        //    pdfs.transform.Find("ScrollingObjectCollection").gameObject.SetActive(false);
        //}


        quad.localScale = new Vector3(quad.localScale.x, quadScaleY, quad.localScale.z);

        backplatePdfs.transform.localPosition = new Vector3(backplatePdfs.transform.localPosition.x, (quadScaleY / 2f) * -1, backplatePdfs.transform.localPosition.z);

        float finalOffset = 0;
        float centerOffset = 0;
        if (offset != 0)
        {
            finalOffset = offset + offsetX;
        }
        else
        {
            centerOffset = 0.12f + offsetX;
        }

        pdfs.transform.localPosition = new Vector3(0.12f + offsetX, pdfs.transform.localPosition.y, pdfs.transform.localPosition.z);

        BoxCollider boxMenu = menu.GetComponent<BoxCollider>();
        boxMenu.size = new Vector3(finalOffset + 0.12f, boxMenu.size.y, boxMenu.size.z);
        boxMenu.center = new Vector3(centerOffset + ((finalOffset + 0.12f) / 2), boxMenu.center.y, boxMenu.center.z);

        float toRet = 0;
        if (pdfs.activeSelf)
        {
            toRet = quadScaleY;
        }
        return toRet;
    }
}
