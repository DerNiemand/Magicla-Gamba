using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
public struct CardData
{
    [JsonInclude]
    public string name;
    [JsonInclude]
    public CardType cardType;

    public EnergyType EnergyType
    {
        get
        {
            switch(cardType)
            {
                case CardType.hero:
                case CardType.summon:
                    return EnergyType.summonEnergy;

                case CardType.spell:
                    return EnergyType.spellEnergy;

                case CardType.field:
                case CardType.item:
                    return EnergyType.specialEnergy;

                default:
                    return EnergyType.universalEnergy;
            }
        }
    }

    [JsonInclude]
    public int health;

    [JsonInclude]
    public int damage;
    [JsonInclude]
    public int damageModifier;

    [JsonInclude]
    public int cost;
    [JsonInclude]
    public int retreatCost;

    [JsonInclude]
    public string abilityName;
    [JsonInclude]
    public int abilityStat;
    [JsonInclude]
    public int abilityCost;
    [JsonInclude]
    public string abilityDescription;

    [JsonInclude]
    public string illustrationName;
    [JsonInclude]
    public string backsideName;
    [JsonInclude]
    public string frameName;


    public override bool Equals([NotNullWhen(true)] object obj)
    {
        return base.Equals(obj);
    }

    public static bool operator ==(CardData left, CardData right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(CardData left, CardData right)
    {
        return !(left == right);
    }
}

public class CardModel
{
    private string name;

    private CardType cardType;

    private int health;

    private int damage;
    private int damageModifier;

    private int cost;
    private int retreatCost;

    private string abilityName;
    private int abilityStat;
    private int abilityCost;
    private string abilityDescription;

    private string illustrationName;
    private string backsideName;
    private string frameName;


    public CardData State
    {
        get
        {
            return new CardData
            {
                name = name,
                cardType = cardType,
                health = health,
                damage = damage,
                damageModifier = damageModifier,
                cost = cost,
                retreatCost = retreatCost,
                abilityName = abilityName,
                abilityStat = abilityStat,
                abilityCost = abilityCost,
                abilityDescription = abilityDescription,
                illustrationName = illustrationName,
                backsideName = backsideName,
                frameName = frameName
            };
        }
    }

    public CardModel(CardData cardData)
    {
        name = cardData.name;
        cardType = cardData.cardType;

        health = cardData.health;

        damage = cardData.damage;
        damageModifier = cardData.damageModifier;

        cost = cardData.cost;
        retreatCost = cardData.retreatCost;

        abilityName = cardData.abilityName;
        abilityStat = cardData.abilityStat;
        abilityCost = cardData.abilityCost;
        abilityDescription = cardData.abilityDescription;

        illustrationName = cardData.illustrationName;
        backsideName = cardData.backsideName;
        frameName = cardData.frameName;
    }

    public bool TakeDamage(int damage)
    {
        health -= damage;
        return health > 0;
    }

    public void TakeHeal(int healing)
    {
        health += healing;
    }
}
