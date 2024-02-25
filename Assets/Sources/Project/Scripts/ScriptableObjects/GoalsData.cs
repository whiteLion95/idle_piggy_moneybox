using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "GoalsData", menuName = "ScriptableObjects/GoalsData")]
public class GoalsData : ScriptableObject
{
    [field: SerializeField] public Goal[] Goals { get; private set; }
    [SerializeField] private float characterCost = 1000;
    [SerializeField] private float baseCost;
    [SerializeField] [Range(0f, 2.5f)] private float costMultiplier;

    [Space(20f)]
    [Header("После покупки всех целей")]
    [SerializeField]
    [Tooltip("Время в минутах для накопления первой цели в списке, учитывая текущий доход в минуту")]
    private float minSaveUpTime;

    [SerializeField]
    [Tooltip("Время в минутах для накопления последней цели в списке, учитывая текущий доход в минуту")]
    private float maxSaveUpTime;

    public float MinSaveUpTime
    {
        get { return minSaveUpTime; }
    }

    public float MaxSaveUpTime
    {
        get { return maxSaveUpTime; }
    }

    private void OnEnable()
    {
        //SetPrices();
        SetSaveUpTime();
    }

    //[Button]
    //private void SortGoalsByPrice()
    //{
    //    var query = Goals.OrderBy(goal => goal.price);
    //    Goals = query.ToArray();
    //}

    private void SetSaveUpTime()
    {
        float delta = (maxSaveUpTime - minSaveUpTime) / Goals.Length;

        for (int i = 0; i < Goals.Length; i++)
        {
            if (i == 0)
                Goals[i].SaveUpTime = minSaveUpTime;
            else if (i == Goals.Length - 1)
                Goals[i].SaveUpTime = maxSaveUpTime;
            else
                Goals[i].SaveUpTime = minSaveUpTime + (i * delta);
        }
    }

    [Button]
    private void ResetPricesToOriginal()
    {
        foreach (Goal goal in Goals)
        {
            goal.price = goal.OriginalPrice;
        }
    }

    [Button]
    private void SetPrices()
    {
        for (int i = 0; i < Goals.Length; i++)
        {
            if (i < 4)
            {
                Goals[i].price = (ulong)characterCost;
                Goals[i].OriginalPrice = Goals[i].price;
            }
            else
            {
                Goals[i].price = (ulong)(baseCost * Mathf.Pow(costMultiplier, i));
                Goals[i].OriginalPrice = Goals[i].price;
            }
        }
    }
}

[System.Serializable]
public class Goal
{
    public int id;
    public string name;
    public ulong price;
    [JsonIgnore] public Sprite icon;
    public float SaveUpTime { get; set; }
    public ulong OriginalPrice { get; set; }
}