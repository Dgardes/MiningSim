using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//
public static class MineCavesUIBuilder
{
    private const string MenuPath = "Tools/Mine Caves/UI/";

    private static readonly Color AccentColor = new Color(0.98f, 0.82f, 0.68f);
    private static readonly Color AccentDarkColor = new Color(0.55f, 0.36f, 0.26f);
    private static readonly Color ButtonColor = new Color(0.30f, 0.27f, 0.25f);
    private static readonly Color TabInactiveColor = new Color(0.62f, 0.55f, 0.50f);
    private static readonly Color PageColor = new Color(0f, 0f, 0f, 0.25f);
    private static readonly Color TrackColor = new Color(0.18f, 0.16f, 0.15f);
    private static readonly Color DarkTextColor = new Color(0.25f, 0.16f, 0.12f);
    private static readonly Color LightTextColor = new Color(0.96f, 0.93f, 0.90f);
    private static readonly Color HeaderTextColor = new Color(0.98f, 0.82f, 0.68f);

    private const float ButtonHeight = 80f;
    private const float ButtonFont = 34f;
    private const float TabHeight = 64f;
    private const float TabFont = 30f;
    private const float HeaderHeight = 40f;
    private const float HeaderFont = 28f;
    private const float RowHeight = 54f;
    private const float RowFont = 26f;
    private const float ToggleBox = 38f;
    private const float ListSpacing = 6f;
    private const float LabelWidth = 260f;

    #region Пункти меню

    [MenuItem(MenuPath + "Build All")]
    private static void BuildAll()
    {
        if (!Confirm()) return;
        BuildInventoryPopupsInternal();
        BuildPauseMenuInternal();
        BuildDebugOverlayInternal();
        SetupJoystickHintsInternal();
        SetupAudioInternal();
        SetupInventoryFillerInternal();
        SetupChunkBordersInternal();
        Finish();
    }

    [MenuItem(MenuPath + "Build Inventory Popups")]
    private static void BuildInventoryPopups()
    {
        if (!Confirm()) return;
        BuildInventoryPopupsInternal();
        Finish();
    }

    [MenuItem(MenuPath + "Build Pause Menu")]
    private static void BuildPauseMenu()
    {
        if (!Confirm()) return;
        BuildPauseMenuInternal();
        Finish();
    }

    [MenuItem(MenuPath + "Build Debug Overlay")]
    private static void BuildDebugOverlay()
    {
        if (!Confirm()) return;
        BuildDebugOverlayInternal();
        Finish();
    }

    [MenuItem(MenuPath + "Add Joystick Hints + Audio + Test Items + Chunk Borders")]
    private static void SetupMisc()
    {
        SetupJoystickHintsInternal();
        SetupAudioInternal();
        SetupInventoryFillerInternal();
        SetupChunkBordersInternal();
        Finish();
    }

    private static bool Confirm()
    {
        return EditorUtility.DisplayDialog("Mine Caves UI Builder",
            "Об'єкти, раніше створені цим інструментом (ItemNameLabel, SplitPopup, MenuContent, DebugOverlay), " +
            "буде видалено й створено заново. Ручні зміни в них зникнуть.\n\nПродовжити?",
            "Так", "Скасувати");
    }

    private static void Finish()
    {
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        Debug.Log("[MineCavesUIBuilder] Готово. Не забудь зберегти сцену (Ctrl+S).");
    }

    #endregion

    #region Інвентар: плашка назви + вікно розділення

    private static void BuildInventoryPopupsInternal()
    {
        GameObject canvas = FindInScene("Canvas_Inventory");
        if (canvas == null)
        {
            Debug.LogError("[MineCavesUIBuilder] Не знайдено Canvas_Inventory у сцені");
            return;
        }

        RemoveChild(canvas.transform, "ItemNameLabel");
        RemoveChild(canvas.transform, "SplitPopup");

        ItemNameLabel label = CreateNameLabel(canvas.transform);
        SplitPopup popup = CreateSplitPopup(canvas.transform);

        InventoryInteraction interaction = canvas.GetComponent<InventoryInteraction>();
        if (interaction == null) interaction = Undo.AddComponent<InventoryInteraction>(canvas);

        Wire(interaction, "nameLabel", label, "splitPopup", popup);
    }

