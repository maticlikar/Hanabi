using UnityEngine;
using System.Collections.Generic;

public class PopUpsManager : MonoBehaviour {
    private static PopUpsManager _instance;
    public static PopUpsManager Instance { get { return _instance; } }

    public List<PopUpWindow> popUps;

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    public PopUpWindow GetPopUp(string popUpName) {
        PopUpWindow popUp = popUps.Find((p) => p.name == popUpName);

        return popUp;
    }
}
