using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RewardedAdsMovingRaycastObject : RewardedAdsRaycastObject
{
    [SerializeField] private Transform _spawnPlace;
    [SerializeField] private Transform _targetPlace;
    [SerializeField] private bool _lookToTarget;

    protected override void OnEnable()
    {
        base.OnEnable();

        if (!_randSpawnPos)
            transform.position = _spawnPlace.position;

        transform.DOMove(_targetPlace.position, _showingDuration).SetEase(Ease.Linear);

        if (_lookToTarget)
            transform.LookAt(_targetPlace);
    }
}
