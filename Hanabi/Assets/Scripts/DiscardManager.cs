using UnityEngine;
using System.Threading.Tasks;
using Firebase.Database;
using Photon.Pun;
using System.Collections.Generic;

public class DiscardManager : MonoBehaviour {
    private static DiscardManager _instance;
    public static DiscardManager Instance { get { return _instance; } }

    public Transform discardPileParent;

    public PhotonView photonView;

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    [PunRPC]
    public async Task UpdateDiscardPileUI() {
        string latestRoomPath = "user2latest_room/" + PlayerPrefs.GetString("uid");
        DataSnapshot latestRoomSnapshot = await FirebaseData.Instance.reference.Child(latestRoomPath).GetValueAsync();

        string currentRoom = latestRoomSnapshot.Value.ToString();

        string discardPilePath = "active_rooms/" + currentRoom + "/discard_pile";
        DataSnapshot discardPileSnapshot = await FirebaseData.Instance.reference.Child(discardPilePath).GetValueAsync();

        List<CardInfo> discardPile = CardsManager.Instance.DataSnapshotToCards(discardPileSnapshot);

        Utility.Instance.DestroyAllChildren(discardPileParent);

        foreach (CardInfo c in discardPile) {
            GameObject card = CardsManager.Instance.InstantiateNewCard(c, discardPileParent);
            card.transform.localPosition = Vector3.zero;
        }
    }

    public async Task InitializeDiscardTop() {
        await SaveDiscardTop(0);
    }

    public async Task SaveDiscardTop(int value) {
        string latestRoomPath = "user2latest_room/" + PlayerPrefs.GetString("uid");
        DataSnapshot latestRoomSnapshot = await FirebaseData.Instance.reference.Child(latestRoomPath).GetValueAsync();

        string currentRoom = latestRoomSnapshot.Value.ToString();

        string discardTopPath = "active_rooms/" + currentRoom + "/discard_top";

        await FirebaseData.Instance.reference.Child(discardTopPath).SetValueAsync(value);
    }

    public async Task DiscardCard(Card c) {
        string latestRoomPath = "user2latest_room/" + PlayerPrefs.GetString("uid");
        DataSnapshot latestRoomSnapshot = await FirebaseData.Instance.reference.Child(latestRoomPath).GetValueAsync();

        string currentRoom = latestRoomSnapshot.Value.ToString();

        string discardTopPath = "active_rooms/" + currentRoom + "/discard_top";
        DataSnapshot discardTopSnapshot = await FirebaseData.Instance.reference.Child(discardTopPath).GetValueAsync();

        int discardTop = int.Parse(discardTopSnapshot.Value.ToString());

        CardInfo card = new CardInfo(c.color, c.number);

        string discardPilePath = "active_rooms/" + currentRoom + "/discard_pile/" + Utility.Instance.PadNumber(discardTop.ToString());

        await FirebaseData.Instance.reference.Child(discardPilePath).SetRawJsonValueAsync(JsonUtility.ToJson(card));

        await SaveDiscardTop(discardTop + 1);
    }
}
