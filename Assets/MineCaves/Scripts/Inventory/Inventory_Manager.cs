using UnityEngine;

public class Inventory_Manager : MonoBehaviour
{
    public static Inventory_Manager instance;

    private IInventoryUIModule topModule;
    private IInventoryUIModule bottomModule;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void RegisterTopModule(IInventoryUIModule module)
    {
        topModule = module;
        TriggerUIUpdate();
    }

    public void RegisterBottomModule(IInventoryUIModule module)
    {
        bottomModule = module;
        TriggerUIUpdate();
    }

    public void UnregisterTopModule() => topModule = null;
    public void UnregisterBottomModule() => bottomModule = null;

    public void TriggerUIUpdate()
    {
        if (bottomModule != null) bottomModule.RefreshModuleUI();
        if (topModule != null) topModule.RefreshModuleUI();
    }
}