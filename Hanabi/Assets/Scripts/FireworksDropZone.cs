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

        string fireworkProgressPath = "active_rooms/" + currentRoom + "/firework_progress/" + color + "_progress";
        DataSnapshot fireworkProgressSnapshot = await FirebaseData.Instance.reference.Child(fireworkProgressPath).GetValueAsync();

        int fireworkProgress = int.Parse(fireworkProgressSnapshot.Value.ToString());

        await CheckIfValidPlacement(fireworkProgress, card);

        HandsManager.Instance.SetHandCardsRaycasts(true);
    }

    public async Task CheckIfValidPlacement(int fireworkProgress, Card card) {
        string color = card.color;
        int number = card.number;

        // Check if this card can be placed
        if (/*number == fireworkProgress + 1*/true) {
            // Set up card
            Transform parent = FireworksManager.Instance.ColorToFireworkProgressParent(color);
            card.transform.SetParent(parent);
            card.GetComponent<DraggableCard>().DestroyPlaceholder();

            await FireworksManager.Instance.SaveNewFireworkPart(new CardInfo(color, number));

            FireworksManager.Instance.photonView.RPC(
                "UpdateFireworksProgressUI",
                RpcTarget.All,
                color
            );

            await HandsManager.Instance.DrawCard(card.GetComponent<DraggableCard>().initialSiblingIndex);
        } else {
            card.GetComponent<DraggableCard>().BackToHand();
        }
    }
}
