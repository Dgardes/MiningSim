using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class WGenGizmos : MonoBehaviour
{
    [Header("Загальне")]
    [SerializeField] private bool drawGizmos = true;
    [SerializeField] private Transform center;
    [SerializeField, Range(1, 10)] private int drawRadius = 3;

    [Header("Межі чанків")]
    [SerializeField] private bool drawChunkBorders = true;
    [SerializeField] private Color borderColor = new Color(1f, 1f, 1f, 0.35f);

    [Header("Чанк гравця")]
    [SerializeField] private bool highlightCenterChunk = true;
    [SerializeField] private Color centerChunkColor = new Color(1f, 0.85f, 0f, 1f);

    [Header("Завантажені чанки")]
    [SerializeField] private bool drawLoadedChunks = true;
    [SerializeField] private ActiveChunkManager chunkLoader;
    [SerializeField] private Color loadedChunkColor = new Color(0.2f, 0.8f, 1f, 0.12f);

    [Header("Підписи координат")]
    [SerializeField] private bool drawChunkLabels = true;
    [SerializeField] private Color labelColor = Color.white;

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;

        Vector3 centerPosition = center != null ? center.position : transform.position;
        ChunkCord centerChunk = ChunkCord.worldToCell(centerPosition);

        if (drawLoadedChunks) DrawLoadedChunks();

        for (int x = -drawRadius; x <= drawRadius; x++)
        {
            for (int y = -drawRadius; y <= drawRadius; y++)
            {
                ChunkCord chunk = new ChunkCord(centerChunk.x + x, centerChunk.y + y);

                if (drawChunkBorders)
                {
                    Gizmos.color = borderColor;
                    Gizmos.DrawWireCube(GetChunkCenter(chunk), GetChunkSize());
                }

                if (drawChunkLabels) DrawLabel(chunk);
            }
        }

        if (highlightCenterChunk)
        {
            Gizmos.color = centerChunkColor;
            Gizmos.DrawWireCube(GetChunkCenter(centerChunk), GetChunkSize());
        }
    }

    private void DrawLoadedChunks()
    {
        if (chunkLoader == null || chunkLoader.LoadedChunks == null) return;

        Gizmos.color = loadedChunkColor;
        foreach (Chunk chunk in chunkLoader.LoadedChunks)
        {
            if (chunk == null || chunk.chunkCord == null) continue;
            Gizmos.DrawCube(GetChunkCenter(chunk.chunkCord), GetChunkSize());
        }
    }

    private void DrawLabel(ChunkCord chunk)
    {
#if UNITY_EDITOR
        Vector3 origin = GetChunkOrigin(chunk);
        Vector3 labelPosition = origin + new Vector3(0.3f, ChunkCord.chunkSize - 0.3f, 0f);

        GUIStyle style = new GUIStyle();
        style.normal.textColor = labelColor;
        style.fontSize = 11;

        Handles.Label(labelPosition, chunk.x + ", " + chunk.y, style);
#endif
    }

    private Vector3 GetChunkOrigin(ChunkCord chunk)
    {
        return new Vector3(chunk.x * ChunkCord.chunkSize, chunk.y * ChunkCord.chunkSize, 0f);
    }

    private Vector3 GetChunkCenter(ChunkCord chunk)
    {
        float half = ChunkCord.chunkSize / 2f;
        return GetChunkOrigin(chunk) + new Vector3(half, half, 0f);
    }

    private Vector3 GetChunkSize()
    {
        return new Vector3(ChunkCord.chunkSize, ChunkCord.chunkSize, 0f);
    }
}
