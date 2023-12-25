public enum VeggieType
{
    cabbage,
    carrot,
    corn,
    cucumber,
    eggplant,
    green_onion,
    onion,
    pepper,
    potato,
    tomato,
    bug
}

public class Veggie : MoveableNodeAction
{
    public VeggieType veggiesType;

    public bool isMatched;
    public Veggie(int x, int y) : base(x, y) { }
}
