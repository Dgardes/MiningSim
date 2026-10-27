using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum DebugStat
{
    MoveSpeed,
    JumpForce,
    MiningRange,
    MiningEfficiency,
    Luck,
    ViewDistance,
    MaxHealth,
    SafeFallSpeed,
    FallDamageMultiplier,
    IncomingDamageMultiplier
}

public class DebugStatRow : MonoBehaviour
{
    [SerializeField] private DebugStat stat;
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private float min = 0f;
    [SerializeField] private float max = 20f;

    private PlayerStats stats;
    private bool listening;

    public void Setup(DebugStat newStat, float newMin, float newMax)
    {
        stat = newStat;
        min = newMin;
        max = newMax;
    }

    public void Bind(PlayerStats playerStats)
    {
        stats = playerStats;

        if (!listening)
        {
            slider.onValueChanged.AddListener(OnSliderChanged);
            listening = true;
        }

        slider.minValue = min;
        slider.maxValue = max;
        Refresh();
    }

    public void Refresh()
    {
        if (stats == null) return;
        slider.SetValueWithoutNotify(GetValue());
        UpdateLabel();
    }

    private void OnSliderChanged(float value)
    {
        if (stats == null) return;
        float delta = value - GetValue();
        Modify(delta);
        UpdateLabel();
    }

    private void UpdateLabel()
    {
        label.text = stat + ": " + GetValue().ToString("0.##");
    }

    private float GetValue()
    {
        switch (stat)
        {
            case DebugStat.MoveSpeed: return stats.MoveSpeed;
            case DebugStat.JumpForce: return stats.JumpForce;
            case DebugStat.MiningRange: return stats.MiningRange;
            case DebugStat.MiningEfficiency: return stats.MiningEfficiency;
            case DebugStat.Luck: return stats.Luck;
            case DebugStat.ViewDistance: return stats.ViewDistance;
            case DebugStat.MaxHealth: return stats.MaxHealth;
            case DebugStat.SafeFallSpeed: return stats.SafeFallSpeed;
            case DebugStat.FallDamageMultiplier: return stats.FallDamageMultiplier;
            case DebugStat.IncomingDamageMultiplier: return stats.IncomingDamageMultiplier;
        }
        return 0f;
    }

    private void Modify(float delta)
    {
        switch (stat)
        {
            case DebugStat.MoveSpeed: stats.ModifyMoveSpeed(delta); break;
            case DebugStat.JumpForce: stats.ModifyJumpForce(delta); break;
            case DebugStat.MiningRange: stats.ModifyMiningRange(delta); break;
            case DebugStat.MiningEfficiency: stats.ModifyEfficiency(delta); break;
            case DebugStat.Luck: stats.ModifyLuck(delta); break;
            case DebugStat.ViewDistance: stats.ModifyViewDistance(delta); break;
            case DebugStat.MaxHealth: stats.ModifyMaxHealth(delta); break;
            case DebugStat.SafeFallSpeed: stats.ModifySafeFallSpeed(delta); break;
            case DebugStat.FallDamageMultiplier: stats.ModifyFallDamageMultiplier(delta); break;
            case DebugStat.IncomingDamageMultiplier: stats.ModifyIncomingDamageMultiplier(delta); break;
        }
    }
}
