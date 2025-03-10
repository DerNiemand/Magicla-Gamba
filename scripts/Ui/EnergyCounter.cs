using Godot;
using System.Collections.Generic;
using System;

public partial class EnergyCounter : HBoxContainer
{
    [Export]
    TextureRect universalEnergiesIcon;

    [Export]
    TextureRect spellEnergiesIcon;

    [Export]
    TextureRect summonEnergiesIcon;

    [Export]
    TextureRect specialEnergiesIcon;

    [Export]
    int iconSize;

    public void SetEnergies(Dictionary<EnergyType, int> energies)
    {
        Vector2 size;

        size = universalEnergiesIcon.Size;
        size.X = energies[EnergyType.universalEnergy] * iconSize;
        universalEnergiesIcon.CustomMinimumSize = size;
        universalEnergiesIcon.Visible = energies[EnergyType.universalEnergy] > 0;

        size = spellEnergiesIcon.Size;
        size.X = energies[EnergyType.spellEnergy] * iconSize;
        spellEnergiesIcon.CustomMinimumSize = size;
        spellEnergiesIcon.Visible = energies[EnergyType.spellEnergy] > 0;

        size = summonEnergiesIcon.Size;
        size.X = energies[EnergyType.summonEnergy] * iconSize;
        summonEnergiesIcon.CustomMinimumSize = size;
        summonEnergiesIcon.Visible = energies[EnergyType.summonEnergy] > 0;

        size = specialEnergiesIcon.Size;
        size.X = energies[EnergyType.specialEnergy] * iconSize;
        specialEnergiesIcon.CustomMinimumSize = size;
        specialEnergiesIcon.Visible = energies[EnergyType.specialEnergy] > 0;

        QueueSort();
    }
}
