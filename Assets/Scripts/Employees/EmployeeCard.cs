using UnityEngine;
using UnityEngine.UI;

public class EmployeeCard : MonoBehaviour
{
    [Header("Employee Data")]
    public Sprite employeeCardSprite; // Imagem que aparece no painel central
    public Sprite iconSprite; // Ícone que aparece no painel esquerdo

    [Header("Robot Options")]
    public Sprite robot1Sprite;
    public Sprite robot2Sprite;

    [Header("Robot Card Sprites (for popup)")]
    public Sprite robot1CardSprite;
    public Sprite robot2CardSprite;
    
    [Header("TopSelection Image")]
    public Sprite topSelectionSprite;

    public int correctRobotIndex;

    [Header("State")]
    public bool isProcessed = false;
    public CardState currentState = CardState.Available;

    [Header("UI Reference")]
    public Image cardImage;

    public enum CardState
    {
        Available,
        Selected,
        Processing,
        Completed
    }

    void Awake()
    {
        if (cardImage == null)
            cardImage = GetComponent<Image>();

        if (cardImage == null)
            cardImage = gameObject.AddComponent<Image>();
    }

    public void Initialize(Sprite cardSprite, Sprite robot1, Sprite robot2, int correctRobot)
    {
        employeeCardSprite = cardSprite;
        robot1Sprite = robot1;
        robot2Sprite = robot2;
        correctRobotIndex = correctRobot;
        isProcessed = false;
        currentState = CardState.Available;

        UpdateVisual();
    }

    public void Initialize(Sprite cardSprite, Sprite iconSprite, Sprite robot1, Sprite robot2, Sprite robot1Card, Sprite robot2Card, int correctRobot, Sprite topSelection = null)
    {
        employeeCardSprite = cardSprite;
        this.iconSprite = iconSprite;
        robot1Sprite = robot1;
        robot2Sprite = robot2;
        robot1CardSprite = robot1Card;
        robot2CardSprite = robot2Card;
        correctRobotIndex = correctRobot;
        topSelectionSprite = topSelection; // Opcional - pode ser NULL
        
        // CRÍTICO: Resetar estado completamente
        isProcessed = false;
        currentState = CardState.Available;
        
        // CRÍTICO: Resetar visual (remover cinza/desabilitado)
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        
        Image cardImage = GetComponent<Image>();
        if (cardImage != null)
        {
            cardImage.color = Color.white;
        }

        UpdateVisual();
    }

    public void UpdateVisual()
    {
        if (cardImage == null)
            cardImage = GetComponent<Image>();
        
        // Atualizar sprite do ícone (painel esquerdo)
        if (cardImage != null && iconSprite != null)
        {
            cardImage.sprite = iconSprite;
            cardImage.enabled = true;
        }
    }

    void OnEnable()
    {
        if (iconSprite != null)  // Corrigido: verifica iconSprite ao invés de employeeCardSprite
            UpdateVisual();
    }

    public void SetState(CardState newState)
    {
        currentState = newState;
    }

    public bool IsCorrectRobot(int robotIndex)
    {
        return robotIndex == correctRobotIndex;
    }

    public void MarkAsProcessed()
    {
        isProcessed = true;
        currentState = CardState.Completed;
    }
}
