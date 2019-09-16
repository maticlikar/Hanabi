using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Persistence : MonoBehaviour {
    private void Awake() {
        DontDestroyOnLoad(this);
    }
}
