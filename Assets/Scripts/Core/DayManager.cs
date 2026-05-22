using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DayManager : MonoBehaviour
{
    [Header("Settings")]
    public int employeesPerDay = 3;

    [Header("Employee Card Slots")]
    public EmployeeCard[] employeeCardSlots;

    [HideInInspector]
    public List<EmployeeCardData> allAvailableCards = new List<EmployeeCardData>();
    private int employeesProcessed = 0;

    void Awake()
    {
        LoadAllCardsFromResources();
    }

    void LoadAllCardsFromResources()
    {
        EmployeeCardData[] loadedCards = Resources.LoadAll<EmployeeCardData>("Data");

        if (loadedCards != null && loadedCards.Length > 0)
        {
            allAvailableCards = loadedCards.ToList();
        }
    }

    public void StartNewDay()
    {
        employeesProcessed = 0;
        LoadRandomCardsForDay();
    }

    void LoadRandomCardsForDay()
    {
        if (allAvailableCards == null || allAvailableCards.Count < employeesPerDay)
        {
            return;
        }

        if (employeeCardSlots == null || employeeCardSlots.Length < employeesPerDay)
        {
            return;
        }

        // Obter lista de fichas usadas do GameManager (persiste entre cenas)
        List<EmployeeCardData> usedCards = new List<EmployeeCardData>();
        if (GameManager.Instance != null)
        {
            usedCards = GameManager.Instance.GetUsedCards();
        }
        
        foreach (var card in allAvailableCards)
        {
            bool isUsed = usedCards.Contains(card);
        }
        
        List<EmployeeCardData> availableCards = allAvailableCards
            .Where(card => card != null && !usedCards.Contains(card))
            .ToList();

        // Removido o reset de fichas - queremos que não repitam durante todo o jogo
        // Se não houver fichas suficientes, é porque o jogo acabou

        for (int i = 0; i < employeesPerDay && i < employeeCardSlots.Length; i++)
        {
            if (availableCards.Count == 0)
            {
                break;
            }

            int randomIndex = Random.Range(0, availableCards.Count);
            EmployeeCardData selectedCard = availableCards[randomIndex];

            if (employeeCardSlots[i] != null && selectedCard != null)
            {
                // VERIFICAÇÃO CRÍTICA: Verificar se sprites essenciais estão NULL ANTES de usar
                bool hasErrors = false;               
                
                if (selectedCard.iconSprite == null)
                {
                    hasErrors = true;
                }
                if (selectedCard.employeeCardSprite == null)
                {
                    hasErrors = true;
                }
                if (selectedCard.robot1Sprite == null)
                {
                    hasErrors = true;
                }
                if (selectedCard.robot2Sprite == null)
                {
                    hasErrors = true;
                }
                
                // Se houver erros, pular esta ficha e tentar a próxima
                if (hasErrors)
                {
                    continue; // Pular para próxima iteração
                }
                
                employeeCardSlots[i].Initialize(
                    selectedCard.employeeCardSprite,
                    selectedCard.iconSprite,
                    selectedCard.robot1Sprite,
                    selectedCard.robot2Sprite,
                    selectedCard.robot1CardSprite,
                    selectedCard.robot2CardSprite,
                    selectedCard.correctRobotIndex,
                    selectedCard.topSelectionSprite
                );

                employeeCardSlots[i].UpdateVisual();

                // NÃO adicionar aqui! Será adicionado quando o jogador processar a ficha
                
                availableCards.RemoveAt(randomIndex);
            }
        }
        
        int totalUsed = GameManager.Instance != null ? GameManager.Instance.GetUsedCards().Count : 0;
    }

    public void MarkEmployeeAsProcessed()
    {
        employeesProcessed++;
    }

    public bool IsAllEmployeesProcessed()
    {
        return employeesProcessed >= employeesPerDay;
    }

    public int GetRemainingEmployees()
    {
        return employeesPerDay - employeesProcessed;
    }

}
