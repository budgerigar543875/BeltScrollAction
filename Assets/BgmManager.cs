using System.Collections;
using UnityEngine;

public class BgmManager : MonoBehaviour
{
    [SerializeField] AudioClip intro;
    [SerializeField] AudioClip loop;

    private AudioSource audioSource;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.clip = intro;
        audioSource.Play();
        yield return new WaitWhile(()  => audioSource.isPlaying);
        audioSource.Stop();
        audioSource.clip = loop;
        audioSource.loop = true;
        audioSource.Play();
    }
}
