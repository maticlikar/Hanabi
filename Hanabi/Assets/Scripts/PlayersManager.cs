using UnityEngine;
using System.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using Firebase.Database;

public class PlayersManager : MonoBehaviour {
    private static PlayersManager _instance;
    public static PlayersManager Instance { get { return _instance; } }

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    public async Task InitializePlayerOrder() {
        Player[] players = PhotonNetwork.PlayerList;

        string[] playerUIDs = players.Select((p) => p.UserId).ToArray();

        Utility.Instance.ShuffleArray(playerUIDs);

        string playersJson = Utility.Instance.ArrayToJson(playerUIDs);

        await SavePlayerOrder(playersJson);
    }

    public async Task SavePlayerOrder(string playersJson) {
        string latestRoomPath = "user2latest_room/" + PlayerPrefs.GetString("uid");
        DataSnapshot latestRoomSnapshot = await FirebaseData.Instance.reference.Child(latestRoomPath).GetValueAsync();

        string currentRoom = latestRoomSnapshot.Value.ToString();

        await FirebaseData.Instance.reference.Child("active_rooms").Child(currentRoom).Child("players").SetRawJsonValueAsync(playersJson);
    }
}
