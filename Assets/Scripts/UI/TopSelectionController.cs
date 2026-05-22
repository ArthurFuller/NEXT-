using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[ExecuteInEditMode]
public class TopSelectionController : MonoBehaviour
{
    private Image topSelectionImage; // Referência ao componente Image do próprio GameObject
    private RectTransform rectTransform; // Referência ao RectTransform
    private CanvasGroup canvasGroup; // Para controlar alpha durante animação
    
    [Header("Default")]
    public Sprite defaultSprite; // Sprite padrão quando nenhuma ficha está selecionada
    
    [Header("Tamanho e Posição Customizáveis")]
    [Tooltip("Largura do card em pixels")]
    public float cardWidth = 200f;
    
    [Tooltip("Altura do card em pixels")]
    public float cardHeight = 280f;
    
    [Tooltip("Posição X do card (relativa aos anchors)")]
    public float cardPositionX = 0f;
    
    [Tooltip("Posição Y do card (relativa aos anchors)")]
    public float cardPositionY = 0f;
    
    [Tooltip("Aplicar configurações automaticamente ao iniciar")]
    public bool applyOnStart = true;
    
    [Header("Animação Popup")]
    [Tooltip("Ativar animação de popup ao aparecer")]
    public bool enablePopupAnimation = true;
    
    [Tooltip("Duração da animação em segundos")]
    [Range(0.1f, 2f)]
    public float animationDuration = 0.3f;
    
    [Tooltip("Escala inicial (0 = invisível, 1 = tamanho normal)")]
    [Range(0f, 1f)]
    public float startScale = 0f;
    
    [Tooltip("Escala de overshoot (>1 = efeito bounce)")]
    [Range(1f, 1.5f)]
    public float overshootScale = 1.1f;
    
    [Tooltip("Usar efeito de fade junto com escala")]
    public bool useFadeEffect = true;
    
    void Awake()
    {
        InitializeComponents();
    }
    
    void OnEnable()
    {
        InitializeComponents();
    }
    
