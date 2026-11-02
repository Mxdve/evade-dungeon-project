using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemplateRoomFiller : MonoBehaviour
{
    [Header("1x1 Room Prefabs")]
    public GameObject secondFloorIllusionRoom;
    public GameObject ceoOffice;
    public GameObject logoARoom;
    public GameObject logoBRoom;
    public GameObject breakRoom;

    public GameObject storageRoom;
    public GameObject forkLiftRoom;
    public GameObject meetingRoom;
    public GameObject normalOffice;

    [Header("2x1 Room Prefabs")]
    public GameObject mailRoom;
    public GameObject conveyorRoom;
    public GameObject bathroom;

    [Header("Treasure")]
    public GameObject treasureChest;
    [Range(0f, 1f)]
    public float chestSpawnChance = 0.6f;

    public bool GenerationComplete { get; private set; }

    private List<Transform> extraRoomSpots =
        new List<Transform>();

    IEnumerator Start()
    {
        yield return null;

        PlaceBathrooms();
        FillRegularRooms();
        FillSpecialRooms();

        GenerationComplete = true;
    }

    void FillRegularRooms()
    {
        List<GameObject> roomPool =
            new List<GameObject>()
            {
                secondFloorIllusionRoom,

                ceoOffice,

                logoARoom,
                logoBRoom,

                breakRoom,

                storageRoom,
                storageRoom,

                forkLiftRoom,
                forkLiftRoom,

                meetingRoom,
                meetingRoom,

                normalOffice,
                normalOffice
            };

        Shuffle(roomPool);

        RoomData[] placeholders =
            FindObjectsOfType<RoomData>();

        Debug.Log(
            $"Found {placeholders.Length} total RoomData objects"
        );

        List<Transform> regularTemplates =
            new List<Transform>();

        foreach (RoomData room in placeholders)
        {
            if (room.gameObject.name.StartsWith("BathroomSpot"))
            {
                Debug.Log(
                    "FOUND BATHROOM SPOT: " +
                    room.gameObject.name
                );
            }

            if (
                room.gameObject.name.StartsWith(
                    "RoomTemplate("
                )
            )
            {
                regularTemplates.Add(
                    room.transform
                );
            }
        }

        Debug.Log(
            $"Found {regularTemplates.Count} regular templates"
        );

        foreach (Transform room in regularTemplates)
        {
            Debug.Log(
                "REGULAR TEMPLATE: " +
                room.name
            );
        }

        foreach (Transform extraSpot in extraRoomSpots)
        {
            regularTemplates.Add(
                extraSpot
            );
        }

        Shuffle(regularTemplates);

        for (
            int i = 0;
            i < Mathf.Min(
                roomPool.Count,
                regularTemplates.Count
            );
            i++
        )
        {
            Transform t =
                regularTemplates[i];

            /*Instantiate(
                roomPool[i],
                t.position,
                t.rotation
            );*/

            int[] rotations = { 0, 90, 180, 270 };

            int randomRotation =
                rotations[Random.Range(0, rotations.Length)];

            Quaternion roomRotation =
                t.rotation * Quaternion.Euler(0, randomRotation, 0);

            GameObject spawnedRoom = Instantiate(
                roomPool[i],
                t.position,
                roomRotation
            );

            // Random chance to spawn a treasure chest.
            if (Random.value <= chestSpawnChance)
            {
                Transform treasureSpawn =
                    spawnedRoom.transform.Find("TreasureSpawn");

                if (treasureSpawn != null)
                {
                    Instantiate(
                        treasureChest,
                        treasureSpawn.position,
                        treasureSpawn.rotation
                    );
                }
                else
                {
                    Debug.LogWarning(
                        spawnedRoom.name + " has no TreasureSpawn!"
                    );
                }
            }

            Debug.Log(
                $"Replacing {t.name} with {roomPool[i].name}"
            );

            Destroy(t.gameObject);
        }
    }

    void FillSpecialRooms()
    {
        RoomData[] placeholders =
            FindObjectsOfType<RoomData>();

        List<RoomData> specialTemplates =
            new List<RoomData>();

        foreach (RoomData room in placeholders)
        {
            if (room.gameObject.name.StartsWith("SpecialTemplate"))
            {
                specialTemplates.Add(room);
            }
        }

        Debug.Log(
            $"Found {specialTemplates.Count} special room templates"
        );

        if (specialTemplates.Count != 2)
        {
            Debug.LogWarning("Expected 2 special room templates.");
            return;
        }

        // Randomly decide which placeholder gets which room.
        Shuffle(specialTemplates);

        // Place MailRoom in the first placeholder.
        Instantiate(
            mailRoom,
            specialTemplates[0].transform.position,
            specialTemplates[0].transform.rotation
        );

        // Place ConveyorRoom in the second placeholder.
        Instantiate(
            conveyorRoom,
            specialTemplates[1].transform.position,
            specialTemplates[1].transform.rotation
        );

        // Remove the original placeholders.
        foreach (RoomData room in specialTemplates)
        {
            Destroy(room.gameObject);
        }
    }

    float GetBathroomRotation(string spotName)
    {
        switch (spotName)
        {
            case "BathroomSpotTop1":
            case "BathroomSpotTop3":
                return 270f;

            case "BathroomSpotTop2":
            case "BathroomSpotTop4":
                return 90f;

            case "BathroomSpotSide1":
            case "BathroomSpotSide3":
                return 0f;

            case "BathroomSpotSide2":
            case "BathroomSpotSide4":
                return 180f;
        }

        return 0f;
    }

    void PlaceBathrooms()
    {
        RoomData[] placeholders =
            FindObjectsOfType<RoomData>();

        List<RoomData> topSpots =
            new List<RoomData>();

        List<RoomData> sideSpots =
            new List<RoomData>();

        foreach (RoomData room in placeholders)
        {
            if (
                room.gameObject.name.StartsWith(
                    "BathroomSpotTop"
                )
            )
            {
                topSpots.Add(room);
            }

            if (
                room.gameObject.name.StartsWith(
                    "BathroomSpotSide"
                )
            )
            {
                sideSpots.Add(room);
            }
        }

        bool useTop =
            Random.value > 0.5f;

        List<RoomData> chosenGroup =
            useTop ? topSpots : sideSpots;

        Shuffle(chosenGroup);

        // First 2 become bathrooms
        for (int i = 0; i < 2; i++)
        {
            float rotationY =
                GetBathroomRotation(
                    chosenGroup[i].gameObject.name
                );

            Instantiate(
                bathroom,
                chosenGroup[i].transform.position,
                Quaternion.Euler(
                    0,
                    rotationY,
                    0
                )
            );

            Debug.Log(
                "Placed bathroom at " +
                chosenGroup[i].gameObject.name
            );

            Destroy(
                chosenGroup[i].gameObject
            );
        }

        // Remaining spots become regular room locations
        for (int i = 2; i < chosenGroup.Count; i++)
        {
            extraRoomSpots.Add(
                chosenGroup[i].transform
            );
        }

        // The OTHER hallway group's spots
        // also become normal room locations
        List<RoomData> unusedGroup =
            useTop ? sideSpots : topSpots;

        foreach (RoomData spot in unusedGroup)
        {
            extraRoomSpots.Add(
                spot.transform
            );
        }

        Debug.Log(
            "Bathrooms placed on " +
            (useTop ? "Top" : "Side")
        );
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex =
                Random.Range(
                    i,
                    list.Count
                );

            T temp = list[i];

            list[i] =
                list[randomIndex];

            list[randomIndex] =
                temp;
        }
    }
}
