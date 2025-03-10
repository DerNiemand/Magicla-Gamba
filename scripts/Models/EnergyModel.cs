using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public struct EnergyData
{
    [JsonInclude]
    public Dictionary<EnergyType, int> energies;
}

public class EnergyModel
{
    private Dictionary<EnergyType, int> energies = new Dictionary<EnergyType, int>()
    {
        {EnergyType.universalEnergy, 0 },
        {EnergyType.spellEnergy, 0 },
        {EnergyType.summonEnergy, 0 },
        {EnergyType.specialEnergy, 0 }
    };


    public EnergyData State
    {
        get
        {
            return new EnergyData { energies = energies };
        }
    }

    public void SetEnergy(EnergyType type, int energy)
    {
        energies[type] = energy;
    }

    public void AddEnergy(EnergyType type, int energy)
    {
        energies[type] += energy;
    }

    public void SubtractEnergy(EnergyType type, int energy)
    {
        var typeSubtract = Math.Min(energies[type], energy);
        var univSubtract = Math.Max(energy - energies[type],0);
        energies[type] -= typeSubtract;
        energies[EnergyType.universalEnergy] -= univSubtract;
    }

    public void ClearEnergies()
    {
        foreach(var type in energies.Keys)
        {
            energies[type] = 0;
        }
    }

    public bool HasSufficientEnergies(EnergyType type, int amount)
    {
        if (type != EnergyType.universalEnergy)
        {
            return (energies[EnergyType.universalEnergy] + energies[type]) >= amount;
        }
        return energies[EnergyType.universalEnergy] >= amount;
    }
}

public enum EnergyType
{
    universalEnergy,
    spellEnergy,
    summonEnergy,
    specialEnergy
}

