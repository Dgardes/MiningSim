public class EquippedEffect
{
    public readonly BlockEffect effect;
    public readonly EffectRuntime runtime;

    public EquippedEffect(BlockEffect effect)
    {
        this.effect = effect;
        runtime = effect.CreateRuntime();
    }
}