    private static ItemNameLabel CreateNameLabel(Transform parent)
    {
        RectTransform root = NewRect("ItemNameLabel", parent);
        root.anchorMin = root.anchorMax = new Vector2(0.5f, 0.5f);
        root.pivot = new Vector2(0.5f, 0f);
        AddImage(root.gameObject, AccentColor);

        HorizontalLayoutGroup layout = root.gameObject.AddComponent<HorizontalLayoutGroup>();
        layout.padding = new RectOffset(28, 28, 12, 12);
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        ContentSizeFitter fitter = root.gameObject.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        RectTransform tail = CreateTail(root, AccentColor);
        TextMeshProUGUI text = NewText(root, "Name", "Item name", 34, DarkTextColor, TextAlignmentOptions.Center);

        ItemNameLabel label = root.gameObject.AddComponent<ItemNameLabel>();
        Wire(label, "nameText", text, "tail", tail);
        SetFloat(label, "gap", 20f);

        root.gameObject.SetActive(false);
        Undo.RegisterCreatedObjectUndo(root.gameObject, "Build ItemNameLabel");
        return label;
    }

    private static SplitPopup CreateSplitPopup(Transform parent)
    {
        RectTransform root = NewRect("SplitPopup", parent);
        root.anchorMin = root.anchorMax = new Vector2(0.5f, 0.5f);
        root.pivot = new Vector2(0.5f, 0f);
        root.sizeDelta = new Vector2(480f, 230f);
        AddImage(root.gameObject, AccentColor);

        RectTransform tail = CreateTail(root, AccentColor);

        TextMeshProUGUI title = NewText(root, "Title", "Item name", 34, DarkTextColor, TextAlignmentOptions.Center);
        RectTransform titleRect = title.rectTransform;
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.sizeDelta = new Vector2(-30f, 60f);
        titleRect.anchoredPosition = new Vector2(0f, -10f);

        Button minus = NewButton(root, "BTN_Minus", "-", 52, AccentDarkColor, LightTextColor);
        PlaceSide((RectTransform)minus.transform, 0f, 22f);
        Button plus = NewButton(root, "BTN_Plus", "+", 52, AccentDarkColor, LightTextColor);
        PlaceSide((RectTransform)plus.transform, 1f, -22f);

        Slider slider = NewSlider(root, "Slider", TrackColor, AccentDarkColor, LightTextColor);
        RectTransform sliderRect = (RectTransform)slider.transform;
        sliderRect.anchorMin = new Vector2(0f, 0.5f);
        sliderRect.anchorMax = new Vector2(1f, 0.5f);
        sliderRect.offsetMin = new Vector2(112f, -32f);
        sliderRect.offsetMax = new Vector2(-112f, 22f);

        TextMeshProUGUI value = NewText(root, "Value", "24", 40, DarkTextColor, TextAlignmentOptions.Center);
        RectTransform valueRect = value.rectTransform;
        valueRect.anchorMin = new Vector2(0f, 0f);
        valueRect.anchorMax = new Vector2(1f, 0f);
        valueRect.pivot = new Vector2(0.5f, 0f);
        valueRect.sizeDelta = new Vector2(0f, 60f);
        valueRect.anchoredPosition = new Vector2(0f, 8f);

        SplitPopup popup = root.gameObject.AddComponent<SplitPopup>();
        Wire(popup, "titleText", title, "valueText", value, "minusButton", minus, "plusButton", plus,
            "slider", slider, "tail", tail);
        SetFloat(popup, "gap", 20f);

        root.gameObject.SetActive(false);
        Undo.RegisterCreatedObjectUndo(root.gameObject, "Build SplitPopup");
        return popup;
    }

    private static void PlaceSide(RectTransform rt, float anchorX, float x)
    {
        rt.anchorMin = rt.anchorMax = new Vector2(anchorX, 0.5f);
        rt.pivot = new Vector2(anchorX, 0.5f);
        rt.sizeDelta = new Vector2(76f, 76f);
        rt.anchoredPosition = new Vector2(x, -5f);
    }

    private static RectTransform CreateTail(RectTransform parent, Color color)
    {
        RectTransform tail = NewRect("Tail", parent);
        tail.anchorMin = tail.anchorMax = new Vector2(0.5f, 0f);
        tail.pivot = new Vector2(0.5f, 0.5f);
        tail.sizeDelta = new Vector2(26f, 26f);
        tail.anchoredPosition = Vector2.zero;
        tail.localRotation = Quaternion.Euler(0f, 0f, 45f);
        Image img = tail.gameObject.AddComponent<Image>();
        img.color = color;
        img.raycastTarget = false;
        tail.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
        tail.SetAsFirstSibling();
        return tail;
    }

