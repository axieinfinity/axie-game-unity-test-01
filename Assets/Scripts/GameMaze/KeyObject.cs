using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyObject : MonoBehaviour, IMazeObject
{
    SpriteRenderer[] models;
    public Vector2Int mapPos { get; private set; }
    public int level { get; private set; }

    private void Awake()
    {
        models = gameObject.GetComponentsInChildren<SpriteRenderer>();
    }

    public void Setup(int level)
    {
        this.level = level;
        for (int i = 0; i < models.Length; i++)
        {
            models[i].enabled = (i == level);
        }
    }

    public void SetMapPos(int mapX, int mapY)
    {
        this.mapPos = new Vector2Int(mapX, mapY);
        transform.localPosition = new Vector3(0.5f + mapX - 6, mapY - 6 + 0.5f, 0f);
    }
}

