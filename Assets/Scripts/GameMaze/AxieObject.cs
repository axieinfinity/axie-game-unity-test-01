using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

public class AxieObject : MonoBehaviour, IMazeObject
{
    public AxieFigure figure { get; private set; }
    public Vector2Int mapPos { get; private set; }

    private void Awake()
    {
        figure = gameObject.GetComponentInChildren<AxieFigure>();
    }

    public void SetMapPos(int mapX, int mapY)
    {
        this.mapPos = new Vector2Int(mapX, mapY);
        transform.localPosition = new Vector3(0.5f + mapX - 6, mapY - 6 + 0.5f, 0f);
    }
}
