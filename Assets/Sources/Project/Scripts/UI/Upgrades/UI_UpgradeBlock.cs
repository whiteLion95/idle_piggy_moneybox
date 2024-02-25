using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_UpgradeBlock : PurchaseBlock
{
    [SerializeField] private UpgradeType upType;
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private ReducedBigText upgradePriceText;
    [SerializeField] private GameObject moneyImage;
    [SerializeField] private GameObject dim;
    [SerializeField] private TMP_Text maxLevelText;
    [Header("Progress info")]
    [SerializeField] private ReducedBigText curLevelValue;
    [SerializeField] private GameObject arrow;
    [SerializeField] private ReducedBigText nextLevelValue;
    [Header("Relation")]
    [SerializeField] private bool initiator;
    [ShowIf("initiator")][SerializeField] private List<GameObject> relativeBlocks;
    [SerializeField] private bool floatFormat;

    private UpgradesManager _upManager;
    private BalanceManager _balanceManager;
    private int _curLevel;
    private uint _maxLevel;
    private float _upgradePrice;
    private RunnersManager _runnersManager;

    public override bool EnoughMoney { get; protected set; }

    // Start is called before the first frame update
    private void Start()
    {
        _upManager = UpgradesManager.Instance;
        _balanceManager = BalanceManager.Instance;
        _maxLevel = _upManager.GetMaxLevel(upType);
        _runnersManager = RunnersManager.Instance;

        Init();

        _upManager.OnRunnersCountLevelChanged += HandleRunnersCountLevelChanged;
    }

    private void Init()
    {
        SetValues();

        button.onClick.AddListener(() =>
        {
            ActionsOnButtonClick();
        });
    }

    private void ActionsOnButtonClick()
    {
        if (_curLevel < (_maxLevel - 1) && EnoughMoney)
        {
            _balanceManager.SpendMoney(_upgradePrice);
            _upManager.Upgrade(upType);
            SetValues();
            
            if (_curLevel == (_maxLevel - 1))
            {
                EnoughMoney = false;
            }
        }
    }

    private void SetValues()
    {
        _curLevel = _upManager.GetCurrentLevel(upType);
        levelText.text = "Level " + (_curLevel + 1);

        if (_curLevel < (_maxLevel - 1))
        {
            _upgradePrice = _upManager.GetLevelUpPrice(upType);
            upgradePriceText.SetValue(_upgradePrice);
            curLevelValue.SetValue(_upManager.GetValue(upType), floatFormat);
            nextLevelValue.SetValue(_upManager.GetValue(upType, _curLevel + 1), floatFormat);
        }
        else
        {
            upgradePriceText.gameObject.SetActive(false);
            moneyImage.SetActive(false);
            maxLevelText.gameObject.SetActive(true);
            dim.SetActive(true);

            curLevelValue.SetValue(_upManager.GetValue(upType));
            curLevelValue.GetComponent<TMP_Text>().color = nextLevelValue.GetComponent<TMP_Text>().color;
            nextLevelValue.gameObject.SetActive(false);
            arrow.SetActive(false);
        }

        if (initiator && _curLevel > 0)
        {
            foreach (var relBlock in relativeBlocks)
            {
                if (!relBlock.activeInHierarchy)
                {
                    relBlock.SetActive(true);
                }
            }
        }
    }

    private bool _maxLevelReached;
    private void FixedUpdate()
    {
        if (!_maxLevelReached)
        {
            if (_curLevel < (_maxLevel - 1))
            {
                CheckMoney();
            }
            else
            {
                _maxLevelReached = true;
                EnoughMoney = false;
            }
        }
    }

    protected override void CheckMoney()
    {
        if (_curLevel < (_maxLevel - 1))
        {
            if (_balanceManager.Money >= _upgradePrice && !EnoughMoney)
            {
                EnoughMoney = true;
                dim.SetActive(false);
                button.interactable = true;
            }
            else if (_balanceManager.Money < _upgradePrice && EnoughMoney)
            {
                EnoughMoney = false;
                dim.SetActive(true);
                button.interactable = false;
            }
        }
    }

    private void HandleRunnersCountLevelChanged(int level)
    {
        if (upType == UpgradeType.RUNNERS_COUNT)
        {
            SetValues();
        }
    }
}
