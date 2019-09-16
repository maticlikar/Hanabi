using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Threading.Tasks;
using Firebase.Database;

public class PhotonRooms : MonoBehaviourPunCallbacks {
    private static PhotonRooms _instance;
    public static PhotonRooms Instance { get { return _instance; } }

    [NonSerialized]
    public string joiningRoomId;

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    public void CreateNewRoom() {
        if (PhotonNetwork.IsConnected) {
            string newRoomId = Guid.NewGuid().ToString();
            PhotonNetwork.CreateRoom(newRoomId, new RoomOptions { MaxPlayers = 5, PublishUserId = true });
        }
    }

    public void ClearJoiningRoom() {
        joiningRoomId = "";

        PopUpsManager.Instance.GetPopUp("RoomInvitePopUp").RemoveListener("AcceptButton", PhotonChatHandler.Instance.joinFriendsLatestRoomListener);

        PhotonChatHandler.Instance.joinFriendsLatestRoomListener = null;
    }

    public void JoinRoom(string roomName) {
        if (PhotonNetwork.IsConnected) {
            PhotonNetwork.JoinRoom(roomName);
        }
    }

    public void LeaveRoom() {
        if (PhotonNetwork.IsConnected) {
            PhotonNetwork.LeaveRoom();
        }
    }

    public async Task JoinFriendsLatestRoom(string friendUID) {
        string latestRoomPath = "user2latest_room/" + friendUID;

        DataSnapshot friendSnapshot = await FirebaseDatabase.DefaultInstance.GetReference(latestRoomPath).GetValueAsync();

        string latestRoomId = friendSnapshot.Value.ToString();

        joiningRoomId = latestRoomId;

        // Once you leave the room, the OnConnectedToMaster method should just
        // transfer you to the friend's room
        LeaveRoom();
    }

    public async Task UpdateLatestRoom(string latestRoomId) {
        string myUID = PlayerPrefs.GetString("uid");
        await FirebaseData.Instance.reference.Child("user2latest_room").Child(myUID).SetValueAsync(latestRoomId);
    }

    public async override void OnJoinedRoom() {
        PhotonReadyUp.Instance.ResetReady();
        await UpdateLatestRoom(PhotonNetwork.CurrentRoom.Name);
        await MainScreenManager.Instance.InitializeRoomMemberList();

        ScreenManager.Instance.SwitchTo("MainScreen");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        string uid = PlayerPrefs.GetString("uid");

        PhotonReadyUp.Instance.ResetReady();
        MainScreenManager.Instance.RemoveRoomMemberFromUI(uid);
    }

    public async override void OnPlayerEnteredRoom(Player newPlayer) {
        string uid = newPlayer.UserId;
        
        PhotonReadyUp.Instance.ResetReady();
        await MainScreenManager.Instance.AddRoomMemberToUI(uid);
    }

    public override void OnJoinRoomFailed(short returnCode, string message) {
        Debug.Log("PhotonRooms: OnJoinRandomFailed(). " + returnCode + ": " + message);
    }
}
