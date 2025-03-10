
using Godot;
using System;
using System.Text.Json.Serialization;

public struct BoardData
{
    [JsonInclude]
    public BoardCardData ownBoardCards;
    [JsonInclude]
    public BoardCardData enemyBoardCards;
}



public class BoardModel
{
    private BoardCardModel player1BoardCards = new BoardCardModel();
    private BoardCardModel player2BoardCards = new BoardCardModel();

    public BoardData GetState(bool Player1)
    {
        if(Player1)
        {
            return new BoardData { ownBoardCards = player1BoardCards.State, enemyBoardCards = player2BoardCards.State };
        }

        return new BoardData { ownBoardCards = player2BoardCards.State, enemyBoardCards = player1BoardCards.State };
    }

    public bool TryPlayCard(bool player1, CardData card, int benchIndex)
    {
        if(player1)
        {
            return player1BoardCards.TryPlayCard(card, benchIndex);
        }
        return player2BoardCards.TryPlayCard(card, benchIndex);
    }

    public bool TryAttack(bool player1, int damage, int targetIndex, out bool alive)
    {
        if(player1)
        {
            return player2BoardCards.TryAttackCard(targetIndex, damage, out alive);
        }

        return player1BoardCards.TryAttackCard(targetIndex, damage, out alive);
    }

    public bool TryHeal(bool player1, CardData card, int targetIndex, int amount)
    {
        if(player1)
        {
            return player1BoardCards.TryHealCard(card, targetIndex, amount);
        }
        return player2BoardCards.TryHealCard(card, targetIndex, amount);
    }

    public bool SideContainsCard(bool player1, int benchIndex, out CardData card)
    {
        card = new CardData();
        if(player1)
        {
            return player1BoardCards.HasCardAtIndex(benchIndex,out card);
        }
        return player2BoardCards.HasCardAtIndex(benchIndex,out card);
    }

    public bool SideContainsHeroCard(bool player1)
    {

        if(player1)
        {
            return player1BoardCards.HasHeroCard();
        }
        return player2BoardCards.HasHeroCard();
    }

    internal bool TrySwitchActiveCard(bool player1, int benchIndex)
    {
        if(player1)
        {
            return player1BoardCards.TrySwitchActiveCard(benchIndex);
        }
        return player2BoardCards.TrySwitchActiveCard(benchIndex);
    }
}
