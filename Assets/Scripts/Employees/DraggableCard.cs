using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private EmployeeCard employeeCard;
    
    void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        employeeCard = GetComponent<EmployeeCard>();
        
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (employeeCard != null && employeeCard.isProcessed)
        {
            return;
        }        
        
        // Salvar posição no momento do drag (não no Start)
        originalPosition = rectTransform.anchoredPosition;
        
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (employeeCard != null && employeeCard.isProcessed)
        {
            return;
        }
        
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        if (employeeCard != null && employeeCard.isProcessed)
        {
            return;
        }       
        
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        
        rectTransform.anchoredPosition = originalPosition;
    }
}
