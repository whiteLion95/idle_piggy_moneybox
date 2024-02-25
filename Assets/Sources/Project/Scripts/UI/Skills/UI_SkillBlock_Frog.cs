using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SkillBlock_Frog : UI_SkillBlock
{
    [SerializeField] private FrogSkillName skillName;

    protected override void Start()
    {
        _skillManager = FrogSkillsManager.Instance;
        _expController = Frog.Instance.ExpController;

        base.Start();
    }
    
    protected override void PurchaseUpgrade()
    {
        (_skillManager as FrogSkillsManager).PurchaseUpgrade(skillName);
    }

    protected override void SetSkill()
    {
        _mySkill = (_skillManager as FrogSkillsManager).GetSkill(skillName);
    }
}
