using System;
using System.Collections.Generic;

public class DiceModel
{
    private Dictionary<EnergyType, int>[] faces = new Dictionary<EnergyType, int>[6]
    {
        new Dictionary<EnergyType, int>
        {
            { EnergyType.universalEnergy, 1 }
        },      
        new Dictionary<EnergyType, int>
        {
            { EnergyType.universalEnergy, 1 }
        },  
        new Dictionary<EnergyType, int>
        {
            { EnergyType.universalEnergy, 1 }
        },  
        new Dictionary<EnergyType, int>
        {
            { EnergyType.universalEnergy, 1 }
        },
        new Dictionary<EnergyType, int>
        {
            { EnergyType.universalEnergy, 1 }
        }, 
        new Dictionary<EnergyType, int>
        {
            { EnergyType.universalEnergy, 1 }
        }

    };

    public DiceModel(Dictionary<EnergyType, int>[] faces)
    {
        if(faces.Length == 6)
        {
            this.faces = faces;
        }
    }

    public Dictionary<EnergyType, int> RollDice()
    {
        Dictionary<EnergyType, int> retVal;

        int roll = Random.NextToSix();

        retVal = faces[roll];

        return retVal;
    }
}

