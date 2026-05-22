using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public delegate void OnCardDropped(EmployeeCard card);
    public static event OnCardDropped CardDropped;
    
    private UnityEngine.UI.Image image;
    private Color originalColor;
    private Color highlightColor = new Color(1f, 1f, 0.7f, 1f);
    
    void Awake()
    {
        image = GetComponent<UnityEngine.UI.Image>();
        if (image != null)
            originalColor = image.color;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            EmployeeCard card = eventData.pointerDrag.GetComponent<EmployeeCard>();
            if (card != null && !card.isProcessed && image != null)
            {
                image.color = highlightColor;
            }
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (image != null)
            image.color = originalColor;
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        if (image != null)
            image.color = originalColor;
        
        EmployeeCard card = eventData.pointerDrag.GetComponent<EmployeeCard>();
        
        if (card != null && !card.isProcessed)
        {
            CardDropped?.Invoke(card);
        }
    }
}
