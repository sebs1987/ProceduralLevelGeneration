using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = System.Random;

public class LayoutGeneratorRooms : MonoBehaviour
{
    [SerializeField] private LevelBuilder levelBuilder;
    [SerializeField] private GameObject levelLayoutDisplay;
    [SerializeField] private List<Hallway> openDoorways;

    [SerializeField] private RoomLevelLayoutConfiguration levelConfig;

    private Random random;
    private Level level;
    private Dictionary<RoomTemplate, int> avaiableRooms;


    [ContextMenu("Generate Level Layout")]
    public Level GenerateLevel()
    {
        SharedLevelData.Instance.ResetRandom();

        random = SharedLevelData.Instance.Rand;

        avaiableRooms = levelConfig.GetAvaiableRooms();

        openDoorways = new List<Hallway>();
        level = new Level(levelConfig.Width, levelConfig.Length);

        RoomTemplate startRoomTemplate = avaiableRooms.Keys.ElementAt(random.Next(0, avaiableRooms.Count));

        RectInt roomRect = GetStartRoomRect(startRoomTemplate);

        Room room = CreateNewRoom(roomRect, startRoomTemplate);

        List<Hallway> hallways = room.CalculateAllPossibleDoorways(room.Area.width, room.Area.height, levelConfig.DoorDistanceFromEdge);

        hallways.ForEach(h => h.StartRoom = room);
        hallways.ForEach(h => openDoorways.Add(h));

        level.AddRoom(room);

        Hallway selectedEntryway = openDoorways[random.Next(openDoorways.Count)];

        AddRooms();
        AddHallwaysToRoom();
        DrawLayout(selectedEntryway, roomRect);

        int startRoomIndex = random.Next(0, level.Rooms.Length);
        level.playerStartRoom = level.Rooms[startRoomIndex];

        return level;
    }

    private void AddHallwaysToRoom()
    {
        foreach (Room room in level.Rooms)
        {
            Hallway[] hallwaysStartingAtRoom = Array.FindAll(level.Hallways, hallway => hallway.StartRoom == room);
            Array.ForEach(hallwaysStartingAtRoom, hallway => room.AddHallway(hallway));

            Hallway[] hallwaysEndingAtRoom = Array.FindAll(level.Hallways, hallway => hallway.EndRoom == room);
            Array.ForEach(hallwaysEndingAtRoom, hallway => room.AddHallway(hallway));
        }
    }

    [ContextMenu("Generate New Seed")]
    public void GenerateNewSeed()
    {
        SharedLevelData.Instance.GenerateSeed();
    }

    [ContextMenu("Generate new Seed and Level")]
    public void GenerateNewSeedAndLevel()
    {
        GenerateNewSeed();
        GenerateLevel();
    }

    private RectInt GetStartRoomRect(RoomTemplate roomTemplate)
    {
        RectInt roomSize = roomTemplate.GenerateRoomCandidateRect(random);

        // Get random room width
        int roomWidth = roomSize.width;
        // Calculate the avaiable space
        int availableWidthX = levelConfig.Width / 2 - roomWidth;
        // Get random x-Coordinate
        int randomX = random.Next(0, availableWidthX);
        // Calculate x position for room 
        int roomX = randomX + (levelConfig.Width / 4);

        int roomLength = roomSize.height;
        int availableLengthY = levelConfig.Length / 2 - roomLength;
        int randomY = random.Next(0, availableLengthY);
        int roomY = randomY + (levelConfig.Length / 4);

        return new RectInt(roomX, roomY, roomWidth, roomLength);
    }

    private void DrawLayout(Hallway selectedEntryway = null, RectInt roomCandidateRect = new RectInt(), bool isDebug = false)
    {
        Renderer renderer = levelLayoutDisplay.GetComponent<Renderer>();

        Texture2D layoutTexture = (Texture2D)renderer.sharedMaterial.mainTexture;

        layoutTexture.Reinitialize(levelConfig.Width, levelConfig.Length);

        int scale = SharedLevelData.Instance.Scale;
        levelLayoutDisplay.transform.localScale = new Vector3(levelConfig.Width * scale, levelConfig.Length * scale, 1);

        float xPos = level.Width * scale / 2.0f - scale;
        float zPos = level.Length * scale / 2.0f - scale;
        levelLayoutDisplay.transform.position = new Vector3(xPos, 0.1f, zPos);

        layoutTexture.FillWithColor(Color.black);

        foreach (Room room in level.Rooms)
        {
            if (room.LayoutTexture != null)
            {
                layoutTexture.DrawTexture(room.LayoutTexture, room.Area);
            }
            else
            {
                layoutTexture.DrawRectangle(room.Area, Color.white);
            }

            Debug.Log(room.Area + " " + room.Connectedness);
        }

        Array.ForEach(level.Hallways, hallway => layoutTexture.DrawLine(hallway.StartPositionAbsolute, hallway.EndPositionAbsolute, Color.white));

        layoutTexture.ConvertToBlackAndWhite();

        if (isDebug)
        {
            layoutTexture.DrawRectangle(roomCandidateRect, Color.blue);

            openDoorways.ForEach(hallway => layoutTexture.SetPixel(hallway.StartPositionAbsolute.x, hallway.StartPositionAbsolute.y, hallway.StartDirection.GetColor()));
        }

        if (isDebug && selectedEntryway != null)
        {
            layoutTexture.SetPixel(selectedEntryway.StartPositionAbsolute.x, selectedEntryway.StartPositionAbsolute.y, Color.red);
        }

        layoutTexture.SaveAsset();
    }

