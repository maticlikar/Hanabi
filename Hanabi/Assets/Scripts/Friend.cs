using UnityEngine.UI;
using UnityEngine;

public class Friend : MonoBehaviour {
	private string uid;

	public GameObject friendListButton;
	public GameObject friendListStatus;

	public string UID { get { return uid; } set { uid = value; } }

	public override string ToString() {
		return uid;
	}

	public override bool Equals(object other) {
		Friend f = (Friend)other;

		return f.uid == this.uid;
	}

    public override int GetHashCode() {
        return base.GetHashCode();
    }
}
