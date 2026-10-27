using UnityEngine;

public class UI_ButtonsManager : MonoBehaviour
{
    [SerializeField] private GameObject Canvas_Inventory;
    [SerializeField] private GameObject Canvas_UI;

    [Header("Containers Zones")]
    [SerializeField] private Transform bottomContainer;
    [SerializeField] private Transform topContainer;

    [Header("Modular Prefabs")]
    [SerializeField] private GameObject backpackPrefab;

    private GameObject activeBackpackInstance; 

    public void openInventory()
    {
        Canvas_Inventory.SetActive(true);
        Canvas_UI.SetActive(false);

        if (backpackPrefab != null && bottomContainer != null)
        {
            activeBackpackInstance = Instantiate(backpackPrefab, bottomContainer);

            RectTransform rect = activeBackpackInstance.GetComponent<RectTransform>();
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = Vector2.zero;

            if (activeBackpackInstance.TryGetComponent(out IInventoryUIModule module))
            {
                Inventory_Manager.instance.RegisterBottomModule(module);
            }
        }
    }

    public void closeInventory()
    {
        Canvas_Inventory.SetActive(false);
        Canvas_UI.SetActive(true);

        if (activeBackpackInstance != null)
        {
            Destroy(activeBackpackInstance);
        }

        if (Inventory_Manager.instance != null)
        {
            Inventory_Manager.instance.UnregisterBottomModule();
        }
    }
}