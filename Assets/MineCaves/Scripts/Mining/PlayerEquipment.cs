using UnityEngine;
using System.Collections.Generic;

public class PlayerEquipment : MonoBehaviour
{
    [SerializeField] Inventory_PlayerInvLogic inventory;
    [SerializeField] PlayerStats playerStats;

    public Pickaxe equipedPickaxe;
    public BackPackSO backpack;

    public List<EquippedEffect> effects = new List<EquippedEffect>();

    public bool EquipBackPackIfCan(BackPackSO newBackpack)
    {
        return false;
    }

    public void EquipPickaxe(Pickaxe pickaxe)
    {
        if (equipedPickaxe != null) UnEquipPickaxe();

        playerStats.ModifyEfficiency(pickaxe.pickaxeSO.bonusSpeed);
        if(pickaxe.pickaxeSO.pickaxeAbility != null)
        { 
            foreach (var e in pickaxe.pickaxeSO.pickaxeAbility.blockEffects) { effects.Add(new EquippedEffect(e)); }
        }

        foreach(var e in pickaxe.enchants)
        {
            foreach (var effect in e.blockEffects) { effects.Add(new EquippedEffect(effect)); }
            foreach (var mod in e.modifiers) { playerStats.ModifyStat(mod.stat, mod.value); }
        }
        equipedPickaxe = pickaxe;
    }

    public void UnEquipPickaxe()
    {
        if(equipedPickaxe == null) return;
        playerStats.ModifyEfficiency(-equipedPickaxe.pickaxeSO.bonusSpeed);
        effects.Clear();
        foreach(var e in equipedPickaxe.enchants)
        {
            foreach (var mod in e.modifiers) { playerStats.ModifyStat(mod.stat, -mod.value); }
        }
        equipedPickaxe = null;
    }

}
