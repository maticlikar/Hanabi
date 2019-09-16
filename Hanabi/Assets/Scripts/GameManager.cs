using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour {
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public const int CARDS_IN_DECK = 50;

    public const int TWO_PLAYERS_DEAL = 5;
    public const int THREE_PLAYERS_DEAL = 5;
    public const int FOUR_PLAYERS_DEAL = 4;
    public const int FIVE_PLAYERS_DEAL = 4;

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    public int GetPlayersDeal(int playerCount) {
        // TODO: REMOVE. SHOULDN'T PLAY ALONE
        if (playerCount == 1) {
            return 4;
        }

        if (playerCount == 2) {
            return TWO_PLAYERS_DEAL;
        } else if (playerCount == 3) {
            return THREE_PLAYERS_DEAL;
        } else if (playerCount == 4) {
            return FOUR_PLAYERS_DEAL;
        } else if (playerCount == 5) {
            return FIVE_PLAYERS_DEAL;
        }

        return 0;
    }
}
