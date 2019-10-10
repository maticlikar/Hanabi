using UnityEngine.EventSystems;
using Firebase.Database;
using UnityEngine;
using System.Threading.Tasks;
using Photon.Pun;

public class FireworksDropZone : MonoBehaviour, IDropHandler {
    public async void OnDrop(PointerEventData eventData) {
        Card card = eventData.pointerDrag.GetComponent<Card>();

        string color = card.color;

        string latestRoomPath = "user2latest_room/" + PlayerPrefs.GetString("uid");
        DataSnapshot latestRoomSnapshot = await FirebaseData.Instance.reference.Child(latestRoomPath).GetValueAsync();

        string currentRoom = latestRoomSnapshot.Value.ToString();

        string turnPath = "active_rooms/" + currentRoom + "/turn";
        DataSnapshot turnSnapshot = await FirebaseData.Instance.reference.Child(turnPath).GetValueAsync();

        string turn = turnSnapshot.Value.ToString();

        string fireworkProgressPath = "active_rooms/" + currentRoom + "/firework_progress/" + color + "_progress";
        DataSnapshot fireworkProgressSnapshot = await FirebaseData.Instance.reference.Child(fireworkProgressPath).GetValueAsync();

        int fireworkProgress = int.Parse(fireworkProgressSnapshot.Value.ToString());

        if (turn.Equals(PlayerPrefs.GetString("uid"))) {
            await CheckIfValidPlacement(fireworkProgress, card);
        } else {
            // TODO: Display error
            Debug.Log("Not your turn");
            card.GetComponent<DraggableCard>().BackToHand();
        }

        HandsManager.Instance.SetHandCardsRaycasts(true);
    }

    public async Task CheckIfValidPlacement(int fireworkProgress, Card card) {
        string color = card.color;
        int number = card.number;

        print("color: " + color);
        print("number: " + number);
        print("fireworkProgress: " + fireworkProgress);

        // Check if this card can be placed
        if (number == fireworkProgress + 1) {
            // Set up card
            Transform parent = FireworksManager.Instance.ColorToFireworkProgressParent(color);
            card.transform.SetParent(parent);
            card.GetComponent<DraggableCard>().DestroyPlaceholder();

            await FireworksManager.Instance.SaveNewFireworkPart(new CardInfo(color, number));

            //await FireworksManager.Instance.SaveNewFireworkPart(new CardInfo(color, number));

            FireworksManager.Instance.photonView.RPC(
                "UpdateFireworksProgressUI",
                RpcTarget.All,
                color
            );

            await HandsManager.Instance.DrawCard(card.GetComponent<DraggableCard>().initialSiblingIndex);
            await TurnManager.Instance.NextTurn();
        } else {
            card.GetComponent<DraggableCard>().BackToHand();
        }
    }
}
