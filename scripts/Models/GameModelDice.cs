
using System.Collections.Generic;

public partial class GameModel
{
    readonly DiceModel[] Dice = new DiceModel[8]
    {
        new DiceModel(
            new Dictionary<EnergyType, int>[]
            {
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.universalEnergy,1}
                },
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.universalEnergy,1},
                    {EnergyType.summonEnergy, 1 }
                },
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.universalEnergy,1}
                },
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.universalEnergy,1},
                    {EnergyType.specialEnergy,1}
                },
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.universalEnergy,1}
                },
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.universalEnergy,1},
                    {EnergyType.spellEnergy,1}
                }
            }
        ),
        new DiceModel(
            new Dictionary<EnergyType, int>[]
            {
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.universalEnergy,1}
                },
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.universalEnergy,1},
                    {EnergyType.summonEnergy, 1 }
                },
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.universalEnergy,1}
                },
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.universalEnergy,1},
                    {EnergyType.specialEnergy,1}
                },
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.universalEnergy,1}
                },
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.universalEnergy,1},
                    {EnergyType.spellEnergy,1}
                }
            }
        ),
        new DiceModel(
            new Dictionary<EnergyType, int>[]
            {
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.spellEnergy,2}
                },
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.spellEnergy,1},
                    {EnergyType.specialEnergy,1 }
                },
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.specialEnergy,2}
                },
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.universalEnergy,1}
                },
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.spellEnergy,1}
                },
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.specialEnergy,1}
                }
            }
        ),
        new DiceModel(
            new Dictionary<EnergyType, int>[]
            {
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.spellEnergy,2}
                },
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.spellEnergy,1},
                    {EnergyType.specialEnergy,1 }
                },
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.specialEnergy,2}
                },
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.universalEnergy,1}
                },
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.spellEnergy,1}
                },
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.specialEnergy,1}
                }
            }
        ),
        new DiceModel(
            new Dictionary<EnergyType, int>[]
            {
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.summonEnergy,2}
                },
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.summonEnergy,1},
                    {EnergyType.spellEnergy,1 }
                },
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.spellEnergy,2}
                },
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.universalEnergy,1}
                },
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.summonEnergy,1}
                },
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.spellEnergy,1}
                }
            }
        ),
        new DiceModel(
            new Dictionary<EnergyType, int>[]
            {
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.summonEnergy,2}
                },
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.summonEnergy,1},
                    {EnergyType.spellEnergy,1 }
                },
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.spellEnergy,2}
                },
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.universalEnergy,1}
                },
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.summonEnergy,1}
                },
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.spellEnergy,1}
                }
            }
        ),
        new DiceModel(
            new Dictionary<EnergyType, int>[]
            {
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.summonEnergy,2}
                },
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.summonEnergy,1},
                    {EnergyType.specialEnergy,1 }
                },
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.specialEnergy,2}
                },
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.universalEnergy,1}
                },
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.summonEnergy,1}
                },
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.specialEnergy,1}
                }
            }
        ),
        new DiceModel(
            new Dictionary<EnergyType, int>[]
            {
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.summonEnergy,2}
                },
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.summonEnergy,1},
                    {EnergyType.specialEnergy,1 }
                },
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.specialEnergy,2}
                },
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.universalEnergy,1}
                },
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.summonEnergy,1}
                },
                new Dictionary<EnergyType, int>
                {
                    {EnergyType.specialEnergy,1}
                }
            }
        )
    };
}

