using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.AI;

public class Runner : MonoBehaviour
{
    [SerializeField] private RunnerData data;
    [field: SerializeField] public Transform coinSpawnPoint;
    [field: SerializeField] public CoinsData CoinsData { get; private set; }

    public static Action OnRunnerSpawned;
    public static Action OnRunnerLevelUp;
    public static Action<Runner> OnRunnerJumped;
    public Action<int> OnSpeedSet;
    public static Action<float> OnSpeedSetStatic;

    private NavMeshAgent _agent;
    private RunnerPath _myPath;
    private Transform _currentPoint;
    private Transform _targetPoint;
    private Animator _anim;
    private bool _jumped;
    private bool _coinSpawned;
    private RunnerAnimationListener _animListener;
    private bool _inJumpArea;
    private bool _isSuperman;
    private UpgradesManager _upManager;
    private RunnersManager _runnersManager;
    private ExpController _expController;
    private CoinsManager _coinsManager;

    public Coin SpawnedCoin { get; private set; }
    public bool CoinContributed { get; set; }
    public bool CoinSpawned { get { return _coinSpawned; } }
    public static float CurrentSpeed { get; private set; }
    public bool IsSuperman => _isSuperman;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _anim = GetComponentInChildren<Animator>();
        _animListener = GetComponentInChildren<RunnerAnimationListener>();

        RunnersManager.OnStatsInit += HandleStatsInit;
    }

    // Start is called before the first frame update
    private void Start()
    {
        _animListener.OnReadyToSpawnCoin += SpawnCoin;
        PrepareToSpawnCoin();
        GoToOtherPoint();
        OnRunnerSpawned?.Invoke();
        _upManager = UpgradesManager.Instance;

        if (!_runnersManager)
            _runnersManager = RunnersManager.Instance;
        if (!_expController)
            _expController = _runnersManager.ExpController;
        _coinsManager = CoinsManager.Instance;

        _runnersManager.OnStatUpdated += HandleStatUpdated;
        _runnersManager.OnLevelUp += HandleLevelUp;
        _runnersManager.OnSkillUpgraded += HandleSkillUpgraded;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!_agent.pathPending && _agent.remainingDistance < 0.1f)
            GoToOtherPoint();
    }

    private void OnEnable()
    {
        CoinsDespawnPlace.OnCoinTriggered += HandlerCoinDespawn;
    }

    private void OnDisable()
    {
        CoinsDespawnPlace.OnCoinTriggered -= HandlerCoinDespawn;
        RunnersManager.OnStatsInit -= HandleStatsInit;

        _runnersManager.OnStatUpdated -= HandleStatUpdated;
        _runnersManager.OnLevelUp -= HandleLevelUp;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("JumpArea"))
        {
            _inJumpArea = true;

            if (!_jumped)
            {
                Jump();
            }
        }

        if (other.CompareTag("RunnerCoinSpawnArea"))
        {
            CoinContributed = false;
            PrepareToSpawnCoin();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("JumpArea"))
        {
            if (_inJumpArea) _inJumpArea = false;
        }
    }

    private void HandlerCoinDespawn(Coin coin)
    {
        if (coin.Equals(SpawnedCoin))
        {
            CoinContributed = true;
            _coinSpawned = false;
        }
    }

    private void PrepareToSpawnCoin()
    {
        if (!CoinContributed)
        {
            _anim.SetTrigger(SPAWN_COIN_TRIGGER);
        }
    }

    private void SpawnCoin()
    {
        float runnerCoinValue = _runnersManager.CoinValue * _runnersManager.GetCritMult();

        if (_isSuperman)
            runnerCoinValue *= 2;

        SpawnedCoin = _coinsManager.SpawnCoin(coinSpawnPoint, runnerCoinValue, CoinSource.RUNNER);
        SpawnedCoin.transform.parent = coinSpawnPoint;
        _coinSpawned = true;

        if (_inJumpArea)
        {
            Jump();
        }
    }

    private void HandleStatUpdated(Stat stat)
    {
        if (stat == Stat.POWER_BONUS)
        {
            SpawnedCoin.Value = _runnersManager.CoinValue;
        }
    }

    public void SetSpeed(float multiplier, int level)
    {
        _agent.speed = _runnersManager.BaseSpeed * multiplier;

        if (CurrentSpeed != _agent.speed)
            CurrentSpeed = _agent.speed;

        OnSpeedSet?.Invoke(level);
        OnSpeedSetStatic?.Invoke(CurrentSpeed);
    }

    private void HandleLevelUp()
    {
        if (SpawnedCoin)
            SpawnedCoin.Value = _runnersManager.CoinValue;

        OnRunnerLevelUp?.Invoke();
    }

    private void HandleSkillUpgraded(RunnersSkill skill)
    {
        if (skill.Name == RunnersSkillName.WingFoot)
        {
            SetSpeed(_runnersManager.GetStatValue(Stat.SPEED), _expController.CurrentLevel);
        }
    }

    private void HandleStatsInit()
    {
        _runnersManager = RunnersManager.Instance;
        _expController = _runnersManager.ExpController;

        SetSpeed(_runnersManager.GetStatValue(Stat.SPEED), _expController.CurrentLevel);

        if (SpawnedCoin)
            SpawnedCoin.Value = _runnersManager.CoinValue;
    }

    public void SetSupermanMode(int supermanId)
    {
        _isSuperman = true;
        transform.GetComponent<RunnerSkinManager>().SetSpecialSkin(100 + supermanId);
    }

    #region Movement
    public void SetPath(RunnerPath path, Transform currentPoint)
    {
        _myPath = path;
        _currentPoint = currentPoint;
    }

    public void SetPath(RunnerPath path, int targetPointID)
    {
        _myPath = path;
        _currentPoint = (targetPointID == 1) ? _myPath.Point2 : _myPath.Point1;
    }

    private void GoToOtherPoint()
    {
        _targetPoint = (_myPath.Point1.Equals(_currentPoint)) ? _myPath.Point2 : _myPath.Point1;
        _agent.destination = _targetPoint.position;
        _currentPoint = _targetPoint;
    }

    public int GetTargetPointID()
    {
        int ID = (_targetPoint.Equals(_myPath.Point1)) ? 1 : 2;
        return ID;
    }
    #endregion

    #region Animation
    private const string JUMP_TRIGGER = "Jump";
    private const string LANDED_TRIGGER = "Landed";
    private const string SPAWN_COIN_TRIGGER = "Spawn_Coin";
    private void Jump()
    {
        _jumped = true;
        _anim.ResetTrigger(SPAWN_COIN_TRIGGER);
        _anim.ResetTrigger(LANDED_TRIGGER);
        _anim.SetTrigger(JUMP_TRIGGER);
        transform.DOJump(transform.forward, data.JumpPower, 1, data.JumpDuration).SetRelative(true).onComplete +=
            () =>
            {
                _jumped = false;
                _anim.ResetTrigger(JUMP_TRIGGER);
                _anim.ResetTrigger(SPAWN_COIN_TRIGGER);
                _anim.SetTrigger(LANDED_TRIGGER);
            };

        OnRunnerJumped?.Invoke(this);
    }
    #endregion
}
