using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RunnerData", menuName = "ScriptableObjects/RunnerData")]
public class RunnerData : ScriptableObject
{
    [field: SerializeField] public float JumpPower = 0.5f;
    [field: SerializeField] public float JumpDuration = 0.8f;
}
