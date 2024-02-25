using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CoinsData", menuName = "ScriptableObjects/CoinsData")]
public class CoinsData : ScriptableObject
{
    [field: SerializeField] public float FlySpeed { get; private set; }
    [field: SerializeField] public float FlyPower { get; private set; }
}
