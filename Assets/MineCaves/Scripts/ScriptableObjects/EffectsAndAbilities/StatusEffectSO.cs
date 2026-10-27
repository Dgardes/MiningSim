using System.Collections.Generic;
using UnityEngine;

public enum EffectType
{
    StatModifier,
    DamageOverTime
}

public enum StatType
{
    None,
    MoveSpeed,
    JumpForce,
    MiningEfficiency,
    MiningRange,
    Luck,
    ViewDistance,
    SafeFallSpeed,
    FallDamageMultiplier,
    IncomingDamageMultiplier,
    MaxHealth
}

[System.Serializable]
public struct StatModifier
{
    public StatType stat;
    public float value;
}

[CreateAssetMenu(fileName = "Status Effect", menuName = "Scriptable Objects/Status Effect")]
public class StatusEffectSO :ScriptableObject
{
    [Header("UI Info")]
    public string effectName;
    public string description;
    public Sprite icon;

    [Header("Settings")]
    public EffectType type;
    public bool isPersistentUntilReset = false; 
    public float defaultDuration = 10f;         

    [Header("For Stat Modifier")]
    public List<StatModifier> modifiers = new List<StatModifier>();

    [Header("For Damage Over Time")]
    public float damagePerTick = 5f;
    public float tickInterval = 1f;

    public ActiveEffect CreateActiveInstance()
    {
        return new ActiveEffect(this);
    }
}