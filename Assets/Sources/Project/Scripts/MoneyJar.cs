using DG.Tweening;
using UnityEngine;

public class MoneyJar : MonoBehaviour
{
    [SerializeField] private MoneyJarData data;
    [field: SerializeField] public Transform CoinsSpawnPlace { get; private set; }
    [field: SerializeField] public Transform CoinsHole { get; private set; }
    [field: SerializeField] public Transform CoinsDespawnPlace { get; private set; }

    [SerializeField] private Transform jarBody;
    private Material jarBodyMaterial;

    public static System.Action<MoneyJar> OnLoaded;

    private Vector3 _originalRotation;
    private CoinsManager _coinsManager;

    public static MoneyJar Instance { get; private set; }

    private void OnDestroy()
    {
        UpgradesManager.Instance.OnUpgrade -= HandleUpgrade;
    }

    private void Awake()
    {
        Instance = this;

        _originalRotation = transform.rotation.eulerAngles;

    }

    private void Start()
    {
        jarBodyMaterial = jarBody.GetComponent<Renderer>().material;
        OnLoaded?.Invoke(this);

        _coinsManager = CoinsManager.Instance;

        _coinsManager.OnCoinGained += OnCoinGainedHandler;

        UpgradesManager.Instance.OnUpgrade += HandleUpgrade;

        UpdateBodyColor(UpgradeType.MONEY_JAR_COIN_VALUE);
    }

    private void OnCoinGainedHandler(Coin coin)
    {
        ShakeMoneyJar();
    }

    private void ShakeMoneyJar()
    {
        transform.DOShakeRotation(data.ShakeDuration, data.ShakeStrength, data.ShakeVibrato, data.ShakeRandomness)
            .SetEase(data.EaseType)
            .OnComplete(() => transform.DORotate(_originalRotation, data.ReturnTime));
    }

    private void HandleUpgrade(UpgradeType type, CoinSource coinSource)
    {
        UpdateBodyColor(type);
    }

    public void UpdateBodyColor(UpgradeType type)
    {
        if (type != UpgradeType.MONEY_JAR_COIN_VALUE) return;

        int lvl = UpgradesManager.Instance.GetCurrentLevel(UpgradeType.MONEY_JAR_COIN_VALUE);
        Color c;

        if (lvl > 65)
            c = data.JarBodyColor[5];
        else if (lvl > 45)
            c = data.JarBodyColor[4];
        else if (lvl > 30)
            c = data.JarBodyColor[3];
        else if (lvl > 14)
            c = data.JarBodyColor[2];
        else if (lvl > 2)
            c = data.JarBodyColor[1];
        else
            c = data.JarBodyColor[0];

        jarBodyMaterial.color = c;
    }
}