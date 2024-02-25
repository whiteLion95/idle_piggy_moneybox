using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerAnimationListener : MonoBehaviour
{
    private Runner _myRunner;

    public static System.Action<Coin, CoinsData> OnReadyToThrowCoin;
    public System.Action OnReadyToSpawnCoin;

    private void Awake()
    {
        _myRunner = GetComponentInParent<Runner>();
    }

    public void StartThrowingCoin()
    {
        OnReadyToThrowCoin?.Invoke(_myRunner.SpawnedCoin, _myRunner.CoinsData);
    }

    public void StartSpawningCoin()
    {
        if (!_myRunner.CoinSpawned)
            OnReadyToSpawnCoin?.Invoke();
    }
}
