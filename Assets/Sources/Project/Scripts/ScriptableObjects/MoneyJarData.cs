using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "MoneyJarData", menuName = "ScriptableObjects/MoneyJarData")]
public class MoneyJarData : ScriptableObject
{
    [Header("Shaking")]
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeStrength = 30f;
    [SerializeField] private int shakeVibrato = 10;
    [SerializeField] private float shakeRandomness = 20f;
    [SerializeField] private Ease easeType;
    [SerializeField] [Tooltip("How fast to return to the original rotation")] private float returnTime = 0.3f;
    [SerializeField] private Color[] jarBodyColor;

    public float ShakeDuration { get => shakeDuration; }
    public float ShakeStrength { get => shakeStrength; }
    public int ShakeVibrato { get => shakeVibrato; }
    public float ShakeRandomness { get => shakeRandomness; }
    public Ease EaseType { get => easeType; }
    public float ReturnTime { get => returnTime; }
    
    public Color[] JarBodyColor { get => jarBodyColor; }
}