using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private MenuManager menuManager;

    [Header("Ліва колонка")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private TextMeshProUGUI musicButtonLabel;
    [SerializeField] private Button quitButton;

    [Header("Вкладки")]
    [SerializeField] private Button generalTabButton;
    [SerializeField] private Button debugTabButton;
    [SerializeField] private GameObject generalPage;
    [SerializeField] private GameObject debugPage;
    [SerializeField] private Color activeTabColor = new Color(0.95f, 0.78f, 0.62f);
    [SerializeField] private Color inactiveTabColor = new Color(0.62f, 0.55f, 0.50f);

    [Tooltip("Вимкни для релізу — вкладку Debug буде приховано")]
    [SerializeField] private bool showDebugTab = true;

    private bool debugTabOpen;

    private void Awake()
    {
        continueButton.onClick.AddListener(OnContinue);
        musicButton.onClick.AddListener(OnToggleMusic);
        quitButton.onClick.AddListener(OnQuit);
        generalTabButton.onClick.AddListener(OnGeneralTab);
        debugTabButton.onClick.AddListener(OnDebugTab);
    }

    private void OnEnable()
    {
        debugTabButton.gameObject.SetActive(showDebugTab);
        if (!showDebugTab) debugTabOpen = false;

        ShowTab(debugTabOpen);
        RefreshMusicLabel();
    }

    private void OnDisable()
    {
        GameSettings.Save();
    }

    #region Ліва колонка

    private void OnContinue()
    {
        if (menuManager != null) menuManager.closeMenu();
    }

    private void OnToggleMusic()
    {
        GameSettings.SetMusicEnabled(!GameSettings.MusicEnabled);
        RefreshMusicLabel();
    }

    private void RefreshMusicLabel()
    {
        if (musicButtonLabel != null) musicButtonLabel.text = GameSettings.MusicEnabled ? "Music: On" : "Music: Off";
    }

    private void OnQuit()
    {
        GameSettings.Save();
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    #endregion

    #region Вкладки

    private void OnGeneralTab() => ShowTab(false);
    private void OnDebugTab() => ShowTab(true);

    private void ShowTab(bool debug)
    {
        debugTabOpen = debug;
        generalPage.SetActive(!debug);
        debugPage.SetActive(debug);

        SetTabColor(generalTabButton, !debug);
        SetTabColor(debugTabButton, debug);
    }

    private void SetTabColor(Button tab, bool active)
    {
        if (tab == null || tab.targetGraphic == null) return;
        tab.targetGraphic.color = active ? activeTabColor : inactiveTabColor;
    }

    #endregion
}
