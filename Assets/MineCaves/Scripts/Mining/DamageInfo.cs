using System.Collections.Generic;
using UnityEngine;

public enum DamageSource
{ 
    pickaxe,
    explosion,
    dot,
}

public class DamageInfo 
{
    public int damage { get; set; }
    public DamageSource source { get; set; }
    public PlayerStats playerStats { get; set; }
    public Pickaxe pickaxe { get; set; }
    public List<EquippedEffect> effects { get; set; }
    public Vector3 miningDirection { get; set; }
}
