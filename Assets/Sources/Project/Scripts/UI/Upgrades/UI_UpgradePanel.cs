using Deslab.UI;
using System.Collections.Generic;
using UnityEngine;

public class UI_UpgradePanel : MonoBehaviour
{
    [SerializeField] private UI_UnitButton myButton;
    [SerializeField] private List<GameObject> tabGroups;
    [SerializeField] private GameObject defaultGroup;
    [SerializeField] private TabButton defaultTab;
    [SerializeField] private TabGroup tabGroup;

    public System.Action<bool> OnEnoughMoney;

    private PurchaseBlock[] _blocks;
    private bool _enoughMoney;

    public CanvasGroupWindow Window { get; private set; }

    private void Awake()
    {
        Window = GetComponent<CanvasGroupWindow>();
        _blocks = GetComponentsInChildren<PurchaseBlock>(true);
        tabGroup = GetComponentInChildren<TabGroup>();

        CanvasGroupWindow.OnWindowHid += HandleWindowHid;
    }

    // Start is called before the first frame update
    private void Start()
    {
        //Init();   

        if (!tabGroup)
        {
            tabGroup = GetComponentInChildren<TabGroup>();
        }
    }

    //private void Init()
    //{
    //    foreach (PurchaseBlock block in _blocks)
    //    {
    //        block.OnEnoughMoney += (x) => OnEnoughMoney?.Invoke(x);
    //    }
    //}

    private void OnDisable()
    {
        CanvasGroupWindow.OnWindowHid -= HandleWindowHid;
    }

    private void FixedUpdate()
    {
        CheckEnoughMoney();
    }

    private void CheckEnoughMoney()
    {
        foreach (PurchaseBlock block in _blocks)
        {
            if (block.EnoughMoney)
            {
                _enoughMoney = true;
                break;
            }

            _enoughMoney = false;
        }

        if (_enoughMoney && !myButton.RedDot.activeSelf)
            myButton.RedDot.SetActive(true);
        else if (!_enoughMoney && myButton.RedDot.activeSelf)
            myButton.RedDot.SetActive(false);
    }

    private void HandleWindowHid(CanvasGroupWindow window)
    {
        if (window && window.Equals(Window))
        {
            if (defaultTab)
            {
                tabGroup.OnTabSelected(defaultTab);
            }

            if (tabGroups != null)
            {
                foreach (var tabGroup in tabGroups)
                {
                    tabGroup.gameObject.SetActive(false);
                }
            }

            if (defaultGroup)
            {
                defaultGroup.SetActive(true);
            }
        }
    }
}
