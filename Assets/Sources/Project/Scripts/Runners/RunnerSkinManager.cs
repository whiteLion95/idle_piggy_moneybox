using UnityEngine;

[System.Serializable]
public struct SRunnerSkin
{
    public int Id;
    public GameObject[] GameObjects;
}


public class RunnerSkinManager : MonoBehaviour
{
    [SerializeField] private SRunnerSkin[] _runnerSkins;

    private bool _isSuperman;
    private RunnersManager _runnersManager;

    private void Start()
    {
        _runnersManager = RunnersManager.Instance;

        SetSkin(_runnersManager.ExpController.CurrentLevel);

        _runnersManager.ExpController.OnLevelUp += HandleLevelUp;
    }

    public void SetSkin(int level)
    {
        if (_isSuperman) return;

        int number;

        if (level > 24)
            number = 6;
        else if (level > 16)
            number = 5;
        else if (level > 11)
            number = 4;
        else if (level > 7)
            number = 3;
        else if (level > 4)
            number = 2;
        else if (level > 1)
            number = 1;
        else
            number = 0;

        SwitchSkin(number);
    }

    private void HandleLevelUp(int level)
    {
        SetSkin(level);
    }

    public void SetSpecialSkin(int number)
    {
        _isSuperman = true;
        SwitchSkin(number);
    }

    private void SwitchSkin(int number)
    {
        for (int i = 0; i < _runnerSkins.Length; i++)
        {
            foreach (GameObject go in _runnerSkins[i].GameObjects)
            {
                if(go)
                    go.SetActive(false);
            }
        }

        int index = 0;
        for (int i = 0; i < _runnerSkins.Length; i++)
        {
            if (_runnerSkins[i].Id == number)
            {
                index = i;
                break;
            }
        }
        foreach (GameObject go in _runnerSkins[index].GameObjects)
        {
            if (go)
                go.SetActive(true);
        }
    }
}