using UnityEngine;
using UnityEngine.UI;
using Photon.Chat;
using Firebase.Database;
using TMPro;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

public class SocialScreenManager : MonoBehaviour {
    private static SocialScreenManager _instance;
    public static SocialScreenManager Instance { get { return _instance; } }

    // Parent object
    public TMP_InputField friendUIDInputField;
    public Transform friendList;

    public GameObject friendPrefab;

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    public async Task AddFriendToUI(string friendUID) { 
        string usernamePath = "users/" + friendUID;

        DataSnapshot usernameSnapshot = await FirebaseData.Instance.reference.Child(usernamePath).GetValueAsync();
        string friendName = usernameSnapshot.Child("username").Value.ToString();

        GameObject f = Instantiate(friendPrefab, friendList, false);
        Friend friend = f.GetComponent<Friend>();

        friend.UID = friendUID;
        friend.friendListButton.GetComponentInChildren<TextMeshProUGUI>().text = friendName;
        friend.friendListStatus.GetComponent<TextMeshProUGUI>().text = "Offline";

        friend.friendListButton.GetComponent<Button>().onClick.AddListener(() => {
            PhotonChatHandler.Instance.SendRoomInvite(friend.UID);
        });
    }

    public async Task InitializeFriendList() {
        string friendsPath = "friends/" + PlayerPrefs.GetString("uid");

        DataSnapshot friendsSnapshot = await FirebaseData.Instance.reference.Child(friendsPath).GetValueAsync();

        List<string> friendUIDs = friendsSnapshot.Children.ToList().Select((f) => f.Key).ToList();

        // For all of your friends, add each of to UI
        for (int i = 0; i < friendUIDs.Count; i++) {
            await AddFriendToUI(friendUIDs[i]);
        }
    }

    public async void AddFriend() { 
        string friendUID = friendUIDInputField.text;
        string myUID = PlayerPrefs.GetString("uid");

        string friendPath = "users/" + friendUID;
        string usernamePath = "users/" + myUID;
        string friendsPath = "friends/" + myUID;

        DataSnapshot friendSnapshot = await FirebaseData.Instance.reference.Child(friendPath).GetValueAsync();
        DataSnapshot usernameSnapshot = await FirebaseData.Instance.reference.Child(usernamePath).GetValueAsync();
        DataSnapshot friendsSnapshot = await FirebaseData.Instance.reference.Child(friendsPath).GetValueAsync();

        string friendName = (string)friendSnapshot.Child("username").Value;
        string myName = (string)usernameSnapshot.Child("username").Value;
        List<string> friendUIDs = friendsSnapshot.Children.ToList().Select((f) => f.Key).ToList();

        if (friendUID != myUID) {
            if (!string.IsNullOrEmpty(friendName)) {
                if (!friendUIDs.Contains(friendUID)) {
                    await AddFriendsToDatabase(myUID, friendUID, myName, friendName);

                    await AddFriendToUI(friendUID);

                    PhotonChatHandler.Instance.AddFriendToChat(friendUID);

                    PhotonChatHandler.Instance.SendAddFriend(friendUID);
                } else {
                    // TODO: Display error already friends
                    Debug.Log("Already friends");
                }
            } else {
                // TODO: Display error user doesn't exist
                Debug.Log("User doesn't exist");
            }
        } else {
            // TODO: Display error can't add yourself
            Debug.Log("Can't add yourself");
        }

        friendUIDInputField.text = "";
    }

    public async Task AddFriendsToDatabase(string myUID, string friendUID, string myName, string friendName) {
        await FirebaseData.Instance.reference.Child("friends").Child(myUID).Child(friendUID).SetValueAsync(friendName);
        await FirebaseData.Instance.reference.Child("friends").Child(friendUID).Child(myUID).SetValueAsync(myName);
    }

    public void UpdateFriendStatus(string updateStatusFriendUID, int updateStatus) {
        foreach (Transform child in friendList) {
            Friend f = child.GetComponent<Friend>();

            if (f.UID.Equals(updateStatusFriendUID)) {
                // Set the new status of the friend with a string representing that status
                f.friendListStatus.GetComponent<TextMeshProUGUI>().text = StatusIntToString(updateStatus);

                if (updateStatus == ChatUserStatus.Offline) {
                    f.friendListButton.GetComponent<Button>().interactable = false;
                } else {
                    f.friendListButton.GetComponent<Button>().interactable = true;
                }
            }
        }
    }

    private string StatusIntToString(int status) {
        if (status == ChatUserStatus.Offline) {
            return "Offline";
        } else if (status == ChatUserStatus.Invisible) {
            return "Invisible";
        } else if (status == ChatUserStatus.Online) {
            return "Online";
        } else if (status == ChatUserStatus.Away) {
            return "Away";
        } else if (status == ChatUserStatus.DND) {
            return "DND";
        } else if (status == ChatUserStatus.LFG) {
            return "LFG";
        } else if (status == ChatUserStatus.Playing) {
            return "Playing";
        }

        return "";
    }
}
