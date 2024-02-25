using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class RunnersData
{
    public int level;
    public float curExp;
    public List<int> oldPathsIDs = new List<int>();
    public List<CustomVector3> oldPositions;
    public List<CustomVector3> oldRotations;
    public int[] oldTargetPointIDs;
    public bool[] oldCoinContributed;
    public bool supermans;
    public int superMenCount;
}

[System.Serializable]
public struct CustomVector3
{
    public float x;
    public float y;
    public float z;
}

[RequireComponent(typeof(ExpController))]
public class RunnersManager : MonoBehaviour
{
    [SerializeField] private int maxRunners = 30;
    [SerializeField][Range(0f, 10f)] private float pathsLength = 6f;
    [SerializeField] private Runner runnerPrefab;
    [SerializeField] private Transform runnersParent;
    [SerializeField] private RunnerPath runnerPathPrefab;
    [SerializeField] private Transform runnerPathsParent;
    [SerializeField] private float baseCoinValue = 5;
    [SerializeField] private float coinValueMultiplier = 1.5f;
    [SerializeField] private float baseExpReward = 1f;
    [SerializeField] private float baseSpeedMultiplier = 1f;
    [SerializeField] private float baseSpeed = 1f;
    [SerializeField] private GameObject autoLvlProgBar;
    [SerializeField] private List<StatValue> statsValues;

    private List<RunnerPath> _paths = new List<RunnerPath>();
    private List<RunnerPath> _availablePaths = new List<RunnerPath>();
    private List<Runner> _spawnedRunners = new List<Runner>();
    private RunnersData _persistentData;
    private float _coinValue;
    private float _expReward;
    private ExpController _expController;
    private RunnersSkillsManager _skillsManager;

    private const string SAVING_KEY = "Runners data";

    public Action OnRunnersSpawned;
    public Action<RunnersSkill> OnSkillUpgraded;
    public Action<Runner> OnNewRunnerSpawned;
    public Action<Runner> OnRunnerDespawned;
    public Action<Stat> OnStatUpdated;
    public static Action OnStatsInit;
    public Action OnLevelUp;

    public static RunnersManager Instance { get; private set; }
    public float CoinValue { get => _coinValue; }
    public float ExpReward { get => _expReward; }
    public int RunnersAmount { get => _spawnedRunners.Count; }
    public int NonSupermanRunnersAmount { get => _spawnedRunners.Count - _persistentData.superMenCount; }
    public ExpController ExpController { get => _expController; }
    public float BaseSpeed => baseSpeed;

    private void Awake()
    {
        LoadData();

        Instance = this;
        _expController = GetComponent<ExpController>();

        _expController.Init(_persistentData.level, _persistentData.curExp);
    }

    private void Start()
    {
        _skillsManager = RunnersSkillsManager.Instance;
        _expController.Init(_persistentData.level, _persistentData.curExp);

        GeneratePaths();
        SpawnOldRunners();
        InitStats();

        if (_spawnedRunners.Count > 0)
            ActivateAutoUpProgBar();

        _skillsManager = RunnersSkillsManager.Instance;

        _skillsManager.OnSkillUpgrade += HandleSkillUpgraded;
        _skillsManager.OnSkillReset += HandleSkillReset;
        _skillsManager.OnSkillStatsRefresh += UpdateStats;
        OnNewRunnerSpawned += CheckHelpfulCarlSkill;
        OnRunnerDespawned += CheckHelpfulCarlSkill;
        _expController.OnLevelUp += HandleLevelUp;
    }

