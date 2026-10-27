using UnityEngine;

[CreateAssetMenu(fileName = "MithrilEffect", menuName = "Scriptable Objects/Mining/Effects/Mithril")]
public class MithrilPickaxeEffect : BlockEffect
{
    [SerializeField] StatusEffectSO mithrilBuff;

    public override void AfterHit(DamageContext damageContext, EffectRuntime runtime)
    {

        float maxBonusSpeed = 0.6f;
        float bonusPerHit = 0.05f;
        float timeBeforeReset = 3f;

        MithrilPickaxeRuntime er = (MithrilPickaxeRuntime)runtime;
        
        if(er.lastHitTime + timeBeforeReset <= Time.time) er.currentBonus = 0;

        if(er.currentBonus != maxBonusSpeed)
        { 
            er.currentBonus += bonusPerHit;
            er.currentBonus = Mathf.Min(er.currentBonus, maxBonusSpeed);
        }

        ActiveEffect effect = new ActiveEffect(mithrilBuff);
        effect.remainingDuration = timeBeforeReset;
        StatModifier mod = new StatModifier { stat = StatType.MiningEfficiency, value = er.currentBonus};
        
        effect.modifiers.Clear();
        effect.modifiers.Add(mod);

        damageContext.damageInfo.playerStats.UpdateEffect(effect);
        er.lastHitTime = Time.time;
    }

    public override EffectRuntime CreateRuntime()
    {
        return new MithrilPickaxeRuntime();
    }
}
