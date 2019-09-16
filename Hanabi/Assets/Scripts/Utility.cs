using UnityEngine;
using System.Collections.Generic;

public class Utility : MonoBehaviour {
    private static Utility _instance;
    public static Utility Instance { get { return _instance; } }

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    public void DestroyAllChildren(Transform t) {
        foreach (Transform c in t) {
            Destroy(c.gameObject);
        }
    }

    public string PadNumber(string n) {
        if(int.Parse(n) < 10) {
            n = "0" + n;
        }

        return n;
    }

    public string ArrayToJson(object[] array) {
        string json = "{";

        for (int i = 0; i < array.Length; i++) {
            string n = i.ToString();

            n = PadNumber(n);

            json += "\"" + n + "\":";
            json += JsonUtility.ToJson(array[i]);

            if (i < array.Length - 1) {
                json += ",";
            }
        }

        json += "}";

        return json;
    }

    public string ArrayToJson(string[] array) {
        string json = "{";

        for (int i = 0; i < array.Length; i++) {
            string n = i.ToString();

            n = PadNumber(n);

            json += "\"" + n + "\":";
            json += "\"" + array[i] + "\"";

            if (i < array.Length - 1) {
                json += ",";
            }
        }

        json += "}";

        return json;
    }

    public void ShuffleArray(object[] array) {
        for (int i = array.Length - 1; i > 0; i--) {
            int j = Random.Range(0, i + 1);

            object temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }

    public List<Transform> GetAllChildren(Transform parent) {
        List <Transform> children = new List<Transform>();

        foreach (Transform t in parent) {
            children.Add(t);
        }

        return children;
    }
}
