using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DayChoice
{
    public EmployeeCardData cardData;
    public int chosenRobotIndex; // 0 = Robot1, 1 = Robot2
    public bool isCorrect;
    public string feedbackText;
}

public class DayResults : MonoBehaviour
{
    public static DayResults Instance;

    [Header("Day Results")]
    public List<DayChoice> currentDayChoices = new List<DayChoice>();

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

    public void AddChoice(EmployeeCardData cardData, int chosenRobotIndex, bool isCorrect, string feedbackText)
    {
        DayChoice choice = new DayChoice
        {
            cardData = cardData,
            chosenRobotIndex = chosenRobotIndex,
            isCorrect = isCorrect,
            feedbackText = feedbackText
        };

        currentDayChoices.Add(choice);
    }

    public void ClearResults()
    {
        currentDayChoices.Clear();
    }

    public int GetCorrectChoicesCount()
    {
        return currentDayChoices.FindAll(c => c.isCorrect).Count;
    }

    public int GetTotalChoices()
    {
        return currentDayChoices.Count;
    }
}
