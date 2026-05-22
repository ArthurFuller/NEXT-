using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleScreenController : MonoBehaviour
{
    [Header("UI Buttons")]
    public Button startGameButton;
    public Button controlsButton;
    public Button quitButton;

    [Header("Transition Settings")]
    [Tooltip("Delay antes de trocar de cena (para tocar som)")]
    public float transitionDelay = 0.5f;

    void Start()
    {
        // Verificar e configurar listeners dos botões
        if (startGameButton != null)
        {
            startGameButton.onClick.AddListener(StartGame);
        }

        if (controlsButton != null)
        {
            controlsButton.onClick.AddListener(ShowControls);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
    }

    void StartGame()
    {
        // Desabilitar botão para evitar cliques múltiplos
        if (startGameButton != null)
            startGameButton.interactable = false;
        
        // Iniciar transição com fade
        StartCoroutine(StartGameWithFade());
    }

    IEnumerator StartGameWithFade()
    {
        // Buscar FadeController na cena
        FadeController fadeController = FindFirstObjectByType<FadeController>();
        if (fadeController != null)
        {
            // Fade out começa imediatamente
            yield return StartCoroutine(fadeController.FadeOut());
        }
        else
        {
            // Se não houver FadeController, aguardar o delay para o som tocar
            yield return new WaitForSeconds(transitionDelay);
        }
        
        // Carregar cena
        SceneManager.LoadScene("Gameplay");
    }

    void ShowControls()
    {
        // TODO: Implementar tela de controles
        // SceneManager.LoadScene("Controls");
    }

    void QuitGame()
    {
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
