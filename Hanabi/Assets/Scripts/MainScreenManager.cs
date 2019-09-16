using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;
using Firebase.Database;
using System.Threading.Tasks;

public class MainScreenManager : MonoBehaviour {
    private static MainScreenManager _instance;
    public static MainScreenManager Instance { get { return _instance; } }

    // Parent object
    public Transform roomMemberList;

    public GameObject roomMemberPrefab;

    public GameObject readyButton;

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    public async Task InitializeRoomMemberList() {
        Player[] roomMembers = PhotonNetwork.PlayerList;

        Utility.Instance.DestroyAllChildren(roomMemberList);

        foreach (Player p in roomMembers) {
            await AddRoomMemberToUI(p.UserId);
        }
    }

    public async Task AddRoomMemberToUI(string uid) {
        string usernamePath = "users/" + uid;

        DataSnapshot usernameSnapshot = await FirebaseDatabase.DefaultInstance.GetReference(usernamePath).GetValueAsync();
        string username = usernameSnapshot.Child("username").Value.ToString();

        GameObject r = Instantiate(roomMemberPrefab, roomMemberList, false);
        RoomMember roomMember = r.GetComponent<RoomMember>();

        roomMember.UID = uid;
        roomMember.usernameText.text = username;
        roomMember.readyText.text = "Not Ready";
    }

    public void RemoveRoomMemberFromUI(string uid) {
        GameObject roomMember = FindRoomMember(uid);

        Destroy(roomMember);
    }

    public GameObject FindRoomMember(string uid) {
        foreach (Transform r in roomMemberList) {
            RoomMember roomMember = r.GetComponent<RoomMember>();

            if (roomMember.UID.Equals(uid)) {
                return roomMember.gameObject;
            }
        }

        return null;
    }
}
