using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using EZ_Pooling;

public class PopUpNumber : MonoBehaviour
{
    [SerializeField] private PopUpNumbersData data;

    private float _originalFade;

    public TMP_Text Text { get; private set; }
    public ReducedBigText BigText { get; private set; }
    public CoinSource Source { get; set; }

    private void Awake()
    {
        Text = GetComponentInChildren<TMP_Text>();
        BigText = GetComponentInChildren<ReducedBigText>();
        _originalFade = Text.color.a;
    }

    private void OnEnable()
    {
        ResetFade();
        //StartCoroutine(Tween());
    }

    //private IEnumerator Tween()
    //{
    //    ResetFade();

    //    if (Source == CoinSource.RUNNER || Source == CoinSource.TAP)
    //    {
    //        transform.DOMoveY(data.DeltaY, data.TweenDuration).SetRelative(true);
    //        Text.DOFade(0f, data.TweenDuration);
    //        yield return new WaitForSeconds(data.TweenDuration);
    //    }
    //    else
    //    {
    //        transform.DOMoveY(data.DeltaY, data.TweenDuration).SetRelative(true).SetDelay(1f);
    //        Text.DOFade(0f, data.TweenDuration).SetDelay(1f);
    //        yield return new WaitForSeconds(data.TweenDuration + 1f);
    //    }
        
    //    EZ_PoolManager.Despawn(transform);
    //}

    private void ResetFade()
    {
        Color temp = Text.color;
        temp.a = _originalFade;
        Text.color = temp;
    }
}
