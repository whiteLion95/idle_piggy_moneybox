using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;

namespace Deslab.FX
{
    /// <summary>
    /// Class for dealing with number effects. Add this script to the GameObject with the TextMeshProUGUI component
    /// </summary>
    public class NumbersFXCore : MonoBehaviour
    {
        [SerializeField] protected NumbersFXData _numbersData;

        protected TextMeshProUGUI _numberText;
        protected Vector3 _startingNumberScale;

        private void Awake()
        {
            _numberText = GetComponent<TextMeshProUGUI>();
            _startingNumberScale = _numberText.transform.localScale;
        }

        private void Start()
        {
            ResetNumber();
        }

        public virtual void ShowNumber(int number, bool positive)
        {
            if (positive)
            {
                _numberText.text = string.Format($"+{number}");
                _numberText.color = Color.green;
            }
            else
            {
                _numberText.text = string.Format($"-{number}");
                _numberText.color = Color.red;
            }

            StartCoroutine(ShowNumberRoutine(_numbersData.ShowingDuration));
        }

        protected virtual IEnumerator ShowNumberRoutine(float duration)
        {
            _numberText.DOFade(255f, _numbersData.InOutDuration);
            _numberText.transform.DOScale(1f, _numbersData.InOutDuration).SetEase(_numbersData.EaseType);
            _numberText.transform.DOMoveY(transform.parent.position.y, _numbersData.InOutDuration).SetEase(_numbersData.EaseType);
            yield return new WaitForSeconds(duration);
            _numberText.DOFade(0f, _numbersData.InOutDuration);
            _numberText.transform.DOScale(_startingNumberScale, _numbersData.InOutDuration).SetEase(_numbersData.EaseType);
            _numberText.transform.DOMoveY(transform.parent.position.y + _numbersData.DeltaY, 0f);
        }

        protected virtual void ResetNumber()
        {
            StopAllCoroutines();
            _numberText.transform.localScale = _startingNumberScale;
            _numberText.alpha = 0f;
            _numberText.transform.DOMoveY(transform.parent.position.y + _numbersData.DeltaY, 0f);
        }
    }
}
