using UnityEngine;
using System;
using System.Collections.Generic;
using Firebase.Database;
using System.Threading.Tasks;

public class ScreenManager : MonoBehaviour {
    private static ScreenManager _instance;
    public static ScreenManager Instance { get { return _instance; } }

    private static GameObject activeScreen;
    public static GameObject ActiveScreen { get { return activeScreen; } }

    // List of all screens (screens[0] should be what appears first. screens[1] is the alternative)
    public List<GameObject> screens;

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    public async void Start() {
        if (!HasAccount()) {
            activeScreen = screens[0];
            activeScreen.SetActive(true);
        } else {
            activeScreen = screens[1];
            activeScreen.SetActive(true);

            await GetUsernameAndConnect();
        }
    }

    public bool HasAccount() {
        return !string.IsNullOrEmpty(PlayerPrefs.GetString("uid", ""));
    }

    public async Task GetUsernameAndConnect() {
        string uid = PlayerPrefs.GetString("uid");

        string userPath = "users/" + uid;
   
        DataSnapshot usernameSnapshot  = await FirebaseData.Instance.reference.Child(userPath).GetValueAsync();
        string username = usernameSnapshot.Child("username").Value.ToString();

        await PhotonConnect.Instance.Connect(username, PlayerPrefs.GetString("uid", ""));
    }

    public void SwitchTo(string screenName) {
        GameObject screen = Array.Find(screens.ToArray(), (s) => s != null && s.name == screenName);

        if (activeScreen != null) {
            activeScreen.SetActive(false);
        }

        screen.SetActive(true);

        activeScreen = screen;
    }
}
