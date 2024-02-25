using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds spawn places for units like hand and frog
/// </summary>
public class StaticCoinThrowersManager : MonoBehaviour
{
    [SerializeField] private Hand handPrefab;
    [SerializeField] private Frog frogPrefab;
    [SerializeField] private Transform handSpawnPlace;
    [SerializeField] private Transform frogSpawnPlace;

    public static StaticCoinThrowersManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CheckPurchase();
    }

    public void SpawnHand()
    {
        Instantiate(handPrefab, handSpawnPlace.position, Quaternion.identity, handSpawnPlace.transform);
    }

    public void SpawnFrog()
    {
        Instantiate(frogPrefab, frogSpawnPlace.position, frogSpawnPlace.rotation, frogSpawnPlace.transform);
    }

    private void CheckPurchase()
    {
        if (UpgradesManager.Instance.HandPurchased)
        {
            SpawnHand();
        }

        if (UpgradesManager.Instance.FrogPurchased)
        {
            SpawnFrog();
        }
    }
}
