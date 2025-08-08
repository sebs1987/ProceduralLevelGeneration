using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;

public class LevelBuilder : MonoBehaviour
{
    [SerializeField] private LayoutGeneratorRooms layoutGeneratorRooms;
    [SerializeField] private MarchingSquares marchingSquares;
    [SerializeField] private NavMeshSurface navMeshSurface;
    [SerializeField] private RoomLevelLayoutConfiguration roomLevelLayoutConfiguration;

    private Level level;

    private void Start()
    {
        GenerateRandom();
    }

    [ContextMenu("Generate Random")]
    public void GenerateRandom()
    {
        SharedLevelData.Instance.GenerateSeed();

        Generate();

        if (level.Rooms.Length < roomLevelLayoutConfiguration.MinRoomCount)
        {
            GenerateRandom();
        }
    }

    [ContextMenu("Generate")]
    private void Generate()
    {
        level = layoutGeneratorRooms.GenerateLevel();

        marchingSquares.CreateLevelGeometry();
        navMeshSurface.BuildNavMesh();

        Room startRoom = level.playerStartRoom;
        Vector2 roomCenter = startRoom.Area.center;
        Vector3 playerPosition = LevelPositionToWorldPosition(roomCenter);

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        NavMeshAgent playerNavMeshAgent = player.GetComponent<NavMeshAgent>();

        if (playerNavMeshAgent == null)
        {
            player.transform.position = playerPosition;
        } 
        else
        {
            playerNavMeshAgent.Warp(playerPosition);
        }
    }

    private Vector3 LevelPositionToWorldPosition(Vector2 levelPosition)
    {
        int scale = SharedLevelData.Instance.Scale;
        return new Vector3((levelPosition.x - 1) * scale, 0, (levelPosition.y - 1) * scale);
    }
}
