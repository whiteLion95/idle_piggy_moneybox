using System.Collections;
using System.Collections.Generic;
using Deslab.Scripts.Deslytics.Ads;
using UnityEngine;
using Lean.Touch;

public class RewardedAdsRaycastObject : RewardedAdsObject
{
    private Camera _mainCam;
    private int _layerMask;

    private void Awake()
    {
        _mainCam = Camera.main;
        _layerMask = 1 << gameObject.layer;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        LeanTouch.OnFingerDown += HandlerOnFingerDown;
    }

    private void OnDisable()
    {
        LeanTouch.OnFingerDown -= HandlerOnFingerDown;
    }

    private void HandlerOnFingerDown(LeanFinger finger)
    {
        Ray ray = _mainCam.ScreenPointToRay(finger.ScreenPosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, _layerMask, QueryTriggerInteraction.Collide))
        {
            if (hitInfo.collider.gameObject.Equals(gameObject) && !AdsManager._interstitialTimerOver)
            {
                OnActivated?.Invoke();
            }
        }
    }
}
