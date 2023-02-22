using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public void SetPDF(string pdf)
    {
        Debug.Log("SELECT PDF " + pdf);
    }

    public void OpenSelectedPDF()
    {
        Debug.Log("OPEN SELECTED PDF");
    }

    //// KeywordsManager.cs
    //public void test()
    //{
    //    ind = ind + 1;
    //    if (ind == 1)
    //    {
    //        SetFirstPhotoTaken();
    //        test1();
    //    }
    //    else if (ind == 2)
    //    {
    //        test2();
    //    }
    //    else if (ind == 3)
    //    {
    //        test3();
    //    }
    //    else if (ind == 4)
    //    {
    //        test4();
    //    }
    //    else if (ind == 5)
    //    {
    //        test1();
    //    }
    //    else
    //    {
    //        testVoid();
    //    }
    //}

    //public void test1()
    //{
    //    string[] test = { "ikea" };
    //    UpdateKeywordsCollection(test);
    //}
    //public void test2()
    //{
    //    string[] test = { "daje roma" };
    //    UpdateKeywordsCollection(test);
    //}
    //public void test3()
    //{
    //    string[] test = { "progettoooo" };
    //    UpdateKeywordsCollection(test);
    //}
    //public void test4()
    //{
    //    string[] test = { "cane cane cane", "ALEXA" };
    //    UpdateKeywordsCollection(test);
    //}
    //public void test5()
    //{
    //    string[] test = { "pranzo", "cena" };
    //    UpdateKeywordsCollection(test);
    //}
    //public void testVoid()
    //{
    //    Debug.Log(ind);
    //    string[] test = { };
    //    UpdateKeywordsCollection(test);
    //}
}
