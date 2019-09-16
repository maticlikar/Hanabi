using UnityEngine.EventSystems;
using UnityEngine;

public class NotValidDropZone : MonoBehaviour, IDropHandler {
    public void OnDrop(PointerEventData eventData) {
        DraggableCard card = eventData.pointerDrag.GetComponent<DraggableCard>();

        card.BackToHand();
    }
}