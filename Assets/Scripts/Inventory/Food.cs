﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Food", menuName = "Item/Consumable/Food")]
public class Food : Consumable
{
    public int healthRestore;

    public List<EffectObject> preBattleEffects;

    public override bool Consume(int i)
    {
        switch (consumptionType)
        {
            case ConsumptionType.Single:
                {
                    ConsumeFood(TeamManager.Instance.heroes.LivingMembers[i]);
                }
                break;
            case ConsumptionType.Party:
                {
                    foreach (Unit unit in TeamManager.Instance.heroes.LivingMembers)
                    {
                        ConsumeFood(unit);
                    }
                }
                break;
            case ConsumptionType.None:
                break;
        }

        InventoryManager.Instance.UpdateCharacterAttributes(HeroManager.Instance.CurrentHero(), -1);

        return true;
    }

    private void ConsumeFood(Unit unit)
    {
        if (healthRestore > 0)
        {
            AbilityValue abilityValue = new AbilityValue(null, null, 1, false, false, healthRestore, AbilitySchool.Healing, AbilityType.Protection, true, true, unit, unit, Color.green, true, null);

            abilityValue.CalculateValue();

            abilityValue.Trigger();
        }

        if (preBattleEffects.Count > 0)
        {
            foreach (EffectObject effectObject in preBattleEffects)
            {
                unit.effectManager.PreparePreBattleEffect(effectObject);
            }
        }
    }

    public override string GetTooltip(TooltipObject tooltipInfo)
    {
        string healthRestoreText = "";
        string effectText = "";
        string effectDescriptions = "";

        if (healthRestore > 0)
        {
            string text;

            if (consumptionType == ConsumptionType.Single)
                text = "Use: Restore {0} Health to a single member.";
            else
                text = "Use: Restore {0} Health to all members.";

            healthRestoreText = string.Format("<color={1}>\n" + text + "</color>", healthRestore, ColorDatabase.EffectColor());
        }

        if (preBattleEffects.Count > 0)
        {
            string text;

            List<string> effectNames = preBattleEffects.Select(x => x.name).ToList();

            if (consumptionType == ConsumptionType.Single)
                text = "Use: Apply {0} to a single member at the start of the next battle.";
            else
                text = "Use: Apply {0} to all members at the start of the next battle.";

            effectText = string.Format("<color={1}>\n" + text + "</color>", GeneralUtilities.JoinString(effectNames, ", ", " and ", ColorDatabase.EffectColor()), ColorDatabase.EffectColor());

            foreach (EffectObject effectObject in preBattleEffects)
            {
                effectDescriptions += string.Format("\n\n{0}", effectObject.GetCompleteTooltip(tooltipInfo));
                effectDescriptions += "\nDuration: " + EffectObject.DurationText(effectObject);
            }          
        }

        string stacks = "";

        if (stackable)
            stacks = string.Format("\nMaximum Stacks: {0}", maxStacks);
        else
            stacks = "\nMaximum Stacks: 1";

        return stacks +
               healthRestoreText +         
               effectText + 
               effectDescriptions;
    }
}
