using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    const int TITLE = 0;
    const int ENDING = 3;

    [SerializeField, Range(0, 9)] int rest = 3;
    [SerializeField, Range(TITLE + 1, ENDING - 1)] int stageNo = 1;

    private int currentRest;

    private string[] stageName = {
        "Title",
        "Stage1",
        "Stage2",
        "Ending",
    };
    private static GameManager instance = null;

    public int Rest
    {
        get => currentRest;
        set => currentRest = value;
    }

    public int StageNo => stageNo;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            currentRest = rest;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GameStart()
    {
        SceneManager.LoadScene(stageName[stageNo] + "Scene");
    }

    public bool HasNextStage()
    {
        return stageNo < stageName.Length - 2;
    }

    public void GoToNextStage()
    {
        SceneManager.LoadScene(stageName[++stageNo] + "Scene");
    }

    public void GoToTitleScene()
    {
        currentRest = rest;
        stageNo = 1;
        SceneManager.LoadScene(stageName[0] + "Scene");
    }
}