    void InitializeComponents()
    {
        // Pegar o componente Image do próprio GameObject
        if (topSelectionImage == null)
            topSelectionImage = GetComponent<Image>();
        
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
        
        // Pegar ou adicionar CanvasGroup para animação
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null && Application.isPlaying)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }
        
        if (topSelectionImage == null)
        {
            return;
        }
        
        if (rectTransform == null)
        {
            return;
        }
        
        // Salvar sprite padrão (Source Image atual) apenas em Play Mode
        if (Application.isPlaying)
        {
            if (defaultSprite == null)
            {
                defaultSprite = topSelectionImage.sprite;
            }
            
            // Começar invisível ao iniciar o jogo
            topSelectionImage.enabled = false;
            rectTransform.localScale = Vector3.zero;
            if (canvasGroup != null)
                canvasGroup.alpha = 0f;
        }
        
        // Aplicar configurações de tamanho e posição
        if (applyOnStart)
        {
            ApplyCustomSettings();
        }
    }
    
    #if UNITY_EDITOR
    void OnValidate()
    {
        // Chamado quando valores mudam no Inspector
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
        
        if (applyOnStart && rectTransform != null)
        {
            ApplyCustomSettings();
        }
    }
    #endif
    
    /// <summary>
    /// Aplica as configurações customizadas de tamanho e posição
    /// </summary>
    public void ApplyCustomSettings()
    {
        if (rectTransform == null)
        {
            return;
        }
        
        // Aplicar tamanho (sizeDelta define o tamanho em pixels)
        rectTransform.sizeDelta = new Vector2(cardWidth, cardHeight);
        
        // Aplicar posição
        rectTransform.anchoredPosition = new Vector2(cardPositionX, cardPositionY);
        
        // IMPORTANTE: Garantir que a escala está em 1 quando não está animando
        // (a animação vai modificar localScale temporariamente)
        if (!Application.isPlaying || !enablePopupAnimation)
        {
            rectTransform.localScale = Vector3.one;
        }
    }
    
    /// <summary>
    /// Atualiza o sprite do Top Selection e torna visível com animação
    /// </summary>
    public void UpdateTopSelection(Sprite newSprite)
    {
        if (topSelectionImage != null && newSprite != null)
        {
            topSelectionImage.sprite = newSprite;
            
            // IMPORTANTE: Aplicar tamanho ANTES de tornar visível e animar
            if (applyOnStart)
            {
                ApplyCustomSettings();
            }
            
            // Tornar visível
            topSelectionImage.enabled = true;
            
            // Iniciar animação de popup
            if (Application.isPlaying && enablePopupAnimation)
            {
                StartCoroutine(PopupAnimation());
            }
            else
            {
                // Sem animação - aparecer instantaneamente
                rectTransform.localScale = Vector3.one;
                if (canvasGroup != null)
                    canvasGroup.alpha = 1f;
            }
        }
    }
    
    /// <summary>
    /// Reseta o Top Selection para o sprite padrão e torna invisível com animação
    /// </summary>
    public void ResetTopSelection()
    {
        if (topSelectionImage != null)
        {
            if (defaultSprite != null)
            {
                topSelectionImage.sprite = defaultSprite;
            }
            
            // Iniciar animação de fechamento
            if (Application.isPlaying && enablePopupAnimation)
            {
                StartCoroutine(CloseAnimation());
            }
            else
            {
                // Sem animação - desaparecer instantaneamente
                topSelectionImage.enabled = false;
                rectTransform.localScale = Vector3.zero;
                if (canvasGroup != null)
                    canvasGroup.alpha = 0f;
            }
            
            // Reaplicar configurações após resetar
            if (applyOnStart)
            {
                ApplyCustomSettings();
            }
        }
    }
    
    /// <summary>
    /// Animação de popup (aparecer)
    /// </summary>
    IEnumerator PopupAnimation()
    {
        float elapsed = 0f;
        
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / animationDuration;
            
            // Curva de animação com overshoot
            float scale;
            if (progress < 0.7f)
            {
                // Fase 1: Crescer até overshoot
                float phase1Progress = progress / 0.7f;
                scale = Mathf.Lerp(startScale, overshootScale, phase1Progress);
            }
            else
            {
                // Fase 2: Voltar para escala normal
                float phase2Progress = (progress - 0.7f) / 0.3f;
                scale = Mathf.Lerp(overshootScale, 1f, phase2Progress);
            }
            
            rectTransform.localScale = Vector3.one * scale;
            
            // Fade in
            if (useFadeEffect && canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, progress);
            }
            
            yield return null;
        }
        
        // Garantir valores finais
        rectTransform.localScale = Vector3.one;
        if (canvasGroup != null)
            canvasGroup.alpha = 1f;
    }
    
    /// <summary>
    /// Animação de fechamento (desaparecer)
    /// </summary>
    IEnumerator CloseAnimation()
    {
        float elapsed = 0f;
        float closeDuration = animationDuration * 0.5f; // Fechar mais rápido
        
        while (elapsed < closeDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / closeDuration;
            
            // Diminuir escala
            float scale = Mathf.Lerp(1f, 0f, progress);
            rectTransform.localScale = Vector3.one * scale;
            
            // Fade out
            if (useFadeEffect && canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, progress);
            }
            
            yield return null;
        }
        
        // Garantir valores finais
        rectTransform.localScale = Vector3.zero;
        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
        
        topSelectionImage.enabled = false;
    }
    
    /// <summary>
    /// Oculta o Top Selection
    /// </summary>
    public void HideTopSelection()
    {
        if (topSelectionImage != null)
        {
            topSelectionImage.enabled = false;
        }
    }
    
    /// <summary>
    /// Mostra o Top Selection
    /// </summary>
    public void ShowTopSelection()
    {
        if (topSelectionImage != null)
        {
            topSelectionImage.enabled = true;
        }
    }
    
    #if UNITY_EDITOR
    /// <summary>
    /// Método para testar configurações no Editor (botão no Inspector)
    /// </summary>
    [ContextMenu("Aplicar Configurações Agora")]
    void ApplySettingsInEditor()
    {
        rectTransform = GetComponent<RectTransform>();
        ApplyCustomSettings();
    }
    
    /// <summary>
    /// Método para resetar para valores padrão
    /// </summary>
    [ContextMenu("Resetar para Valores Padrão")]
    void ResetToDefaults()
    {
        cardWidth = 200f;
        cardHeight = 280f;
        cardPositionX = 0f;
        cardPositionY = 0f;
        ApplyCustomSettings();
    }
    #endif
}
