public class CardInfo {
    public string color;
    public int number;

    public CardInfo(string color, int number) {
        this.color = color;
        this.number = number;
    }

    public override string ToString() {
        return color + " " + number;
    }
}
