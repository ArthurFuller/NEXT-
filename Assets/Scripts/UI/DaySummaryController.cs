using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DaySummaryController : MonoBehaviour
{
    [Header("UI References")]
    public Text dayTitleText;
    public Transform choicesContainer;
    public Button nextDayButton;
    public Button menuButton;

    [Header("Icons (Deprecated - não usado)")]
    public Sprite correctIcon;
    public Sprite incorrectIcon;

    [Header("Tamanhos das Imagens (Aplicado a Todas as Fichas)")]
    [Tooltip("Largura do container de cada ficha")]
    public float containerWidth = 600f;
    [Tooltip("Altura do container de cada ficha")]
    public float containerHeight = 200f;
    
    [Header("Day Summary Image (Esquerda)")]
    [Tooltip("Largura da imagem do resumo")]
    public float daySummaryWidth = 200f;
    [Tooltip("Altura da imagem do resumo")]
    public float daySummaryHeight = 200f;
    [Tooltip("Posição X da imagem do resumo")]
    public float daySummaryPosX = -150f;
    
    [Header("Result Card (Direita)")]
    [Tooltip("Largura do card de resultado")]
    public float resultCardWidth = 200f;
    [Tooltip("Altura do card de resultado")]
    public float resultCardHeight = 200f;
    [Tooltip("Posição X do card de resultado")]
    public float resultCardPosX = 150f;

    [Header("Posição Y de Cada Ficha")]
    [Tooltip("Posição Y do container da Ficha 1")]
    public float ficha1PosY = 200f;
    [Tooltip("Posição Y do container da Ficha 2")]
    public float ficha2PosY = 0f;
    [Tooltip("Posição Y do container da Ficha 3")]
    public float ficha3PosY = -200f;

    void Start()
    {
        // ADICIONAR: Som de transição ao carregar a cena
        SceneAudioController audio = FindFirstObjectByType<SceneAudioController>();
        if (audio != null)
        {
            audio.PlayDayTransition();
        }
        
        if (nextDayButton != null)
            nextDayButton.onClick.AddListener(OnNextDayClicked);

        if (menuButton != null)
            menuButton.onClick.AddListener(OnMenuClicked);

        DisplayDaySummary();
    }

    void DisplayDaySummary()
    {
        // Atualizar título do dia
        if (dayTitleText != null && GameManager.Instance != null)
        {
            int currentDay = GameManager.Instance.GetCurrentDay();
            dayTitleText.text = $"Dia {currentDay} Concluído";
        }

        // Limpar container anterior
        if (choicesContainer != null)
        {
            foreach (Transform child in choicesContainer)
            {
                Destroy(child.gameObject);
            }
        }

        // Mostrar escolhas do dia
        if (DayResults.Instance != null)
        {
            int choiceIndex = 0;
            foreach (DayChoice choice in DayResults.Instance.currentDayChoices)
            {
                CreateChoiceItem(choice, choiceIndex);
                choiceIndex++;
            }
        }
    }

    void CreateChoiceItem(DayChoice choice, int index)
    {
        if (choicesContainer == null)
        {
            return;
        }

        // Selecionar posição Y baseada no índice
        float containerPosY = ficha1PosY;
        string fichaName = "Ficha 1";
        
        if (index == 1)
        {
            containerPosY = ficha2PosY;
            fichaName = "Ficha 2";
        }
        else if (index == 2)
        {
            containerPosY = ficha3PosY;
            fichaName = "Ficha 3";
        }

        // Criar GameObject simples diretamente no código
        GameObject choiceItem = new GameObject($"ChoiceItem_{fichaName}");
        choiceItem.transform.SetParent(choicesContainer, false);

        // Adicionar RectTransform
        RectTransform rectTransform = choiceItem.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(containerWidth, containerHeight);
        rectTransform.anchoredPosition = new Vector2(0, containerPosY);

        // SEM Layout Group - posicionamento manual

        // ===== IMAGEM DO DAY SUMMARY =====
        GameObject daySummaryImageObj = new GameObject("DaySummaryImage");
        daySummaryImageObj.transform.SetParent(choiceItem.transform, false);

        Image daySummaryImage = daySummaryImageObj.AddComponent<Image>();
        RectTransform daySummaryRect = daySummaryImageObj.GetComponent<RectTransform>();
        daySummaryRect.sizeDelta = new Vector2(daySummaryWidth, daySummaryHeight);
        daySummaryRect.anchoredPosition = new Vector2(daySummaryPosX, 0);

        // Definir sprite do Day Summary
        if (choice.cardData != null && choice.cardData.daySummarySprite != null)
        {
            daySummaryImage.sprite = choice.cardData.daySummarySprite;
            daySummaryImage.color = Color.white;
        }
        else
        {
            daySummaryImage.sprite = null;
            daySummaryImage.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }

        // ===== CARD DE RESULTADO (CORRETO OU INCORRETO) =====
        GameObject resultCardObj = new GameObject("ResultCard");
        resultCardObj.transform.SetParent(choiceItem.transform, false);

        Image resultCardImage = resultCardObj.AddComponent<Image>();
        RectTransform resultCardRect = resultCardObj.GetComponent<RectTransform>();
        resultCardRect.sizeDelta = new Vector2(resultCardWidth, resultCardHeight);
        resultCardRect.anchoredPosition = new Vector2(resultCardPosX, 0);

        // Definir sprite do card de resultado baseado se foi correto ou incorreto
        if (choice.cardData != null && choice.chosenRobotIndex >= 0)
        {
            // Escolher card correto ou incorreto baseado no resultado
            Sprite resultSprite = choice.isCorrect ? 
                choice.cardData.correctChoiceCardSprite : 
                choice.cardData.incorrectChoiceCardSprite;

            if (resultSprite != null)
            {
                resultCardImage.sprite = resultSprite;
                resultCardImage.color = Color.white;
                string resultType = choice.isCorrect ? "CORRETO" : "INCORRETO";
            }
            else
            {
                resultCardImage.sprite = null;
                resultCardImage.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                string missingSprite = choice.isCorrect ? "correctChoiceCardSprite" : "incorrectChoiceCardSprite";
            }
        }
        else if (choice.chosenRobotIndex == -1)
        {
            // Caso "Não Ajudar" - mostrar card incorreto (não ajudou = errado)
            if (choice.cardData != null && choice.cardData.incorrectChoiceCardSprite != null)
            {
                resultCardImage.sprite = choice.cardData.incorrectChoiceCardSprite;
                resultCardImage.color = Color.white;
            }
            else
            {
                resultCardImage.sprite = null;
                resultCardImage.color = new Color(0.3f, 0.3f, 0.3f, 0.5f);
            }
        }
        else
        {
            resultCardImage.sprite = null;
            resultCardImage.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }

        // Forçar ativação de tudo
        choiceItem.SetActive(true);
        daySummaryImageObj.SetActive(true);
        resultCardObj.SetActive(true);
    }

    void OnNextDayClicked()
    {
        if (GameManager.Instance != null)
        {
            int currentDay = GameManager.Instance.GetCurrentDay();
            int totalDays = GameManager.Instance.totalDays;
            
            // Se for o último dia, ir para GameEnd
            if (currentDay >= totalDays)
            {
                GameManager.Instance.EndGame();
            }
            else
            {
                GameManager.Instance.ContinueToNextDay();
            }
        }
    }

    void OnMenuClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToMainMenu();
        }
    }
}
