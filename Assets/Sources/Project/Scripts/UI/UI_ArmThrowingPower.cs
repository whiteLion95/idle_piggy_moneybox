using UnityEngine;

public class UI_ArmThrowingPower : MonoBehaviour
{
    [SerializeField] private ReducedBigText powerText;

    private Hand _hand;

    private void Start()
    {
        _hand = Hand.Instance;

        SetPowerText(_hand.MoneyPerThrow);

        _hand.OnMoneyPerThrowChanged += HandleMoneyPerThrowChanged;
    }

    private void SetPowerText(float value)
    {
        powerText.SetValue(value);
    }

    private void HandleMoneyPerThrowChanged(ulong value)
    {
        SetPowerText(value);
    }
}