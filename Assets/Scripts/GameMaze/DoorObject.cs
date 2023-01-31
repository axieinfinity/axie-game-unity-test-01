using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorObject : MonoBehaviour, IMazeObject
{
    SpriteRenderer[] models;
    public Vector2Int mapPos { get; private set; }
    public Vector2Int colMapPos { get; private set; }
    public int level { get; private set; }

    private void Awake()
    {
        models = gameObject.GetComponentsInChildren<SpriteRenderer>();
    }

    public void Setup(int level, int colMapX, int colMapY)
    {
        this.level = level;
        this.colMapPos = new Vector2Int(colMapX, colMapY);

        bool isWallX = colMapX % 2 == 1;

        for (int i = 0; i < models.Length; i++)
        {
            models[i].enabled = (i == level);

        }
        transform.eulerAngles = new Vector3(0f, 0f, isWallX ? 0f : 90f);
    }

    public void SetMapPos(int mapX, int mapY)
    {
        this.mapPos = new Vector2Int(mapX, mapY);
        transform.localPosition = new Vector3(mapX - 6, mapY - 6, 0f);
    }
}

