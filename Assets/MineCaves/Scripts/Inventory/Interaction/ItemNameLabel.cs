using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemNameLabel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private RectTransform tail;
    [SerializeField] private float gap = 6f;

    private void Awake()
    {
        foreach (Graphic g in GetComponentsInChildren<Graphic>(true)) g.raycastTarget = false;
    }

    public void Show(RectTransform target, string itemName)
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
        nameText.text = itemName;
        UIPopupPlacement.PlaceAbove((RectTransform)transform, target, tail, gap);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
