
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public struct BoardCardData
{
    [JsonInclude]
    public CardData activeCard;
    [JsonInclude]
    public bool hasActiveCard;
    [JsonInclude]
    public Dictionary<int, CardData> benchCards;

}

public class BoardCardModel
{
    private CardModel activeCard;
    private Dictionary<int, CardModel> benchCards = new Dictionary<int, CardModel>();

    public BoardCardData State
    {
        get
        {
            Dictionary<int, CardData> benchCardsState = new Dictionary<int, CardData>();
            foreach(var entry in benchCards)
            {
                benchCardsState[entry.Key] = entry.Value.State;
            }
            CardData activeCardState;
            if(activeCard != null)
            {
                activeCardState = activeCard.State;
            }
            else
            {
                activeCardState = new CardData();
            }

            return new BoardCardData { activeCard = activeCardState, hasActiveCard = activeCard != null, benchCards = benchCardsState };
        }
    }

    public bool TryPlayCard(CardData card, int benchIndex)
    {
        if(benchIndex == -1)
        {
            if(activeCard == null)
            {
                activeCard = new CardModel(card);
                return true;
            }
        }
        else if(!benchCards.ContainsKey(benchIndex))
        {
            benchCards[benchIndex] = new CardModel(card);
            return true;
        }


        return false;
    }

    public bool TryAttackCard(int benchIndex, int Damage, out bool alive)
    {
        alive = true;
        if(benchIndex == -1 && activeCard != null)
        {
            alive = activeCard.TakeDamage(Damage);
            if(!alive)
            {
                activeCard = null;
            }
            return true;
        }
        else if(benchCards.ContainsKey(benchIndex))
        {
            alive = benchCards[benchIndex].TakeDamage(Damage);
            if(!alive)
            {
                benchCards.Remove(benchIndex);
            }
            return true;
        }


        return false;
    }

    public bool HasCardAtIndex(int benchIndex, out CardData card)
    {
        card = new CardData();
        if( benchIndex  == -1 && activeCard != null)
        {
            card = activeCard.State;
            return true;
        }
        if (benchCards.ContainsKey(benchIndex))
        {
            card = benchCards[benchIndex].State;
            return true;
        }
        return false;
    }

    public bool HasHeroCard()
    {
        if(activeCard != null)
        {
            if(activeCard.State.cardType == CardType.hero)
            {
                return true;
            }
        }

        foreach(var card in benchCards.Values)
        {
            if(card.State.cardType == CardType.hero)
            {
                return true;
            }
        }

        return false;
    }

    internal bool TryHealCard(CardData card, int targetIndex, int amount)
    {
        if(targetIndex == -1 && activeCard != null)
        {
            activeCard.TakeHeal(amount);
            return true;
        }
        else if(benchCards.ContainsKey(targetIndex))
        {
            benchCards[targetIndex].TakeHeal(amount);
            return true;
        }

        return false;
    }

    internal bool TrySwitchActiveCard(int benchIndex)
    {
        if(!benchCards.ContainsKey(benchIndex))
        {
            return false;
        }

        var previousActiveCard = activeCard;
        activeCard = benchCards[benchIndex];
        if(previousActiveCard != null)
        {
        benchCards[benchIndex] = previousActiveCard;
        }
        else
        {
            benchCards.Remove(benchIndex);
        }

        return true;
    }
}
