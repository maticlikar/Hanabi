using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using ExitGames.Client.Photon;
using TMPro;

public class PhotonReadyUp : MonoBehaviour, IOnEventCallback {
    private static PhotonReadyUp _instance;
    public static PhotonReadyUp Instance { get { return _instance; } }

    // The amount of people that clicked ready
    [NonSerialized]
    public int peopleReady;

    // Is this user ready
    [NonSerialized]
    public bool ready;

    private readonly byte ReadyToggleEvent = 0;

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }

        ResetReady();
    }

    public void ResetReady() {
        peopleReady = 0;
        ready = false;
    }

    public void OnEnable() {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public void OnDisable() {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void ReadyToggle() {
        ready = !ready;

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        SendOptions sendOptions = new SendOptions { Reliability = true };

        string uid = PlayerPrefs.GetString("uid");

        GameObject roomMember = MainScreenManager.Instance.FindRoomMember(uid);

        if (ready) {
            MainScreenManager.Instance.readyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Un-Ready";
            PhotonNetwork.RaiseEvent(ReadyToggleEvent, "1 " + PlayerPrefs.GetString("uid"), raiseEventOptions, sendOptions);
        } else {
            MainScreenManager.Instance.readyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Ready Up";
            PhotonNetwork.RaiseEvent(ReadyToggleEvent, "-1 " + PlayerPrefs.GetString("uid"), raiseEventOptions, sendOptions);
        }
    }

    public void OnEvent(EventData photonEvent) {
        byte eventCode = photonEvent.Code;

        if (eventCode == ReadyToggleEvent) {
            string[] data = ((string)photonEvent.CustomData).Split(' ');

            int inc = int.Parse(data[0]);
            string uid = data[1];

            GameObject roomMember = MainScreenManager.Instance.FindRoomMember(uid);

            if(inc > 0) {
                roomMember.GetComponent<RoomMember>().readyText.text = "Ready";
            } else {
                roomMember.GetComponent<RoomMember>().readyText.text = "Not Ready";
            }

            peopleReady += inc;

            if (peopleReady == PhotonNetwork.CurrentRoom.PlayerCount) {
                PhotonConnect.Instance.Play();
            }
        }
    }
}
