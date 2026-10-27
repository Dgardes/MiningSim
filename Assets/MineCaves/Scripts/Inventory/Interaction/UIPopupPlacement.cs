using UnityEngine;
using UnityEngine.UI;

public static class UIPopupPlacement
{
    private static readonly Vector3[] targetCorners = new Vector3[4];
    private static readonly Vector3[] popupCorners = new Vector3[4];
    private static readonly Vector3[] canvasCorners = new Vector3[4];

    public static void PlaceAbove(RectTransform popup, RectTransform target, RectTransform tail, float gap)
    {
        if (popup == null || target == null) return;

        Canvas canvas = popup.GetComponentInParent<Canvas>();
        if (canvas == null) return;
        RectTransform canvasRect = (RectTransform)canvas.rootCanvas.transform;
        float scale = canvasRect.lossyScale.y;

        LayoutRebuilder.ForceRebuildLayoutImmediate(popup);

        target.GetWorldCorners(targetCorners);
        Vector3 topCenter = (targetCorners[1] + targetCorners[2]) * 0.5f;

        popup.pivot = new Vector2(0.5f, 0f);
        popup.position = topCenter + Vector3.up * gap * scale;

        popup.GetWorldCorners(popupCorners);
        canvasRect.GetWorldCorners(canvasCorners);

        Vector3 shift = Vector3.zero;
        if (popupCorners[0].x < canvasCorners[0].x) shift.x = canvasCorners[0].x - popupCorners[0].x;
        else if (popupCorners[2].x > canvasCorners[2].x) shift.x = canvasCorners[2].x - popupCorners[2].x;
        if (popupCorners[1].y > canvasCorners[1].y) shift.y = canvasCorners[1].y - popupCorners[1].y;
        popup.position += shift;

        if (tail != null)
        {
            popup.GetWorldCorners(popupCorners);
            float margin = tail.rect.width * tail.lossyScale.x;
            float minX = popupCorners[0].x + margin;
            float maxX = popupCorners[2].x - margin;
            float x = minX < maxX ? Mathf.Clamp(topCenter.x, minX, maxX) : (minX + maxX) * 0.5f;
            tail.position = new Vector3(x, tail.position.y, tail.position.z);
        }
    }
}
