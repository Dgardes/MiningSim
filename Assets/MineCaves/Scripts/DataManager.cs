using System.IO;
using UnityEngine;

public static class DataManager
{

    private static readonly string BaseFolderPath = Path.Combine(Application.persistentDataPath, "MineCaves Data");
    private static readonly string PlayerFolderPath = Path.Combine(BaseFolderPath, "Player");
    private static readonly string InventoryFilePath = Path.Combine(PlayerFolderPath, "Inventory.json");

    public static void SaveInventoryData(string jsonText)
    {
        try
        {
            if (!Directory.Exists(PlayerFolderPath))
            {
                Directory.CreateDirectory(PlayerFolderPath);
            }

            File.WriteAllText(InventoryFilePath, jsonText);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[DataManager] Помилка збереження інвентарю: {e.Message}");
        }
    }

    public static string LoadInventoryData()
    {
        if (!File.Exists(InventoryFilePath))
        {
            return null;
        }

        try
        {
            return File.ReadAllText(InventoryFilePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[DataManager] Помилка завантаження інвентарю: {e.Message}");
            return null;
        }
    }
}