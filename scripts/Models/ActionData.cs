
using System.Text.Json.Serialization;

public class ActionData
{
    [JsonInclude]
    public readonly PlayerId player;

    [JsonInclude]
    public readonly ActionType type;

    [JsonInclude]
    public readonly int instigatingBenchIndex;

    [JsonInclude]
    public readonly int targetedBenchIndex;

    [JsonInclude]
    public readonly CardData card;


    public ActionData(PlayerId player, ActionType type, int instigatingBenchIndex, int targetedBenchIndex, CardData card = new CardData())
    {
        this.player = player;
        this.type = type;
        this.instigatingBenchIndex = instigatingBenchIndex;
        this.targetedBenchIndex = targetedBenchIndex;
        this.card = card;
    }

    public override string ToString()
    {
        string retval = "";
        retval += player.Value.ToString() + ",";
        retval += type.ToString() + ",";
        retval += instigatingBenchIndex.ToString() + ",";
        retval += targetedBenchIndex.ToString() + ",";
        retval += card.ToString();
        return retval;
    }
}


public enum ActionType
{
    UnitPlayed = 1,
    SpellPlayed = 2,
    CardDrawn = 3,
    UnitAttacks = 4,
    UnitAbilityUsed = 5,
    ActiveCardSwitched = 6,
    GameWin = 7
}

