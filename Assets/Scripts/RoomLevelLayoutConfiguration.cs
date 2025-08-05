using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Room Level Layout", menuName = "Custom/Procedural Generation/RoomLevelLayoutConfiguration")]
public class RoomLevelLayoutConfiguration : ScriptableObject
{
    [SerializeField] private int width = 64;
    [SerializeField] private int length = 64;
    [SerializeField] private RoomTemplate[] roomTemplates;
    [SerializeField] private int doorDistanceFromEdge = 1;
    [SerializeField] private int minCorridorLength = 2;
    [SerializeField] private int maxCorridorLength = 5;
    [SerializeField] private int maxRoomCount = 10;
    [SerializeField] private int minRoomDistance = 1;

    public int Width => width;
    public int Length => length;
    public RoomTemplate[] RoomTemplates => roomTemplates;
    public int DoorDistanceFromEdge => doorDistanceFromEdge;
    public int MinCorridorLength => minCorridorLength;
    public int MaxCorridorLength => maxCorridorLength;
    public int MaxRoomCount => maxRoomCount;
    public int MinRoomDistance => minRoomDistance;

    public Dictionary<RoomTemplate, int> GetAvaiableRooms()
    {
        Dictionary<RoomTemplate, int> avaiableRooms = new Dictionary<RoomTemplate, int>();

        for (int i = 0; i < roomTemplates.Length; i++)
        {
            avaiableRooms.Add(roomTemplates[i], roomTemplates[i].NumberOfRooms);
        }

        avaiableRooms = avaiableRooms.Where(kvp => kvp.Value > 0).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        return avaiableRooms;
    }
}
