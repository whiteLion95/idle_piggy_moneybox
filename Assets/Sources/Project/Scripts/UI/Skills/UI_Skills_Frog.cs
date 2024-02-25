using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Skills_Frog : UI_Skills
{
    protected override void Init()
    {
        _skillsManager = FrogSkillsManager.Instance;
        base.Init();
    }
}
