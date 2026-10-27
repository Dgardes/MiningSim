using System;
using System.Collections.Generic;
using UnityEngine;

public class BlockLogic : MonoBehaviour
{
    private BlockData blockData;
    private SpriteRenderer spriteRenderer;

    private int durability;
    private Guid blockID;

    private Vector3Int worldPos;
    private TileLayer layer;

    private void Awake()
    {
        blockID = Guid.NewGuid();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(Vector3Int worldPos, TileLayer layer, BlockData blockData, int durability)
    {
        this.worldPos = worldPos;
        this.layer = layer;
        this.blockData = blockData;
        this.durability = durability;

        if (spriteRenderer != null && blockData != null)
        {
            spriteRenderer.sprite = blockData.blockSprite;
        }
    }

    public void takeDamage(DamageInfo info)
    {
        DamageContext context = new DamageContext 
        {
            blockData = this.blockData,  
            damageInfo = info
        };
        List<EquippedEffect> effects = info.effects;

        if(effects != null) foreach (var e in effects) { e.effect.BeforeHit(context, e.runtime); }
        
        DamageResult dmgRes = new DamageResult();
        dmgRes.healthBefore = durability;
        durability -= context.damageInfo.damage;
        dmgRes.healthAfter = durability;
        if(dmgRes.healthAfter <= 0) dmgRes.isDestroyed = true;

        context.damageResult = dmgRes;

        if(effects != null) foreach (var e in effects) { e.effect.AfterHit(context, e.runtime); }

        if(!context.damageResult.isDestroyed) return;
        
        if(effects != null) foreach(var e in effects) { e.effect.OnBroken(context, e.runtime); }

        

    }

    //
    //
    private void AwardDropToPlayer(PlayerStats minerStats)
    {
        if (blockData == null || blockData.dropItem == null) return;

        int finalDropAmount = CalculateDropAmount(minerStats);
        if (finalDropAmount <= 0) return;

        if (Inventory_PlayerInvLogic.instance != null)
        {
            int amountTaken = Inventory_PlayerInvLogic.instance.TryReceiveItem(blockData.dropItem, finalDropAmount);

            if (amountTaken > 0)
            {
                Inventory_PlayerInvLogic.instance.OnDataChanged();
            }
        }
    }


    //
    //
    private int CalculateDropAmount(PlayerStats minerStats)
    {
        if (blockData == null) return 0;

        int minBase = Mathf.Min(blockData.minBaseDrop, blockData.maxBaseDrop);
        int maxBase = Mathf.Max(blockData.minBaseDrop, blockData.maxBaseDrop);
        int baseDrop = UnityEngine.Random.Range(minBase, maxBase + 1);

        float luckLevel = 0.0f;
        if (minerStats != null)
        {
            luckLevel = minerStats.Luck;
        }
        else
        {
            var playerStats = FindAnyObjectByType<PlayerStats>();
            if (playerStats != null) luckLevel = playerStats.Luck;
        }

        float effectiveLuck = luckLevel * blockData.luckSensitivity;
        int bonusDrop = 0;

        if (effectiveLuck > 0f && blockData.maxBonusDrop > 0)
        {
            int currentBonusDrop = blockData.maxBonusDrop;

            while (currentBonusDrop > 0)
            {
                int C = currentBonusDrop;

                float chance = Mathf.Pow(0.5f, (float)C / effectiveLuck);
                float hit = UnityEngine.Random.value;

                if (hit < chance)
                {
                    bonusDrop = currentBonusDrop;
                    break;
                }

                currentBonusDrop--;
            }
        }

        return baseDrop + bonusDrop;
    }

    public Guid getBlockID() => blockID;
    public int getDurability() => durability;
    public BlockData getBlockData() => blockData;
    public Vector3Int getWorldPos() => worldPos;
    public TileLayer getLayer() => layer;

    
}