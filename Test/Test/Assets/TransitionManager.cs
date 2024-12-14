using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1f; 

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    public void LoadSceneWithFade(string sceneName)
    {
        StartCoroutine(FadeOut(sceneName));
    }

    private IEnumerator FadeIn()
    {
        float timer = 0f;
        Color color = fadeImage.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = 1f - (timer / fadeDuration); 
            fadeImage.color = color;
            yield return null;
        }

        color.a = 0f; 
        fadeImage.color = color;
    }

    private IEnumerator FadeOut(string sceneName)
    {
        float timer = 0f;
        Color color = fadeImage.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = timer / fadeDuration; 
            fadeImage.color = color;
            yield return null;
        }

        color.a = 1f; 
        fadeImage.color = color;

        SceneManager.LoadScene(sceneName);
    }
}
