using System.Collections.Generic;
using UnityEngine;

public class ChunkBordersRuntime : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField, Range(1, 6)] private int radius = 2;
    [SerializeField] private float lineWidth = 0.06f;
    [SerializeField] private Color gridColor = new Color(1f, 1f, 1f, 0.35f);
    [SerializeField] private Color currentChunkColor = new Color(1f, 0.85f, 0f, 0.9f);
    [SerializeField] private Material lineMaterial;
    [SerializeField] private string sortingLayerName = "Default";
    [SerializeField] private int sortingOrder = 1000;

    private readonly List<LineRenderer> verticalLines = new List<LineRenderer>();
    private readonly List<LineRenderer> horizontalLines = new List<LineRenderer>();
    private LineRenderer currentChunkLine;
    private Transform linesRoot;
    private ChunkCord lastChunk;
    private int builtRadius = -1;

    private void Awake()
    {
        linesRoot = new GameObject("Lines").transform;
        linesRoot.SetParent(transform, false);

        if (lineMaterial == null)
        {
            Shader shader = Shader.Find("Sprites/Default");
            if (shader != null) lineMaterial = new Material(shader);
            else Debug.LogWarning("[ChunkBordersRuntime] Не знайдено шейдер Sprites/Default, признач Line Material вручну");
        }
    }

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
        bool show = GameSettings.ShowChunkBorders;
        linesRoot.gameObject.SetActive(show);
        if (show) lastChunk = null;
    }

    private void LateUpdate()
    {
        if (player == null || !linesRoot.gameObject.activeSelf) return;

        if (builtRadius != radius) BuildLines();

        ChunkCord chunk = ChunkCord.worldToCell(player.position);
        if (lastChunk != null && lastChunk.Equals(chunk)) return;

        lastChunk = chunk;
        Redraw(chunk);
    }

    private void BuildLines()
    {
        foreach (Transform child in linesRoot) Destroy(child.gameObject);
        verticalLines.Clear();
        horizontalLines.Clear();

        int count = radius * 2 + 2;
        for (int i = 0; i < count; i++)
        {
            verticalLines.Add(CreateLine("V" + i, 2, gridColor, false));
            horizontalLines.Add(CreateLine("H" + i, 2, gridColor, false));
        }
        currentChunkLine = CreateLine("Current", 4, currentChunkColor, true);

        builtRadius = radius;
        lastChunk = null;
    }

    private LineRenderer CreateLine(string lineName, int points, Color color, bool loop)
    {
        GameObject go = new GameObject(lineName, typeof(LineRenderer));
        go.transform.SetParent(linesRoot, false);

        LineRenderer line = go.GetComponent<LineRenderer>();
        line.useWorldSpace = true;
        line.positionCount = points;
        line.loop = loop;
        line.widthMultiplier = lineWidth;
        line.numCapVertices = 0;
        line.numCornerVertices = 0;
        line.material = lineMaterial;
        line.startColor = color;
        line.endColor = color;
        line.sortingLayerName = sortingLayerName;
        line.sortingOrder = loop ? sortingOrder + 1 : sortingOrder;
        line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        line.receiveShadows = false;
        return line;
    }

    private void Redraw(ChunkCord center)
    {
        int size = ChunkCord.chunkSize;
        float minX = (center.x - radius) * size;
        float minY = (center.y - radius) * size;
        float maxX = (center.x + radius + 1) * size;
        float maxY = (center.y + radius + 1) * size;

        for (int i = 0; i < verticalLines.Count; i++)
        {
            float x = minX + i * size;
            verticalLines[i].SetPosition(0, new Vector3(x, minY, 0f));
            verticalLines[i].SetPosition(1, new Vector3(x, maxY, 0f));
        }

        for (int i = 0; i < horizontalLines.Count; i++)
        {
            float y = minY + i * size;
            horizontalLines[i].SetPosition(0, new Vector3(minX, y, 0f));
            horizontalLines[i].SetPosition(1, new Vector3(maxX, y, 0f));
        }

        float cx = center.x * size;
        float cy = center.y * size;
        currentChunkLine.SetPosition(0, new Vector3(cx, cy, 0f));
        currentChunkLine.SetPosition(1, new Vector3(cx + size, cy, 0f));
        currentChunkLine.SetPosition(2, new Vector3(cx + size, cy + size, 0f));
        currentChunkLine.SetPosition(3, new Vector3(cx, cy + size, 0f));
    }
}