    #endregion

    #region Меню паузи

    private static void BuildPauseMenuInternal()
    {
        GameObject panel = FindInScene("mainMenuPanel");
        if (panel == null)
        {
            Debug.LogError("[MineCavesUIBuilder] Не знайдено mainMenuPanel у сцені");
            return;
        }

        RemoveChild(panel.transform, "MenuContent");

        RectTransform content = NewRect("MenuContent", panel.transform);
        Stretch(content, 32f, 32f, 70f, 32f);
        content.SetAsFirstSibling();
        HorizontalLayoutGroup columns = content.gameObject.AddComponent<HorizontalLayoutGroup>();
        columns.spacing = 32f;
        columns.childControlWidth = true;
        columns.childControlHeight = true;
        columns.childForceExpandWidth = false;
        columns.childForceExpandHeight = true;

        RectTransform left = NewRect("LeftColumn", content);
        SetLayout(left.gameObject, -1f, -1f, 1f, -1f);
        VerticalLayoutGroup leftLayout = left.gameObject.AddComponent<VerticalLayoutGroup>();
        leftLayout.spacing = 16f;
        leftLayout.childAlignment = TextAnchor.MiddleCenter;
        leftLayout.childControlWidth = true;
        leftLayout.childControlHeight = true;
        leftLayout.childForceExpandWidth = true;
        leftLayout.childForceExpandHeight = false;

        Button continueButton = NewButton(left, "BTN_Continue", "Continue", ButtonFont, AccentColor, DarkTextColor);
        SetLayout(continueButton.gameObject, ButtonHeight);
        Button musicButton = NewButton(left, "BTN_Music", "Music: On", ButtonFont, ButtonColor, LightTextColor);
        SetLayout(musicButton.gameObject, ButtonHeight);
        Button quitButton = NewButton(left, "BTN_Quit", "Quit game", ButtonFont, ButtonColor, LightTextColor);
        SetLayout(quitButton.gameObject, ButtonHeight);

        RectTransform right = NewRect("RightColumn", content);
        SetLayout(right.gameObject, -1f, -1f, 2.4f, -1f);
        VerticalLayoutGroup rightLayout = right.gameObject.AddComponent<VerticalLayoutGroup>();
        rightLayout.spacing = 12f;
        rightLayout.childControlWidth = true;
        rightLayout.childControlHeight = true;
        rightLayout.childForceExpandWidth = true;
        rightLayout.childForceExpandHeight = false;

        RectTransform tabs = NewRect("Tabs", right);
        SetLayout(tabs.gameObject, TabHeight);
        HorizontalLayoutGroup tabsLayout = tabs.gameObject.AddComponent<HorizontalLayoutGroup>();
        tabsLayout.spacing = 12f;
        tabsLayout.childControlWidth = true;
        tabsLayout.childControlHeight = true;
        tabsLayout.childForceExpandWidth = true;
        tabsLayout.childForceExpandHeight = true;
        Button generalTab = NewButton(tabs, "TAB_General", "General", TabFont, AccentColor, DarkTextColor);
        Button debugTab = NewButton(tabs, "TAB_Debug", "Debug", TabFont, TabInactiveColor, DarkTextColor);

        RectTransform pages = NewRect("Pages", right);
        SetLayout(pages.gameObject, -1f, -1f, -1f, 1f);
        AddImage(pages.gameObject, PageColor);

        GameObject generalPage = BuildGeneralPage(pages);
        GameObject debugPage = BuildDebugPage(pages);

        PauseMenu menu = content.gameObject.AddComponent<PauseMenu>();
        Wire(menu,
            "menuManager", FindComponent<MenuManager>(),
            "continueButton", continueButton,
            "musicButton", musicButton,
            "musicButtonLabel", musicButton.GetComponentInChildren<TextMeshProUGUI>(),
            "quitButton", quitButton,
            "generalTabButton", generalTab,
            "debugTabButton", debugTab,
            "generalPage", generalPage,
            "debugPage", debugPage);

        Undo.RegisterCreatedObjectUndo(content.gameObject, "Build Pause Menu");
    }

