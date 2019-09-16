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
        DataSnapshot latestRoomSnapshot = await FirebaseDatabase.DefaultInstance.GetReference(latestRoomPath).GetValueAsync();

        string currentRoom = latestRoomSnapshot.Value.ToString();

        string fireworkProgressPath = "active_rooms/" + currentRoom + "/firework_progress/" + color + "_progress";
        DataSnapshot fireworkProgressSnapshot = await FirebaseDatabase.DefaultInstance.GetReference(fireworkProgressPath).GetValueAsync();

        int fireworkProgress = int.Parse(fireworkProgressSnapshot.Value.ToString());

        await CheckIfValidPlacement(fireworkProgress, card);

        HandsManager.Instance.SetHandCardsRaycasts(true);
    }

    public async Task CheckIfValidPlacement(int fireworkProgress, Card card) {
        string color = card.color;
        int number = card.number;

        // Check if this card can be placed
        if (number == fireworkProgress + 1) {
            Transform parent = FireworksManager.Instance.ColorToFireworkProgressParent(color);

            card.transform.SetParent(parent);

            await FireworksManager.Instance.SaveNewFireworkPart(new CardInfo(color, number));

            print("About to call RPC UpdateFireworksProgressUI");

            FireworksManager.Instance.photonView.RPC(
                "UpdateFireworksProgressUI",
                RpcTarget.Others,
                color
            );
        } else {
            card.GetComponent<DraggableCard>().BackToHand();
        }
    }
}
