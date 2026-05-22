using UnityEngine;
using UnityEngine.UI;

public class RobotImageSelector : MonoBehaviour
{
    [Header("Robot Images")]
    public Image robot1Image;
    public Image robot2Image;

    [Header("References")]
    public EmployeeCard employeeCard;

    void Start()
    {
        SetupRobotImages();
        SetupClickEvents();
    }

    public void SetupRobotImages()
    {
        if (employeeCard == null)
        {
            return;
        }

        if (robot1Image != null && employeeCard.robot1Sprite != null)
        {
            robot1Image.sprite = employeeCard.robot1Sprite;
            robot1Image.color = Color.white;
        }

        if (robot2Image != null && employeeCard.robot2Sprite != null)
        {
            robot2Image.sprite = employeeCard.robot2Sprite;
            robot2Image.color = Color.white;
        }
    }

    public void SetupClickEvents()
    {
        if (robot1Image != null)
        {
            Button btn1 = robot1Image.GetComponent<Button>();
            if (btn1 == null)
                btn1 = robot1Image.gameObject.AddComponent<Button>();

            btn1.onClick.AddListener(() => OnRobotImageClicked(0));
        }

        if (robot2Image != null)
        {
            Button btn2 = robot2Image.GetComponent<Button>();
            if (btn2 == null)
                btn2 = robot2Image.gameObject.AddComponent<Button>();

            btn2.onClick.AddListener(() => OnRobotImageClicked(1));
        }
    }

    void OnRobotImageClicked(int robotIndex)
    {
        if (employeeCard == null)
        {
            return;
        }

        if (employeeCard.isProcessed)
        {
            return;
        }

        // Encontrar e mostrar popup de confirmação
        RobotSelectionPopup popup = FindFirstObjectByType<RobotSelectionPopup>();
        if (popup != null)
        {
            popup.ShowConfirmationPopup(employeeCard, robotIndex);
        }
    }

    public void UpdateVisualState()
    {
        if (employeeCard == null || !employeeCard.isProcessed)
            return;

        // Desabilitar interações quando processado
        if (robot1Image != null)
        {
            Button btn1 = robot1Image.GetComponent<Button>();
            if (btn1 != null)
                btn1.interactable = false;
        }

        if (robot2Image != null)
        {
            Button btn2 = robot2Image.GetComponent<Button>();
            if (btn2 != null)
                btn2.interactable = false;
        }


    }
}
