using UnityEngine;

public abstract class BlockEffect : ScriptableObject
{
    public virtual EffectRuntime CreateRuntime() => null;

    public virtual void BeforeHit(DamageContext damageContext, EffectRuntime runtime) { }
    public virtual void AfterHit(DamageContext damageContext, EffectRuntime runtime) { }
    public virtual void OnBroken(DamageContext damageContext, EffectRuntime runtime) { }
    public virtual void ModifyDrop(DamageContext damageContext, EffectRuntime runtime) { }

}

public abstract class EffectRuntime
{

}