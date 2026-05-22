using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("Game Settings")]
    public int totalDays = 3;
    
    [Header("References")]
    public DayManager dayManager;

    private int currentDay = 1;
    private GameState currentGameState = GameState.MainMenu;
    
    // Lista de fichas usadas durante todo o jogo (persiste entre cenas)
    private List<EmployeeCardData> usedCardsThisGame = new List<EmployeeCardData>();
    
    public enum GameState
    {
        MainMenu,
        DayStart,
        Playing,
        DayEnd,
        GameOver
    }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
        // Se voltou para TitleScreen, resetar tudo
        if (scene.name == "TitleScreen")
        {
            currentDay = 1;
            usedCardsThisGame.Clear();
        }
        
        if (scene.name == "Gameplay")
        {
            // Se é o dia 1, resetar lista de fichas usadas
            if (currentDay == 1)
            {
                usedCardsThisGame.Clear();
            }
            StartCoroutine(InitializeGameplayScene());
        }
    }
    
    IEnumerator InitializeGameplayScene()
    {
        // Aguardar 1 frame para garantir que tudo foi carregado
        yield return null;        
        
        // Buscar DayManager na cena
        DayManager sceneDayManager = FindFirstObjectByType<DayManager>();
        
        if (sceneDayManager != null)
        {
            dayManager = sceneDayManager;
            
            // Inicializar o dia
            StartNewDay(currentDay);
            
            // Atualizar contador de dias se existir
            UpdateDayCounter();
        }
    }
    
    void UpdateDayCounter()
    {
        // Buscar texto do contador de dias
        TextMeshProUGUI[] allTexts = FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);
        
        foreach (TextMeshProUGUI text in allTexts)
        {
            // Procurar por texto que contenha "Dia" ou esteja vazio
            if (text.text.Contains("Dia") || text.text.Contains("/") || string.IsNullOrEmpty(text.text))
            {
                text.text = $"Dia {currentDay}/3";
                break;
            }
        }
    }
    
    public void StartGame()
    {
        currentDay = 1;
        currentGameState = GameState.DayStart;
        
        // Resetar fichas usadas ao iniciar novo jogo
        usedCardsThisGame.Clear();
        
        SceneManager.LoadScene("Gameplay");
    }
    
    public void StartNewDay(int dayNumber)
    {
        
        currentDay = dayNumber;
        currentGameState = GameState.Playing;       
        
        if (dayManager != null)
        {
            dayManager.StartNewDay();
        }
    }
    
    // Método público para outros scripts chamarem
    public void TriggerEndDay()
    {
        StartCoroutine(EndDayWithFade());
    }
    
    public void EndDay()
    {
        currentGameState = GameState.DayEnd;
        StartCoroutine(EndDayWithFade());
    }
    
    IEnumerator EndDayWithFade()
    {        
        // Buscar FadeController local na cena atual
        FadeController fadeController = FindFirstObjectByType<FadeController>();
        if (fadeController != null)
        {
            yield return StartCoroutine(fadeController.FadeOut());
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        // SEMPRE mostrar DaySummary após completar um dia
        SceneManager.LoadScene("DaySummary");
    }
    
    public void EndGame()
    {
        currentGameState = GameState.GameOver;
        StartCoroutine(EndGameWithFade());
    }
    
    IEnumerator EndGameWithFade()
    {
        // Buscar FadeController local na cena atual
        FadeController fadeController = FindFirstObjectByType<FadeController>();
        if (fadeController != null)
        {
            yield return StartCoroutine(fadeController.FadeOut());
        }
        
        SceneManager.LoadScene("GameEnd");
        // FadeIn será automático na nova cena
    }
    
    public void ContinueToNextDay()
    {       
        // Limpar resultados do dia anterior antes de continuar
        if (DayResults.Instance != null)
        {
            DayResults.Instance.ClearResults();
        }

        currentDay++;
        
        StartCoroutine(ContinueToNextDayWithFade());
    }
    
    IEnumerator ContinueToNextDayWithFade()
    {       
        // Buscar FadeController local na cena atual
        FadeController fadeController = FindFirstObjectByType<FadeController>();
        if (fadeController != null)
        {
            yield return StartCoroutine(fadeController.FadeOut());
        }
        else
        {
            yield return new WaitForSeconds(0.5f); // Pequeno delay mesmo sem fade
        }
        
        SceneManager.LoadScene("Gameplay");
        // FadeIn será automático na nova cena
    }
    
    public void ReturnToMainMenu()
    {
        currentDay = 1;
        currentGameState = GameState.MainMenu;
        
        // Resetar fichas usadas ao voltar ao menu
        usedCardsThisGame.Clear();
        
        SceneManager.LoadScene("TitleScreen");
    }
    
    // Métodos para gerenciar fichas usadas (chamados pelo DayManager)
    public List<EmployeeCardData> GetUsedCards()
    {
        return usedCardsThisGame;
    }
    
    public void AddUsedCard(EmployeeCardData card)
    {
        if (card != null && !usedCardsThisGame.Contains(card))
        {
            usedCardsThisGame.Add(card);
        }
    }
    
    public int GetCurrentDay()
    {
        return currentDay;
    }
    
    public GameState GetGameState()
    {
        return currentGameState;
    }
}
