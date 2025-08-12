using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

[Serializable]
public class RuleAvailability
{
    public BaseDecoratorRule rule;
    public int maxAvailability;

    public RuleAvailability(RuleAvailability other)
    {
        rule = other.rule;
        maxAvailability = other.maxAvailability;
    }
}

public class RoomDecorator : MonoBehaviour
{
    [SerializeField] private GameObject parent;
    [SerializeField] private LayoutGeneratorRooms layoutGenerator;
    [SerializeField] private Texture2D levelTexture;
    [SerializeField] private Texture2D decoratedTexture;
    [SerializeField] private RuleAvailability[] availableRules;

    private Random random;

    [ContextMenu("Place Items")]
    public void PlaceItemsFromMenu()
    {
        SharedLevelData.Instance.ResetRandom();
        Level level = layoutGenerator.GenerateLevel();

        PlaceItems(level);
    }

    public void PlaceItems(Level level)
    {
        random = SharedLevelData.Instance.Rand;
        Transform decorationsTransform = parent.transform.Find("Decorations");

        if (decorationsTransform == null)
        {
            GameObject decorationGameObject = new GameObject("Decorations");
            decorationsTransform = decorationGameObject.transform;
            decorationsTransform.SetParent(parent.transform);
        }
        else
        {
            decorationsTransform.DestroyAllChildren();
        }

        TileType[,] levelDecorated = InitializeDecoratorArray();

        foreach(Room room in level.Rooms)
        {
            DecorateRoom(levelDecorated, room, decorationsTransform);
        }

        GenerateTextureFromTileType(levelDecorated);
    }

    private void DecorateRoom(TileType[,] levelDecorated, Room room, Transform decorationsTransform)
    {
        int currentTries = 0;
        int maxTries = 50;

        int currentNumberOfDecorations = 0;
        int maxNumberOfDecorations = room.Area.width * room.Area.height * 2;

        List<RuleAvailability> availableRulesForRoom = CopyRuleAvailability();

        while (currentTries < maxTries && availableRulesForRoom.Count > 0 && currentNumberOfDecorations < maxNumberOfDecorations)
        {
            int selectedRuleIndex = random.Next(0, availableRulesForRoom.Count);
            RuleAvailability selectedRuleAvailabilty = availableRulesForRoom[selectedRuleIndex];

            BaseDecoratorRule selectedRule = selectedRuleAvailabilty.rule;

            if (selectedRule.CanBeApplied(levelDecorated, room))
            {
                selectedRule.Apply(levelDecorated, room, decorationsTransform);

                currentNumberOfDecorations++;

                if (selectedRuleAvailabilty.maxAvailability > 0)
                {
                    selectedRuleAvailabilty.maxAvailability--;
                }

                if (selectedRuleAvailabilty.maxAvailability == 0)
                {
                    availableRulesForRoom.Remove(selectedRuleAvailabilty);
                }
            }

            currentTries++;
        }
    }

    private List<RuleAvailability> CopyRuleAvailability()
    {
        List<RuleAvailability> availableRulesForRoom = new List<RuleAvailability>();
        availableRules.ToList().ForEach(original => availableRulesForRoom.Add(new RuleAvailability(original)));

        return availableRulesForRoom;
    }

    private TileType[,] InitializeDecoratorArray()
    {
        TileType[,] levelDecorated = new TileType[levelTexture.width, levelTexture.height];

        for (int y = 0; y < levelTexture.height; y++)
        {
            for (int x = 0; x < levelTexture.width; x++)
            {
                Color pixelColor = levelTexture.GetPixel(x, y);

                if (pixelColor == Color.black)

                {
                    levelDecorated[x, y] = TileType.Wall;
                }
                else
                {
                    levelDecorated[x, y] = TileType.Floor;
                }
            }
        }

        return levelDecorated;
    }

    private void GenerateTextureFromTileType(TileType[,] tileTypes)
    {
        int width = tileTypes.GetLength(0);
        int length = tileTypes.GetLength(1);

        Color32[] pixels = new Color32[width * length];

        for (int y = 0; y < length; y++)
        {
            for (int x = 0; x < width; x++)
            {
                pixels[x + y * width] = tileTypes[x, y].GetColor();
            }
        }


        decoratedTexture.Reinitialize(width, length);
        decoratedTexture.SetPixels32(pixels);
        decoratedTexture.Apply();
        decoratedTexture.SaveAsset();
    }
}
