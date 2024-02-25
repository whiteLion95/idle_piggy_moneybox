using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PopUpNumbersData", menuName = "ScriptableObjects/PopUpNumbersData")]
public class PopUpNumbersData : ScriptableObject
{
    [Header("Random spawning")]
    [SerializeField] private float xMin;
    [SerializeField] private float xMax;
    [SerializeField] private float zMin;
    [SerializeField] private float zMax;
    [SerializeField] private float yOffset;
    [Header("Numbers attributes")]
    [SerializeField] private List<NumberAttributes> numberAttributes;
    [Header("Tweening")]
    [SerializeField] private float tweenDuration;
    [SerializeField] private float deltaY;

    public NumberAttributes GetNumberAttributes(CoinSource source)
    {
        foreach (NumberAttributes a in numberAttributes)
        {
            if (a.coinSource == source)
                return a;
        }

        return null;
    }

    public float XMin { get => xMin; }
    public float XMax { get => xMax; }
    public float ZMin { get => zMin; }
    public float ZMax { get => zMax; }
    public float YOffset { get => yOffset; }
    public float TweenDuration { get => tweenDuration; }
    public float DeltaY { get => deltaY; }
}

[System.Serializable]
public class NumberAttributes
{
    public CoinSource coinSource;
    public float fontSize;
    public Color color;
}

public enum CoinSource
{
    TAP,
    RUNNER,
    HAND,
    FROG,
    ADS
}
