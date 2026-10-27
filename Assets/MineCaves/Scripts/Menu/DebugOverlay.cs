using System.Text;
using TMPro;
using UnityEngine;

public class DebugOverlay : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private Transform player;
    [SerializeField] private ChunkGenerator generator;
    [SerializeField] private float refreshInterval = 0.25f;

    private readonly StringBuilder sb = new StringBuilder(256);
    private int frames;
    private float timer;
    private float worstFrame;

    private void OnEnable()
    {
        GameSettings.Changed += ApplyVisibility;
        ApplyVisibility();
    }

    private void OnDisable()
    {
        GameSettings.Changed -= ApplyVisibility;
    }

    private void ApplyVisibility()
    {
        if (panelRoot != null) panelRoot.SetActive(GameSettings.ShowDebugOverlay);
    }

    private void Update()
    {
        if (panelRoot == null || !panelRoot.activeSelf) return;

        frames++;
        timer += Time.unscaledDeltaTime;
        if (Time.unscaledDeltaTime > worstFrame) worstFrame = Time.unscaledDeltaTime;
        if (timer < refreshInterval) return;

        float fps = frames / timer;
        float ms = timer * 1000f / frames;
        float worstMs = worstFrame * 1000f;
        frames = 0;
        timer = 0f;
        worstFrame = 0f;

        sb.Clear();
        sb.Append("FPS: ").Append(Mathf.RoundToInt(fps)).Append("  (").Append(ms.ToString("0.0")).Append(" ms)\n");
        sb.Append("Worst frame: ").Append(worstMs.ToString("0.0")).Append(" ms\n");

        if (player != null)
        {
            Vector3 pos = player.position;
            ChunkCord chunk = ChunkCord.worldToCell(pos);
            sb.Append("Depth: ").Append(Mathf.Max(0, -Mathf.FloorToInt(pos.y))).Append('\n');
            sb.Append("Pos: ").Append(Mathf.FloorToInt(pos.x)).Append(", ").Append(Mathf.FloorToInt(pos.y)).Append('\n');
            sb.Append("Chunk: ").Append(chunk.x).Append(", ").Append(chunk.y).Append('\n');
        }

        if (generator != null) sb.Append("Seed: ").Append(generator.Seed).Append('\n');
        sb.Append("TimeScale: ").Append(Time.timeScale.ToString("0.##"));

        infoText.SetText(sb);
    }
}
