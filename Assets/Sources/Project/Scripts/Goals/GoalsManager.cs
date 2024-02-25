using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Deslab;
using Deslab.Deslytics;
using Sirenix.OdinInspector;
using TMPro;

public class GoalsManager : MonoBehaviour, ISaveable
{
    [SerializeField] private GoalsData data;
    [SerializeField] private GoalChooseBlock[] goalChooseBlocks;
    [SerializeField] private PurchasedGoalMenuBlock purchasedBlockPrefab;
    [SerializeField] private Transform purchasedBlocksParent;

    [FoldoutGroup("После завершения всех целей")] [SerializeField]
    private TextMeshProUGUI _congratsButtonLabel;

    [FoldoutGroup("После завершения всех целей")] [SerializeField]
    private string _congratsButtonLabelText = "";
    
    private List<PurchasedGoalMenuBlock> _purchasedBlocks;

    private GoalsSaving _savingData;

    public System.Action OnGoalChoose;

    public static GoalsManager Instance { get; private set; }
    public Goal CurrentGoal { get; private set; }

    public int AmountGoalsComplete
    {
        get { return _savingData.purchasedGoals.Count; }
    }

    public bool AllGoalsComplete
    {
        get { return _savingData.purchasedGoals.Count == (data.Goals.Length / 3); }
    }

    private void Awake()
    {
        Instance = this;
        LoadData();
    }

    void Start()
    {
        InitPurchasedGoalBlocks();
    }

    public void InitGoalChooseBlocks()
    {
        List<Goal> purchasedGoals = _savingData.purchasedGoals;
        int count;
        List<Goal> showedNotPurchasedGoals = new List<Goal>();
        List<Goal> showedGoals = new List<Goal>();

        int selectFrom;
        if (purchasedGoals.Count == 0)
        {
            goalChooseBlocks.Last().gameObject.SetActive(true);
            count = 4;
            selectFrom = 0;
        }
        else
        {
            goalChooseBlocks.Last().gameObject.SetActive(false);
            count = 3;
            selectFrom = purchasedGoals.Count * 3 + 1;
        }


        for (int i = 0; i < count; i++)
        {
            goalChooseBlocks[i].SetGoal(data.Goals[selectFrom + i], count == 4);
            showedNotPurchasedGoals.Add(goalChooseBlocks[i].MyGoal);
            showedGoals.Add(goalChooseBlocks[i].MyGoal);
        }

        /*
         //old goals system
         List<Goal> showedRandGoals = new List<Goal>();
         bool randomGoals = _savingData.randomGoals;
         
         for (int i = 0; i < goalChooseBlocks.Length; i++)
        {
            if (randomGoals)
            {
                Goal randGoal = GetRandomGoal(showedGoals.ToArray());
                showedGoals.Add(randGoal);
                showedRandGoals.Add(randGoal);
                goalChooseBlocks[i].SetGoal(randGoal);
            }
            else
            {
                for (int j = count; j < data.Goals.Length; j++)
                {
                    if (purchasedGoals != null && purchasedGoals.Any((g) => g.name == data.Goals[j].name))
                    {
                        if (j == (data.Goals.Length - 1))
                        {
                            randomGoals = true;
                            Goal randGoal = GetRandomGoal(showedGoals.ToArray());
                            showedGoals.Add(randGoal);
                            goalChooseBlocks[i].SetGoal(randGoal);

                            if (showedNotPurchasedGoals.Count == 0)
                                _savingData.randomGoals = true;
                        }

                        continue;
                    }
                    else
                    {
                        goalChooseBlocks[i].SetGoal(data.Goals[j]);
                        showedNotPurchasedGoals.Add(goalChooseBlocks[i].MyGoal);
                        showedGoals.Add(goalChooseBlocks[i].MyGoal);
                        count = j + 1;

                        if (count == data.Goals.Length)
                        {
                            randomGoals = true;
                        }

                        break;
                    }
                }
            }
        }

        if (showedRandGoals.Count > 1)
        {
            var query = showedRandGoals.OrderBy(goal => goal.price);
            showedRandGoals = query.ToList();
            count = 0;

            for (int i = 0; i < showedRandGoals.Count; i++)
            {
                for (int j = count; j < goalChooseBlocks.Length; j++)
                {
                    if (showedRandGoals.Contains(goalChooseBlocks[j].MyGoal))
                    {
                        goalChooseBlocks[j].SetGoal(showedRandGoals[i]);
                        count = j + 1;
                        break;
                    }
                }
            }
        }*/
    }

