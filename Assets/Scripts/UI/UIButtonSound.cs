using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [Header("Sound Settings")]
    [Tooltip("Tocar som ao passar o mouse (hover)")]
    public bool playHoverSound = false;
    
    [Tooltip("Tocar som ao clicar")]
    public bool playClickSound = true;
    
    [Tooltip("Som customizado para este botão (opcional)")]
    public AudioClip customClickSound;
    
    [Tooltip("Volume do som (0-1)")]
    [Range(0f, 1f)]
    public float volume = 1f;
    
    private Button button;
    
    void Awake()
    {
        button = GetComponent<Button>();
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (playHoverSound && button != null && button.interactable)
        {
            if (AudioManager.Instance != null)
            {
                // Pode adicionar som de hover no futuro
                // AudioManager.Instance.PlaySFX(hoverSound, volume * 0.5f);
            }
        }
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (playClickSound && button != null && button.interactable)
        {
            if (AudioManager.Instance != null)
            {
                if (customClickSound != null)
                {
                    AudioManager.Instance.PlaySFX(customClickSound, volume);
                }
                else
                {
                    AudioManager.Instance.PlayButtonClick();
                }
            }
        }
    }
}
