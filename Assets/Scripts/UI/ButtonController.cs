using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonController : MonoBehaviour
{
    [Header("Button References")]
    public Button helpButton;
    public Button dontHelpButton;
    
    private bool buttonsEnabled = false;
    
    void Start()
    {
        if (helpButton != null)
            helpButton.onClick.AddListener(OnHelpButtonClicked);
        
        if (dontHelpButton != null)
            dontHelpButton.onClick.AddListener(OnDontHelpButtonClicked);
        
        DisableButtons();
    }
    
    public void EnableButtons()
    {
        buttonsEnabled = true;
        
        if (helpButton != null)
            helpButton.interactable = true;
        
        if (dontHelpButton != null)
            dontHelpButton.interactable = true;
    }
    
    public void DisableButtons()
    {
        buttonsEnabled = false;
        
        if (helpButton != null)
            helpButton.interactable = false;
        
        if (dontHelpButton != null)
            dontHelpButton.interactable = false;
    }
    
    void OnHelpButtonClicked()
    {
        if (!buttonsEnabled)
            return;
        
        DisableButtons();
        
        RobotSelectionPopup popup = FindFirstObjectByType<RobotSelectionPopup>();
        if (popup != null)
            popup.ShowPopup();
    }
    
    void OnDontHelpButtonClicked()
    {
        if (!buttonsEnabled)
            return;

        DisableButtons();

        CardSelector cardSelector = FindFirstObjectByType<CardSelector>();
        if (cardSelector != null)
        {
            EmployeeCard currentCard = cardSelector.GetCurrentCard();
            if (currentCard != null)
            {
                currentCard.MarkAsProcessed();
                UpdateCardVisual(currentCard);

                // Registrar escolha "Não ajudar" nos resultados do dia
                if (DayResults.Instance != null && currentCard.employeeCardSprite != null)
                {
                    // Encontrar o EmployeeCardData correspondente
                    DayManager dayManager = FindFirstObjectByType<DayManager>();
                    if (dayManager != null)
                    {
                        foreach (EmployeeCardData cardData in dayManager.allAvailableCards)
                        {
                            if (cardData != null && cardData.employeeCardSprite == currentCard.employeeCardSprite)
                            {
                                // "Não ajudar" é sempre considerado incorreto (não escolheu robô correto)
                                string feedbackText = "Você optou por não ajudar este funcionário. Às vezes, a melhor solução é delegar para um especialista robótico.";
                                DayResults.Instance.AddChoice(cardData, -1, false, feedbackText); // -1 = não ajudou
                                
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
            }

            cardSelector.ClearSelection();
        }
    }
    
    void UpdateCardVisual(EmployeeCard card)
    {
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
