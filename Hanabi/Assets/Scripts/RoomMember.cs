using UnityEngine;
using TMPro;

public class RoomMember : MonoBehaviour {
    private string uid;

    public string UID { get { return uid; } set { uid = value; } }

    public TextMeshProUGUI usernameText;
    public TextMeshProUGUI readyText;

    public override string ToString() {
        return uid;
    }
}
