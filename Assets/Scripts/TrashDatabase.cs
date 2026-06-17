using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct TrashData
{
    public string id;          // RFID識別番号
    public string name;        // ごみの名前
    public int targetBinType;  // 0:可燃, 1:プラ, 2:缶・ペットボトル
    [TextArea]
    public string description; // 解説文
}

[CreateAssetMenu(fileName = "TrashDatabase", menuName = "SeriousGame/TrashDatabase")]
public class TrashDatabase : ScriptableObject
{
    public List<TrashData> items = new List<TrashData>();

    public TrashData GetById(string id)
    {
        return items.Find(t => t.id == id);
    }
}