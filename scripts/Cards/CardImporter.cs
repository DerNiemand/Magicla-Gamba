using Godot;
using System.Collections.Generic;
using System;
using System.Linq;


public partial class CardImporter : Node
{

    private List<CardData> cards = new List<CardData>();

    public List<CardData> Cards
    {
        get { return cards.ToList(); }
    }
    public void ImportCards()
    {
        var file = FileAccess.Open("res://Resources/cards.txt", FileAccess.ModeFlags.Read);
        file.GetCsvLine();

        while(file.GetPosition() < file.GetLength())
        {
            var cardCsv = file.GetCsvLine();
            var card = new CardData();
            card.name = cardCsv[1];
            switch(Enum.TryParse(cardCsv[2],true, out CardType type))
            {
                case true:
                    card.cardType = type;
                    break;
                case false:
                    card.cardType = CardType.none;
                    GD.PrintErr("couldnt read the card type");
                    break;
            }
            card.cost = int.Parse(cardCsv[3]);
            card.health = int.Parse(cardCsv[4]);
            card.damage = int.Parse(cardCsv[5]);
            card.retreatCost = int.Parse(cardCsv[6]);
            card.abilityName = cardCsv[7];
            card.abilityStat = int.Parse(cardCsv[8]);
            card.abilityCost = int.Parse(cardCsv[9]);
            card.abilityDescription = cardCsv[11];
            cards.Add(card);
        }
    }

    public bool TryGetCardByName(string name, out CardData card)
    {
        card = new CardData();
        foreach(var data in cards)
        {
            if(data.name == name)
            {
                card = data;
                return true;
            }
        }
        return false;
    }
}
