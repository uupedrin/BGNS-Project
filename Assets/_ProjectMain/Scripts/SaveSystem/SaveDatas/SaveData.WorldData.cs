using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public partial class SaveData
{
    private WorldData _worldData;
    public WorldData worldData
    {
        get => _worldData ??= new WorldData();
        set => _worldData = value;
    }
}

[System.Serializable]
public class WorldData
{
    public List<string> collectedContainerIds = new();
}
