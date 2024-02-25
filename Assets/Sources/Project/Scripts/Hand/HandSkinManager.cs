using UnityEngine;

public class HandSkinManager : MonoBehaviour
{
    [SerializeField] private SRunnerSkin[] _runnerSkins;

    private Hand _hand;

    private void Start()
    {
        _hand = Hand.Instance;

        SetSkin(_hand.MoneyPerThrow);

        UpgradesManager.Instance.OnUpgrade += HandleHandUpgrade;
    }

    public void SetSkin(float moneyPerThrow)
    {
        int number;

        if (moneyPerThrow > 3000000f)
            number = 7;
        else if (moneyPerThrow > 1400000f)
            number = 6;
        else if (moneyPerThrow > 750000f)
            number = 5;
        else if (moneyPerThrow > 350000f)
            number = 4;
        else if (moneyPerThrow > 150000f)
            number = 3;
        else if (moneyPerThrow > 60000f)
            number = 2;
        else if (moneyPerThrow > 35000f)
            number = 1;
        else
            number = 0;

        for (int i = 0; i < _runnerSkins.Length; i++)
        {
            foreach (GameObject go in _runnerSkins[i].GameObjects)
            {
                go.SetActive(false);
            }
        }

        foreach (GameObject go in _runnerSkins[number].GameObjects)
        {
            go.SetActive(true);
        }
    }

    private void HandleHandUpgrade(UpgradeType upType, CoinSource coinSource)
    {
        if (coinSource == CoinSource.HAND)
        {
            SetSkin(_hand.MoneyPerThrow);
        }
    }
}