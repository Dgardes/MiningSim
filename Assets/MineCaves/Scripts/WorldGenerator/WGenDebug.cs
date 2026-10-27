using UnityEngine;

public class WGenDebug : MonoBehaviour
{
    [SerializeField] private WorldGenSettingsSO settings;
    [SerializeField] private ActiveChunkManager chunkLoader;
    [SerializeField] private float delay = 0.3f;

    private float reloadAt = -1f;

    private void OnEnable()
    {
        settings.Changed += ScheduleReload;
    }

    private void OnDisable()
    {
        settings.Changed -= ScheduleReload;
        reloadAt = -1f;
    }

    private void ScheduleReload()
    {
        reloadAt = Time.unscaledTime + delay;
    }

    public void ReloadWorld()
    {
        reloadAt = -1f;
        chunkLoader.unloadAll();
    }

    private void Update()
    {
        if (reloadAt > 0f && Time.unscaledTime >= reloadAt)
        {
            reloadAt = -1f;
            chunkLoader.unloadAll();
        }
    }
}
