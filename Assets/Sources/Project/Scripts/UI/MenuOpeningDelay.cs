using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Deslab.UI
{
    public class MenuOpeningDelay : MonoBehaviour
    {
        [System.Serializable]
        private struct SWindowOpenning
        {
            public CanvasGroupWindow WindowToShow;
            public CanvasGroupWindow WindowToHide;
            public float Delay;
        }

        [SerializeField] private SWindowOpenning[] _windowOpennings;
        
        public void ShowWindow(int index)
        {
            StopAllCoroutines();
            StartCoroutine(ShowWindowRoutine(
                _windowOpennings[index].WindowToShow,
                _windowOpennings[index].WindowToHide,
                _windowOpennings[index].Delay));
        }

        private IEnumerator ShowWindowRoutine(CanvasGroupWindow windowShow,CanvasGroupWindow windowHide, float delay)
        {
            CoinsManager.Instance.CanThrowCoinsFromTap = false;
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(delay);
            Time.timeScale = 1f;
            CoinsManager.Instance.CanThrowCoinsFromTap = true;
            windowShow.ShowWindow();
            windowHide.HideWindow();
            CameraMover.MoveToStart();
        }

    }
}