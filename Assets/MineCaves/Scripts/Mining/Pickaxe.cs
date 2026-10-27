using System.Collections.Generic;
using UnityEngine;

public class Pickaxe 
{
    public PickaxeSO pickaxeSO;
    public List<EnchantmerntSO> enchants = new List<EnchantmerntSO>();

    public Pickaxe(PickaxeSO so, List<EnchantmerntSO> e)
    {
        pickaxeSO = so;
        enchants = new List<EnchantmerntSO>(e);
    }

    public void changeEnchants(List<EnchantmerntSO> newEnchants)
    {
        if(newEnchants.Count > pickaxeSO.maxEnchantsCount) return;
        enchants = new List<EnchantmerntSO>(newEnchants);
    }

}
