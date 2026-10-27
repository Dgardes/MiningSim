using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsGeneralTab : MonoBehaviour
{
    [Header("Звук")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private TextMeshProUGUI musicValueText;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TextMeshProUGUI sfxValueText;

    [Header("Мова")]
    [SerializeField] private Button languageButton;
    [SerializeField] private TextMeshProUGUI languageText;

    [Header("Керування")]
    [SerializeField] private Toggle joystickToggle;

    private void Awake()
    {
        musicSlider.minValue = 0f;
        musicSlider.maxValue = 1f;
        sfxSlider.minValue = 0f;
        sfxSlider.maxValue = 1f;

        musicSlider.onValueChanged.AddListener(OnMusicChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxChanged);
        languageButton.onClick.AddListener(OnNextLanguage);
        joystickToggle.onValueChanged.AddListener(OnJoystickToggled);
    }

    private void OnEnable()
    {
        musicSlider.SetValueWithoutNotify(GameSettings.MusicVolume);
        sfxSlider.SetValueWithoutNotify(GameSettings.SfxVolume);
        joystickToggle.SetIsOnWithoutNotify(GameSettings.ShowJoystick);
        RefreshTexts();
    }

    private void OnMusicChanged(float value)
    {
        GameSettings.SetMusicVolume(value);
        RefreshTexts();
    }

    private void OnSfxChanged(float value)
    {
        GameSettings.SetSfxVolume(value);
        RefreshTexts();
    }

    private void OnNextLanguage()
    {
        int next = (GameSettings.LanguageIndex + 1) % GameSettings.LanguageCodes.Length;
        GameSettings.SetLanguage(GameSettings.LanguageCodes[next]);
        RefreshTexts();
    }

    private void OnJoystickToggled(bool value)
    {
        GameSettings.SetShowJoystick(value);
    }

    private void RefreshTexts()
    {
        musicValueText.text = Mathf.RoundToInt(GameSettings.MusicVolume * 100f) + "%";
        sfxValueText.text = Mathf.RoundToInt(GameSettings.SfxVolume * 100f) + "%";
        languageText.text = "< " + GameSettings.LanguageNames[GameSettings.LanguageIndex] + " >";
    }
}
