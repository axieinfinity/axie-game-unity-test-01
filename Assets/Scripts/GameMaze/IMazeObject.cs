using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMazeObject
{
    GameObject gameObject { get; }
    Vector2Int mapPos { get; }
}
