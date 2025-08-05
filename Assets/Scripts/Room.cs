using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    RectInt area;

    public RectInt Area => area;
    public Texture2D LayoutTexture { get; }

    public Room(RectInt area)
    {
        this.area = area;
    }

    public Room(int x, int y, Texture2D layoutTexture)
    {
        area = new RectInt(x, y, layoutTexture.width, layoutTexture.height);
        LayoutTexture = layoutTexture;
    }

    public List<Hallway> CalculateAllPossibleDoorways(int width, int length, int minDistanceFromEdge)
    {
        if (LayoutTexture == null)
        {
            return CalculateAllPossibleDoorwaysForRectangularRooms(width, length, minDistanceFromEdge);
        }
        else
        {
            return CalculateAllPossibleDoorwayPositions(LayoutTexture);
        }
    }

    public List<Hallway> CalculateAllPossibleDoorwaysForRectangularRooms(int width, int length, int minDistanceFromEdge)
    {
        List<Hallway> hallwayCandidates = new List<Hallway>();

        int top = length - 1;
        int minX = minDistanceFromEdge;
        int maxX = width - minDistanceFromEdge;

        for (int x = minX; x < maxX; x++)
        {
            hallwayCandidates.Add(new Hallway(HallwayDirection.Bottom, new Vector2Int(x, 0)));
            hallwayCandidates.Add(new Hallway(HallwayDirection.Top, new Vector2Int(x, top)));
        }

        int right = width - 1;
        int minY = minDistanceFromEdge;
        int maxY = length - minDistanceFromEdge;

        for (int y = minY; y < maxY; y++)
        {
            hallwayCandidates.Add(new Hallway(HallwayDirection.Left, new Vector2Int(0, y)));
            hallwayCandidates.Add(new Hallway(HallwayDirection.Right, new Vector2Int(right, y)));
        }

        Debug.Log(hallwayCandidates[0]);

        return hallwayCandidates;
    }

    private List<Hallway> CalculateAllPossibleDoorwayPositions(Texture2D layoutTexture)
    {
        List<Hallway> possibleHallwayPositions = new List<Hallway>();

        Hallway testHallway = new Hallway(HallwayDirection.Bottom, Vector2Int.zero);
        Hallway testHallway2 = new Hallway(HallwayDirection.Left, Vector2Int.zero);

        possibleHallwayPositions.Add(testHallway);
        possibleHallwayPositions.Add(testHallway2);

        return possibleHallwayPositions;
    }
}
