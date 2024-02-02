using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TitleTextManager : MonoBehaviour
{
    [SerializeField] Text[] titleTexts;
    [SerializeField] float initialInterval;
    [SerializeField] float displayInterval;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private IEnumerator Start()
    {
        foreach (Text text in titleTexts)
        {
            TitleText titleText = text.gameObject.GetComponent<TitleText>();
            titleText.UpdateCompleted += PlayAudio;
            text.gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(initialInterval);
        foreach (Text text in titleTexts)
        {
            text.gameObject.SetActive(true);
            yield return new WaitForSeconds(displayInterval);
        }
    }

    private void PlayAudio() { 
        audioSource.Stop();
        audioSource.Play();
    }
}