    private void OnEnable()
    {
        CoinsDespawnPlace.OnCoinTriggered += HandleCoinDespawned;
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

    private void OnDestroy()
    {
        SaveData();
        CoinsDespawnPlace.OnCoinTriggered -= HandleCoinDespawned;
    }

    private void OnApplicationPause(bool pause)
    {
        SaveData();
    }

    private void GeneratePaths()
    {
        float rotationY = 0f;
        int totalRunners = maxRunners + _persistentData.superMenCount;
        float deltaY = 180 / totalRunners;

        for (int i = 0; i < totalRunners; i++)
        {
            SpawnPath(rotationY);
            rotationY += deltaY;
        }
    }

    private void SpawnPath(float rotY)
    {
        Quaternion rot = Quaternion.Euler(0f, rotY, 0f);
        RunnerPath path = Instantiate(runnerPathPrefab, Vector3.zero, rot, runnerPathsParent);
        path.Point1.localPosition = new Vector3(0f, 0f, -pathsLength / 2);
        path.Point2.localPosition = new Vector3(0f, 0f, pathsLength / 2);
        _paths.Add(path);
        _availablePaths.Add(path);
    }

    private float GetRandRotationY()
    {
        int randPathId = UnityEngine.Random.Range(0, _paths.Count - 1);
        float rotY1 = _paths[randPathId].transform.eulerAngles.y;
        float rotY2 = _paths[randPathId + 1].transform.eulerAngles.y;
        float offset = (rotY2 - rotY1) * 0.3f;
        float minRot = rotY1 + offset;
        float maxRot = rotY2 - offset;
        float randRotY = UnityEngine.Random.Range(minRot, maxRot);
        return randRotY;
    }

    public void SpawnNewRunner()
    {
        if ((_spawnedRunners.Count + 1) > maxRunners)
        {
            SpawnPath(GetRandRotationY());
        }

        RunnerPath randPath = GetRandomPath();
        Runner spawnedRunner = null;

        if (randPath != null)
            spawnedRunner = SpawnRunner(randPath);

        if (_spawnedRunners.Count == 1)
            ActivateAutoUpProgBar();

        if (spawnedRunner)
            OnNewRunnerSpawned?.Invoke(spawnedRunner);
    }

    public void DespawnRunner()
    {
        if (_spawnedRunners != null && NonSupermanRunnersAmount > 0)
        {
            foreach (var runner in _spawnedRunners)
            {
                if (!runner.IsSuperman)
                {
                    Destroy(runner.gameObject);
                    _paths.RemoveAt(_spawnedRunners.Count - 1);
                    _persistentData.oldPathsIDs.RemoveAt(_spawnedRunners.Count - 1);
                    _spawnedRunners.Remove(runner);

                    OnRunnerDespawned?.Invoke(runner);
                    break;
                }
            }
        }
    }

    private void SpawnOldRunners()
    {
        List<int> oldPathsIDs = _persistentData.oldPathsIDs;

        if (oldPathsIDs != null && oldPathsIDs.Count != 0)
        {
            for (int i = 0; i < oldPathsIDs.Count; i++)
            {
                RunnerPath path = _paths[oldPathsIDs[i]];
                SpawnRunner(path, _persistentData.oldPositions[i], _persistentData.oldRotations[i],
                    _persistentData.oldTargetPointIDs[i], _persistentData.oldCoinContributed[i]);
                _availablePaths.Remove(path);
            }
        }

        if (_persistentData.supermans)
        {
            int skinIndex = 0;

            for (int i = _spawnedRunners.Count - 1; i > (_spawnedRunners.Count - _persistentData.superMenCount) - 1; i--)
            {
                skinIndex++;
                _spawnedRunners[i].SetSupermanMode(skinIndex);

                if (skinIndex == 2)
                    skinIndex = 0;
            }
        }
    }

    private void RefreshRunners()
    {
        if (_spawnedRunners != null && _spawnedRunners.Count > 0)
        {
            SaveData();

            foreach (var runner in _spawnedRunners)
            {
                Destroy(runner.gameObject);
            }

            _spawnedRunners.Clear();
            _paths.Clear();
        }
    }

    private Runner SpawnRunner(RunnerPath path)
    {
        Transform point = path.GetRandomPoint();
        Vector3 spawnPos = point.position;
        Quaternion spawnRot = (point.Equals(path.Point1))
            ? path.transform.rotation
            : Quaternion.Euler(0, path.transform.rotation.eulerAngles.y + 180, 0);
        Runner spawnedRunner = Instantiate(runnerPrefab, spawnPos, spawnRot, runnersParent);
        spawnedRunner.SetPath(path, point);
        _spawnedRunners.Add(spawnedRunner);

        return spawnedRunner;
    }

    private void SpawnRunner(RunnerPath path, CustomVector3 oldPos, CustomVector3 oldRot, int oldTargetPointID,
        bool oldCoinContributed)
    {
        Vector3 spawnPos = new Vector3(oldPos.x, oldPos.y, oldPos.z);
        Vector3 spawnRot = new Vector3(oldRot.x, oldRot.y, oldRot.z);
        Runner spawnedRunner = Instantiate(runnerPrefab, spawnPos, Quaternion.Euler(spawnRot), runnersParent);
        spawnedRunner.SetPath(path, oldTargetPointID);
        spawnedRunner.CoinContributed = oldCoinContributed;
        _spawnedRunners.Add(spawnedRunner);
    }

    private RunnerPath GetRandomPath()
    {
        if (_availablePaths.Count != 0)
        {
            int randIndex = UnityEngine.Random.Range(0, _availablePaths.Count);
            RunnerPath randPath = _availablePaths[randIndex];
            _persistentData.oldPathsIDs.Add(_paths.IndexOf(randPath));
            _availablePaths.RemoveAt(randIndex);
            return randPath;
        }

        Debug.LogWarning("No available paths!");
        return null;
    }

    private void SaveRunnersStatus()
    {
        if (_spawnedRunners != null && _spawnedRunners.Count != 0)
        {
            _persistentData.oldPositions = new List<CustomVector3>();
            _persistentData.oldRotations = new List<CustomVector3>();
            _persistentData.oldTargetPointIDs = new int[_spawnedRunners.Count];
            _persistentData.oldCoinContributed = new bool[_spawnedRunners.Count];

            for (int i = 0; i < _spawnedRunners.Count; i++)
            {
                Vector3 pos = _spawnedRunners[i].transform.position;
                Vector3 rot = _spawnedRunners[i].transform.rotation.eulerAngles;

                _persistentData.oldPositions.Add(new CustomVector3()
                {
                    x = pos.x,
                    y = pos.y,
                    z = pos.z
                });
                _persistentData.oldRotations.Add(new CustomVector3()
                {
                    x = rot.x,
                    y = rot.y,
                    z = rot.z
                });
                _persistentData.oldTargetPointIDs[i] = _spawnedRunners[i].GetTargetPointID();
                _persistentData.oldCoinContributed[i] = _spawnedRunners[i].CoinContributed;
            }
        }
    }

    private int _amountSupermans;
    public void SpawnNewSupermans()
    {
        _persistentData.superMenCount += 2;

        for (int i = 0; i < 2; i++)
        {
            SpawnNewRunner();
            SetSupermanRunner();
        }

        _amountSupermans = 0;
    }

    private void SetSupermanRunner()
    {
        _persistentData.supermans = true;
        _amountSupermans++;
        _spawnedRunners[_spawnedRunners.Count - 1].SetSupermanMode(_amountSupermans);
    }

    private void ActivateAutoUpProgBar()
    {
        autoLvlProgBar.gameObject.SetActive(true);
    }

    public void HandleCoinDespawned(Coin coin)
    {
        if (coin.Source == CoinSource.RUNNER)
        {
            _expController.GetExp(_expReward);
        }
    }

    public int GetProfitPerMinute()
    {
        int runnersCount = _spawnedRunners.Count;

        if (runnersCount != 0)
        {
            float timeToGainMoney = pathsLength / Runner.CurrentSpeed;
            float moneyPerSecond = CoinValue / timeToGainMoney;
            float moneyPerMinute = moneyPerSecond * 60;
            float moneyPerMinuteAll = moneyPerMinute * runnersCount;
            return Mathf.RoundToInt(moneyPerMinuteAll);
        }

        return 0;
    }

    private void HandleSkillUpgraded(Skill skill)
    {
        foreach (var stat in skill.Stats)
        {
            StatValue statValue = statsValues.First((s) => s.Name == stat);
            statValue.SetValue(_skillsManager.GetStatValue(stat));
        }

        UpdateStats(skill);

        if ((skill as RunnersSkill).Name == RunnersSkillName.RunnerSacrifice)
        {
            DespawnRunner();
        }

        OnSkillUpgraded?.Invoke(skill as RunnersSkill);
    }

    private void HandleSkillReset(Skill skill)
    {
        HandleSkillUpgraded(skill);
    }

    public float GetCritMult()
    {
        float result = UnityEngine.Random.Range(0f, 1f);

        if (result != 0f && result <= GetStatValue(Stat.CRIT_CHANCE))
        {
            float superResult = UnityEngine.Random.Range(0f, 1f);

            if (superResult != 0 && superResult <= GetStatValue(Stat.SUPER_CRIT_CHANCE))
                return GetStatValue(Stat.SUPER_CRIT_MULT);

            return GetStatValue(Stat.CRIT_MULT);
        }

        return 1f;
    }

    private void CheckHelpfulCarlSkill(Runner runner)
    {
        RunnersSkill helpfulCarlSkill = _skillsManager.GetSkill(RunnersSkillName.HelpfulCarl);
        if (helpfulCarlSkill.CurLevel > 0)
        {
            UpdateStats(helpfulCarlSkill);
        }
    }

    private void HandleLevelUp(int level)
    {
        _coinValue = (baseCoinValue + baseCoinValue * GetStatValue(Stat.POWER_BONUS)) * Mathf.Pow(coinValueMultiplier, _expController.CurrentLevel);
        OnLevelUp?.Invoke();
    }

    #region Runners stats
    private void InitStats()
    {
        for (int i = 0; i < statsValues.Count; i++)
        {
            StatValue stat = statsValues[i];
            stat.SetValue(_skillsManager.GetStatValue(stat.Name));
        }

        _coinValue = (baseCoinValue + baseCoinValue * GetStatValue(Stat.POWER_BONUS)) * Mathf.Pow(coinValueMultiplier, _expController.CurrentLevel);
        _expReward = baseExpReward * GetStatValue(Stat.EXP_GAIN);

        OnStatsInit?.Invoke();
    }

    public float GetStatValue(Stat name)
    {
        return statsValues.First((s) => s.Name == name).Value;
    }

    private void UpdateStats(Skill skill)
    {
        foreach (var stat in skill.Stats)
        {
            StatValue statValue = statsValues.First((s) => s.Name == stat);
            statValue.SetValue(_skillsManager.GetStatValue(stat));

            switch (stat)
            {
                case Stat.POWER_BONUS:
                    _coinValue = (baseCoinValue + baseCoinValue * GetStatValue(Stat.POWER_BONUS)) * Mathf.Pow(coinValueMultiplier, _expController.CurrentLevel);
                    break;
                case Stat.EXP_GAIN:
                    _expReward = baseExpReward * GetStatValue(Stat.EXP_GAIN);
                    break;
                case Stat.MANA_REGEN:
                    break;
            }

            OnStatUpdated?.Invoke(stat);
        }
    }
    #endregion

    #region PersistentData
    public void SaveData()
    {
        if (_persistentData == null) return;
        SaveRunnersStatus();

        _persistentData.level = _expController.CurrentLevel;
        _persistentData.curExp = _expController.CurExp;

        SaveManager.Save(SAVING_KEY, _persistentData);
    }

    public void LoadData()
    {
        _persistentData = new RunnersData();
        _persistentData = SaveManager.Load<RunnersData>(SAVING_KEY);
        if (_persistentData == null) _persistentData = new RunnersData();
    }
    #endregion
}