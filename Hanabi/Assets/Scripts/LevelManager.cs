using Photon.Pun;
using UnityEngine;
using System.Threading.Tasks;

public class LevelManager : MonoBehaviour {
    private static LevelManager _instance;
    public static LevelManager Instance { get { return _instance; } }

    public PhotonView photonView;

    public GameObject SettingUpScreen;
    public GameObject GameScreen;

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    private async void Start() {
        SetUpScreenManager();
        ScreenManager.Instance.SwitchTo("SettingUpScreen");

        await InitializeLevel();
    }

    public async Task InitializeLevel() {
        if (PhotonNetwork.IsMasterClient) {
            await FireworksManager.Instance.InitializeFireworkProgress();
            await PlayersManager.Instance.InitializePlayerOrder();
            await TurnManager.Instance.InitializeFirstTurn();
            await DiscardManager.Instance.InitializeDiscardTop();
            await CardsManager.Instance.ShuffleAndSaveCards();
            await HandsManager.Instance.DistributeHands();

            photonView.RPC(
                "InitializeUI",
                RpcTarget.All
            );
        }
    }

    [PunRPC]
    public async Task InitializeUI() {
        await HandsManager.Instance.InitializeHandIDs();
        await HandsManager.Instance.InitializeHandNamesUI();
        await HandsManager.Instance.InitializeHandsCardsUI();
        ScreenManager.Instance.SwitchTo("GameScreen");
    }

    public void SetUpScreenManager() {
        ScreenManager.Instance.screens.Add(SettingUpScreen);
        ScreenManager.Instance.screens.Add(GameScreen);
    }
}
