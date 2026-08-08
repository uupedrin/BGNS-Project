using System.Xml.Serialization;
using UnityEngine;

[System.Serializable]
public partial class SaveData
{
    private static SaveData _current;
    public static SaveData current
    {
        get
        {
            return _current ??= new SaveData();
        }

        set
        {
            _current = value;
        }
    }
}
