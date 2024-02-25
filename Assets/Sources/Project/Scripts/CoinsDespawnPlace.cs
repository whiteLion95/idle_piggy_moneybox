using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinsDespawnPlace : MonoBehaviour
{
    public static System.Action<Coin> OnCoinTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            Coin triggeredCoin = other.GetComponent<Coin>();

            if (triggeredCoin.WentThroughJarHole || triggeredCoin.Source == CoinSource.TAP)
                OnCoinTriggered?.Invoke(other.GetComponent<Coin>());
        }
    }
}
