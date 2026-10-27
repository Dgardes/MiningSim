using TMPro;
using UnityEngine;
using UnityEngine.UI;

//                            [-] ====o==== [+]
//                                   24
public class SplitPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private Button minusButton;
    [SerializeField] private Button plusButton;
    [SerializeField] private Slider slider;
    [SerializeField] private RectTransform tail;
    [SerializeField] private float gap = 6f;

    private bool initialized;

    public int Amount => slider != null ? Mathf.RoundToInt(slider.value) : 1;

    private void Init()
    {
        if (initialized) return;
        initialized = true;

        slider.wholeNumbers = true;
        slider.onValueChanged.AddListener(OnSliderChanged);
        minusButton.onClick.AddListener(OnMinus);
        plusButton.onClick.AddListener(OnPlus);
    }

    public void Open(RectTransform target, string itemName, int maxAmount, int startAmount)
    {
        Init();
        gameObject.SetActive(true);
        transform.SetAsLastSibling();

        titleText.text = itemName;

        int max = Mathf.Max(1, maxAmount);
        slider.minValue = 1;
        slider.maxValue = max;
        slider.SetValueWithoutNotify(Mathf.Clamp(startAmount, 1, max));
        UpdateValueText();

        UIPopupPlacement.PlaceAbove((RectTransform)transform, target, tail, gap);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private void OnMinus() => Step(-1);
    private void OnPlus() => Step(1);

    private void Step(int delta)
    {
        slider.value = slider.value + delta;
    }

    private void OnSliderChanged(float value)
    {
        UpdateValueText();
    }

    private void UpdateValueText()
    {
        valueText.text = Amount.ToString();
        minusButton.interactable = slider.value > slider.minValue;
        plusButton.interactable = slider.value < slider.maxValue;
    }
}
