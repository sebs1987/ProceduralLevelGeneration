using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILevel
{
    public int Length { get; }
    public int Width { get; }

    public bool IsBlocked(int x, int y);
    public int Floor(int x, int y);
}
