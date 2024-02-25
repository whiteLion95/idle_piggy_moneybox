using System;
using UnityEngine;

public class TimeManager : MonoBehaviour, ISaveable
{
    [Serializable]
    public struct PersistentData
    {
        public float totalHoursPlayed;
    }

    private PersistentData _persistentData;

    private static string saveKey = "Time data";

    public static TimeManager Instance { get; private set; }
    public float TotalHoursPlayed { get => _persistentData.totalHoursPlayed + Time.timeSinceLevelLoad / 3600f; }

    private void Awake()
    {
        Instance = this;
        LoadData();
    }

    private void OnApplicationQuit()
    {
        _persistentData.totalHoursPlayed += Time.timeSinceLevelLoad / 3600f;
        SaveData();
    }

    private void OnDestroy()
    {
        SaveData();
    }

    private void OnApplicationPause(bool pause)
    {
        SaveData();
    }

    public void SaveData()
    {
        SaveManager.Save(saveKey, _persistentData);
    }

    public void LoadData()
    {
        _persistentData = SaveManager.Load<PersistentData>(saveKey);
    }
}