    private static RectTransform NewScrollPage(RectTransform pages, string name)
    {
        RectTransform page = NewRect(name, pages);
        Stretch(page, 0f, 0f, 0f, 0f);
        Image hitArea = page.gameObject.AddComponent<Image>();
        hitArea.color = new Color(0f, 0f, 0f, 0f);

        RectTransform viewport = NewRect("Viewport", page);
        Stretch(viewport, 0f, 0f, 0f, 0f);
        viewport.gameObject.AddComponent<RectMask2D>();

        RectTransform list = NewRect("Content", viewport);
        list.anchorMin = new Vector2(0f, 1f);
        list.anchorMax = new Vector2(1f, 1f);
        list.pivot = new Vector2(0.5f, 1f);
        list.offsetMin = Vector2.zero;
        list.offsetMax = Vector2.zero;
        VerticalLayoutGroup layout = list.gameObject.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(28, 28, 16, 20);
        layout.spacing = ListSpacing;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;
        ContentSizeFitter fitter = list.gameObject.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        ScrollRect scroll = page.gameObject.AddComponent<ScrollRect>();
        scroll.viewport = viewport;
        scroll.content = list;
        scroll.horizontal = false;
        scroll.vertical = true;
        scroll.movementType = ScrollRect.MovementType.Clamped;
        scroll.scrollSensitivity = 40f;

        return list;
    }

    private static GameObject BuildGeneralPage(RectTransform pages)
    {
        RectTransform list = NewScrollPage(pages, "Page_General");
        GameObject page = list.parent.parent.gameObject;

        NewHeader(list, "Sound");
        RectTransform musicRow = NewRow(list, "Row_Music");
        NewRowLabel(musicRow, "Music volume", LabelWidth);
        Slider musicSlider = NewSlider(musicRow, "Slider", TrackColor, AccentColor, LightTextColor);
        SetLayout(musicSlider.gameObject, -1f, -1f, 1f, -1f);
        TextMeshProUGUI musicValue = NewRowLabel(musicRow, "80%", 90f);
        musicValue.alignment = TextAlignmentOptions.MidlineRight;

        RectTransform sfxRow = NewRow(list, "Row_Sfx");
        NewRowLabel(sfxRow, "Effects volume", LabelWidth);
        Slider sfxSlider = NewSlider(sfxRow, "Slider", TrackColor, AccentColor, LightTextColor);
        SetLayout(sfxSlider.gameObject, -1f, -1f, 1f, -1f);
        TextMeshProUGUI sfxValue = NewRowLabel(sfxRow, "100%", 90f);
        sfxValue.alignment = TextAlignmentOptions.MidlineRight;

        NewHeader(list, "Language");
        RectTransform languageRow = NewRow(list, "Row_Language");
        NewRowLabel(languageRow, "Language", LabelWidth);
        Button languageButton = NewButton(languageRow, "BTN_Language", "< English >", RowFont, ButtonColor, LightTextColor);
        SetLayout(languageButton.gameObject, -1f, -1f, 1f, -1f);

        NewHeader(list, "Controls");
        Toggle joystickToggle = NewToggle(list, "Toggle_Joystick", "Show joystick hint");

        SettingsGeneralTab tab = page.AddComponent<SettingsGeneralTab>();
        Wire(tab,
            "musicSlider", musicSlider,
            "musicValueText", musicValue,
            "sfxSlider", sfxSlider,
            "sfxValueText", sfxValue,
            "languageButton", languageButton,
            "languageText", languageButton.GetComponentInChildren<TextMeshProUGUI>(),
            "joystickToggle", joystickToggle);

        return page;
    }

