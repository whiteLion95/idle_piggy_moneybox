using UnityEngine;

[System.Serializable]
public class FrogSkill : Skill
{
    [SerializeField] private FrogSkillName name;

    public FrogSkillName Name => name;

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
            case Stat.POWER_BONUS:
                value = 0.07f * level;
                break;
            case Stat.EXP_GAIN:
                value = 0.07f * level;
                break;
            case Stat.MANA_REGEN_ADDING:
                value = level == 0 ? 0 : 10f + 2f * level;
                break;
            case Stat.MANA_BOOST_DURATION:
                value = level == 0 ? 0 : 1.8f + 0.2f * level;
                break;
            case Stat.MANA_BOOST_COOLDOWN:
                value = 60f;
                break;
            case Stat.RUNNER_EXP:
                value = level * 2f;
                break;
            case Stat.MANA_COST_REDUCTION:
                value = (1.0f - 1.0f / (1.0f + level * 0.06f)) * 100f;
                break;
        }

        return value;
    }
}
