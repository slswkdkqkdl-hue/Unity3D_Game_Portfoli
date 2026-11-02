using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
    [SerializeField] private Image fadeImage;

    public float fadeTime = 1f; 

    void Awake()
    {
        fadeImage.gameObject.SetActive(false);
    }

    public void DoFadeOut(Action onComplete = null)
    {   
        float time = fadeTime;
        StartCoroutine(CoFadeOut(time, onComplete));
    }

    private IEnumerator CoFadeOut(float duration, Action onComplete)
    {   
        fadeImage.gameObject.SetActive(true);
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        fadeImage.color = Color.black;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / duration); 
            fadeImage.color = color;
            yield return null;
        }

        color.a = 1f;
        fadeImage.color = color;

        onComplete?.Invoke();
    }
}