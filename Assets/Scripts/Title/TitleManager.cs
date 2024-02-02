using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject boss;
    [SerializeField] GameObject[] enemies;
    [SerializeField] Button gameStartButton;

    private IEnumerator Start()
    {
        gameStartButton.onClick.AddListener(GameStart);
        yield return new WaitForSeconds(0.5f);
        boss.GetComponent<Animator>().GetComponent<Animator>().SetBool("victory", true);
        yield return new WaitForSeconds(1f);
        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<Animator>().SetBool("victory", true);
        }
        yield return new WaitForSeconds(1f);
        player.GetComponent<Animator>().SetBool("title", true);
    }

    public void GameStart()
    {
        // ˜A‘Å–hŽ~
        gameStartButton.enabled = false;
        StartCoroutine(GameStartImpl());
    }

    private IEnumerator GameStartImpl()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        yield return new WaitWhile(() => audioSource.isPlaying);
        GameObject.Find("GameManager").GetComponent<GameManager>().GameStart();
    }
}