    private static GameObject BuildDebugPage(RectTransform pages)
    {
        RectTransform list = NewScrollPage(pages, "Page_Debug");
        GameObject page = list.parent.parent.gameObject;

        NewHeader(list, "World");
        Toggle bordersToggle = NewToggle(list, "Toggle_ChunkBorders", "Chunk borders");

        RectTransform seedRow = NewRow(list, "Row_Seed");
        NewRowLabel(seedRow, "Seed", 110f);
        TMP_InputField seedInput = NewInputField(seedRow, "Input_Seed");
        SetLayout(seedInput.gameObject, -1f, -1f, 1f, -1f);
        Button applySeed = NewButton(seedRow, "BTN_ApplySeed", "Apply", RowFont, AccentColor, DarkTextColor);
        SetLayout(applySeed.gameObject, -1f, 140f);
        Button randomSeed = NewButton(seedRow, "BTN_RandomSeed", "Random", RowFont, ButtonColor, LightTextColor);
        SetLayout(randomSeed.gameObject, -1f, 150f);

        Button teleport = NewButton(list, "BTN_Teleport", "Teleport to surface", RowFont, ButtonColor, LightTextColor);
        SetLayout(teleport.gameObject, RowHeight);
        Toggle overlayToggle = NewToggle(list, "Toggle_Overlay", "Info overlay (FPS, depth...)");

        NewHeader(list, "Inventory");
        RectTransform itemsRow = NewRow(list, "Row_Items");
        Button giveItems = NewButton(itemsRow, "BTN_GiveItems", "Give test items", RowFont, AccentColor, DarkTextColor);
        SetLayout(giveItems.gameObject, -1f, -1f, 1f, -1f);
        Button clearInventory = NewButton(itemsRow, "BTN_ClearInventory", "Clear", RowFont, ButtonColor, LightTextColor);
        SetLayout(clearInventory.gameObject, -1f, 160f);

        NewHeader(list, "Player");
        Button fullHealth = NewButton(list, "BTN_FullHealth", "Full health", RowFont, ButtonColor, LightTextColor);
        SetLayout(fullHealth.gameObject, RowHeight);

        NewStatRow(list, DebugStat.MoveSpeed, 0f, 20f);
        NewStatRow(list, DebugStat.JumpForce, 0f, 30f);
        NewStatRow(list, DebugStat.MiningRange, 0.5f, 10f);
        NewStatRow(list, DebugStat.MiningEfficiency, 0f, 5f);
        NewStatRow(list, DebugStat.Luck, 0f, 5f);
        NewStatRow(list, DebugStat.ViewDistance, 1f, 30f);
        NewStatRow(list, DebugStat.MaxHealth, 10f, 500f);
        NewStatRow(list, DebugStat.SafeFallSpeed, -40f, 0f);
        NewStatRow(list, DebugStat.FallDamageMultiplier, 0f, 20f);
        NewStatRow(list, DebugStat.IncomingDamageMultiplier, 0f, 3f);

        PlayerStats stats = FindComponent<PlayerStats>();
        GameObject spawner = FindInScene("Spawner");

        SettingsDebugTab tab = page.AddComponent<SettingsDebugTab>();
        Wire(tab,
            "menuManager", FindComponent<MenuManager>(),
            "worldDebug", FindComponent<WGenDebug>(),
            "generator", FindComponent<ChunkGenerator>(),
            "playerStats", stats,
            "player", stats != null ? stats.transform : null,
            "surfacePoint", spawner != null ? spawner.transform : null,
            "chunkBordersToggle", bordersToggle,
            "seedInput", seedInput,
            "applySeedButton", applySeed,
            "randomSeedButton", randomSeed,
            "teleportButton", teleport,
            "overlayToggle", overlayToggle,
            "giveItemsButton", giveItems,
            "clearInventoryButton", clearInventory,
            "fullHealthButton", fullHealth);

        page.SetActive(false);
        return page;
    }

    private static void NewStatRow(Transform parent, DebugStat stat, float min, float max)
    {
        RectTransform row = NewRow(parent, "Stat_" + stat);
        TextMeshProUGUI label = NewRowLabel(row, stat + ": 0", 380f);
        Slider slider = NewSlider(row, "Slider", TrackColor, AccentColor, LightTextColor);
        SetLayout(slider.gameObject, -1f, -1f, 1f, -1f);

        DebugStatRow statRow = row.gameObject.AddComponent<DebugStatRow>();
        statRow.Setup(stat, min, max);
        Wire(statRow, "slider", slider, "label", label);
        EditorUtility.SetDirty(statRow);
    }

    #endregion

    #region Debug-оверлей

