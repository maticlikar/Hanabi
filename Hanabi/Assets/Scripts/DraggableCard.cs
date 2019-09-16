using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

    public GameObject placeholder;
    public int initialSiblingIndex;

    public void OnBeginDrag(PointerEventData eventData) {
        initialSiblingIndex = transform.GetSiblingIndex();

        placeholder = new GameObject();
        RectTransform rt = placeholder.AddComponent<RectTransform>();
        rt.sizeDelta = transform.GetComponent<RectTransform>().sizeDelta;

        placeholder.transform.SetParent(HandsManager.Instance.myHand.transform, false);
        placeholder.transform.SetSiblingIndex(transform.GetSiblingIndex());

        transform.SetParent(CardsManager.Instance.draggingParent, false);
        SetRaycast(false);

        HandsManager.Instance.SetHandCardsRaycasts(false);
    }

    public void OnDrag(PointerEventData eventData) {
        transform.position = eventData.position;

        int newSiblingIndex = HandsManager.Instance.myHand.transform.childCount;

        foreach (Transform c in HandsManager.Instance.myHand.transform) {
            if(transform.position.x < c.position.x) {
                newSiblingIndex = c.GetSiblingIndex();

                if (placeholder.transform.GetSiblingIndex() < newSiblingIndex) {
                    newSiblingIndex--;
                }

                break;
            }
        }

        placeholder.transform.SetSiblingIndex(newSiblingIndex);
    }

    public void DestroyPlaceholder() {
        placeholder.transform.SetParent(CardsManager.Instance.draggingParent);
        Destroy(placeholder);
    }

    public void SetRaycast(bool value) {
        transform.GetComponent<CanvasGroup>().blocksRaycasts = value;
    }

    public void BackToHand() {
        DestroyPlaceholder();

        transform.SetParent(HandsManager.Instance.myHand.transform, false);
        transform.SetSiblingIndex(initialSiblingIndex);

        HandsManager.Instance.SetHandCardsRaycasts(true);
    }

    public void OnEndDrag(PointerEventData eventData) {

    }
}
