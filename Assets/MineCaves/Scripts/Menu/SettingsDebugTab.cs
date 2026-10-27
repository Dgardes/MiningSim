using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsDebugTab : MonoBehaviour
{
    [Header("Посилання на світ")]
    [SerializeField] private MenuManager menuManager;
    [SerializeField] private WGenDebug worldDebug;
    [SerializeField] private ChunkGenerator generator;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Transform player;
    [SerializeField] private Transform surfacePoint;
    [SerializeField] private float fallbackSurfaceY = 2f;

    [Header("Світ")]
    [SerializeField] private Toggle chunkBordersToggle;
    [SerializeField] private TMP_InputField seedInput;
    [SerializeField] private Button applySeedButton;
    [SerializeField] private Button randomSeedButton;
    [SerializeField] private Button teleportButton;
    [SerializeField] private Toggle overlayToggle;

    [Header("Інвентар")]
    [SerializeField] private Button giveItemsButton;
    [SerializeField] private Button clearInventoryButton;

    [Header("Гравець")]
    [SerializeField] private Button fullHealthButton;

    private DebugStatRow[] statRows;

    private void Awake()
    {
        statRows = GetComponentsInChildren<DebugStatRow>(true);

        chunkBordersToggle.onValueChanged.AddListener(OnChunkBordersToggled);

        applySeedButton.onClick.AddListener(OnApplySeed);
        randomSeedButton.onClick.AddListener(OnRandomSeed);
        teleportButton.onClick.AddListener(OnTeleportToSurface);
        overlayToggle.onValueChanged.AddListener(OnOverlayToggled);
        fullHealthButton.onClick.AddListener(OnFullHealth);
        if (giveItemsButton != null) giveItemsButton.onClick.AddListener(OnGiveItems);
        if (clearInventoryButton != null) clearInventoryButton.onClick.AddListener(OnClearInventory);
    }

    private void OnEnable()
    {
        chunkBordersToggle.SetIsOnWithoutNotify(GameSettings.ShowChunkBorders);

        if (generator != null) seedInput.SetTextWithoutNotify(generator.Seed.ToString());
        overlayToggle.SetIsOnWithoutNotify(GameSettings.ShowDebugOverlay);

        foreach (DebugStatRow row in statRows) row.Bind(playerStats);
    }

    #region Світ

    private void OnApplySeed()
    {
        if (generator == null || worldDebug == null) return;

        int newSeed;
        if (!int.TryParse(seedInput.text, out newSeed))
        {
            seedInput.SetTextWithoutNotify(generator.Seed.ToString());
            return;
        }

        generator.SetSeed(newSeed);
        worldDebug.ReloadWorld();
    }

    private void OnRandomSeed()
    {
        seedInput.SetTextWithoutNotify(Random.Range(1, 999999).ToString());
        OnApplySeed();
    }

    private void OnTeleportToSurface()
    {
        if (player == null) return;

        Vector3 target = surfacePoint != null
            ? surfacePoint.position
            : new Vector3(player.position.x, fallbackSurfaceY, player.position.z);

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.position = target;
            rb.linearVelocity = Vector2.zero;
        }
        player.position = target;

        if (menuManager != null) menuManager.closeMenu();
    }

    private void OnChunkBordersToggled(bool value)
    {
        GameSettings.SetShowChunkBorders(value);
    }

    private void OnOverlayToggled(bool value)
    {
        GameSettings.SetShowDebugOverlay(value);
    }

    #endregion

    #region Інвентар

    private void OnGiveItems()
    {
        DebugTestItems.GiveAll(Inventory_PlayerInvLogic.instance, 5, 15);
    }

    private void OnClearInventory()
    {
        DebugTestItems.Clear(Inventory_PlayerInvLogic.instance);
    }

    #endregion

    #region Гравець

    private void OnFullHealth()
    {
        if (playerStats == null) return;
        playerStats.Heal(playerStats.MaxHealth);
    }

    #endregion
}
