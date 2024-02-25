using UnityEngine;
using DG.Tweening;
using TMPro;
using System;

/// <summary>
/// Base class for changing ui text according to changing value
/// </summary>
public abstract class ChangingValueText : MonoBehaviour
{
    [SerializeField] protected float smoothness = 0.3f;
    [SerializeField] protected float valueModifier = 1;
    [SerializeField] [Tooltip("To change thousands to K and millions to M")] private bool reduceBigNumbers;

    protected float changingValue;
    private TMP_Text valueText;

    private void OnEnable()
    {
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        UnSubscribeFromEvents();
    }

    private void Awake()
    {
        valueText = GetComponent<TMP_Text>();
    }

    protected abstract void SubscribeToEvents();
    protected abstract void UnSubscribeFromEvents();
    protected abstract void InitChangingValue();
    private void InitValueText()
    {
        SetValueText();
    }

    /// <summary>
    /// Subscribe this method to the decent events. For example, when a player or level is loaded
    /// </summary>
    protected void InitValue()
    {
        InitChangingValue();
        InitValueText();
    }

    /// <summary>
    /// Subscribe this method for the events when targetValue changes to change value text
    /// </summary>
    /// <param name="targetValue"></param>
    public virtual void ChangeValueText(float targetValue, Action onComplete = null)
    {
        DOTween.To(() => changingValue, x => changingValue = x, targetValue * valueModifier, smoothness).OnUpdate(() =>
        {
            changingValue = Mathf.Round(changingValue);
            SetValueText();
        }).OnComplete(() => onComplete?.Invoke());
    }

    public virtual void ChangeValueText(float targetValue, float smoothness)
    {
        DOTween.To(() => changingValue, x => changingValue = x, targetValue * valueModifier, smoothness).OnUpdate(() =>
        {
            changingValue = Mathf.Round(changingValue);
            SetValueText();
        });
    }

    private void SetValueText()
    {
        //Debug.LogError(gameObject.name + " || "+ changingValue + " || 1");
        valueText.text = reduceBigNumbers ? ReducedBigText.GetText(changingValue) : changingValue.ToString();
    }
}
