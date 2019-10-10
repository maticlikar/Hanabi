using UnityEngine.EventSystems;
using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class HandDropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {
    public async void OnDrop(PointerEventData eventData) {
        DraggableCard card = eventData.pointerDrag.GetComponent<DraggableCard>();

        card.transform.SetParent(HandsManager.Instance.myHand.transform, false);
        card.transform.SetSiblingIndex(card.placeholder.transform.GetSiblingIndex());

        card.DestroyPlaceholder();

        List<CardInfo> newHand = GetCardsFromHand();

        await HandsManager.Instance.SaveNewHand(PlayerPrefs.GetString("uid"), newHand);

        HandsManager.Instance.photonView.RPC(
            "UpdateHandUI",
            RpcTarget.All,
            PlayerPrefs.GetString("uid")
        );

        HandsManager.Instance.SetHandCardsRaycasts(true);
    }

    public List<CardInfo> GetCardsFromHand() {
        List<CardInfo> hand = new List<CardInfo>();

        foreach (Transform c in HandsManager.Instance.myHand.transform) {
            Card card = c.GetComponent<Card>();

            CardInfo cardInfo = new CardInfo(card.color, card.number);

            hand.Add(cardInfo);
        }

        return hand;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (eventData.pointerDrag == null) {
            return;
        }

        DraggableCard card = eventData.pointerDrag.GetComponent<DraggableCard>();

        if (card != null) {
            if (card.placeholder != null) {
                card.placeholder.transform.SetParent(HandsManager.Instance.myHand.transform);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if(eventData.pointerDrag == null) {
            return;
        }

        DraggableCard card = eventData.pointerDrag.GetComponent<DraggableCard>();

        if (card != null) {
            if (card.placeholder != null) {
                card.placeholder.transform.SetParent(CardsManager.Instance.draggingParent);
            }
        }
    }
}