    private static void BuildDebugOverlayInternal()
    {
        GameObject canvas = FindInScene("Canvas_UI");
        if (canvas == null)
        {
            Debug.LogError("[MineCavesUIBuilder] Не знайдено Canvas_UI у сцені");
            return;
        }

        RemoveChild(canvas.transform, "DebugOverlay");

        RectTransform root = NewRect("DebugOverlay", canvas.transform);
        root.anchorMin = root.anchorMax = new Vector2(0f, 1f);
        root.pivot = new Vector2(0f, 1f);
        root.anchoredPosition = new Vector2(20f, -20f);
        root.sizeDelta = new Vector2(10f, 10f);
        root.SetAsLastSibling();

        RectTransform panel = NewRect("Panel", root);
        panel.anchorMin = panel.anchorMax = new Vector2(0f, 1f);
        panel.pivot = new Vector2(0f, 1f);
        panel.anchoredPosition = Vector2.zero;
        Image bg = AddImage(panel.gameObject, new Color(0f, 0f, 0f, 0.55f));
        bg.raycastTarget = false;
        VerticalLayoutGroup layout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(16, 16, 12, 12);
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        ContentSizeFitter fitter = panel.gameObject.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        TextMeshProUGUI info = NewText(panel, "Info", "FPS: 60\nDepth: 0", 26, Color.white, TextAlignmentOptions.TopLeft);

        PlayerStats stats = FindComponent<PlayerStats>();
        DebugOverlay overlay = root.gameObject.AddComponent<DebugOverlay>();
        Wire(overlay,
            "panelRoot", panel.gameObject,
            "infoText", info,
            "player", stats != null ? stats.transform : null,
            "generator", FindComponent<ChunkGenerator>());

        Undo.RegisterCreatedObjectUndo(root.gameObject, "Build Debug Overlay");
    }

    #endregion

    #region Джойстики та звук

    private static void SetupJoystickHintsInternal()
    {
        int added = 0;
        foreach (GameObject rootObject in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            foreach (FloatingJoystick joystick in rootObject.GetComponentsInChildren<FloatingJoystick>(true))
            {
                if (joystick.GetComponent<JoystickHint>() != null) continue;
                Undo.AddComponent<JoystickHint>(joystick.gameObject);
                added++;
            }
        }
        Debug.Log("[MineCavesUIBuilder] JoystickHint додано до джойстиків: " + added);
    }

    private static void SetupInventoryFillerInternal()
    {
        if (FindComponent<DebugInventoryFiller>() != null) return;

        Inventory_PlayerInvLogic logic = FindComponent<Inventory_PlayerInvLogic>();
        if (logic == null)
        {
            Debug.LogWarning("[MineCavesUIBuilder] Не знайдено Inventory_PlayerInvLogic — DebugInventoryFiller не додано");
            return;
        }
        Undo.AddComponent<DebugInventoryFiller>(logic.gameObject);
    }

    private static void SetupChunkBordersInternal()
    {
        if (FindComponent<ChunkBordersRuntime>() != null) return;

        GameObject go = new GameObject("ChunkBorders");
        GameObject worldGen = FindInScene("WorldGen");
        if (worldGen != null) go.transform.SetParent(worldGen.transform, false);
        Undo.RegisterCreatedObjectUndo(go, "Add Chunk Borders");

        ChunkBordersRuntime borders = go.AddComponent<ChunkBordersRuntime>();
        PlayerStats stats = FindComponent<PlayerStats>();
        Wire(borders, "player", stats != null ? stats.transform : null);
    }

    private static void SetupAudioInternal()
    {
        if (FindComponent<AudioSettingsApplier>() != null) return;

        GameObject managers = FindInScene("Managers");
        if (managers == null)
        {
            Debug.LogWarning("[MineCavesUIBuilder] Не знайдено об'єкт Managers — AudioSettingsApplier не додано");
            return;
        }
        Undo.AddComponent<AudioSettingsApplier>(managers);
    }

    #endregion

    #region Дрібні будівельні блоки

    private static Sprite BuiltinSprite(string path)
    {
        return AssetDatabase.GetBuiltinExtraResource<Sprite>(path);
    }

    private static RectTransform NewRect(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.layer = parent.gameObject.layer;
        go.transform.SetParent(parent, false);
        return (RectTransform)go.transform;
    }

