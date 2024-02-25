using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class GoalChooseBlock : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private ReducedBigText priceText;

    private Button _button;
    private GoalsManager _manager;

    public Goal MyGoal { get; private set; }

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _manager = GoalsManager.Instance;
        _button.onClick.AddListener(() =>
        {
            _manager.ChooseGoal(MyGoal);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetGoal(Goal goal, bool bigIcon)
    {
        MyGoal = goal;
        iconImage.sprite = goal.icon;
        nameText.text = goal.name;
        priceText.SetValue(goal.price);

        iconImage.GetComponent<RectTransform>().localScale =
            bigIcon ? new Vector3(2.6f, 2.6f, 2.6f) : new Vector3(1.3f, 1.3f, 1.3f);
    }
}
