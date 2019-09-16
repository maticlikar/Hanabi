using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

public class PopUpWindow : MonoBehaviour {
    public Button[] buttons;

    public void OpenPopUp() {
        gameObject.SetActive(true);
    }

    public void ClosePopUp() {
        gameObject.SetActive(false);
    }

    public Button FindButton(string buttonName) {
        Button button = Array.Find(buttons, b => b.name.Equals(buttonName));

        return button;
    }

    public void AddListener(string buttonName, UnityAction action) {
        Button b  = FindButton(buttonName);

        b.onClick.AddListener(action);
    }

    public void RemoveListener(string buttonName, UnityAction action) {
        Button b = FindButton(buttonName);

        b.onClick.RemoveListener(action);
    }
}
