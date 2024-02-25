using System.Collections;
using System.Collections.Generic;
using Deslab.Utils;
using DG.Tweening;
using Mamboo.Internal.Purchase;
using Sirenix.OdinInspector;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [SerializeField] private static CameraMover instance;
    [SerializeField] [ReadOnly] private Vector3 startPos;
    [SerializeField] private Vector3 endPos;
    [Space] [SerializeField] private Vector3 dreamNearPos;
    [Space] [SerializeField] private Vector3 dreamFarPos;

    void Start()
    {
        instance = this;
        startPos = transform.position;
        MoveToStart();
    }

    public static void MoveToStart()
    {
        AdsDisabled();
        instance.transform.DOMove(instance.startPos, 0.2f).SetEase(Ease.InQuart);
        VibrationExtention.LightVibrate();
    }

    public static void MoveToEnd()
    {
        instance.transform.DOMove(instance.endPos, 0.2f).SetEase(Ease.InQuart);
    }

    public static void AdsDisabled()
    {
        if (MaxAdsManager.instance.IsAdsRemoved)
            instance.startPos = new Vector3(0, 4.82f, -4.37f);
        else
            instance.startPos = new Vector3(0, 4.5f, -5f);
    }

    public void MoveToDream()
    {
        CheckAmountCoals();
        instance.transform.DOMove(instance.startPos, 0.2f).SetEase(Ease.InQuart);
        VibrationExtention.LightVibrate();
    }

    private void CheckAmountCoals()
    {
        instance.startPos = GoalsManager.Instance.AmountGoalsComplete < 4 ? dreamNearPos : dreamFarPos;
    }
}