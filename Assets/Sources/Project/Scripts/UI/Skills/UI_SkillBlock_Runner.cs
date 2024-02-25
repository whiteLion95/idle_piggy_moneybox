using UnityEngine;

public class UI_SkillBlock_Runner : UI_SkillBlock
{
    [SerializeField] private RunnersSkillName skillName;

    private RunnersManager _runnersManager;

    protected override void Start()
    {
        _skillManager = RunnersSkillsManager.Instance;
        _expController = RunnersManager.Instance.ExpController;
        _runnersManager = RunnersManager.Instance;

        base.Start();
    }

    protected override void SetSkill()
    {
        _mySkill = (_skillManager as RunnersSkillsManager).GetSkill(skillName);
    }

    protected override void PurchaseUpgrade()
    {
        if (_runnersManager.NonSupermanRunnersAmount == 0)
            return;

        (_skillManager as RunnersSkillsManager).PurchaseUpgrade(skillName);
    }
}