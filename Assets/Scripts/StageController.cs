using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StageController : MonoBehaviour
{
    [SerializeField] Timer timer;
    [SerializeField] Text timeOverText;
    [SerializeField] Text gameOverText;
    [SerializeField] Text clearText;
    [SerializeField] Text goText;
    [SerializeField] Text stageStartText;
    [SerializeField] ItemManager itemManager;
    [SerializeField] Player player;
    [SerializeField] EnemySpawner enemySpawner;
    [SerializeField] Background background;
    [SerializeField] bool scrolling;
    [SerializeField] AudioClip bgmIntro;
    [SerializeField] AudioClip bgmLoop;
    [SerializeField] AudioClip gameOverAudio;
    [SerializeField] AudioClip gameClearAudio;

    private bool isGameOver;
    private float scrollDistance;
    private GameManager gameManager;
    private AudioSource audioSource;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        audioSource = GetComponent<AudioSource>();
        timer.TimeOver += TimeOver;
        player.OnDie += GameOver;
        player.OnMove += PlayerMoved;
        enemySpawner.OnStageClear += GameClear;
        enemySpawner.OnStartScroll += StartScroll;
        enemySpawner.OnStopScroll += StopScroll;
        isGameOver = false;
        scrollDistance = 0f;
        StartCoroutine(ShowStageStartText());
        StartCoroutine(PlayBgm());
    }

    private IEnumerator ShowStageStartText()
    {
        yield return new WaitForSeconds(1f);
        stageStartText.text = "STAGE" + gameManager.StageNo;
        GameObject obj = stageStartText.gameObject;
        obj.SetActive(true);
        yield return new WaitForSeconds(1f);
        obj.SetActive(false);
        yield return new WaitForSeconds(1f);
        stageStartText.text = "START!";
        obj.SetActive(true);
        yield return new WaitForSeconds(1f);
        obj.SetActive(false);
        yield return new WaitForSeconds(1f);
        itemManager.Spawn();
        enemySpawner.Spawn(0);
        timer.StartTimer();
    }

    private IEnumerator PlayBgm()
    {
        audioSource.loop = false;
        audioSource.clip = bgmIntro;
        audioSource.Play();
        yield return new WaitWhile(() => audioSource.isPlaying);
        audioSource.Stop();
        audioSource.clip = bgmLoop;
        audioSource.loop = true;
        audioSource.Play();
    }

    private void Update()
    {
        if (isGameOver)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            player.SpecialAttack();
            return;
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            if (itemManager.GetItem(player.transform.position, out GameObject item))
            {
                player.GetItem(item);
            }
            else
            {
                player.Attack();
            }
            return;
        }
        Vector3 direction = Vector3.zero;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            direction += Vector3.left;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            direction += Vector3.right;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            direction += Vector3.up;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            direction += Vector3.down;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.Jump(direction);
        }
        player.Move(direction);
    }

    private void PlayerMoved(Vector3 move)
    {
        if (scrolling)
        {
            float dx = move.x;
            if (dx > 0f && player.transform.position.x >= MovableArea.CENTER_X)
            {
                scrollDistance += dx;
                enemySpawner.Spawn(dx);
                enemySpawner.AdjustEnemyPosition(move);
                itemManager.Move(dx);
                background.Move(dx);
            }
        }
        else
        {
            scrollDistance = 0;
        }
    }

    private void TimeOver()
    {
        if(isGameOver)
        {
            return;
        }
        isGameOver = true;
        timeOverText.gameObject.SetActive(true);
        timer.TimeOver -= TimeOver;
        enemySpawner.GameOver();
        player.Defeat();
        PlayAudio(gameOverAudio, false);
        StartCoroutine(GoToTitleScene());
    }

    private void GameOver()
    {
        if (isGameOver)
        {
            return;
        }
        isGameOver = true;
        gameOverText.gameObject.SetActive(true);
        timer.StopTimer();
        enemySpawner.GameOver();
        PlayAudio(gameOverAudio, false);
        StartCoroutine(GoToTitleScene());
    }

    private void PlayAudio(AudioClip clip, bool loop)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.Play();
    }

    private IEnumerator GoToTitleScene()
    {
        yield return new WaitWhile(()=> audioSource.isPlaying);
        // ‚¿‚å‚Á‚Æ‚¾‚¯—]‰C
        yield return new WaitForSeconds(1f);
        gameManager.GoToTitleScene();
    }

    private void GameClear()
    {
        if (isGameOver)
        {
            return;
        }
        isGameOver = true;
        if (gameManager.HasNextStage())
        {
            clearText.text = "STAGE CLEAR";
        }
        else
        {
            clearText.text = "GAME CLEAR";
        }
        clearText.gameObject.SetActive(true);
        timer.StopTimer();
        player.Move(Vector3.zero);
        PlayAudio(gameClearAudio, false);
        StartCoroutine(GoToNextScene());
    }

    private IEnumerator GoToNextScene()
    {
        yield return new WaitWhile(() => audioSource.isPlaying);
        gameManager.GoToNextStage();
    }

    private void StartScroll()
    {
        scrolling = true;
        player.SetScrolling(true);
        StartCoroutine(ShowGoText());
    }

    private void StopScroll()
    {
        scrolling = false;
        player.SetScrolling(false);
    }

    private IEnumerator ShowGoText()
    {
        GameObject goTxt = goText.gameObject;
        float dist = MovableArea.WIDTH / 4;
        do
        {
            goTxt.SetActive(!goTxt.activeSelf);
            yield return new WaitForSeconds(0.5f);
        } while (scrollDistance < dist);
        goTxt.SetActive(false);
    }
}
