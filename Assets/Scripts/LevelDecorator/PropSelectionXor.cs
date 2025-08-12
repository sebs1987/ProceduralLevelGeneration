using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class PropSelectionXor : MonoBehaviour
{
    [SerializeField] private GameObject[] gameObjects;

    [ContextMenu("GenerateVaration")]
    public void GenerateVaration()
    {
        Random random = SharedLevelData.Instance.Rand;

        foreach (GameObject gameObject in gameObjects)
        {
            gameObject.SetActive(false);
        }

        int selectedIndex = random.Next(0, gameObjects.Length);
        GameObject selectedGameObject = gameObjects[selectedIndex];
        selectedGameObject.SetActive(true);
    }
}
