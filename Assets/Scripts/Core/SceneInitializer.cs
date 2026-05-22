using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SceneInitializer : MonoBehaviour
{
    [Header("UI References - Texto (Opcional)")]
    [Tooltip("Referência ao texto do contador de dias (deixe vazio se usar imagem)")]
    public TextMeshProUGUI dayCounterText;
    
    [Header("UI References - Imagem (Opcional)")]
    [Tooltip("Referência à imagem do contador de dias (deixe vazio se usar texto)")]
    public Image dayCounterImage;
    
    [Header("Sprites dos Dias")]
    [Tooltip("Sprite para o Dia 1")]
    public Sprite day1Sprite;
    [Tooltip("Sprite para o Dia 2")]
    public Sprite day2Sprite;
    [Tooltip("Sprite para o Dia 3")]
    public Sprite day3Sprite;
    
    void Start()
    {
        InitializeGameplayScene();
    }
    
    void OnEnable()
    {
        // Atualizar Day Counter sempre que a cena for ativada
        UpdateDayCounter();
    }
    
    void InitializeGameplayScene()
    {
        
        
        if (GameManager.Instance != null)
        {
            int currentDay = GameManager.Instance.GetCurrentDay();
                        
            // Atualizar TEXTO (se configurado)
            if (dayCounterText != null)
            {
                dayCounterText.text = "Dia " + currentDay + "/3";

            }
            
            // Atualizar IMAGEM (se configurado)
            if (dayCounterImage != null)
            {
                Sprite spriteToShow = GetSpriteForDay(currentDay);
                
                if (spriteToShow != null)
                {
                    dayCounterImage.sprite = spriteToShow;
                    dayCounterImage.enabled = true;
                }
            }

            DayManager dayManager = FindFirstObjectByType<DayManager>();

            if (dayManager != null)
            {
                GameManager.Instance.dayManager = dayManager;
                GameManager.Instance.StartNewDay(currentDay);                
            }
        }
    }
    
    void UpdateDayCounter()
    {
        if (GameManager.Instance != null)
        {
            int currentDay = GameManager.Instance.GetCurrentDay();
            
            // Atualizar TEXTO (se configurado)
            if (dayCounterText != null)
            {
                dayCounterText.text = "Dia " + currentDay + "/3";
            }
            
            // Atualizar IMAGEM (se configurado)
            if (dayCounterImage != null)
            {
                Sprite spriteToShow = GetSpriteForDay(currentDay);
                
                if (spriteToShow != null)
                {
                    dayCounterImage.sprite = spriteToShow;
                    dayCounterImage.enabled = true;
                }
            }
        }
    }
    
    Sprite GetSpriteForDay(int day)
    {
        switch (day)
        {
            case 1:
                return day1Sprite;
            case 2:
                return day2Sprite;
            case 3:
                return day3Sprite;
            default:
                return null;
        }
    }
}
