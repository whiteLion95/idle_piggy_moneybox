using UnityEngine;

public class RunnerTrail : MonoBehaviour
{
    [SerializeField] private float _minVelocity;
    [SerializeField] private float _maxVelocity;
    [SerializeField] private float _minNoizeStrength;
    [SerializeField] private float _maxNoizeStrength;
    [SerializeField] private int _maxValuesLevel = 30;

    private Runner _runner;
    private ParticleSystem _particles;
    private float _velPerLevel;
    private float _noizeStrengthPerLevel;

    private void Awake()
    {
        _runner = GetComponentInParent<Runner>();
        _particles = GetComponent<ParticleSystem>();

        _runner.OnSpeedSet += HandlerOnSpeedSet;

        CalculateValuesPerLevel();
    }

    // Start is called before the first frame update
    private void Start()
    {

    }

    private void CalculateValuesPerLevel()
    {
        _velPerLevel = (_maxVelocity - _minVelocity) / _maxValuesLevel;
        _noizeStrengthPerLevel = (_maxNoizeStrength - _minNoizeStrength) / _maxValuesLevel;
    }

    private void HandlerOnSpeedSet(int level)
    {
        if (!_particles.isPlaying && level > 0)
            _particles.Play();

        ParticleSystem.VelocityOverLifetimeModule velMod = _particles.velocityOverLifetime;
        velMod.z = Mathf.Clamp(_minVelocity + (_velPerLevel * level), _minVelocity, _maxVelocity);

        ParticleSystem.NoiseModule noizeMod = _particles.noise;
        noizeMod.strength = Mathf.Clamp(_minNoizeStrength + (_noizeStrengthPerLevel * level), _minNoizeStrength, _maxNoizeStrength);
    }
}