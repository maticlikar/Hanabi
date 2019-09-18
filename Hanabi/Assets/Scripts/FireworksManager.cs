using UnityEngine;
using Photon.Pun;
using System.Threading.Tasks;
using Firebase.Database;

public class FireworksManager : MonoBehaviour {
    private static FireworksManager _instance;
    public static FireworksManager Instance { get { return _instance; } }

    public Transform whiteProgressParent;
    public Transform yellowProgressParent;
    public Transform greenProgressParent;
    public Transform blueProgressParent;
    public Transform redProgressParent;

    public PhotonView photonView;

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    public async Task InitializeFireworkProgress() {
        string fireworkProgressJson = "{" + 
            "\"white_progress\": 0," +
            "\"yellow_progress\": 0," +
            "\"green_progress\": 0," +
            "\"blue_progress\": 0," +
            "\"red_progress\": 0" +
            "}";

        string latestRoomPath = "user2latest_room/" + PlayerPrefs.GetString("uid");
        DataSnapshot latestRoomSnapshot = await FirebaseData.Instance.reference.Child(latestRoomPath).GetValueAsync();

        string currentRoom = latestRoomSnapshot.Value.ToString();

        await FirebaseData.Instance.reference.Child("active_rooms").Child(currentRoom).Child("firework_progress").SetRawJsonValueAsync(fireworkProgressJson);
    }

    public async Task SaveNewFireworkPart(CardInfo c) {
        // Get latest roomxs
        string latestRoomPath = "user2latest_room/" + PlayerPrefs.GetString("uid");
        DataSnapshot latestRoomSnapshot = await FirebaseData.Instance.reference.Child(latestRoomPath).GetValueAsync();

        string currentRoom = latestRoomSnapshot.Value.ToString();

        string color = c.color;
        int number = c.number;

        await FirebaseData.Instance.reference.Child("active_rooms").Child(currentRoom).Child("firework_progress").Child(color + "_progress").SetValueAsync(number);
    }

    public Transform ColorToFireworkProgressParent(string color) {
        if(color.Equals("white")) {
            return whiteProgressParent;
        } else if(color.Equals("yellow")) {
            return yellowProgressParent;
        } else if (color.Equals("green")) {
            return greenProgressParent;
        } else if (color.Equals("blue")) {
            return blueProgressParent;
        } else if (color.Equals("red")) {
            return redProgressParent;
        }

        // Should never happen
        return null;
    }   

    [PunRPC]
    public void UpdateFireworksProgressUI(string color, int number) {
        //print("UpdateFireworksProgressUI");
        //// Get latest room
        //string latestRoomPath = "user2latest_room/" + PlayerPrefs.GetString("uid");
        //DataSnapshot latestRoomSnapshot = await FirebaseData.Instance.reference.Child(latestRoomPath).GetValueAsync();

        //string currentRoom = latestRoomSnapshot.Value.ToString();

        //// Get firework progress
        //string fireworkProgressPath = "active_rooms/" + currentRoom + "/firework_progress";
        //DataSnapshot fireworkProgressSnapshot = await FirebaseData.Instance.reference.Child(fireworkProgressPath).GetValueAsync();

        //ClearFireworksUI(color);

        //int progress = int.Parse(fireworkProgressSnapshot.Child(color + "_progress").Value.ToString());

        //UpdateFireworkColor(color, progress, ColorToFireworkProgressParent(color));
        //print("/UpdateFireworksProgressUI");

        CardsManager.Instance.InstantiateNewCard(new CardInfo(color, number), ColorToFireworkProgressParent(color));
    }

    public void UpdateFireworkColor(string color, int progress, Transform parent) {
        for (int i = 0; i < progress; i++) {
            Card c = Instantiate(CardsManager.Instance.cardPrefab, parent, false).GetComponent<Card>();
            c.color = color;
            c.number = i + 1;

            c.backgroundImage.color = CardsManager.Instance.StringToColor(color);
            c.numberText.text = (i + 1).ToString();
        }
    }


    public void ClearFireworksUI(string color) {
        Utility.Instance.DestroyAllChildren(ColorToFireworkProgressParent(color));
    }
}
