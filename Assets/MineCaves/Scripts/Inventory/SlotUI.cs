using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Image itemIconDisplay;
    [SerializeField] private TextMeshProUGUI countTextDisplay;

    [Header("Виділення")]
    [SerializeField] private GameObject selectionFrame;          
    [SerializeField] private float selectedScale = 1.08f;
    [SerializeField] private float longPressTime = 0.45f;

    private Button slotButton;
    private RectTransform rectTransform;
    private int slotIndex;
    private IInventoryLogic owner;

    private PointerEventData pressData;
    
    private float pressStartTime;
    private bool longPressFired;
    
    private Tween denyTween;

    public int Index => slotIndex;
    public IInventoryLogic Owner => owner;
    public RectTransform Rect => rectTransform;

    private void Awake()
    {
        slotButton = GetComponent<Button>();
        rectTransform = (RectTransform)transform;
        if (selectionFrame != null) selectionFrame.SetActive(false);
    }

    public void InitSlot(int index, IInventoryLogic ownerLogic)
    {
        slotIndex = index;
        owner = ownerLogic;

        if (slotButton != null) slotButton.onClick.RemoveAllListeners();

        itemIconDisplay.enabled = false;
        countTextDisplay.gameObject.SetActive(false);
        SetSelected(false);
    }

    public void UpdateSlotVisual(InventorySlot slotData)
    {
        if (slotData == null || slotData.item == null)
        {
            itemIconDisplay.enabled = false;
            countTextDisplay.gameObject.SetActive(false);
            return;
        }

        itemIconDisplay.enabled = true;
        itemIconDisplay.sprite = slotData.item.itemSprite;

        countTextDisplay.gameObject.SetActive(true);
        countTextDisplay.text = slotData.count.ToString();
    }

    #region Натискання

    public void OnPointerDown(PointerEventData eventData)
    {
        pressData = eventData;
        pressStartTime = Time.unscaledTime;
        longPressFired = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pressData = null;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pressData = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (longPressFired)
        {
            longPressFired = false;
            return;
        }

        if (InventoryInteraction.instance != null) InventoryInteraction.instance.OnSlotTapped(this);
    }

    private void Update()
    {
        if (pressData == null || longPressFired) return;

        if (pressData.dragging)
        {
            pressData = null;
            return;
        }

        if (Time.unscaledTime - pressStartTime >= longPressTime)
        {
            longPressFired = true;
            pressData = null;
            if (InventoryInteraction.instance != null) InventoryInteraction.instance.OnSlotLongPressed(this);
        }
    }

    #endregion

    #region Візуал

    public void SetSelected(bool selected)
    {
        if (selectionFrame != null) selectionFrame.SetActive(selected);
        transform.localScale = selected ? Vector3.one * selectedScale : Vector3.one;
    }

    public void PlayDeny()
    {
        if (!isActiveAndEnabled) return;

        denyTween?.Kill();
        transform.localRotation = Quaternion.identity;
        denyTween = transform.DOPunchRotation(new Vector3(0f, 0f, 12f), 0.3f, 10, 1f)
            .SetUpdate(true)
            .SetLink(gameObject)
            .OnComplete(() => transform.localRotation = Quaternion.identity);
    }

    #endregion

    private void OnDisable()
    {
        pressData = null;
        longPressFired = false;
        denyTween?.Kill();
        transform.localRotation = Quaternion.identity;
        if (InventoryInteraction.instance != null) InventoryInteraction.instance.OnSlotDisabled(this);
    }
}
