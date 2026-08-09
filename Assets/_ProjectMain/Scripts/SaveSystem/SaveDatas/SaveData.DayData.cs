using UnityEngine;

public partial class SaveData
{
    private DayData _dayData;
    public DayData dayData
    {
        get => _dayData ??= new DayData();
        set => _dayData = value;
    }
}

[System.Serializable]
public class DayData
{
    public int currentDay = 1;
}