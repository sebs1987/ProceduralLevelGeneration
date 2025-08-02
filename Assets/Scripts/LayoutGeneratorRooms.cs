using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutGeneratorRooms : MonoBehaviour
{
    [SerializeField] private int width = 64;
    [SerializeField] private int length = 64;

    [SerializeField] private int roomWidthMin = 3;
    [SerializeField] private int roomWidthMax = 5;
    [SerializeField] private int roomLengthMin = 5;
    [SerializeField] private int roomLengthMax = 5;

    [SerializeField] private GameObject levelLayoutDisplay;

    
    private List<Hallway> openDoorways;

    System.Random random;


    [ContextMenu("Generate Level Layout")]
    public void GenerateLevel()
    {
        random = new System.Random();
        openDoorways = new List<Hallway>();

        RectInt roomRect = GetStartRoomRect();
        Debug.Log(roomRect);

        Room room = new Room(roomRect);

        List<Hallway> hallways = room.CalculateAllPossibleDoorways(room.Area.width, room.Area.height, 1);

        hallways.ForEach(h => h.StartRoom = room);
        hallways.ForEach(h => openDoorways.Add(h));

        DrawLayout(roomRect);
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

    private void DrawLayout(RectInt roomCandidateRect = new RectInt())
    {
        Renderer renderer = levelLayoutDisplay.GetComponent<Renderer>();

        Texture2D layoutTexture = (Texture2D)renderer.sharedMaterial.mainTexture;

        layoutTexture.Reinitialize(width, length);

        levelLayoutDisplay.transform.localScale = new Vector3(width, length, 1);

        layoutTexture.FillWithColor(Color.black);
        layoutTexture.DrawRectangle(roomCandidateRect, Color.cyan);

        foreach (Hallway hallway in openDoorways)
        {
            layoutTexture.SetPixel(hallway.StartPositionAbsolute.x, hallway.StartPositionAbsolute.y, Color.red);
        }

        layoutTexture.SaveAsset();
    }
}
