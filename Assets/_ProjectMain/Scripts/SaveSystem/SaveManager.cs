using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private const string SAVE_NAME = "l2d";
    public static bool pendingLoad;

    public static bool HasSave() => SerializationManager.Exists(SAVE_NAME);

    public static void SaveGame()
    {
        if(InventoryManager.Instance != null)
            SaveData.current.inventoryData = InventoryManager.Instance.CaptureState();
        if (DayNightManager.Instance != null)
            SaveData.current.dayData.currentDay = DayNightManager.Instance.CurrentDay;
        if (HouseHealth.Instance != null)
            SaveData.current.houseData.currentHealth = HouseHealth.Instance.CurrentHealth;

        SerializationManager.Save(SAVE_NAME, SaveData.current);
    }

    public static void LoadGame()
    {
        SaveData loaded = SerializationManager.Load<SaveData>(SAVE_NAME);
        if (loaded == null) return;
        SaveData.current = loaded;
    }

    public static void ApplyPendingLoad()
    {
        if (!pendingLoad || InventoryManager.Instance == null) return;
        InventoryManager.Instance.RestoreState(SaveData.current.inventoryData);
    }

    public static void DeleteSave()
    {
        SerializationManager.Delete(SAVE_NAME);
    }
}
