using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LayoutGeneratorRooms : MonoBehaviour
{
    [SerializeField] private int width = 64;
    [SerializeField] private int length = 64;

    [SerializeField] private int roomWidthMin = 3;
    [SerializeField] private int roomWidthMax = 5;
    [SerializeField] private int roomLengthMin = 3;
    [SerializeField] private int roomLengthMax = 5;
    [SerializeField] private int minCorridorLength = 2;
    [SerializeField] private int maxCorridorLength = 5;
    [SerializeField] private int maxRoomCount = 10;

    [SerializeField] private GameObject levelLayoutDisplay;
    [SerializeField] private List<Hallway> openDoorways;

    private System.Random random;
    private Level level;


    [ContextMenu("Generate Level Layout")]
    public void GenerateLevel()
    {
        random = new System.Random();
        openDoorways = new List<Hallway>();
        level = new Level(width, length);

        RectInt roomRect = GetStartRoomRect();

        Debug.Log(roomRect);

        Room room = new Room(roomRect);

        List<Hallway> hallways = room.CalculateAllPossibleDoorways(room.Area.width, room.Area.height, 1);

        hallways.ForEach(h => h.StartRoom = room);
        hallways.ForEach(h => openDoorways.Add(h));

        level.AddRoom(room);

        Hallway selectedEntryway = openDoorways[random.Next(openDoorways.Count)];

        AddRooms();

        DrawLayout(selectedEntryway, roomRect);
    }

    private RectInt GetStartRoomRect()
    {
        // Get random room width
        int roomWidth = random.Next(roomWidthMin, roomWidthMax);
        // Calculate the avaiable space
        int availableWidthX = width / 2 - roomWidth;
        // Get random x-Coordinate
        int randomX = random.Next(0, availableWidthX);
        // Calculate x position for room 
        int roomX = randomX + (width / 4);

        int roomLength = random.Next(roomLengthMin, roomLengthMax);
        int availableLengthY = length / 2 - roomLength;
        int randomY = random.Next(0, availableLengthY);
        int roomY = randomY + (length / 4);

        return new RectInt(roomX, roomY, roomWidth, roomLength);
    }

    private void DrawLayout(Hallway selectedEntryway = null, RectInt roomCandidateRect = new RectInt())
    {
        Renderer renderer = levelLayoutDisplay.GetComponent<Renderer>();

        Texture2D layoutTexture = (Texture2D)renderer.sharedMaterial.mainTexture;

        layoutTexture.Reinitialize(width, length);

        levelLayoutDisplay.transform.localScale = new Vector3(width, length, 1);

        layoutTexture.FillWithColor(Color.black);

        Array.ForEach(level.Rooms, room => layoutTexture.DrawRectangle(room.Area, Color.white));
        Array.ForEach(level.Hallways, hallway => layoutTexture.DrawLine(hallway.StartPositionAbsolute, hallway.EndPositionAbsolute, Color.white));

        layoutTexture.DrawRectangle(roomCandidateRect, Color.blue);

        openDoorways.ForEach(hallway => layoutTexture.SetPixel(hallway.StartPositionAbsolute.x, hallway.StartPositionAbsolute.y, hallway.StartDirection.GetColor()));

        if (selectedEntryway != null)
        {
            layoutTexture.SetPixel(selectedEntryway.StartPositionAbsolute.x, selectedEntryway.StartPositionAbsolute.y, Color.red);
        }

        layoutTexture.SaveAsset();
    }

    private Hallway SelectHallwayCandidate(RectInt roomCandidateRect, Hallway entryway)
    {
        Room room = new Room(roomCandidateRect);

        List<Hallway> candidates = room.CalculateAllPossibleDoorways(room.Area.width, room.Area.height, 1);

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
        RectInt roomCandidateRect = new RectInt
        {
            width = random.Next(roomWidthMin, roomWidthMax),
            height = random.Next(roomLengthMin, roomLengthMax)
        };

        Hallway selectedExit = SelectHallwayCandidate(roomCandidateRect, selectedEntryway);

        if (selectedExit == null) return null;

        int distance = random.Next(minCorridorLength, maxCorridorLength + 1);
        Vector2Int roomCandidatePosition = CalculateRoomPosition
                                                                (
                                                                    selectedEntryway, 
                                                                    roomCandidateRect.width, 
                                                                    roomCandidateRect.height, 
                                                                    distance, 
                                                                    selectedExit.StartPosition
                                                                );

        roomCandidateRect.position = roomCandidatePosition;
        Room newRoom = new Room(roomCandidateRect);

        selectedEntryway.EndRoom = newRoom;
        selectedEntryway.EndPosition = selectedExit.StartPosition;

        return newRoom;
    }

    private void AddRooms()
    {
        while (openDoorways.Count > 0 && level.Rooms.Length < maxRoomCount)
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

            List<Hallway> newOpenHallways = newRoom.CalculateAllPossibleDoorways(newRoom.Area.width, newRoom.Area.height, 1);

            newOpenHallways.ForEach(hallway => hallway.StartRoom = newRoom);

            openDoorways.Remove(selectedEntryway);
            openDoorways.AddRange(newOpenHallways);
        }
    }
}
