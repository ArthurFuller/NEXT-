using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GamePauseManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuPanel;
    public Image backgroundPanel;
    public RectTransform menuPanel;
    public Button continueButton;
    public Button quitButton;
    public Button pauseButton;

    [Header("Animation Settings")]
    public float animationDuration = 0.3f;
    public bool useFadeEffect = true;

    private bool isPaused = false;
    private Coroutine currentAnimation;

    void Start()
    {
        // Configurar listeners
        if (pauseButton != null)
            pauseButton.onClick.AddListener(TogglePause);

        if (continueButton != null)
            continueButton.onClick.AddListener(ResumeGame);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitToTitle);

        // Estado inicial
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        if (backgroundPanel != null)
        {
            Color bgColor = backgroundPanel.color;
            bgColor.a = 0;
            backgroundPanel.color = bgColor;
            backgroundPanel.raycastTarget = false; // Desabilitar raycast quando invisível
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);

        if (useFadeEffect)
        {
            if (currentAnimation != null)
                StopCoroutine(currentAnimation);
            currentAnimation = StartCoroutine(AnimateMenuIn());
        }
        else
        {
            // Sem efeitos - mostrar imediatamente
            SetMenuVisible(true);
        }
    }

    void ResumeGame()
    {
        if (useFadeEffect)
        {
            if (currentAnimation != null)
                StopCoroutine(currentAnimation);
            currentAnimation = StartCoroutine(AnimateMenuOut());
        }
        else
        {
            // Sem efeitos - esconder imediatamente
            CompleteResume();
        }
    }

    void CompleteResume()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        // Reabilitar raycast no background quando o menu for fechado
        if (backgroundPanel != null)
            backgroundPanel.raycastTarget = false;
    }

    void QuitToTitle()
    {
        // Retomar tempo antes de sair
        Time.timeScale = 1f;

        // Usar FadeController se disponível
        FadeController fadeController = FindFirstObjectByType<FadeController>();
        if (fadeController != null)
        {
            StartCoroutine(QuitWithFade(fadeController));
        }
        else
        {
            SceneManager.LoadScene("TitleScreen");
        }


    }

    IEnumerator QuitWithFade(FadeController fadeController)
    {
        yield return StartCoroutine(fadeController.FadeOut());
        SceneManager.LoadScene("TitleScreen");
    }

    IEnumerator AnimateMenuIn()
    {
        float elapsed = 0f;

        // Estado inicial
        if (backgroundPanel != null)
        {
            Color bgColor = backgroundPanel.color;
            bgColor.a = 0;
            backgroundPanel.color = bgColor;
        }

        if (menuPanel != null)
        {
            menuPanel.localScale = Vector3.one * 0.8f;
            CanvasGroup menuCanvas = menuPanel.GetComponent<CanvasGroup>();
            if (menuCanvas != null)
                menuCanvas.alpha = 0;
        }

        while (elapsed < animationDuration)
        {
            elapsed += Time.unscaledDeltaTime; // Usar unscaled para funcionar durante pause
            float t = elapsed / animationDuration;
            t = Mathf.SmoothStep(0, 1, t); // Easing suave

            // Fade background
            if (backgroundPanel != null)
            {
                Color bgColor = backgroundPanel.color;
                bgColor.a = Mathf.Lerp(0, 0.7f, t);
                backgroundPanel.color = bgColor;
            }

            // Scale e fade menu
            if (menuPanel != null)
            {
                menuPanel.localScale = Vector3.Lerp(Vector3.one * 0.8f, Vector3.one, t);

                CanvasGroup menuCanvas = menuPanel.GetComponent<CanvasGroup>();
                if (menuCanvas != null)
                    menuCanvas.alpha = Mathf.Lerp(0, 1, t);
            }

            yield return null;
        }

        // Garantir estado final
        SetMenuVisible(true);
    }

    IEnumerator AnimateMenuOut()
    {
        float elapsed = 0f;

        while (elapsed < animationDuration * 0.7f) // Mais rápido para fechar
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / (animationDuration * 0.7f);
            t = Mathf.SmoothStep(0, 1, t);

            // Fade background
            if (backgroundPanel != null)
            {
                Color bgColor = backgroundPanel.color;
                bgColor.a = Mathf.Lerp(0.7f, 0, t);
                backgroundPanel.color = bgColor;
            }

            // Scale e fade menu
            if (menuPanel != null)
            {
                menuPanel.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.9f, t);

                CanvasGroup menuCanvas = menuPanel.GetComponent<CanvasGroup>();
                if (menuCanvas != null)
                    menuCanvas.alpha = Mathf.Lerp(1, 0, t);
            }

            yield return null;
        }

        // Completar resume
        CompleteResume();
    }

    void SetMenuVisible(bool visible)
    {
        if (backgroundPanel != null)
        {
            Color bgColor = backgroundPanel.color;
            bgColor.a = visible ? 0.7f : 0;
            backgroundPanel.color = bgColor;
            backgroundPanel.raycastTarget = visible; // Habilitar/desabilitar raycast conforme visibilidade
        }

        if (menuPanel != null)
        {
            menuPanel.localScale = visible ? Vector3.one : Vector3.one * 0.8f;

            CanvasGroup menuCanvas = menuPanel.GetComponent<CanvasGroup>();
            if (menuCanvas != null)
            {
                menuCanvas.alpha = visible ? 1 : 0;
                menuCanvas.interactable = visible; // Habilitar/desabilitar interação
                menuCanvas.blocksRaycasts = visible; // Bloquear/desbloquear raycasts
            }
        }
    }
}