    private static void Stretch(RectTransform rt, float left, float right, float top, float bottom)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = new Vector2(left, bottom);
        rt.offsetMax = new Vector2(-right, -top);
    }

    private static Image AddImage(GameObject go, Color color)
    {
        Image img = go.AddComponent<Image>();
        img.sprite = BuiltinSprite("UI/Skin/UISprite.psd");
        img.type = Image.Type.Sliced;
        img.color = color;
        return img;
    }

    private static LayoutElement SetLayout(GameObject go, float preferredHeight, float preferredWidth = -1f,
        float flexibleWidth = -1f, float flexibleHeight = -1f)
    {
        LayoutElement le = go.GetComponent<LayoutElement>();
        if (le == null) le = go.AddComponent<LayoutElement>();
        if (preferredHeight >= 0f) { le.preferredHeight = preferredHeight; le.minHeight = preferredHeight; }
        if (preferredWidth >= 0f) { le.preferredWidth = preferredWidth; le.minWidth = preferredWidth; }
        if (flexibleWidth >= 0f) le.flexibleWidth = flexibleWidth;
        le.flexibleHeight = flexibleHeight >= 0f ? flexibleHeight : 0f;
        return le;
    }

    private static TextMeshProUGUI NewText(Transform parent, string name, string text, float size, Color color,
        TextAlignmentOptions alignment)
    {
        RectTransform rt = NewRect(name, parent);
        TextMeshProUGUI t = rt.gameObject.AddComponent<TextMeshProUGUI>();
        t.text = text;
        t.fontSize = size;
        t.color = color;
        t.alignment = alignment;
        t.raycastTarget = false;
        t.textWrappingMode = TextWrappingModes.NoWrap;
        return t;
    }

    private static void NewHeader(Transform parent, string title)
    {
        TextMeshProUGUI header = NewText(parent, "Header_" + title, title, HeaderFont, HeaderTextColor, TextAlignmentOptions.BottomLeft);
        header.fontStyle = FontStyles.Bold;
        SetLayout(header.gameObject, HeaderHeight);
    }

    private static RectTransform NewRow(Transform parent, string name)
    {
        RectTransform row = NewRect(name, parent);
        SetLayout(row.gameObject, RowHeight);
        HorizontalLayoutGroup layout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 14f;
        layout.childAlignment = TextAnchor.MiddleLeft;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = true;
        return row;
    }

    private static TextMeshProUGUI NewRowLabel(Transform row, string text, float width)
    {
        TextMeshProUGUI label = NewText(row, "Label", text, RowFont, LightTextColor, TextAlignmentOptions.MidlineLeft);
        SetLayout(label.gameObject, -1f, width);
        return label;
    }

    private static Button NewButton(Transform parent, string name, string label, float fontSize, Color background, Color textColor)
    {
        RectTransform rt = NewRect(name, parent);
        Image img = AddImage(rt.gameObject, background);
        Button button = rt.gameObject.AddComponent<Button>();
        button.targetGraphic = img;

        TextMeshProUGUI text = NewText(rt, "Label", label, fontSize, textColor, TextAlignmentOptions.Center);
        Stretch(text.rectTransform, 10f, 10f, 0f, 0f);
        return button;
    }

    private static Slider NewSlider(Transform parent, string name, Color track, Color fill, Color knob)
    {
        const float handleHalf = 17f;
        const float trackHalf = 7f;

        RectTransform root = NewRect(name, parent);
        root.sizeDelta = new Vector2(300f, 54f);
        Slider slider = root.gameObject.AddComponent<Slider>();

        RectTransform background = NewRect("Background", root);
        CenterBand(background, 0f, trackHalf);
        AddImage(background.gameObject, track);

        RectTransform fillArea = NewRect("Fill Area", root);
        CenterBand(fillArea, handleHalf, trackHalf);

        RectTransform fillRect = NewRect("Fill", fillArea);
        Image fillImage = AddImage(fillRect.gameObject, fill);
        fillImage.raycastTarget = false;

        RectTransform handleArea = NewRect("Handle Slide Area", root);
        CenterBand(handleArea, handleHalf, handleHalf);

        RectTransform handle = NewRect("Handle", handleArea);
        handle.sizeDelta = new Vector2(handleHalf * 2f, 0f);
        Image handleImage = handle.gameObject.AddComponent<Image>();
        handleImage.sprite = BuiltinSprite("UI/Skin/Knob.psd");
        handleImage.color = knob;

        slider.fillRect = fillRect;
        slider.handleRect = handle;
        slider.targetGraphic = handleImage;
        slider.direction = Slider.Direction.LeftToRight;

        fillRect.offsetMin = new Vector2(-handleHalf, 0f);
        fillRect.offsetMax = Vector2.zero;
        return slider;
    }

    private static void CenterBand(RectTransform rt, float sidePadding, float halfHeight)
    {
        rt.anchorMin = new Vector2(0f, 0.5f);
        rt.anchorMax = new Vector2(1f, 0.5f);
        rt.offsetMin = new Vector2(sidePadding, -halfHeight);
        rt.offsetMax = new Vector2(-sidePadding, halfHeight);
    }

    private static Toggle NewToggle(Transform parent, string name, string label)
    {
        RectTransform row = NewRect(name, parent);
        SetLayout(row.gameObject, RowHeight);
        Image hitArea = row.gameObject.AddComponent<Image>();
        hitArea.color = new Color(0f, 0f, 0f, 0f);

        RectTransform box = NewRect("Box", row);
        box.anchorMin = box.anchorMax = new Vector2(0f, 0.5f);
        box.pivot = new Vector2(0f, 0.5f);
        box.sizeDelta = new Vector2(ToggleBox, ToggleBox);
        box.anchoredPosition = Vector2.zero;
        Image boxImage = AddImage(box.gameObject, ButtonColor);

        RectTransform check = NewRect("Checkmark", box);
        Stretch(check, 4f, 4f, 4f, 4f);
        Image checkImage = check.gameObject.AddComponent<Image>();
        checkImage.sprite = BuiltinSprite("UI/Skin/Checkmark.psd");
        checkImage.color = AccentColor;
        checkImage.raycastTarget = false;

        TextMeshProUGUI text = NewText(row, "Label", label, RowFont, LightTextColor, TextAlignmentOptions.MidlineLeft);
        RectTransform textRect = text.rectTransform;
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(ToggleBox + 16f, 0f);
        textRect.offsetMax = Vector2.zero;

        Toggle toggle = row.gameObject.AddComponent<Toggle>();
        toggle.targetGraphic = boxImage;
        toggle.graphic = checkImage;
        toggle.isOn = true;
        return toggle;
    }

    private static TMP_InputField NewInputField(Transform parent, string name)
    {
        TMP_DefaultControls.Resources resources = new TMP_DefaultControls.Resources();
        resources.standard = BuiltinSprite("UI/Skin/UISprite.psd");
        resources.background = BuiltinSprite("UI/Skin/Background.psd");
        resources.inputField = BuiltinSprite("UI/Skin/InputFieldBackground.psd");
        resources.knob = BuiltinSprite("UI/Skin/Knob.psd");
        resources.checkmark = BuiltinSprite("UI/Skin/Checkmark.psd");
        resources.dropdown = BuiltinSprite("UI/Skin/DropdownArrow.psd");
        resources.mask = BuiltinSprite("UI/Skin/UIMask.psd");

        GameObject go = TMP_DefaultControls.CreateInputField(resources);
        go.name = name;
        go.transform.SetParent(parent, false);
        SetLayerRecursively(go, parent.gameObject.layer);

        TMP_InputField input = go.GetComponent<TMP_InputField>();
        input.contentType = TMP_InputField.ContentType.IntegerNumber;
        input.characterLimit = 9;
        input.textComponent.fontSize = RowFont;
        TMP_Text placeholder = input.placeholder as TMP_Text;
        if (placeholder != null)
        {
            placeholder.fontSize = RowFont;
            placeholder.text = "seed";
        }
        return input;
    }

    private static void SetLayerRecursively(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform child in go.transform) SetLayerRecursively(child.gameObject, layer);
    }

    #endregion

    #region Пошук і зв'язування

    private static GameObject FindInScene(string objectName)
    {
        foreach (GameObject rootObject in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            foreach (Transform t in rootObject.GetComponentsInChildren<Transform>(true))
            {
                if (t.name == objectName) return t.gameObject;
            }
        }
        return null;
    }

    private static T FindComponent<T>() where T : Component
    {
        foreach (GameObject rootObject in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            T found = rootObject.GetComponentInChildren<T>(true);
            if (found != null) return found;
        }
        return null;
    }

    private static void RemoveChild(Transform parent, string childName)
    {
        Transform old = parent.Find(childName);
        while (old != null)
        {
            Undo.DestroyObjectImmediate(old.gameObject);
            old = parent.Find(childName);
        }
    }

    private static void Wire(Object target, params object[] pairs)
    {
        SerializedObject so = new SerializedObject(target);
        for (int i = 0; i + 1 < pairs.Length; i += 2)
        {
            string field = (string)pairs[i];
            SerializedProperty property = so.FindProperty(field);
            if (property == null)
            {
                Debug.LogError("[MineCavesUIBuilder] Немає поля " + field + " у " + target.GetType().Name);
                continue;
            }

            Object value = pairs[i + 1] as Object;
            if (value == null) Debug.LogWarning("[MineCavesUIBuilder] " + target.GetType().Name + "." + field + " залишилося порожнім — признач вручну");
            property.objectReferenceValue = value;
        }
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void SetFloat(Object target, string field, float value)
    {
        SerializedObject so = new SerializedObject(target);
        SerializedProperty property = so.FindProperty(field);
        if (property != null) property.floatValue = value;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    #endregion
}
