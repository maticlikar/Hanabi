using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Firebase.Database;

public class CardsManager : MonoBehaviour {
    private static CardsManager _instance;
    public static CardsManager Instance { get { return _instance; } }

    public GameObject cardPrefab;

    public Transform draggingParent;

    public Color white;
    public Color yellow;
    public Color green;
    public Color blue;
    public Color red;

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    public async Task ShuffleAndSaveCards() {
        print("ShuffleAndSaveCards");

        CardInfo[] cards = GetAllCards().ToArray();
        Utility.Instance.ShuffleArray(cards);

        string cardJson = Utility.Instance.ArrayToJson(cards);

        string latestRoomPath = "user2latest_room/" + PlayerPrefs.GetString("uid");
        DataSnapshot latestRoomSnapshot = await FirebaseDatabase.DefaultInstance.GetReference(latestRoomPath).GetValueAsync();

        string currentRoom = latestRoomSnapshot.Value.ToString();

        await FirebaseData.Instance.reference.Child("active_rooms").Child(currentRoom).Child("deck").SetRawJsonValueAsync(cardJson);
        print("/ShuffleAndSaveCards");
    }

    public List<CardInfo> GetAllCards() {
        List<CardInfo> cards = new List<CardInfo>();

        cards.AddRange(GetAllCardsWithColor("white"));
        cards.AddRange(GetAllCardsWithColor("yellow"));
        cards.AddRange(GetAllCardsWithColor("green"));
        cards.AddRange(GetAllCardsWithColor("blue"));
        cards.AddRange(GetAllCardsWithColor("red"));

        return cards;
    }

    public List<CardInfo> GetAllCardsWithColor(string color) {
        List<CardInfo> cards = new List<CardInfo>();

        cards.Add(new CardInfo(color, 1));
        cards.Add(new CardInfo(color, 1));
        cards.Add(new CardInfo(color, 1));

        cards.Add(new CardInfo(color, 2));
        cards.Add(new CardInfo(color, 2));

        cards.Add(new CardInfo(color, 3));
        cards.Add(new CardInfo(color, 3));

        cards.Add(new CardInfo(color, 4));
        cards.Add(new CardInfo(color, 4));

        cards.Add(new CardInfo(color, 5));

        return cards;
    }

    public List<CardInfo> DataSnapshotToCards(DataSnapshot cardsDataSnapshot) {
        List<DataSnapshot> dataSnapshotOfCards = cardsDataSnapshot.Children.ToList();
        List<CardInfo> cards = new List<CardInfo>();

        // Transform the datasnapshots into an array of cards to deal with them easier
        for (int i = 0; i < dataSnapshotOfCards.Count; i++) {
            CardInfo c = JsonToCard(dataSnapshotOfCards[i]);
            cards.Add(c);
        }

        return cards;
    }

    public CardInfo JsonToCard(DataSnapshot cardDataSnapshot) {
        string newCardColor = cardDataSnapshot.Child("color").Value.ToString();
        int newCardNumber = int.Parse(cardDataSnapshot.Child("number").Value.ToString());

        CardInfo c = new CardInfo(newCardColor, newCardNumber);

        return c;
    }

    public void InstantiateNewCard(CardInfo c, Transform parent) {
        Card card = Instantiate(cardPrefab, parent, false).GetComponent<Card>();

        if (parent.name.Equals("MyHand")) {
            card.gameObject.AddComponent<DraggableCard>();
        }

        card.color = c.color;
        card.number = c.number;

        card.backgroundImage.color = StringToColor(c.color);
        card.numberText.text = c.number.ToString();
    }

    public Color StringToColor(string c) {
        if(c.Equals("white")) {
            return white;
        } else if(c.Equals("yellow")) {
            return yellow;
        } else if (c.Equals("green")) {
            return green;
        } else if (c.Equals("blue")) {
            return blue;
        } else if (c.Equals("red")) {
            return red;
        }

        return Color.black;
    }
}
