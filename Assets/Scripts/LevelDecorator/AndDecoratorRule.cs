using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "AndDecoratorRule", menuName = "Custom/Procedural Generation/And Decorator Rule")]
public class AndDecoratorRule : BaseDecoratorRule
{
    [SerializeField] private BaseDecoratorRule[] childRules;

    internal override void Apply(TileType[,] levelDecorated, Room room, Transform parent)
    {
        foreach (BaseDecoratorRule rule in childRules)
        {
            rule.Apply(levelDecorated, room, parent);
        }
    }

    internal override bool CanBeApplied(TileType[,] levelDecorated, Room room)
    {
        foreach (BaseDecoratorRule rule in childRules)
        {
            if (!rule.CanBeApplied(levelDecorated, room))
            {
                return false;
            }
        }

        return true;
    }
}
