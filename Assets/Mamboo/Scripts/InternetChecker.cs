using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InternetChecker : MonoBehaviour
{
    public bool IsInternetAvilable { get; private set; } = false;

    private Coroutine internetCheckCoroutine = null;
    private float coldownTime = 5f;
    private float counter = 0f;

    private IEnumerator CheckInternetConnection()
    {
        WWW www = new WWW("http://google.com");
        yield return www;
        
        if (www.error != null)
        {
            IsInternetAvilable = false;
        } 
        else
        {
            IsInternetAvilable = true;
        }
        
        internetCheckCoroutine = null;
    }

    private bool isRequareInternetCheckRequest => internetCheckCoroutine == null && counter >= coldownTime;
    private void FixedUpdate()
    {
        counter += Time.deltaTime;

        if (isRequareInternetCheckRequest)
        {
            internetCheckCoroutine=StartCoroutine(CheckInternetConnection());
            counter = 0;
        }
    }
}