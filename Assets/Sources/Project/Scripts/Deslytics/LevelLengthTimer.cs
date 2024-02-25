using System.Collections;
using UnityEngine;

namespace Deslab.Deslytics
{
    public class LevelLengthTimer : MonoBehaviour
    {
        private IEnumerator _levelLengthCoroutine;
        public static float Value;

        private void OnEnable()
        {
            DeslyticsManager.OnLevelStart += OnLevelStart;
            DeslyticsManager.OnLevelFinish += OnLevelFinish;
        }
        
        private void OnDisable()
        {
            DeslyticsManager.OnLevelStart -= OnLevelStart;
            DeslyticsManager.OnLevelFinish -= OnLevelFinish; 
        }

        private void OnLevelStart()
        {
            StopAllCoroutines();
            Value = 0f;
            StartCoroutine(LevelLengthCoroutine());
        }

        private void OnLevelFinish()
        {
            StopAllCoroutines();
        }
        
        private IEnumerator LevelLengthCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                Value += 1f;
            }
        }
    }
}