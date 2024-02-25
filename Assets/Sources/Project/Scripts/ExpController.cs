using System;
using UnityEngine;

public class ExpController : MonoBehaviour
{
    [SerializeField] private int baseExpToLevelUp;
    [SerializeField] private float expToLevelUpMultiplier = 1.5f;

    public Action<int> OnLevelUp;
    public Action<float> OnExpGained;

    private float _curExp = 0;
    private int _curLevel;

    public int CurrentLevel { get => _curLevel; }
    public float CurExp { get => _curExp; }

    public void Init(int curLevel, float curExp)
    {
        _curLevel = curLevel;
        _curExp = curExp;
    }

    public void GetExp(float amount)
    {
        _curExp += amount;
        OnExpGained?.Invoke(_curExp);
        float curExpToLevelUp = GetCurExpToLevelUp();

        while (_curExp >= curExpToLevelUp)
        {
            _curExp -= curExpToLevelUp;
            LevelUp();
        }
    }

    public void LevelUp()
    {
        _curLevel++;
        OnLevelUp?.Invoke(_curLevel);
    }

    public void SetLevel(int level)
    {
        _curLevel = level;
        _curExp = 0;
    }

    public float GetCurExpToLevelUp()
    {
        return baseExpToLevelUp * Mathf.Pow(expToLevelUpMultiplier, CurrentLevel);
    }
}