    private Hallway SelectHallwayCandidate(RectInt roomCandidateRect, RoomTemplate roomTemplate, Hallway entryway)
    {
        Room room = CreateNewRoom(roomCandidateRect, roomTemplate, false);

        List<Hallway> candidates = room.CalculateAllPossibleDoorways(room.Area.width, room.Area.height, levelConfig.DoorDistanceFromEdge);

        HallwayDirection requiredDirection = entryway.StartDirection.GetOppositeDirection();

        List<Hallway> filteredHallwayCandidates = candidates.Where(hallwayCandidate => hallwayCandidate.StartDirection == requiredDirection).ToList();

        return filteredHallwayCandidates.Count > 0 ? filteredHallwayCandidates[random.Next(filteredHallwayCandidates.Count)] : null;
    }

    private Vector2Int CalculateRoomPosition(Hallway entryway, int roomWidth, int roomLength, int distance, Vector2Int endPosition)
    {
        Vector2Int roomPosition = entryway.StartPositionAbsolute;

        switch (entryway.StartDirection)
        {
            case HallwayDirection.Left:

                roomPosition.x -= distance + roomWidth;
                roomPosition.y -= endPosition.y;

                break;

            case HallwayDirection.Top:

                roomPosition.x -= endPosition.x;
                roomPosition.y += distance + 1;

                break;

            case HallwayDirection.Right:

                roomPosition.x += distance + 1;
                roomPosition.y -= endPosition.y;

                break;

            case HallwayDirection.Bottom:

                roomPosition.x -= endPosition.x;
                roomPosition.y -= distance + roomLength;

                break;
        }

        return roomPosition;
    }

    private Room ConstructAdjacentRoom(Hallway selectedEntryway)
    {
        RoomTemplate roomTemplate = avaiableRooms.Keys.ElementAt(random.Next(0, avaiableRooms.Count));

        RectInt roomCandidateRect = roomTemplate.GenerateRoomCandidateRect(random);

        Hallway selectedExit = SelectHallwayCandidate(roomCandidateRect, roomTemplate, selectedEntryway);

        if (selectedExit == null) return null;

        int distance = random.Next(levelConfig.MinCorridorLength, levelConfig.MaxCorridorLength + 1);
        Vector2Int roomCandidatePosition = CalculateRoomPosition
                                                                (
                                                                    selectedEntryway, 
                                                                    roomCandidateRect.width, 
                                                                    roomCandidateRect.height, 
                                                                    distance, 
                                                                    selectedExit.StartPosition
                                                                );

        roomCandidateRect.position = roomCandidatePosition;

        if (!IsRoomCandidateValid(roomCandidateRect))
        {
            return null;
        }

        Room newRoom = CreateNewRoom(roomCandidateRect, roomTemplate);

        selectedEntryway.EndRoom = newRoom;
        selectedEntryway.EndPosition = selectedExit.StartPosition;

        return newRoom;
    }

    private void AddRooms()
    {
        while (openDoorways.Count > 0 && level.Rooms.Length < levelConfig.MaxRoomCount && avaiableRooms.Count > 0)
        {
            Hallway selectedEntryway = openDoorways[random.Next(0, openDoorways.Count)];
            Room newRoom = ConstructAdjacentRoom(selectedEntryway);

            if (newRoom == null)
            {
                openDoorways.Remove(selectedEntryway);
                continue;
            }

            level.AddRoom(newRoom);
            level.AddHallway(selectedEntryway);

            selectedEntryway.EndRoom = newRoom;

            List<Hallway> newOpenHallways = newRoom.CalculateAllPossibleDoorways(newRoom.Area.width, newRoom.Area.height, levelConfig.DoorDistanceFromEdge);

            newOpenHallways.ForEach(hallway => hallway.StartRoom = newRoom);

            openDoorways.Remove(selectedEntryway);
            openDoorways.AddRange(newOpenHallways);
        }
    }

    private bool IsRoomCandidateValid(RectInt roomCandidateRect)
    {
        RectInt levelRect = new RectInt(1, 1, levelConfig.Width - 2, levelConfig.Length - 2);

        return levelRect.Contains(roomCandidateRect) && !CheckRoomOverlap(roomCandidateRect, level.Rooms, level.Hallways, levelConfig.MinRoomDistance);
    }

    private bool CheckRoomOverlap(RectInt roomCandidateRect, Room[] rooms, Hallway[] hallways, int minRoomDistance)
    {
        RectInt paddedRoomRect = new RectInt
        {
            x = roomCandidateRect.x - minRoomDistance,
            y = roomCandidateRect.y - minRoomDistance,
            width = roomCandidateRect.width + 2 * minRoomDistance,
            height = roomCandidateRect.height + 2 * minRoomDistance,
        };

        foreach (Room room in rooms)
        {
            if (paddedRoomRect.Overlaps(room.Area))
            {
                return true;
            }
        }

        foreach (Hallway hallway in hallways)
        {
            if (paddedRoomRect.Overlaps(hallway.Area))
            {
                return true;
            }
        }

        return false;
    }

    private void UseUpRoomTemplate(RoomTemplate roomTemplate)
    {
        avaiableRooms[roomTemplate] -= 1;
        if (avaiableRooms[roomTemplate] == 0)
        {
            avaiableRooms.Remove(roomTemplate);
        }
    }

    private Room CreateNewRoom(RectInt roomCandidateRect, RoomTemplate roomTemplate, bool useUp = true)
    {
        if (useUp)
        {
            UseUpRoomTemplate(roomTemplate);
        }

        if (roomTemplate.LayoutTexture == null)
        {
            return new Room(roomCandidateRect);
        }
        else
        {
            return new Room(roomCandidateRect.x, roomCandidateRect.y, roomTemplate.LayoutTexture);
        }
    }
}
