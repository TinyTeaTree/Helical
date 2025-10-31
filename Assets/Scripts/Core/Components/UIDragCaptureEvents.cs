using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Core
{
    public class UIDragCaptureEvents : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public UnityEvent<PointerEventData> OnDragEvent;
        public UnityEvent<PointerEventData> OnBeginDragEvent;
        public UnityEvent<PointerEventData> OnEndDragEvent;
        
        public void OnDrag(PointerEventData eventData)
        {
            OnDragEvent?.Invoke(eventData);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            OnBeginDragEvent?.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnEndDragEvent?.Invoke(eventData);
        }
    }
}