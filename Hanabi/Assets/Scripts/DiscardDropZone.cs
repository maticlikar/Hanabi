using UnityEngine.EventSystems;
using UnityEngine;
using Photon.Pun;

public class DiscardDropZone : MonoBehaviour, IDropHandler {
    public async void OnDrop(PointerEventData eventData) {
        Card card = eventData.pointerDrag.GetComponent<Card>();

        // Set up card
        Transform parent = DiscardManager.Instance.discardPileParent;
        card.transform.SetParent(parent);
        card.transform.localPosition = Vector3.zero;
        card.GetComponent<DraggableCard>().DestroyPlaceholder();

        // Put it into the discard pile
        await DiscardManager.Instance.DiscardCard(card);
        
        DiscardManager.Instance.photonView.RPC(
            "UpdateDiscardPileUI",
            RpcTarget.Others
        );

        await HandsManager.Instance.DrawCard(card.GetComponent<DraggableCard>().initialSiblingIndex);

        HandsManager.Instance.SetHandCardsRaycasts(true);
    }
}