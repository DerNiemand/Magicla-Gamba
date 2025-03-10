//using Godot;
//using System;

//public partial class GodotBoardData : GodotObject
//{
//    private BoardData boardState;

//    public BoardData State
//    {
//        get
//        {
//            return boardState;
//        }
//    }

//    public GodotBoardData(BoardData boardState)
//    {
//        this.boardState = boardState;
//    }
//}

//public partial class GodotDeckData : GodotObject
//{
//    private Godot.Collections.Array<GodotCardData> deck;
//    private int remainingCardCount;


//    private Godot.Collections.Array<GodotCardData> graveyard;

//    public DeckData Deck
//    {
//        get
//        {
//            DeckData retVal = new DeckData();
//            foreach (var card in this.deck)
//            {
//                retVal.deckCards.Enqueue(card.Card);
//            }
//            return retVal ;
//        }
//    }

//    public GodotDeckData(DeckData deckData)
//    {
//        deck = new Godot.Collections.Array<GodotCardData>();
//        graveyard = new Godot.Collections.Array<GodotCardData>();
//        foreach(var card in deckData.deckCards)
//        {
//            deck.Add(new GodotCardData(card));
//        }

//        foreach(var card in deckData.graveyard)
//        {
//            deck.Add(new GodotCardData(card));
//        }
//    }
//}

//public partial class GodotCardData: GodotObject
//{
//    public string name;

//    public int health;

//    public int damage;
//    public int damageModifier;

//    public int cost;

//    public string imageName;

//    public CardData Card
//    {
//        get
//        {
//            CardData retVal = new CardData();

//            retVal.name = name;
//            retVal.health = health;
//            retVal.damage = damage;
//            retVal.damageModifier = damageModifier;
//            retVal.cost = cost;
//            retVal.illustrationName = imageName;

//            return retVal;
//        }
//    }

//    public GodotCardData(CardData card)
//    {
//        name = card.name;
//        health = card.health;
//        damage = card.damage;
//        damageModifier = card.damageModifier;
//        cost = card.cost;
//        imageName = card.illustrationName;
//    }
//}
