using UnityEngine;

[System.Serializable]
public class RoomPrefabEntry
{
    public RoomType roomType;
    public GameObject prefab;
}

public class RoomPrefabDatabase : MonoBehaviour
{
    public RoomPrefabEntry[] roomPrefabs;

    public GameObject GetPrefab(RoomType type)
    {
        foreach (var entry in roomPrefabs)
        {
            if (entry.roomType == type)
                return entry.prefab;
        }

        return null;
    }
}
