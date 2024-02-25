using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using Sirenix.OdinInspector;

public class RewardedAdsObject : MonoBehaviour
{
    [SerializeField] protected float _showingDuration;
    [SerializeField] protected float _rewardMultiplier;
    [SerializeField] protected ReducedBigText _rewardText;
    [SerializeField] protected bool _randSpawnPos;
    [ShowIf("_randSpawnPos")] [SerializeField] protected List<Transform> _spawnPositions;

    protected float _curDuration;
    protected AdsController _adsController;

    public Action OnActivated;
    public Action OnEnded;

    public float ShowingDuration { get => _showingDuration; }
    public float RewardMultiplier { get => _rewardMultiplier; }
    public ReducedBigText RewardText { get => _rewardText; }

    private void Start()
    {
        _adsController = AdsController.Instance;

        OnActivated += () => gameObject.SetActive(false);
    }

    protected virtual void Update()
    {
        Show();
    }

    protected virtual void OnEnable()
    {
        _curDuration = 0f;

        CheckIfRandomSpawnPos();
    }

    private void Show()
    {
        _curDuration += Time.deltaTime;

        if (_curDuration >= _showingDuration)
        {
            OnEnded?.Invoke();
            gameObject.SetActive(false);
        }
    }

    private void CheckIfRandomSpawnPos()
    {
        if (_randSpawnPos)
        {
            int randInd = UnityEngine.Random.Range(0, _spawnPositions.Count);
            transform.position = _spawnPositions[randInd].position;
        }
    }
}
