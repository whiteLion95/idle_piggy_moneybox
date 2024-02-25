using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReducedBigText : MonoBehaviour
{
    private TMP_Text _tMP;

    private void Awake()
    {
        _tMP = GetComponent<TMP_Text>();
    }

    public void SetValue(float value, bool floatFormat = false)
    {
        if (_tMP == null) return;
        _tMP.text = GetText(value, floatFormat);
    }

    public void SetText(string text)
    {
        _tMP.text = text;
    }

    public static string GetText(float value, bool floatFormat = false)
    {
        string result;

        if (value < 1000)
        {
            if (!floatFormat)
                result = value.ToString("F0");
            else
                result = value.ToString("F1").TrimEnd('0').TrimEnd('.');
        }
        else if (value < 1000000)
        {
            if (value % 1000 == 0)
                result = (value / 1000).ToString() + "K";
            else if (value % 100 == 0)
                result = (value / 1000).ToString("F1") + "K";
            else
                result = (value / 1000).ToString("F2") + "K";
        }
        else if (value < 1000000000)
        {
            if (value % 1000000 == 0)
                result = (value / 1000000).ToString() + "M";
            else if (value % 100000 == 0)
                result = (value / 1000000).ToString("F1") + "M";
            else
                result = (value / 1000000).ToString("F2") + "M";
        }
        else
        {
            if (value % 1000000000 == 0)
                result = (value / 1000000000).ToString() + "B";
            else if (value % 1000000 == 0)
                result = (value / 1000000000).ToString("F1") + "B";
            else
                result = (value / 1000000000).ToString("F2") + "B";
        }

        return result;
    }
}
