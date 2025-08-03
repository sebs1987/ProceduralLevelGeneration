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
    [SerializeField] private int roomLengthMin = 5;
    [SerializeField] private int roomLengthMax = 5;

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
        Hallway selectedExit = SelectHallwayCandidate(new RectInt(0, 0, 5, 7), selectedEntryway);

        Debug.Log(selectedExit.StartPosition);
        Debug.Log(selectedExit.StartDirection);

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
}
