using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyLibrary.Audio;

public class AudioManager : AudioManagerCore
{
    [SerializeField] private Button turnOffButton;
    [SerializeField] private Image turnOffButtonDim;
    [SerializeField] private Sprite turnOffButtonSprite;
    [SerializeField] private Sprite turnOnButtonSprite;
    
    private void OnEnable()
    {
        CoinsDespawnPlace.OnCoinTriggered += CoinGainedHandler;
        Hand.OnThrow += HandFlipHandler;
        Frog.OnThrow += FrogThrowHandler;
        StaticCoinThrower.OnLastCoinGained += LastCoinGainedHandler;
        CoinsManager.OnLastAdsCoinGained += LastAdsCoinGainedHandler;

        turnOffButton.onClick.AddListener(SoundSwitch);
    }

    private void OnDisable()
    {
        CoinsDespawnPlace.OnCoinTriggered -= CoinGainedHandler;
        Hand.OnThrow -= HandFlipHandler;
        Frog.OnThrow -= FrogThrowHandler;
        StaticCoinThrower.OnLastCoinGained -= LastCoinGainedHandler;
        CoinsManager.OnLastAdsCoinGained -= LastAdsCoinGainedHandler;
    }

    private void CoinGainedHandler(Coin coin)
    {
        PlayOneShot("Ringing");
    }

    private void HandFlipHandler()
    {
        PlayOneShot("Hand");
    }

    private void FrogThrowHandler()
    {
        PlayOneShot("Frog");
    }

    private void LastCoinGainedHandler(Coin coin)
    {
        PlayOneShot("Ringing2");
    }

    private void LastAdsCoinGainedHandler(Coin coin)
    {
        PlayOneShot("Ads reward");
    }

    private void SoundSwitch()
    {
        if (audioSource.mute)
        {
            audioSource.mute = false;
            turnOffButtonDim.sprite = turnOnButtonSprite;
        }
        else
        {
            audioSource.mute = true;
            turnOffButtonDim.sprite = turnOffButtonSprite;
        }
    }
}
