using UnityEngine;

public partial class SaveData
{
    private HouseData _houseData;
    public HouseData houseData
    {
        get => _houseData ??= new HouseData();
        set => _houseData = value;
    }
}

[System.Serializable]
public class HouseData
{
    public int currentHealth;
}
