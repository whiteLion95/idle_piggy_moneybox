using UnityEngine;

public class ParticlesManager : ParticlesManagerCore
{
    private CoinsManager _coinsManager;
    private MoneyJar _moneyJar;

    public static ParticlesManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _coinsManager = CoinsManager.Instance;
        _moneyJar = MoneyJar.Instance;

        Runner.OnRunnerJumped += HandlerOnRunnerJumped;
        Hand.OnThrow += HandlerOnHandThrowCoin;
        Frog.OnThrow += HandlerOnFrogThrowCoin;

        _coinsManager.OnCoinGained += (coin) => PlayParticleOnce(Particles.CoinInJar, _moneyJar.CoinsHole.position, Quaternion.identity);
    }

    private void OnDisable()
    {
        Runner.OnRunnerJumped -= HandlerOnRunnerJumped;
        Hand.OnThrow -= HandlerOnHandThrowCoin;
        Frog.OnThrow -= HandlerOnFrogThrowCoin;
    }

    private void HandlerOnRunnerJumped(Runner runner)
    {
        PlayParticleOnce(Particles.JumpParticles, runner.transform.position, Quaternion.identity);
    }

    private void HandlerOnHandThrowCoin()
    {
        PlayParticleOnce(Particles.HandThrowCoin, Hand.Instance.CoinSpawnPlace.position, Quaternion.identity);
    }

    private void HandlerOnFrogThrowCoin()
    {
        PlayParticleOnce(Particles.FrogJump, Frog.Instance.transform.position, Quaternion.identity);
    }
}
