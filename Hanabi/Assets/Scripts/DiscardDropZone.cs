using UnityEngine.EventSystems;
using UnityEngine;
using Photon.Pun;
using Firebase.Database;

public class DiscardDropZone : MonoBehaviour, IDropHandler {
    public async void OnDrop(PointerEventData eventData) {
        Card card = eventData.pointerDrag.GetComponent<Card>();

        string latestRoomPath = "user2latest_room/" + PlayerPrefs.GetString("uid");
        DataSnapshot latestRoomSnapshot = await FirebaseData.Instance.reference.Child(latestRoomPath).GetValueAsync();

        string currentRoom = latestRoomSnapshot.Value.ToString();

        string turnPath = "active_rooms/" + currentRoom + "/turn";
        DataSnapshot turnSnapshot = await FirebaseData.Instance.reference.Child(turnPath).GetValueAsync();

        string turn = turnSnapshot.Value.ToString();

        if (turn.Equals(PlayerPrefs.GetString("uid"))) {
            // Set up card
            Transform parent = DiscardManager.Instance.discardPileParent;
            card.transform.SetParent(parent);
            card.transform.localPosition = Vector3.zero;
            card.GetComponent<DraggableCard>().DestroyPlaceholder();

            // Put it into the discard pile
            await DiscardManager.Instance.DiscardCard(card);

            DiscardManager.Instance.photonView.RPC(
                "UpdateDiscardPileUI",
                RpcTarget.All
            );

            await HandsManager.Instance.DrawCard(card.GetComponent<DraggableCard>().initialSiblingIndex);
            await TurnManager.Instance.NextTurn();
        } else {
            Debug.Log("Not your turn");
            card.GetComponent<DraggableCard>().BackToHand();
        }

        HandsManager.Instance.SetHandCardsRaycasts(true);
    }
}