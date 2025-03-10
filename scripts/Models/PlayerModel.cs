
using System.Text.Json.Serialization;

public struct PlayerData
{
    [JsonInclude]
    public string name;
    [JsonInclude]
    public PlayerId id;
    [JsonInclude]
    public DeckData deck;
    [JsonInclude]
    public HandData hand;
    [JsonInclude]
    public bool ready;
    [JsonInclude]
    public EnergyData energy;
}

public class PlayerModel
{
    private string name;

    private PlayerId id;

    private DeckModel deck;

    private bool ready;

    private EnergyModel energy = new EnergyModel();

    private bool disarmed;

    public PlayerData State
    {
        get
        {
            return new PlayerData { name = name, id = id, deck = deck.State, hand = deck.HandState, ready = ready, energy = energy.State };
        }
    }

    public PlayerModel(string name, PlayerId id, DeckModel deck)
    {
        this.name = name;
        this.id = id;
        this.deck = deck;
    }

    public void ReadyUp()
    {
        ready = true;
    }

    public void DrawCard()
    {
        deck.DrawCard();
    }

    public void RemoveHandCard(CardData card)
    {
        deck.RemoveHandCard(card);
    }

    public void ShuffleDeck()
    {
        deck.ShuffleDeck();
    }

    public void PutHandIntoDeck()
    {
        deck.PutHandIntoDeck();
    }

    public void SetEnergy(EnergyType type, int amount)
    {
        energy.SetEnergy(type, amount);
    }

    public void AddEnergy(EnergyType type, int amount)
    {
        energy.AddEnergy(type, amount);
    }

    public void SubtractEnergy(EnergyType type, int amount)
    {
        energy.SubtractEnergy(type, amount);
    }

    public void ClearEnergies()
    {
        energy.ClearEnergies();
    }

    public bool HasSufficientEnergies(EnergyType type, int amount)
    {
        return energy.HasSufficientEnergies(type, amount);
    }
}
