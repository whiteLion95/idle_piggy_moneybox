using UnityEngine;

[System.Serializable]
public class RunnersSkill : Skill
{
    [SerializeField] private RunnersSkillName name;

    public RunnersSkillName Name => name;

    public override void Init()
    {
        _saveKey = name.ToString();
        base.Init();
    }

    protected override float CalculateValue(Stat statName, int level)
    {
        float value = 0f;

        switch (statName)
        {
            case Stat.CRIT_CHANCE:
                value = Mathf.Clamp(0.04f * level, 0f, 1f);
                break;
            case Stat.CRIT_MULT:
                value = 0.1f * level;
                break;
            case Stat.POWER_BONUS:
                switch (name)
                {
                    case RunnersSkillName.Strength:
                        value = 0.1f * level;
                        break;
                    case RunnersSkillName.RunnerMaturity:
                        value = 0.01f * level * TimeManager.Instance.TotalHoursPlayed;
                        break;
                    case RunnersSkillName.MoneyBags:
                        value = 0.01f * level;
                        break;
                    case RunnersSkillName.HelpfulCarl:
                        value = 0.015f * level * RunnersManager.Instance.RunnersAmount;
                        break;
                }
                break;
            case Stat.SPEED:
                value = 0.06f * level;
                break;
            case Stat.EXP_GAIN:
                value = 0.08f * level;
                break;
            case Stat.SUPER_CRIT_CHANCE:
                value = Mathf.Clamp(0.025f * level, 0f, 0.5f);
                break;
            case Stat.SUPER_CRIT_MULT:
                value = 1 + 0.04f * level;
                break;
            case Stat.MANA_REGEN:
                value = level;
                break;
        }

        return value;
    }
}