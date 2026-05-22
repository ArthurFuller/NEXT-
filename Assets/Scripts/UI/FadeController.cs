using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeController : MonoBehaviour
{
    [Header("Fade Settings")]
    public Image fadeImage;
    public float fadeDuration = 1f;

    void Awake()
    {
        // Garantir que começa transparente
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }
    }

    void Start()
    {
        // Fade in automático ao carregar a cena
        StartCoroutine(FadeIn());
    }
    
    public IEnumerator FadeOut()
    {
        if (fadeImage == null)
            yield break;
        
        float elapsed = 0f;
        Color c = fadeImage.color;
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }
        
        c.a = 1f;
        fadeImage.color = c;
    }
    
    public IEnumerator FadeIn()
    {
        if (fadeImage == null)
            yield break;
        
        float elapsed = 0f;
        Color c = fadeImage.color;
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }
        
        c.a = 0f;
        fadeImage.color = c;
    }
}
