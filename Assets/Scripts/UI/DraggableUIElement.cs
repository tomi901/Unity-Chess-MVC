using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


[RequireComponent(typeof(RectTransform))]
public class DraggableUIElement : UIBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    [SerializeField]
    private bool moveFromCenter = false;

    private Vector3 beginDragDelta;

    #region Events

    [System.Serializable]
    public class DragEvent : UnityEvent<PointerEventData> { }

    [SerializeField]
    private DragEvent onBeginDragEvent, onDragEvent, onEndDragEvent;

    public event UnityAction<PointerEventData> OnBeginDragEvent
    {
        add => onBeginDragEvent.AddListener(value);
        remove => onBeginDragEvent.RemoveListener(value);
    }

    public event UnityAction<PointerEventData> OnDragEvent
    {
        add => onDragEvent.AddListener(value);
        remove => onDragEvent.RemoveListener(value);
    }

    public event UnityAction<PointerEventData> OnEndDragEvent
    {
        add => onEndDragEvent.AddListener(value);
        remove => onEndDragEvent.RemoveListener(value);
    }

    #endregion

    private RectTransform rectTransform;
    public RectTransform RectTransform => rectTransform ?? (rectTransform = GetComponent<RectTransform>());


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (GetWorldPosition(eventData.position, out Vector3 point))
        {
            beginDragDelta = (point - RectTransform.position) * 0.5f;
        }

        onBeginDragEvent.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (GetWorldPosition(eventData.position, out Vector3 point))
        {
            if (!moveFromCenter) point -= beginDragDelta;

            RectTransform.position = point;
        }

        onDragEvent.Invoke(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        onEndDragEvent.Invoke(eventData);
    }

    private bool GetWorldPosition(Vector2 screenPos, out Vector3 point)
    {
        return RectTransformUtility.ScreenPointToWorldPointInRectangle(RectTransform, 
            screenPos, Camera.main, out point);
    }

}
