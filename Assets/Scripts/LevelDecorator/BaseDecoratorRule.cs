using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseDecoratorRule : ScriptableObject
{
    [SerializeField, EnumFlags] private RoomType roomTypes;

    public RoomType RoomTypes => roomTypes;
    internal abstract bool CanBeApplied(TileType[,] levelDecorated, Room room);
    internal abstract void Apply(TileType[,] levelDecorated, Room room, Transform parent);
}