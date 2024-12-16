using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour
{
    public AudioClip[] audioClips;
    private AudioSource audioSource;
    public Transform player;
    public float maxVolume = 1.0f;
    public float maxDistance = 10.0f;
    public float minDelay = 2.0f;
    public float maxDelay = 5.0f;

    private bool isPlaying = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            float volume = Mathf.Clamp01(1 - (distance / maxDistance)) * maxVolume;
            audioSource.volume = volume;
        }

        if (!audioSource.isPlaying && !isPlaying)
        {
            StartCoroutine(PlayRandomClipWithDelay());
        }
    }

    IEnumerator PlayRandomClipWithDelay()
    {
        isPlaying = true;
        float delay = Random.Range(minDelay, maxDelay);
        yield return new WaitForSeconds(delay);

        if (audioClips.Length > 0)
        {
            int randomIndex = Random.Range(0, audioClips.Length);
            audioSource.clip = audioClips[randomIndex];
            audioSource.Play();
        }

        isPlaying = false;
    }
}
