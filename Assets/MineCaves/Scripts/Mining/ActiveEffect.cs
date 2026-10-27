using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActiveEffect
{
    public StatusEffectSO sourceEffect; 

    public string effectName;
    public string description;
    public Sprite icon;
    public EffectType type;

    public bool isPersistentUntilReset; 

    public List<StatModifier> modifiers = new List<StatModifier>();

    public float damagePerTick;
    public float tickInterval = 1f;
    public float lastTickTime;

    public float remainingDuration;

    public ActiveEffect(StatusEffectSO data)
    {
    this.sourceEffect = data;
    this.effectName = data.effectName;
    this.description = data.description;
    this.icon = data.icon;
    this.type = data.type;
    this.isPersistentUntilReset = data.isPersistentUntilReset; 
    this.remainingDuration = data.defaultDuration;
    this.modifiers = new List<StatModifier>(data.modifiers);
    this.damagePerTick = data.damagePerTick;
    this.tickInterval = data.tickInterval;
    this.lastTickTime = Time.time;
    }
}