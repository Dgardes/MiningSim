using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarProcessor :MonoBehaviour
{
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI progressCounter;
    [SerializeField] private TextMeshProUGUI blockDurability;
    [SerializeField] private TextMeshProUGUI blockName;

    private bool isProgressBarActive = false;

    public void updateProgressBar(float currentProgress, float requiredTime, BlockLogic block)
    {
        if (!isProgressBarActive)
        { 
            showProgressBar(); 
        }

        progressBar.minValue = 0f;
        progressBar.maxValue = requiredTime;
        progressBar.value = Mathf.Clamp(currentProgress, 0f, requiredTime);

        progressCounter.text = progressBar.value.ToString("f1") + "s / " + requiredTime.ToString("f1") + "s";

        if (block != null && block.getBlockData() != null)
        {
            blockDurability.text = block.getDurability().ToString() + " / " + block.getBlockData().durability.ToString();
            blockName.text = block.getBlockData().code;
        }
    }

    public void hideProgressBar()
    { 
        progressBar.gameObject.SetActive(false);
        progressCounter.gameObject.SetActive(false);
        blockDurability.gameObject.SetActive(false);
        blockName.gameObject.SetActive(false);
        isProgressBarActive = false;
    }

    private void showProgressBar()
    {
        progressBar.gameObject.SetActive(true);
        progressCounter.gameObject.SetActive(true);
        blockDurability.gameObject.SetActive(true);
        blockName.gameObject.SetActive(true);
    }

    private void Awake()
    {
        hideProgressBar();
    }

}
