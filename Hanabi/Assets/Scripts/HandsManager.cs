using Firebase.Database;
using UnityEngine;
using System.Threading.Tasks;
using Photon.Pun;
using System.Linq;
using System.Collections.Generic;

public class HandsManager : MonoBehaviour {
    private static HandsManager _instance;
    public static HandsManager Instance { get { return _instance; } }

    public GameObject myHand;
    public GameObject otherHandsParent;

    public PhotonView photonView;

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    public async Task InitializeHandIDs() {
        string latestRoomPath = "user2latest_room/" + PlayerPrefs.GetString("uid");
        DataSnapshot latestRoomSnapshot = await FirebaseDatabase.DefaultInstance.GetReference(latestRoomPath).GetValueAsync();

        string currentRoom = latestRoomSnapshot.Value.ToString();

        string playersPath = "active_rooms/" + currentRoom + "/players";
        DataSnapshot players = await FirebaseDatabase.DefaultInstance.GetReference(playersPath).GetValueAsync();

        List<string> playerUIDs = players.Children.Select((p) => (string)p.Value).ToList();

        int myUIDIndex = playerUIDs.IndexOf(PlayerPrefs.GetString("uid"));

        int childIndex = 0;

        // Set other hand ids
        for (int i = myUIDIndex + 1; i < playerUIDs.Count; i++) {
            Transform parent = otherHandsParent.transform.GetChild(childIndex);
            parent.GetComponent<Hand>().uid = playerUIDs[i];

            childIndex++;
        }

        for (int i = 0; i < myUIDIndex; i++) {
            Transform parent = otherHandsParent.transform.GetChild(childIndex);
            parent.GetComponent<Hand>().uid = playerUIDs[i];

            childIndex++;
        }

        // Set my hand id
        myHand.GetComponent<Hand>().uid = PlayerPrefs.GetString("uid");
    }

    public async Task DistributeHands() {
        // Get the room
        string latestRoomPath = "user2latest_room/" + PlayerPrefs.GetString("uid");
        DataSnapshot latestRoomSnapshot = await FirebaseDatabase.DefaultInstance.GetReference(latestRoomPath).GetValueAsync();

        string currentRoom = latestRoomSnapshot.Value.ToString();

        // Get players
        string playersPath = "active_rooms/" + currentRoom + "/players";
        DataSnapshot playersSnapshot = await FirebaseDatabase.DefaultInstance.GetReference(playersPath).GetValueAsync();

        string[] playerUIDs = playersSnapshot.Children.Select((p) => (string)p.Value).ToArray();

        // Get the cards
        string deckPath = "active_rooms/" + currentRoom + "/deck";
        DataSnapshot deckSnapshot = await FirebaseDatabase.DefaultInstance.GetReference(deckPath).GetValueAsync();

        List<CardInfo> deck = CardsManager.Instance.DataSnapshotToCards(deckSnapshot);

        int playerDeal = GameManager.Instance.GetPlayersDeal(playerUIDs.Length);

        // Set all the hands, starting from the top of the deck
        foreach (string uid in playerUIDs) {
            List<CardInfo> hand = deck.Take(playerDeal).ToList();

            await SaveNewHand(uid, hand);

            deck.RemoveRange(0, playerDeal);
        }

        // Set the deck top
        int deckTop = playerDeal * playerUIDs.Length;

        await FirebaseData.Instance.reference.Child("active_rooms").Child(currentRoom).Child("deck_top").SetValueAsync(deckTop);
    }

    public async Task InitializeHandsUI() {
        // Get the room
        string latestRoomPath = "user2latest_room/" + PlayerPrefs.GetString("uid");
        DataSnapshot latestRoomSnapshot = await FirebaseDatabase.DefaultInstance.GetReference(latestRoomPath).GetValueAsync();

        string currentRoom = latestRoomSnapshot.Value.ToString();

        // Get players
        string playersPath = "active_rooms/" + currentRoom + "/players";
        DataSnapshot playersSnapshot = await FirebaseDatabase.DefaultInstance.GetReference(playersPath).GetValueAsync();

        string[] playerUIDs = playersSnapshot.Children.Select((p) => (string)p.Value).ToArray();

        foreach (string p in playerUIDs) {
            await UpdateHandUI(p);
        }
    }

    [PunRPC]
    public async Task UpdateHandUI(string uid) {
        // Find the hand based on the uid
        Transform hand = FindHand(uid);

        // Get the room
        string latestRoomPath = "user2latest_room/" + uid;
        DataSnapshot latestRoomSnapshot = await FirebaseDatabase.DefaultInstance.GetReference(latestRoomPath).GetValueAsync();

        string currentRoom = latestRoomSnapshot.Value.ToString();

        // Get the hand
        string handPath = "active_rooms/" + currentRoom + "/hands/" + uid;
        DataSnapshot handSnapshot = await FirebaseDatabase.DefaultInstance.GetReference(handPath).GetValueAsync();

        List<CardInfo> handCards = CardsManager.Instance.DataSnapshotToCards(handSnapshot);

        // Update the hand's cards
        Utility.Instance.DestroyAllChildren(hand);

        foreach (CardInfo c in handCards) {
            CardsManager.Instance.InstantiateNewCard(c, hand);
        }
    }

    public Transform FindHand(string uid) {
        if(uid.Equals(PlayerPrefs.GetString("uid"))) {
            return myHand.transform;
        } else {
            List<Transform> hands = Utility.Instance.GetAllChildren(otherHandsParent.transform);

            foreach (Transform h in hands) {
                if (h.GetComponent<Hand>().uid.Equals(uid)) {
                    return h;
                }
            }

            return null;
        }
    }

    public async Task SaveNewHand(string uid, List<CardInfo> hand) {
        // Get the room
        string latestRoomPath = "user2latest_room/" + PlayerPrefs.GetString("uid");
        DataSnapshot latestRoomSnapshot = await FirebaseDatabase.DefaultInstance.GetReference(latestRoomPath).GetValueAsync();

        string currentRoom = latestRoomSnapshot.Value.ToString();

        string handJson = Utility.Instance.ArrayToJson(hand.ToArray());

        await FirebaseData.Instance.reference.Child("active_rooms").Child(currentRoom).Child("hands").Child(uid).SetRawJsonValueAsync(handJson);
    }

    public void SetHandCardsRaycasts(bool value) {
        foreach (Transform c in myHand.transform) {
            DraggableCard card = c.GetComponent<DraggableCard>();

            if(card != null) {
                card.SetRaycast(value);
            }
        }
    }
}
