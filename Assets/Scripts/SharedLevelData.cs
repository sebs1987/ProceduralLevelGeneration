using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

[ExecuteAlways]
[DisallowMultipleComponent]
public class SharedLevelData : MonoBehaviour
{
    public static SharedLevelData Instance { get; private set; }


    [SerializeField] private int scale = 1;
    [SerializeField] private int seed = Environment.TickCount;

    private Random random;

    public int Scale => scale;
    public Random Rand => random;

    [ContextMenu("Generate New Seed")]
    public void GenerateSeed()
    {
        seed = Environment.TickCount;
        random = new Random(seed);
    }

    private void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != null)
        {
            enabled = false;

            Debug.LogWarning("Duplicate SharedLevelData detected and disabled." + this);
        }

        Debug.Log(Instance.GetInstanceID());

        random = new Random(seed);
    }

    public void ResetRandom()
    {
        random = new Random(seed);
    }
}