    public void ChooseGoal(Goal goal)
    {
        CurrentGoal = goal;
        Time.timeScale = 1f;
        OnGoalChoose?.Invoke();
    }

    public void PurchaseCurrentGoal()
    {
        BalanceManager.Instance.SpendMoney(CurrentGoal.price);
        _savingData.purchasedGoals.Add(CurrentGoal);
        SpawnPurchaseGoalBlock(CurrentGoal);
        DeslyticsManager.BuyProgressionItem(CurrentGoal.name);
        
        CurrentGoal = null;

        DreamsManager.instance.UpdateStateDreams(_savingData.purchasedGoals);

        if (AllGoalsComplete)
        {
            _congratsButtonLabel.text = _congratsButtonLabelText;
        }
    }

    private Goal GetRandomGoal(params Goal[] excepts)
    {
        List<Goal> availableGoals = new List<Goal>();

        availableGoals.AddRange(data.Goals);

        foreach (Goal ecxept in excepts)
        {
            availableGoals.Remove(ecxept);
        }

        int randIndex = Random.Range(0, availableGoals.Count);
        Goal randGoal = availableGoals[randIndex];
        randGoal.price = (ulong)(randGoal.SaveUpTime * BalanceManager.Instance.AverageProfitPerMinute);

        if (randGoal.price < randGoal.OriginalPrice)
            randGoal.price += randGoal.OriginalPrice;

        return randGoal;
    }

    private void InitPurchasedGoalBlocks()
    {
        DestroyCurrentPurchaseBlocks();

        List<Goal> purchasedGoals = _savingData.purchasedGoals;
        _purchasedBlocks = new List<PurchasedGoalMenuBlock>();

        foreach (Goal goal in purchasedGoals)
        {
            for (int a = 0; a < data.Goals.Length; a++)
            {
                if (goal.name == data.Goals[a].name)
                    SpawnPurchaseGoalBlock(data.Goals[a]);
            }
        }

        DreamsManager.instance.UpdateStateDreams(_savingData.purchasedGoals);
    }

    public Goal GetCurrentGoal()
    {
        Goal goal = new Goal();
        for (int a = 0; a < data.Goals.Length; a++)
        {
            if (CurrentGoal.name == data.Goals[a].name)
                goal = data.Goals[a];
        }

        return goal;
    }

    private void SpawnPurchaseGoalBlock(Goal goal)
    {
        PurchasedGoalMenuBlock spawnedBlock = Instantiate(purchasedBlockPrefab, purchasedBlocksParent);
        _purchasedBlocks.Add(spawnedBlock);
        spawnedBlock.Init(goal.icon, goal.name);
    }

    private void DestroyCurrentPurchaseBlocks()
    {
        PurchasedGoalMenuBlock[] blocks = purchasedBlocksParent.GetComponentsInChildren<PurchasedGoalMenuBlock>();

        if (blocks.Length > 0)
        {
            foreach (var block in blocks)
            {
                Destroy(block.gameObject);
            }
        }
    }

    #region Data management

    private void OnDestroy()
    {
        SaveData();
    }

    private void OnApplicationPause(bool pause)
    {
        SaveData();
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

    private const string CURRENT_GOAL_KEY = "Current goal";
    private const string GOALS_SAVINGS = "Goals saving";

    public void LoadData()
    {
        CurrentGoal = SaveManager.Load<Goal>(CURRENT_GOAL_KEY);
        _savingData = SaveManager.Load<GoalsSaving>(GOALS_SAVINGS);

        if (_savingData.purchasedGoals == null)
            _savingData.purchasedGoals = new List<Goal>();
    }

    public void SaveData()
    {
        SaveManager.Save(CURRENT_GOAL_KEY, CurrentGoal);
        SaveManager.Save(GOALS_SAVINGS, _savingData);
    }

    #endregion
}

[System.Serializable]
public struct GoalsSaving
{
    public List<Goal> purchasedGoals;
    public bool randomGoals;
}