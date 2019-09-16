using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;
using System;
using TMPro;
using System.Threading.Tasks;

public class FirebaseData : MonoBehaviour {
    private static FirebaseData _instance;
    public static FirebaseData Instance { get { return _instance; } }

    public DatabaseReference reference;

    public TMP_InputField usernameInputField;

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }

        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://hanabi-86ab5.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public async void RegisterAndConnect() {
        string username = usernameInputField.text;
        string uid = Guid.NewGuid().ToString();

        if (username.Length >= 3) {
            PlayerPrefs.SetString("uid", uid);

            await SaveNewUser(username, uid);
            await PhotonConnect.Instance.Connect(username, uid);
        }
    }

    public async Task SaveNewUser(string username, string uid) {
        await reference.Child("users").Child(uid).Child("username").SetValueAsync(username);
    }
}
