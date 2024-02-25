using Deslab.Utils;
using DG.Tweening;
using EZ_Pooling;
using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinsManager : MonoBehaviour
{
    [SerializeField] private Coin coinPrefab;
    [SerializeField] private float moneyBillsRainDelay = 0.8f;

    public System.Action<Coin> OnCoinGained;
    public static System.Action<Coin> OnLastAdsCoinGained;
    public System.Action<Coin> OnCoinInJar;

    public static CoinsManager Instance { get; private set; }
    public bool CanThrowCoinsFromTap { get; set; } = true;

    private MoneyJar _currentMoneyJar;
    private bool _fingerIsDown;
    private UpgradesManager _upManager;
    private FrogSpellsController _frogSpellsController;
    private Frog _frog;

    public float TapCoinValueMultiplier { get; set; } = 1f;

    private void Awake()
    {
        Instance = this;

        MoneyJar.OnLoaded += SetCurrentMoneyJar;
        StaticCoinThrower.OnSpawned += HandleStaticCoinThrowerSpawned;
    }

    private void Start()
    {
        _upManager = UpgradesManager.Instance;
        _frogSpellsController = FrogSpellsController.Instance;
        _droppedCoins = new List<Coin>();
    }

    private void OnEnable()
    {
        LeanTouch.OnFingerUp += OnFingerUpHandler;
        LeanTouch.OnFingerDown += OnFingerDownHandler;
        CoinsDespawnPlace.OnCoinTriggered += DespawnCoin;
        RunnerAnimationListener.OnReadyToThrowCoin += ThrowCoin;
        AdsController.OnRewardedAdWatched += HandleRewardedAdsWatched;
    }

    private void OnDisable()
    {
        LeanTouch.OnFingerUp -= OnFingerUpHandler;
        LeanTouch.OnFingerDown -= OnFingerDownHandler;
        CoinsDespawnPlace.OnCoinTriggered -= DespawnCoin;
        MoneyJar.OnLoaded -= SetCurrentMoneyJar;
        RunnerAnimationListener.OnReadyToThrowCoin -= ThrowCoin;
        AdsController.OnRewardedAdWatched -= HandleRewardedAdsWatched;
        StaticCoinThrower.OnSpawned -= HandleStaticCoinThrowerSpawned;
    }

    private void OnFingerDownHandler(LeanFinger finger)
    {
        if (!finger.IsOverGui)
        {
            _fingerIsDown = true;
        }
    }

    private void OnFingerUpHandler(LeanFinger finger)
    {
        if (!finger.IsOverGui && _fingerIsDown)
        {
            ThrowCoinFromTap();
        }

        _fingerIsDown = false;
    }

    private void SetCurrentMoneyJar(MoneyJar moneyJar)
    {
        _currentMoneyJar = moneyJar;
    }

    [SerializeField]
    [Tooltip("Extra force applied to a coin spawned from a tap")]
    private float extraForce;

    private void ThrowCoinFromTap()
    {
        ulong tapCoinValue = (ulong)(_upManager.GetValue(UpgradeType.MONEY_JAR_COIN_VALUE) * TapCoinValueMultiplier);

        if (CanThrowCoinsFromTap)
        {
            Coin spawnedCoin = SpawnCoin(_currentMoneyJar.CoinsSpawnPlace, tapCoinValue, CoinSource.TAP);

            Rigidbody rB = spawnedCoin.GetComponent<Rigidbody>();
            rB.isKinematic = false;
            rB.AddForce(Vector3.down * extraForce, ForceMode.Impulse);
            VibrationExtention.WaveVibrate();
        }
    }

    public Coin SpawnCoin(Transform spawnPoint, float coinValue, CoinSource coinSource, Coin coinPrefab = null, bool kinematic = true)
    {
        if (coinPrefab == null) coinPrefab = this.coinPrefab;
        Transform spawnedCoin = EZ_PoolManager.Spawn(coinPrefab.transform, spawnPoint.position, spawnPoint.rotation);
        spawnedCoin.transform.parent = spawnPoint;
        spawnedCoin.GetComponent<Rigidbody>().isKinematic = kinematic;
        Coin coin = spawnedCoin.GetComponent<Coin>();
        coin.Value = coinValue;
        coin.Source = coinSource;
        return coin;
    }

    public void ThrowCoin(Coin coin, CoinsData data)
    {
        coin.transform.SetParent(null);

        float distance = Vector3.Distance(coin.transform.position, _currentMoneyJar.CoinsHole.position);
        float duration = distance / data.FlySpeed;

        float distanceInside = Vector3.Distance(_currentMoneyJar.CoinsDespawnPlace.position,
            _currentMoneyJar.CoinsHole.position);
        float durationInside = distanceInside / data.FlySpeed;

        coin.transform.DOJump(_currentMoneyJar.CoinsHole.position, data.FlyPower, 1, duration).onComplete +=
            () =>
            {
                coin.transform.DOMove(_currentMoneyJar.CoinsDespawnPlace.position, durationInside);
                coin.WentThroughJarHole = true;
            };

        coin.transform.DORotateQuaternion(_currentMoneyJar.CoinsHole.rotation, duration);

        if (coin.TweenAnim)
        {
            coin.TweenAnim.DORestart();
        }
    }

    public void CoinRain(ulong money, Coin coinPrefab, int coinsCount, CoinSource coinSource)
    {
        for (int i = 0; i < coinsCount; i++)
        {
            ulong coinValue;

            if (i == coinsCount - 1)
                coinValue = money / (ulong)coinsCount + (money % (ulong)coinsCount);
            else
                coinValue = money / (ulong)coinsCount;

            Coin spawnedCoin = SpawnRainCoin(coinValue, coinSource, coinPrefab);
            spawnedCoin.OnDropped += OnCoinDroppedHandler;
        }
    }

    private Coin SpawnRainCoin(ulong coinValue, CoinSource source, Coin coinPrefab)
    {
        Vector3 spawnPos = GetRandomPos();
        Quaternion spawnRot = Quaternion.Euler(GetRandomRotation());
        Transform spawnedCoin = EZ_PoolManager.Spawn(coinPrefab.transform, spawnPos, spawnRot);
        Coin coin = spawnedCoin.GetComponent<Coin>();
        coin.Value = (uint)coinValue;
        coin.Source = source;

        Rigidbody rB = spawnedCoin.GetComponent<Rigidbody>();
        rB.isKinematic = false;
        rB.AddForce(Vector3.down * extraForce, ForceMode.Impulse);

        return coin;
    }

    private void DespawnCoin(Coin coin)
    {
        EZ_PoolManager.Despawn(coin.transform);
        coin.GetComponent<Rigidbody>().velocity = Vector3.zero;
        OnCoinGained?.Invoke(coin);
        CheckIfLastAdsCoin(coin);
    }

    #region Ads coins

    [Header("Ads coins")][SerializeField] private float xOffset;
    [SerializeField] private float zOffset;
    [SerializeField] private float xRange;
    [SerializeField] private float zRange;
    [SerializeField] private int adsCoinsCount;
    [SerializeField] private CoinsData adsCoinData;
    [SerializeField] private float timeOnThePlane;

    private List<Coin> _droppedCoins;
    private Coin _lastAdsCoin;

    private void OnCoinDroppedHandler(Coin coin)
    {
        coin.OnDropped -= OnCoinDroppedHandler;
        _droppedCoins.Add(coin);

        int coinsCount = 0;

        if (coin.Source == CoinSource.ADS)
            coinsCount = adsCoinsCount;
        else if (coin.Source == CoinSource.FROG)
            coinsCount = _frogSpellsController.CurSpell.numberOfCoins;

        if (_droppedCoins.Count >= coinsCount)
        {
            List<Coin> droppedCoins = new List<Coin>();

            for (int i = 0; i < _droppedCoins.Count; i++)
            {
                droppedCoins.Add(_droppedCoins[i]);
            }

            _droppedCoins.Clear();

            StartCoroutine(ThrowDroppedCoins(droppedCoins));
        }
    }

    private IEnumerator ThrowDroppedCoins(List<Coin> droppedCoins)
    {
        yield return new WaitForSeconds(timeOnThePlane);

        for (int i = 0; i < droppedCoins.Count; i++)
        {
            droppedCoins[i].RB.isKinematic = true;
            ThrowCoin(droppedCoins[i], adsCoinData);

            if (i == droppedCoins.Count - 1)
                _lastAdsCoin = droppedCoins[i];
        }
    }

    //private IEnumerator ThrowDroppedCoin(Coin coin, CoinsData data)
    //{
    //    yield return new WaitForSeconds(timeOnThePlane);
    //    coin.RB.isKinematic = true;
    //    ThrowCoin(coin, data);
    //}

    private void CheckIfLastAdsCoin(Coin coin)
    {
        //if (_droppedCoins != null && _droppedCoins.Contains(coin))
        //{
        //    _droppedCoins.Remove(coin);

        //    if (_droppedCoins.Count == 0 && coin.Source == CoinSource.ADS)
        //        OnLastAdsCoinGained?.Invoke(coin);
        //}

        if (coin.Source == CoinSource.ADS && coin.Equals(_lastAdsCoin))
        {
            _lastAdsCoin = null;
            OnLastAdsCoinGained?.Invoke(coin);
        }
    }

    private Vector3 GetRandomPos()
    {
        float randX;
        float randZ;

        int zOrx = Random.Range(-1, 1);

        if (zOrx == 0)
        {
            randX = GetRandomAxis(xOffset, xRange);
            ;
            randZ = Random.Range(-2f, 2f);
        }
        else
        {
            randZ = GetRandomAxis(xOffset, xRange);
            ;
            randX = Random.Range(-1.5f, 1.5f);
        }

        float yPos = _currentMoneyJar.CoinsSpawnPlace.position.y;

        return new Vector3(randX, yPos, randZ);
    }

    private Vector3 GetRandomRotation()
    {
        float randX = Random.Range(0f, 360f);
        float randY = Random.Range(0f, 360f);
        float randZ = Random.Range(0f, 360f);
        return new Vector3(randX, randY, randZ);
    }

    private float GetRandomAxis(float offset, float range)
    {
        float randAxis;
        int posOrNeg = Random.Range(-1, 1);
        float min, max;

        if (posOrNeg == -1)
        {
            min = -offset - range;
            max = -offset;
            randAxis = -Random.Range(min, max);
        }
        else
        {
            min = offset;
            max = offset + range;
            randAxis = -Random.Range(min, max);
        }

        return randAxis;
    }

    private void HandleRewardedAdsWatched(int money)
    {
        CoinRain((ulong)money, coinPrefab, adsCoinsCount, CoinSource.ADS);
    }
    #endregion

    private void HandleStaticCoinThrowerSpawned(StaticCoinThrower coinThrower)
    {
        if (coinThrower is Frog)
        {
            _frog = Frog.Instance;
            _frogSpellsController = FrogSpellsController.Instance;

            _frogSpellsController.OnSpellCast += HandleFrogSpellCast;
        }
    }

    private void HandleFrogSpellCast(FrogSpell spell)
    {
        switch (spell.name)
        {
            case FrogSpellName.MoneyMagnet:
                StartCoroutine(MoneyBillsRainRoutine(spell));
                break;
            case FrogSpellName.BigCoin:
                Coin coin = SpawnCoin(_currentMoneyJar.CoinsSpawnPlace, spell.coinsReward, CoinSource.FROG, spell.coinPrefab, false);
                coin.WentThroughJarHole = true;
                break;
            case FrogSpellName.GoldenBlessing:
                break;
        }
    }

    private IEnumerator MoneyBillsRainRoutine(FrogSpell spell)
    {
        yield return new WaitForSeconds(moneyBillsRainDelay);
        CoinRain(_frog.MoneyPerThrow, spell.coinPrefab, spell.numberOfCoins, CoinSource.FROG);
    }
}