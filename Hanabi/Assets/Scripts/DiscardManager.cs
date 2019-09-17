using UnityEngine;
using System.Threading.Tasks;
using Firebase.Database;

public class DiscardManager : MonoBehaviour {
    private static DiscardManager _instance;
    public static DiscardManager Instance { get { return _instance; } }

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    public async Task InitializeDiscardTop() {
        string latestRoomPath = "user2latest_room/" + PlayerPrefs.GetString("uid");
        DataSnapshot latestRoomSnapshot = await FirebaseData.Instance.reference.Child(latestRoomPath).GetValueAsync();

        string currentRoom = latestRoomSnapshot.Value.ToString();

        string discardTopPath = "active_rooms/" + currentRoom + "/discard_top";

        await FirebaseData.Instance.reference.Child(discardTopPath).SetValueAsync(0);
    }
}
