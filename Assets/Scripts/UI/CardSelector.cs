using UnityEngine;
using UnityEngine.UI;

public class CardSelector : MonoBehaviour
{
    [Header("UI References")]
    public Image selectedCardDisplay;
    public GameObject placeholder;

    [Header("Robot Display References")]
    public Image robot1Display;
    public Image robot2Display;
    
    private EmployeeCard currentSelectedCard;
    
    void OnEnable()
    {
        DropZone.CardDropped += OnCardDropped;
    }
    
    void OnDisable()
    {
        DropZone.CardDropped -= OnCardDropped;
    }
    
    void Start()
    {
        ShowPlaceholder();
        HideRobotImages(); // Garantir que os robôs estejam ocultos no início
    }
    
    void OnCardDropped(EmployeeCard card)
    {
        SelectCard(card);
    }
    
    public void SelectCard(EmployeeCard card)
    {
        currentSelectedCard = card;
        DisplayCardDetails();
        
        ButtonController buttonController = FindFirstObjectByType<ButtonController>();
        if (buttonController != null)
            buttonController.EnableButtons();
    }
    
    void DisplayCardDetails()
    {
        if (currentSelectedCard == null || selectedCardDisplay == null)
            return;

        HidePlaceholder();

        Sprite spriteToShow = currentSelectedCard.employeeCardSprite;

        if (spriteToShow == null)
        {           
            // NÃO usar fallback do cardImage, pois pode ser o ícone (não a ficha completa)
            // Mostrar placeholder ao invés de imagem errada
            ShowPlaceholder();
            return;
        }

        selectedCardDisplay.sprite = spriteToShow;
        selectedCardDisplay.gameObject.SetActive(true);

        // Display robot images
        DisplayRobotImages();
        
        // NOVO: Atualizar TopSelection
        TopSelectionController topSelection = FindFirstObjectByType<TopSelectionController>();
        if (topSelection != null && currentSelectedCard.topSelectionSprite != null)
        {
            topSelection.UpdateTopSelection(currentSelectedCard.topSelectionSprite);
        }
    }
    
    public void ClearSelection()
    {
        currentSelectedCard = null;

        if (selectedCardDisplay != null)
            selectedCardDisplay.gameObject.SetActive(false);

        // Hide robot images
        HideRobotImages();

        ShowPlaceholder();
        
        // NOVO: Resetar TopSelection
        TopSelectionController topSelection = FindFirstObjectByType<TopSelectionController>();
        if (topSelection != null)
        {
            topSelection.ResetTopSelection();
        }

        ButtonController buttonController = FindFirstObjectByType<ButtonController>();
        if (buttonController != null)
            buttonController.DisableButtons();
    }
    
    void ShowPlaceholder()
    {
        if (placeholder != null)
            placeholder.SetActive(true);
    }
    
    void HidePlaceholder()
    {
        if (placeholder != null)
            placeholder.SetActive(false);
    }
    
    public EmployeeCard GetCurrentCard()
    {
        return currentSelectedCard;
    }

    void DisplayRobotImages()
    {
        if (currentSelectedCard == null)
            return;

        if (robot1Display != null && currentSelectedCard.robot1Sprite != null)
        {
            robot1Display.sprite = currentSelectedCard.robot1Sprite;
            robot1Display.gameObject.SetActive(true);
            robot1Display.raycastTarget = true;

            // Add button component if not exists
            Button btn1 = robot1Display.GetComponent<Button>();
            if (btn1 == null)
                btn1 = robot1Display.gameObject.AddComponent<Button>();

            btn1.onClick.RemoveAllListeners();
            btn1.onClick.AddListener(() => OnRobotClicked(0));
        }

        if (robot2Display != null && currentSelectedCard.robot2Sprite != null)
        {
            robot2Display.sprite = currentSelectedCard.robot2Sprite;
            robot2Display.gameObject.SetActive(true);
            robot2Display.raycastTarget = true;

            // Add button component if not exists
            Button btn2 = robot2Display.GetComponent<Button>();
            if (btn2 == null)
                btn2 = robot2Display.gameObject.AddComponent<Button>();

            btn2.onClick.RemoveAllListeners();
            btn2.onClick.AddListener(() => OnRobotClicked(1));
        }
    }

    void HideRobotImages()
    {
        if (robot1Display != null)
            robot1Display.gameObject.SetActive(false);

        if (robot2Display != null)
            robot2Display.gameObject.SetActive(false);
    }

    void OnRobotClicked(int robotIndex)
    {
        if (currentSelectedCard == null)
            return;

        // Open popup with the selected robot pre-highlighted
        RobotSelectionPopup popup = FindFirstObjectByType<RobotSelectionPopup>();
        if (popup != null)
        {
            popup.ShowConfirmationPopup(currentSelectedCard, robotIndex);
        }
    }
}
