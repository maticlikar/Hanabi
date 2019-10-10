using UnityEngine;
using System.Threading.Tasks;
using Firebase.Database;
using System.Linq;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour {
    private static TurnManager _instance;
    public static TurnManager Instance { get { return _instance; } }

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    public async Task InitializeFirstTurn() {
        string latestRoomPath = "user2latest_room/" + PlayerPrefs.GetString("uid");
        DataSnapshot latestRoomSnapshot = await FirebaseData.Instance.reference.Child(latestRoomPath).GetValueAsync();

        string currentRoom = latestRoomSnapshot.Value.ToString();

        string firstPlayerPath = "active_rooms/" + currentRoom + "/players/00";
        DataSnapshot firstPlayerSnapshot = await FirebaseData.Instance.reference.Child(firstPlayerPath).GetValueAsync();

        string firstPlayerUID = firstPlayerSnapshot.Value.ToString();

        string turnPath = "active_rooms/" + currentRoom + "/turn";

        await FirebaseData.Instance.reference.Child(turnPath).SetValueAsync(firstPlayerUID);
    }

    public async Task NextTurn() {
        string latestRoomPath = "user2latest_room/" + PlayerPrefs.GetString("uid");
        DataSnapshot latestRoomSnapshot = await FirebaseData.Instance.reference.Child(latestRoomPath).GetValueAsync();

        string currentRoom = latestRoomSnapshot.Value.ToString();

        string turnPath = "active_rooms/" + currentRoom + "/turn";
        DataSnapshot turnSnapshot = await FirebaseData.Instance.reference.Child(turnPath).GetValueAsync();

        string turn = turnSnapshot.Value.ToString();

        string playersPath = "active_rooms/" + currentRoom + "/players/";
        DataSnapshot playersSnapshot = await FirebaseData.Instance.reference.Child(playersPath).GetValueAsync();

        List<string> players = playersSnapshot.Children.ToList().Select((p) => p.Value.ToString()).ToList();
        
        int playerIndex = players.IndexOf(turn);

        string nextTurn;

        if(playerIndex >= players.Count - 1) {
            nextTurn = players[0];
        } else {
            nextTurn = players[playerIndex + 1];
        }

        await FirebaseData.Instance.reference.Child(turnPath).SetValueAsync(nextTurn);
    }
}
