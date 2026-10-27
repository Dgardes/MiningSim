using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PickaxeAbility", menuName = "Scriptable Objects/Mining/Pickaxe Ability")]
public class PickaxeAbilitySO : ScriptableObject
{
    public EffectDisplaySO effectDisplay;
    public List<BlockEffect> blockEffects;
}
