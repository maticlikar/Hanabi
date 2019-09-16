using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Threading.Tasks;

public class PhotonConnect : MonoBehaviourPunCallbacks {
    private static PhotonConnect _instance;
    public static PhotonConnect Instance { get { return _instance; } }

    [System.NonSerialized]
    public string gameVersion = "1";

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public async Task Connect(string username, string uid) {
        if (!PhotonNetwork.IsConnected) {
            SetPhotonPlayerNickName(username);
            SetPhotonPlayerUID(uid);

            PhotonNetwork.GameVersion = gameVersion;

            await SocialScreenManager.Instance.InitializeFriendList();

            PhotonNetwork.ConnectUsingSettings();
            PhotonChatHandler.Instance.Connect();

            ScreenManager.Instance.SwitchTo("ConnectingScreen");
        }
    }

    public override void OnConnectedToMaster() {
        if (string.IsNullOrEmpty(PhotonRooms.Instance.joiningRoomId)) {
            PhotonRooms.Instance.CreateNewRoom();
        } else {
            PhotonRooms.Instance.JoinRoom(PhotonRooms.Instance.joiningRoomId);
            PhotonRooms.Instance.ClearJoiningRoom();
        }
    }

    public void SetPhotonPlayerNickName(string value) {
        PhotonNetwork.NickName = value;
    }

    public void SetPhotonPlayerUID(string value) {
        AuthenticationValues authValues = new AuthenticationValues(value);

        PhotonNetwork.AuthValues = authValues;
    }

    public void Play() {
        if (PhotonNetwork.IsMasterClient) {
            //CardsManager.Instance.ShuffleCards();
            //HandsManager.Instance.DistributeHands();
            PhotonNetwork.LoadLevel("Game" + PhotonNetwork.PlayerList.Length);
        }
    }
}
