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
    private GameObject menuPdfs;

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
    private float offsetY = -0.02f;
    [SerializeField]
    private float offsetX = 0.02f;

    private int maxNew = 3;
    private int maxFav = 3;
    private int maxPdfs = 5;

    public void OnFavoritesDownloaded()
    {
        OnHistoryPressed();
    }

    public void OnHistoryPressed()
    {
        rec.SetActive(!rec.activeSelf);

        
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
            y = xyNew[1] + (offsetY * -1) + xyFav[1];
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
            y = xyNew[1] + (offsetY * -1) + xyFav[1];
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
        if (menuPdfs.transform.childCount >= maxPdfs)
        {
            collectionHeight = maxPdfs * buttonWidth;
        }
        else
        {
            collectionHeight = menuPdfs.transform.childCount * buttonWidth;
        }



        float quadScaleY = padding + textHight + padding + collectionHeight;
        if (menuPdfs.transform.childCount > 0)
        {
            quadScaleY += padding;
        }

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
