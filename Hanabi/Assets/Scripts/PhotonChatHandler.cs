using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine;
using Photon.Chat;
using Firebase.Database;
using ExitGames.Client.Photon;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;

public class PhotonChatHandler : MonoBehaviour, IChatClientListener {
    private static PhotonChatHandler _instance;
    public static PhotonChatHandler Instance { get { return _instance; } }

    public ChatClient client;

    private string chatAppId = "639fd3b0-1efa-4b96-a01a-a037ea13f04c";

    public UnityAction joinFriendsLatestRoomListener;

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }

        client = new ChatClient(this);
    }

    private void Update() {
        client.Service();
    }

    public void Connect() {
        client.ChatRegion = "US";
        client.Connect(chatAppId, PhotonConnect.Instance.gameVersion, new AuthenticationValues(PlayerPrefs.GetString("uid")));
    }

    public void SendRoomInvite(string uid) {
        client.SendPrivateMessage(uid, "JoinMe");
    }

    public void SendAddFriend(string uid) {
        client.SendPrivateMessage(uid, "AddMe");
    }

    public void AddFriendToChat(string friendUID) {
        client.AddFriends(new string[] { friendUID });
    }

    public void DebugReturn(DebugLevel level, string message) {

    }

    public void OnChatStateChange(ChatState state) {

    }

    public async void OnConnected() {
        client.Subscribe(new string[] { "global" });
        client.SetOnlineStatus(ChatUserStatus.Online);

        await InitializeChatFriends();
    }

    public void OnDisconnected() {
        client.SetOnlineStatus(ChatUserStatus.Offline);
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages) {

    }

    public async void OnPrivateMessage(string sender, object message, string channelName) {
        string msg = message.ToString();
        string[] msgSplit = msg.Split(' ');

        if (msgSplit[0].Equals("JoinMe") && !sender.Equals(PlayerPrefs.GetString("uid"))) {
            await PopUpRoomInvite(sender);
        } else if (msgSplit[0].Equals("AddMe") && !sender.Equals(PlayerPrefs.GetString("uid"))) {
            await SocialScreenManager.Instance.AddFriendToUI(sender);
            
            AddFriendToChat(sender);
        }
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message) {
        SocialScreenManager.Instance.UpdateFriendStatus(user, status);
    }

    public void OnSubscribed(string[] channels, bool[] results) {

    }

    public void OnUnsubscribed(string[] channels) {

    }

    public void OnUserSubscribed(string channel, string user) {

    }

    public void OnUserUnsubscribed(string channel, string user) {

    }

    public async Task PopUpRoomInvite(string friendUID) {
        string userPath = "users/" + friendUID;

        DataSnapshot friendSnapshot = await FirebaseDatabase.DefaultInstance.GetReference(userPath).GetValueAsync();
        string friendName = friendSnapshot.Child("username").Value.ToString();

        PopUpWindow roomInvitePopup = PopUpsManager.Instance.GetPopUp("RoomInvitePopUp");

        TextMeshProUGUI friendUsernameText = roomInvitePopup.transform.Find("FriendUsernameText").GetComponent<TextMeshProUGUI>();
        friendUsernameText.text = friendName;

        roomInvitePopup.OpenPopUp();

        Button acceptButton = roomInvitePopup.FindButton("AcceptButton");

        if (joinFriendsLatestRoomListener != null) {
            roomInvitePopup.RemoveListener("AcceptButton", joinFriendsLatestRoomListener);
        }

        joinFriendsLatestRoomListener = async () => {
            await PhotonRooms.Instance.JoinFriendsLatestRoom(friendUID);
        };

        roomInvitePopup.AddListener("AcceptButton", joinFriendsLatestRoomListener);
    }

    public async Task InitializeChatFriends() {
        string friendsPath = "friends/" + PlayerPrefs.GetString("uid");

        DataSnapshot friendsSnapshot = await FirebaseDatabase.DefaultInstance.GetReference(friendsPath).GetValueAsync();

        List<string> friendUIDs = friendsSnapshot.Children.ToList().Select((f) => f.Key).ToList();

        // For all of your friends, add each to UI
        for (int i = 0; i < friendUIDs.Count; i++) {
            AddFriendToChat(friendUIDs[i]);
        }
    }
}
