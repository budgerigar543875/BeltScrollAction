using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EndingManager : MonoBehaviour
{
    [SerializeField] Text congratulationsText;
    [SerializeField] GameObject player;
    [SerializeField] GameObject[] enemies;

    private const int ALPHA = 192;
    private Color[] colors = {
        ConvertRGBA(255,   0,   0, ALPHA),
        ConvertRGBA(255, 165,   0, ALPHA),
        ConvertRGBA(255, 255,   0, ALPHA),
        ConvertRGBA(  0, 128,   0, ALPHA),
        ConvertRGBA(  0, 255, 255, ALPHA),
        ConvertRGBA(  0,   0, 255, ALPHA),
        ConvertRGBA(128,   0, 128, ALPHA),
    };
    private int index;
    private int flipCount;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        index = 0;
        congratulationsText.color = colors[index];
        flipCount = 0;
        player.GetComponent<Animator>().SetBool("ending", true);
        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<Animator>().SetBool("victory", true);
        }
        StartCoroutine(GoToTitleScene());
    }

    private void FixedUpdate()
    {
        index = (index + 1) % colors.Length;
        congratulationsText.color = colors[index];

        flipCount = (flipCount + 1) % 50;
        if(flipCount == 0)
        {
            foreach (GameObject enemy in enemies)
            {
                Vector3 scale = enemy.transform.localScale;
                scale.x *= -1f;
                enemy.transform.localScale = scale;
            }
        }
    }

    private IEnumerator GoToTitleScene()
    {
        yield return new WaitWhile(() => audioSource.time < 21.3f);
        audioSource.Stop();
        yield return new WaitForSeconds(2f);
        GameObject.Find("GameManager").GetComponent<GameManager>().GoToTitleScene();
    }

    private static Color ConvertRGBA(int r, int g, int b, int a)
    {
        return new Color(
            r / 255f,
            g / 255f,
            b / 255f,
            a / 255f
        );
    }
}
