using System.Collections.Generic;
using System.Text.Json.Serialization;

public struct DeckData
{
    [JsonInclude]
    public Queue<CardData> deckCards;

    [JsonInclude]
    public List<CardData> graveyard;
}

public struct HandData
{
    [JsonInclude]
    public List<CardData> handCards;
}

public class DeckModel
{

    private Queue<CardData> deckCards;

    private List<CardData> graveyard;

    private List<CardData> handCards;
    public DeckData State
    {
        get
        {
            return new DeckData { deckCards = deckCards, graveyard = graveyard };
        }
    }
    public HandData HandState
    {
        get
        {
            return new HandData { handCards = handCards };
        }
    }

    public DeckModel(List<CardData> deckCards)
    {
        deckCards = Shuffles.FisherYatesShuffle(deckCards);
        this.deckCards = new Queue<CardData>();
        foreach(CardData card in deckCards)
        {
            this.deckCards.Enqueue(card);
        }

        graveyard = new List<CardData>();
        handCards = new List<CardData>();
    }

    public void DrawCard()
    {
        if(deckCards.TryDequeue(out var drawnCard))
        {
            handCards.Add(drawnCard);
        }
    }
    public void RemoveHandCard(CardData card)
    {
        handCards.Remove(card);
    }

    public void ShuffleDeck()
    {
        var tempDeck = new List<CardData>();
        while(deckCards.TryDequeue(out var card))
        {
            tempDeck.Add(card);
        }
        tempDeck = Shuffles.FisherYatesShuffle(tempDeck);
        foreach(CardData card in tempDeck)
        {
            deckCards.Enqueue(card);
        }
    }

    public void PutHandIntoDeck()
    {
        for(int i = handCards.Count - 1; i >= 0; i--)
        {
            var card = handCards[i];
            handCards.Remove(card);
            deckCards.Enqueue(card);
        }
    }
}
