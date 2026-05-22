using UnityEngine;

[CreateAssetMenu(fileName = "NewEmployeeCard", menuName = "Game/Employee Card Data")]
public class EmployeeCardData : ScriptableObject
{
    [Header("Card Info")]
    public string cardName; // Ex: "Rodrigo", "Maria" (só para organização)
    
    [Header("Sprites")]
    public Sprite employeeCardSprite; // Imagem completa da ficha
    public Sprite iconSprite; // Ícone pequeno para o painel esquerdo
    public Sprite robot1Sprite; // Robô 1 específico (sobreposto na ficha)
    public Sprite robot2Sprite; // Robô 2 específico (sobreposto na ficha)

    [Header("Robot Card Sprites (for popup)")]
    public Sprite robot1CardSprite; // Card/ficha completa do Robô 1
    public Sprite robot2CardSprite; // Card/ficha completa do Robô 2
    
    [Header("Day Summary Sprites")]
    public Sprite daySummarySprite; // Imagem que aparece no resumo do dia (lado esquerdo)
    public Sprite correctChoiceCardSprite; // Card que aparece quando a escolha foi CORRETA
    public Sprite incorrectChoiceCardSprite; // Card que aparece quando a escolha foi INCORRETA
    
    [Header("TopSelection Image")]
    public Sprite topSelectionSprite; // Imagem específica para o TopSelection
    
    [Header("Correct Answer")]
    public int correctRobotIndex; // 0 = Robot1 correto, 1 = Robot2 correto

    [Header("Feedback Messages")]
    [TextArea(3, 5)]
    public string robot1Feedback; // Mensagem específica quando escolher robô 1 (errado)
    [TextArea(3, 5)]
    public string robot2Feedback; // Mensagem específica quando escolher robô 2 (errado)
}
