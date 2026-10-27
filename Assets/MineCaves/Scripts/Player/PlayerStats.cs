using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    [SerializeField] private float incomingDamageMultiplier = 1.0f;

    [Header("Stats and Buffs")]
    [SerializeField] private float miningRange = 2.5f;
    [SerializeField] private float miningEfficiency = 0.0f;
    [SerializeField] private float luck = 0.0f;             
    [SerializeField] private float viewDistance = 5.0f;

    [Header("Movement Stats")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7f;

    [Header("Fall & Environment Stats")]
    [SerializeField] private float safeFallSpeed = -12f;      
    [SerializeField] private float fallDamageMultiplier = 5f; 
    [SerializeField] private float maxUnderwaterTime = 10f;
    [SerializeField] private float currentUnderwaterTime = 10f;

    [Header("Saved pickaxe states")]
    private bool obsidianPickaxeBonusActive = false;
    private float mythrilPickaxeEfficiencyBonus = 0;

    private List<ActiveEffect> activeEffects = new List<ActiveEffect>();

    public event Action<float, float> OnHealthChanged;
    public event Action OnPlayerDied;
    public event Action<List<ActiveEffect>> OnEffectsChanged;

    private void Awake()
    {
        currentHealth = maxHealth;
        currentUnderwaterTime = maxUnderwaterTime;
    }

    private void Start()
    {
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Update()
    {
        HandleActiveEffects();
    }

    private void HandleActiveEffects()
    {
        if (activeEffects.Count == 0) return;

        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            var effect = activeEffects[i];
            if (effect.isPersistentUntilReset) continue;

            effect.remainingDuration -= Time.deltaTime;

            if (effect.type == EffectType.DamageOverTime)
            {
                if (Time.time >= effect.lastTickTime + effect.tickInterval)
                {
                    TakeDamage(effect.damagePerTick);
                    effect.lastTickTime = Time.time;
                }
            }

            if (effect.remainingDuration <= 0)
            {
                RemoveEffect(effect);
            }
        }
    }

    public void ClearMineResetBuffs()
    {
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            if (activeEffects[i].isPersistentUntilReset)
            {
                RemoveEffect(activeEffects[i]);
            }
        }
    }

    public void ApplyEffect(StatusEffectSO effectSO)
    {
        if (effectSO != null)
        {
            ApplyEffect(effectSO.CreateActiveInstance());
        }
    }

    public void ApplyEffect(ActiveEffect newEffect)
    {
        if (newEffect == null) return;

        if (newEffect.type == EffectType.StatModifier && newEffect.modifiers != null)
        {
            foreach (var mod in newEffect.modifiers)
            {
                ModifyStat(mod.stat, mod.value);
            }
        }

        activeEffects.Add(newEffect);
        OnEffectsChanged?.Invoke(activeEffects);
    }

    private void RemoveEffect(ActiveEffect effect)
    {
        if (effect.type == EffectType.StatModifier && effect.modifiers != null)
        {
            foreach (var mod in effect.modifiers)
            {
                ModifyStat(mod.stat, -mod.value);
            }
        }

        activeEffects.Remove(effect);
        OnEffectsChanged?.Invoke(activeEffects);
    }

    public void UpdateEffect(ActiveEffect effect)
    {
        if(effect == null) return;
        ActiveEffect effectToUpdate = null;
        foreach (var e in activeEffects)
        {
            if (e.sourceEffect == effect.sourceEffect)
            {
                effectToUpdate = e;
                OnEffectsChanged?.Invoke(activeEffects);
                break;
            }
        }

        if(effectToUpdate == null) { ApplyEffect(effect); return; }

        RemoveEffect(effectToUpdate);
        ApplyEffect(effect);

    }    

    public void ModifyStat(StatType stat, float amount)
    {
        switch (stat)
        {
            case StatType.MoveSpeed: moveSpeed += amount; break;
            case StatType.JumpForce: jumpForce += amount; break;
            case StatType.MiningEfficiency: miningEfficiency += amount; break;
            case StatType.MiningRange: miningRange += amount; break;
            case StatType.Luck: luck += amount; break;
            case StatType.ViewDistance: viewDistance += amount; break;
            case StatType.SafeFallSpeed: safeFallSpeed += amount; break;
            case StatType.FallDamageMultiplier: fallDamageMultiplier += amount; break;
            case StatType.IncomingDamageMultiplier: incomingDamageMultiplier += amount; break;
            
            case StatType.MaxHealth:
                maxHealth += amount;
                if (currentHealth > maxHealth) currentHealth = maxHealth;
                OnHealthChanged?.Invoke(currentHealth, maxHealth);
                break;
        }
    }


    public void TakeDamage(float rawDamage)
    {
        if (rawDamage <= 0 || currentHealth <= 0) return;

        float finalDamage = rawDamage * incomingDamageMultiplier;

        currentHealth = Mathf.Max(currentHealth - finalDamage, 0);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0) Die();
    }

    public void Heal(float amount)
    {
        if (amount <= 0 || currentHealth <= 0) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die() => OnPlayerDied?.Invoke();


    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public float MiningRange => miningRange;
    public float MiningEfficiency => miningEfficiency;
    public float Luck => luck;
    public float ViewDistance => viewDistance;
    public float MoveSpeed => moveSpeed;
    public float JumpForce => jumpForce;
    public float SafeFallSpeed => safeFallSpeed;
    public float FallDamageMultiplier => fallDamageMultiplier;
    public float IncomingDamageMultiplier => incomingDamageMultiplier;

    // Saved pickaxe states
    public bool ObsidianPickaxeBonusActive => obsidianPickaxeBonusActive;
    public float MythrilPickaxeEfficiencyBonus => mythrilPickaxeEfficiencyBonus;

    public void ModifyLuck(float amount) => ModifyStat(StatType.Luck, amount);
    public void ModifyEfficiency(float amount) => ModifyStat(StatType.MiningEfficiency, amount);
    public void ModifyViewDistance(float amount) => ModifyStat(StatType.ViewDistance, amount);
    public void ModifyMoveSpeed(float amount) => ModifyStat(StatType.MoveSpeed, amount);
    public void ModifyJumpForce(float amount) => ModifyStat(StatType.JumpForce, amount);
    public void ModifyMiningRange(float amount) => ModifyStat(StatType.MiningRange, amount);
    public void ModifySafeFallSpeed(float amount) => ModifyStat(StatType.SafeFallSpeed, amount);
    public void ModifyFallDamageMultiplier(float amount) => ModifyStat(StatType.FallDamageMultiplier, amount);
    public void ModifyIncomingDamageMultiplier(float amount) => ModifyStat(StatType.IncomingDamageMultiplier, amount);
    public void ModifyMaxHealth(float amount) => ModifyStat(StatType.MaxHealth, amount);

    //
}