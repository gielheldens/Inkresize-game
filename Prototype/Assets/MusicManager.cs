using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header ("References")]
    [SerializeField] private PlayerController playerController;

    [Header ("Attributes")]
    [SerializeField] private float fadeDuration;
    
    [Header ("Audio")]
    [SerializeField] private AudioSource audioSource;

    // Update is called once per frame
    void Update()
    {
        if (playerController.won)
        {
            FadeOut();
        }
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    private System.Collections.IEnumerator FadeOutCoroutine()
    {
        float startVolume = audioSource.volume;

        float currentTime = 0.0f;

        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0.0f, currentTime / fadeDuration);
            yield return null;
        }

        // Ensure the volume is set to 0 after the fade-out
        audioSource.volume = 0.0f;
    }
}
