using DG.Tweening;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public System.Action<Coin> OnDropped;

    public float Value { get; set; }
    public CoinSource Source { get; set; }
    public bool WentThroughJarHole { get; set; }

    private bool _dropped;
    private DOTweenAnimation _tweenAnim;

    public DOTweenAnimation TweenAnim => _tweenAnim;
    public Rigidbody RB { get; private set; }

    private void Awake()
    {
        _tweenAnim = GetComponentInChildren<DOTweenAnimation>();
        RB = GetComponent<Rigidbody>();
    }

    private void OnDisable()
    {
        if (_tweenAnim)
        {
            _tweenAnim.DORewind();
        }
    }

    private void OnEnable()
    {
        _dropped = false;
        WentThroughJarHole = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Plane"))
        {
            if (!_dropped && Source == CoinSource.ADS || Source == CoinSource.FROG)
            {
                _dropped = true;
                OnDropped?.Invoke(this);
            }

            AudioManager.Instance.PlayOneShot("Coin drop");
            //VibrationExtention.WaveVibrate();
        }
    }
}
