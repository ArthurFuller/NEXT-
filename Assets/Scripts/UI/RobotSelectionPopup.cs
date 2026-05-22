using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RobotSelectionPopup : MonoBehaviour
{
    [Header("Popup References")]
    public GameObject popupContainer;
    public RectTransform popupPanel;

    [Header("New Confirmation UI")]
    public Image robotCardImage;
    public Button confirmButton;
    public Button cancelButton;

    [Header("Legacy References (to be removed)")]
    public Image robot1Image;
    public Image robot2Image;
    public Button closeButton;
    
    private int selectedRobotIndex = -1;
    private EmployeeCard currentCard;
    private GameObject blockerPanel;
    
    void Start()
    {
        CreateBlockerPanel();
        
        if (robot1Image != null)
        {
            Button btn = robot1Image.GetComponent<Button>();
            if (btn != null)
                btn.onClick.AddListener(() => OnRobotSelected(0));
        }
        
        if (robot2Image != null)
        {
            Button btn = robot2Image.GetComponent<Button>();
            if (btn != null)
                btn.onClick.AddListener(() => OnRobotSelected(1));
        }
        
        if (closeButton != null)
            closeButton.onClick.AddListener(HidePopup);
        
        if (popupContainer != null)
            popupContainer.SetActive(false);
    }
    
    void CreateBlockerPanel()
    {
        if (popupContainer == null) return;
        
        // Criar painel bloqueador
        blockerPanel = new GameObject("BlockerPanel");
        blockerPanel.transform.SetParent(popupContainer.transform, false);
        
        // Adicionar RectTransform
        RectTransform blockerRect = blockerPanel.AddComponent<RectTransform>();
        blockerRect.anchorMin = Vector2.zero;
        blockerRect.anchorMax = Vector2.one;
        blockerRect.sizeDelta = Vector2.zero;
        blockerRect.anchoredPosition = Vector2.zero;
        
        // Adicionar Image para bloquear raycasts
        Image blockerImage = blockerPanel.AddComponent<Image>();
        blockerImage.color = new Color(0, 0, 0, 0.5f); // Preto semi-transparente
        blockerImage.raycastTarget = true;
        
        // Mover para o início da hierarquia (atrás do popup)
        blockerPanel.transform.SetAsFirstSibling();
    }
    
    public void ShowPopup()
    {
        // ADICIONAR: Som de popup abrindo
        SceneAudioController audio = FindFirstObjectByType<SceneAudioController>();
        if (audio != null)
        {
            audio.PlayPopupOpen();
        }
        
        CardSelector cardSelector = FindFirstObjectByType<CardSelector>();
        if (cardSelector != null)
        {
            currentCard = cardSelector.GetCurrentCard();
            if (currentCard != null)
            {
                SetupRobotImages(currentCard);
            }
        }

        if (popupContainer != null)
        {
            popupContainer.SetActive(true);
            StartCoroutine(AnimatePopupOpen());
        }
    }

    public void ShowConfirmationPopup(EmployeeCard card, int robotIndex)
    {
        // ADICIONAR: Som de popup abrindo
        SceneAudioController audio = FindFirstObjectByType<SceneAudioController>();
        if (audio != null)
        {
            audio.PlayPopupOpen();
        }
        
        currentCard = card;
        selectedRobotIndex = robotIndex;

        // Setup new confirmation UI
        SetupConfirmationUI(card, robotIndex);

        if (popupContainer != null)
        {
            popupContainer.SetActive(true);
            StartCoroutine(AnimatePopupOpen());
        }
    }
    
    void SetupRobotImages(EmployeeCard card)
    {
        if (robot1Image != null)
            robot1Image.sprite = card.robot1Sprite;

        if (robot2Image != null)
            robot2Image.sprite = card.robot2Sprite;
    }

    void SetupConfirmationUI(EmployeeCard card, int robotIndex)
    {
        // Hide legacy UI elements
        if (robot1Image != null) robot1Image.gameObject.SetActive(false);
        if (robot2Image != null) robot2Image.gameObject.SetActive(false);
        if (closeButton != null) closeButton.gameObject.SetActive(false);

        // Show new confirmation UI
        if (robotCardImage != null)
        {
            // Set the robot card sprite based on selected robot (use card sprites, not overlay sprites)
            Sprite robotCardSprite = (robotIndex == 0) ? card.robot1CardSprite : card.robot2CardSprite;
            if (robotCardSprite != null)
            {
                robotCardImage.sprite = robotCardSprite;
                robotCardImage.gameObject.SetActive(true);
            }
            else
            {
                // Fallback to overlay sprites if card sprites are not set
                Sprite fallbackSprite = (robotIndex == 0) ? card.robot1Sprite : card.robot2Sprite;
                if (fallbackSprite != null)
                {
                    robotCardImage.sprite = fallbackSprite;
                    robotCardImage.gameObject.SetActive(true);
                }
            }
        }

        // Setup button listeners
        if (confirmButton != null)
        {
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() => OnConfirmButtonClicked());
            confirmButton.gameObject.SetActive(true);
        }

        if (cancelButton != null)
        {
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(() => OnCancelButtonClicked());
            cancelButton.gameObject.SetActive(true);
        }
    }

    void OnConfirmButtonClicked()
    {
        if (currentCard != null && selectedRobotIndex >= 0)
        {
            bool isCorrect = currentCard.IsCorrectRobot(selectedRobotIndex);
            ProcessRobotChoice(currentCard, isCorrect);
        }

        HidePopup();
    }

    void OnCancelButtonClicked()
    {
        HidePopup();
    }
    
    public void HidePopup()
    {
        // ADICIONAR: Som de popup fechando
        SceneAudioController audio = FindFirstObjectByType<SceneAudioController>();
        if (audio != null)
        {
            audio.PlayPopupClose();
        }
        
        StartCoroutine(AnimatePopupClose());
        
        // Reabilitar botões ao fechar popup pelo botão X
        ButtonController buttonController = FindFirstObjectByType<ButtonController>();
        if (buttonController != null)
        {
            buttonController.EnableButtons();
        }
    }
    
    void OnRobotSelected(int robotIndex)
    {
        selectedRobotIndex = robotIndex;
        
        if (currentCard != null)
        {
            bool isCorrect = currentCard.IsCorrectRobot(robotIndex);
            ProcessRobotChoice(currentCard, isCorrect);
        }
        
        HidePopup();
    }
    
    void ProcessRobotChoice(EmployeeCard card, bool isCorrect)
    {
        card.MarkAsProcessed();

        // Registrar escolha nos resultados do dia
        if (DayResults.Instance != null && card.employeeCardSprite != null)
        {
            // Encontrar o EmployeeCardData correspondente
            DayManager dayManager = FindFirstObjectByType<DayManager>();
            if (dayManager != null)
            {
                foreach (EmployeeCardData cardData in dayManager.allAvailableCards)
                {
                    if (cardData != null && cardData.employeeCardSprite == card.employeeCardSprite)
                    {
                        string feedbackText = GenerateFeedbackText(cardData, selectedRobotIndex, isCorrect);
                        DayResults.Instance.AddChoice(cardData, selectedRobotIndex, isCorrect, feedbackText);
                        
                        // ADICIONAR à lista de fichas usadas no GameManager
                        if (GameManager.Instance != null)
                        {
                            GameManager.Instance.AddUsedCard(cardData);
                        }
                        
                        break;
                    }
                }
            }
        }

        CanvasGroup canvasGroup = card.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0.5f;
            canvasGroup.interactable = false;
        }

        Image cardImage = card.GetComponent<Image>();
        if (cardImage != null)
        {
            Color grayColor = new Color(0.7f, 0.7f, 0.7f, 1f);
            cardImage.color = grayColor;
        }

        // Marcar no DayManager
        DayManager dayManagerInstance = FindFirstObjectByType<DayManager>();
        if (dayManagerInstance != null)
        {
            dayManagerInstance.MarkEmployeeAsProcessed();

            // Verificar se todas as fichas foram processadas
            if (dayManagerInstance.IsAllEmployeesProcessed())
            {
                StartCoroutine(EndDayAfterDelay(3f));
            }
        }

        CardSelector cardSelector = FindFirstObjectByType<CardSelector>();
        if (cardSelector != null)
        {
            StartCoroutine(ClearSelectionAfterDelay(cardSelector, 1f));
        }
    }
    
    IEnumerator AnimatePopupOpen()
    {
        if (popupPanel != null)
        {
            popupPanel.localScale = Vector3.zero;
            
            float duration = 0.3f;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;
                popupPanel.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, progress);
                yield return null;
            }
            
            popupPanel.localScale = Vector3.one;
        }
    }
    
    IEnumerator AnimatePopupClose()
    {
        if (popupPanel != null)
        {
            float duration = 0.2f;
            float elapsed = 0f;
            Vector3 startScale = popupPanel.localScale;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;
                popupPanel.localScale = Vector3.Lerp(startScale, Vector3.zero, progress);
                yield return null;
            }
            
            popupPanel.localScale = Vector3.zero;
        }
        
        if (popupContainer != null)
            popupContainer.SetActive(false);
    }
    
    IEnumerator ClearSelectionAfterDelay(CardSelector cardSelector, float delay)
    {
        yield return new WaitForSeconds(delay);
        cardSelector.ClearSelection();
    }
    
    string GenerateFeedbackText(EmployeeCardData cardData, int chosenRobotIndex, bool isCorrect)
    {
        if (isCorrect)
        {
            // Textos positivos para escolhas corretas
            string[] positiveTexts = new string[]
            {
                "Excelente escolha! Este robô é perfeito para resolver o problema do funcionário.",
                "Muito bem! Você identificou corretamente o robô mais adequado.",
                "Parabéns! Sua decisão ajudará o funcionário de forma eficiente.",
                "Ótima análise! Este robô otimizará o trabalho do funcionário."
            };
            return positiveTexts[Random.Range(0, positiveTexts.Length)];
        }
        else
        {
            // Usar mensagens específicas do robô errado escolhido
            if (chosenRobotIndex == 0 && !string.IsNullOrEmpty(cardData.robot1Feedback))
            {
                return cardData.robot1Feedback;
            }
            else if (chosenRobotIndex == 1 && !string.IsNullOrEmpty(cardData.robot2Feedback))
            {
                return cardData.robot2Feedback;
            }
            else
            {
                // Fallback para mensagens genéricas se não houver mensagem específica
                string[] negativeTexts = new string[]
                {
                    "Esta escolha pode não resolver completamente o problema. Considere as necessidades específicas do funcionário.",
                    "O robô selecionado pode não ser o mais adequado. Avalie melhor as características do problema.",
                    "Há uma opção melhor disponível. Pense nas habilidades específicas requeridas.",
                    "Esta escolha pode causar ineficiências. O outro robô seria mais apropriado."
                };
                return negativeTexts[Random.Range(0, negativeTexts.Length)];
            }
        }
    }

    IEnumerator EndDayAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Chamar GameManager para terminar o dia
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TriggerEndDay();
        }
    }
}
