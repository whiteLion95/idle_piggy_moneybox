using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnlocksData", menuName = "ScriptableObjects/UnlocksData")]

public class UnlocksData : ScriptableObject
{
    [field: SerializeField] public uint FrogUnlockPrice { get; private set; } 
    [field: SerializeField] public uint HandUnlockPrice { get; private set; } 
}
