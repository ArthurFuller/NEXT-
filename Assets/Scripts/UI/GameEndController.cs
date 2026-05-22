using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameEndController : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Botão para voltar ao menu principal")]
    public Button returnToMenuButton;
    
    [Header("Background Image (Optional)")]
    [Tooltip("Imagem de fundo da tela de fim de jogo")]
    public Image backgroundImage;
    
    [Tooltip("Sprite da imagem de fundo")]
    public Sprite backgroundSprite;
    
    void Start()
    {
        // Configurar botão
        if (returnToMenuButton != null)
        {
            returnToMenuButton.onClick.AddListener(OnReturnToMenuClicked);
        }
        
        // Configurar imagem de fundo se fornecida
        if (backgroundImage != null && backgroundSprite != null)
        {
            backgroundImage.sprite = backgroundSprite;
        }
        
        // Iniciar fade in
        StartCoroutine(FadeInOnStart());
    }
    
    /// <summary>
    /// Fade in ao iniciar a cena
    /// </summary>
    IEnumerator FadeInOnStart()
    {
        FadeController fadeController = FindFirstObjectByType<FadeController>();
        if (fadeController != null)
        {
            yield return StartCoroutine(fadeController.FadeIn());
        }
    }
    
    /// <summary>
    /// Chamado quando o botão de retornar ao menu é clicado
    /// </summary>
    void OnReturnToMenuClicked()
    {
        
        // Desabilitar botão para evitar cliques múltiplos
        if (returnToMenuButton != null)
        {
            returnToMenuButton.interactable = false;
        }
        
        StartCoroutine(ReturnToMenuWithFade());
    }
    
    /// <summary>
    /// Retorna ao menu principal com fade out
    /// </summary>
    IEnumerator ReturnToMenuWithFade()
    {
        // Fade out
        FadeController fadeController = FindFirstObjectByType<FadeController>();
        if (fadeController != null)
        {
            yield return StartCoroutine(fadeController.FadeOut());
        }
        else
        {
            yield return new WaitForSeconds(0.5f); // Pequeno delay se não houver fade
        }
        
        // Limpar dados do jogo
        if (DayResults.Instance != null)
        {
            DayResults.Instance.ClearResults();
        }
        
        // Retornar ao menu via GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToMainMenu();
        }
        else
        {
            // Fallback: Carregar TitleScreen diretamente
            SceneManager.LoadScene("TitleScreen");
        }
    }
    
    void OnDestroy()
    {
        // Remover listener do botão
        if (returnToMenuButton != null)
        {
            returnToMenuButton.onClick.RemoveListener(OnReturnToMenuClicked);
        }
    }
}
