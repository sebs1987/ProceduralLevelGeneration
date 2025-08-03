using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hallway : MonoBehaviour
{
    private Vector2Int startPosition;
    private Vector2Int endPosition;

    private HallwayDirection startDirection;
    private HallwayDirection endDirection;

    private Room startRoom;
    private Room endRoom;

    public Room StartRoom { get { return startRoom; } set { startRoom = value; } }
    public Room EndRoom { get { return endRoom; } set { endRoom = value; } }

    public Vector2Int StartPositionAbsolute { get { return startPosition + startRoom.Area.position; } }
    public Vector2Int EndPositionAbsolute { get { return endPosition + endRoom.Area.position; } }

    public HallwayDirection StartDirection { get { return startDirection; } }
    public HallwayDirection EndDirection { get { return endDirection; } set { endDirection = value; } }

    public Hallway(HallwayDirection startDirection, Vector2Int startPosition, Room startRoom = null)
    {
        this.startDirection = startDirection;
        this.startPosition = startPosition;
        this.startRoom = startRoom;
    }
}
