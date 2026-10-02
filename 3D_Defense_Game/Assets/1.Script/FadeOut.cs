using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
    [SerializeField] private Image fadeImage;

    public float fadeTime = 1f; // 기본값 지정 권장

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
            color.a = Mathf.Clamp01(elapsedTime / duration); // 0f -> 1f
            fadeImage.color = color;
            yield return null;
        }

        color.a = 1f;
        fadeImage.color = color;

        // 페이드아웃 완료 후 등록된 콜백 실행
        onComplete?.Invoke();
    }
}