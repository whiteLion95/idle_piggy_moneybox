using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Skills_Runners : UI_Skills
{
    protected override void Init()
    {
        _skillsManager = RunnersSkillsManager.Instance;
        base.Init();
    }
}
