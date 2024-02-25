using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Deslab.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasGroupWindow : MonoBehaviour
    {
        [SerializeField] private bool hasCloseButton;
        [SerializeField] private bool hideOnInit = true;
        protected CanvasGroup canvasGroup;

        /// <summary>
        /// If Screen|Window have close button set it in inspector 
        /// and set onClick Listener in Start.
        /// </summary>
        [ShowIf("hasCloseButton")]
        [SerializeField] Button closeButton;

        /// <summary>
        /// Tween animation ease.
        /// </summary>
        [SerializeField] private Ease ease;
        [SerializeField] internal float fadeTime;
        [SerializeField] internal float showDelay = 0f;
        [SerializeField] private float hideDelay = 0f;
        [SerializeField] private bool fullInteractable = true;

        public static Action<CanvasGroupWindow> OnWindowStartShowing;
        public static Action<CanvasGroupWindow> OnWindowHid;
        public Action OnWindowShowed;

        public CanvasGroup CanvasGroup { get { return canvasGroup; } }

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            
            if (hideOnInit)
            {
                HideWindow();
            }
        }

        void Start()
        {
            if (hasCloseButton)
            {
                closeButton.onClick.AddListener(() =>
                {
                    HideWindow();
                });
            }
        }
        

        public virtual void ShowWindow(Action onCompleted = null)
        {
            OnWindowStartShowing?.Invoke(this);
            canvasGroup.DOFade(1f, fadeTime).SetEase(ease).OnComplete(() =>
            {
                if (fullInteractable)
                {
                    canvasGroup.blocksRaycasts = true;
                    canvasGroup.interactable = true;
                    canvasGroup.alpha = 1f;
                }
                onCompleted?.Invoke();
                OnWindowShowed?.Invoke();
            }).SetUpdate(true).SetDelay(showDelay);
        }

        /// <summary>
        /// For UnityEvent in inspector
        /// </summary>
        public void ShowWindow()
        {
            OnWindowStartShowing?.Invoke(this);
            canvasGroup.DOFade(1f, fadeTime).SetEase(ease).OnComplete(() =>
            {
                if (fullInteractable)
                {
                    canvasGroup.blocksRaycasts = true;
                    canvasGroup.interactable = true;
                    canvasGroup.alpha = 1f;
                }
                OnWindowShowed?.Invoke();
            }).SetUpdate(true).SetDelay(showDelay);
        }

        public virtual void HideWindow()
        {
            if (canvasGroup != null)
            {
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;
                canvasGroup.DOFade(0f, fadeTime).SetEase(ease).OnComplete(() =>
                {
                    canvasGroup.alpha = 0f;
                    OnWindowHid?.Invoke(this);
                }).SetDelay(hideDelay).SetUpdate(true);
            }
        }

        public virtual void HideWindow(Action onCompleted = null)
        {
            //Debug.LogError("ShowWindow CanvasGroupWindow");
            if (canvasGroup != null)
            {
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;
                canvasGroup.DOFade(0f, fadeTime).SetEase(ease).OnComplete(() =>
                {
                    canvasGroup.alpha = 0f;
                    onCompleted?.Invoke();
                }).SetUpdate(true).SetDelay(hideDelay);
            }
        }

        public virtual void DisableWindow()
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }

        public virtual void EnableWindow()
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }

        private void Update() { }
    }
}
