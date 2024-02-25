using System.Collections.Generic;
using Deslab.Scripts.Deslytics.Ads;
using Deslab.UI;
using TMPro;
using UnityEngine;

namespace Deslab
{
    public class Cheats : MonoBehaviour
    {
        [SerializeField] private int _amountMoney = 5000000;

        [SerializeField] private TextMeshProUGUI _labelAddMoneyButton;
        
        [SerializeField] private List<GameObject> _screens;

        [SerializeField] private GameObject _buttonShow;
        [SerializeField] private GameObject _buttonHide;

        private void Start()
        {
            _labelAddMoneyButton.text = string.Format("Add money({0})", ReducedBigText.GetText(_amountMoney));
        }
        
        public void AddMoney()
        {
            BalanceManager.Instance.GainMoney((ulong)_amountMoney);
        }

        public void HideUI()
        {
            _buttonShow.SetActive(true);
            _buttonHide.SetActive(false);
            foreach (GameObject screen in _screens)
            {
                screen.SetActive(false);
            }
            
            AdsManager.HideBannerAds();
        }

        public void ShowUI()
        {
            _buttonShow.SetActive(false);
            _buttonHide.SetActive(true);
            foreach (GameObject screen in _screens)
            {
                screen.SetActive(true);
            }
            
            AdsManager.ShowBannerAds("BANNER");
        }
    }
}