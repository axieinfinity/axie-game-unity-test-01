using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemState
{
    public int code;
    public int mapX;
    public int mapY;
    public bool available;
}

public class AxieState
{
    public int mapX;
    public int mapY;
    public Dictionary<string, int> consumableItems = new Dictionary<string, int>();
}

public class DoorState
{
    public int level;
    public int colMapX;
    public int colMapY;
    public bool locked;
}

public class FloorState
{
    public List<List<int>> map;
    public List<ItemState> itemStates;
    public List<DoorState> doorStates;
}

public enum MoveResult
{
    Invalid,
    Valid,
    Require_Key_A,
    Require_Key_B,
}

public class MazeState
{
    public const int MAP_SIZE = 12;
    public const int MAP_CODE_WALL = 1000;
    public const int MAP_CODE_CLEAR = 1001;
    public const int MAP_CODE_DOOR_A = 1100;
    public const int MAP_CODE_DOOR_B = 1101;

    public const int MAP_CODE_START = 2000;
    public const int MAP_CODE_END = 2001;
    public const int MAP_CODE_KEY_A = 2100;
    public const int MAP_CODE_KEY_B = 2101;

    public List<FloorState> floors = new List<FloorState>();
    public AxieState axie = new AxieState();
    public int currentFloorIdx;
    public bool isWon;

    public void LoadMaps(string[] floorMaps)
    {
        floors.Clear();
        axie.consumableItems.Clear();
        currentFloorIdx = 0;
        isWon = false;

        for (int k = 0; k < floorMaps.Length; k++)
        {
            var map = new List<List<int>>();
            var itemStates = new List<ItemState>();
            var doorStates = new List<DoorState>();
            for (int i = 0; i < MAP_SIZE * 2 + 1; i++)
            {
                var line = new List<int>();
                map.Add(line);
                for (int j = 0; j < MAP_SIZE * 2 + 1; j++)
                {
                    int code = MAP_CODE_WALL;
                    if (i % 2 == 1 && j % 2 == 0)
                    {
                        int x = (int)(j / 2);
                        int y = (int)(i / 2);
                        char val = floorMaps[k][i * (MAP_SIZE * 2 + 1) + j];
                        if (val == '|')
                        {
                            code = MAP_CODE_WALL;
                        }
                        else if (val == 'D')
                        {
                            code = MAP_CODE_DOOR_A;
                        }
                        else if (val == 'E')
                        {
                            code = MAP_CODE_DOOR_B;
                        }
                        else
                        {
                            code = MAP_CODE_CLEAR;
                        }

                        if (code >= MAP_CODE_DOOR_A && code <= MAP_CODE_DOOR_B)
                        {
                            var doorState = new DoorState();
                            doorState.colMapX = j;
                            doorState.colMapY = i;
                            doorState.level = code - MAP_CODE_DOOR_A;
                            doorState.locked = true;
                            doorStates.Add(doorState);
                        }
                    }
                    else if (i % 2 == 0 && j % 2 == 1)
                    {
                        int x = (int)(j / 2);
                        int y = (int)(i / 2);
                        char val = floorMaps[k][i * (MAP_SIZE * 2 + 1) + j];
                        if (val == '_')
                        {
                            code = MAP_CODE_WALL;
                        }
                        else if (val == 'D')
                        {
                            code = MAP_CODE_DOOR_A;
                        }
                        else if (val == 'E')
                        {
                            code = MAP_CODE_DOOR_B;
                        }
                        else
                        {
                            code = MAP_CODE_CLEAR;
                        }

                        if (code >= MAP_CODE_DOOR_A && code <= MAP_CODE_DOOR_B)
                        {
                            var doorState = new DoorState();
                            doorState.colMapX = j;
                            doorState.colMapY = i;
                            doorState.level = code - MAP_CODE_DOOR_A;
                            doorState.locked = true;
                            doorStates.Add(doorState);
                        }
                    }
                    else if (i % 2 == 1 && j % 2 == 1)
                    {
                        int x = (int)(j / 2);
                        int y = (int)(i / 2);
                        var val = floorMaps[k][i * (MAP_SIZE * 2 + 1) + j];
                        switch (val)
                        {
                            case 's':
                                {
                                    code = MAP_CODE_START;
                                    if (k == 0)
                                    {
                                        this.axie.mapX = x;
                                        this.axie.mapY = y;
                                    }
                                    break;
                                }
                            case 't': code = MAP_CODE_END; break;

                            case 'k': code = MAP_CODE_KEY_A; break;
                            case 'l': code = MAP_CODE_KEY_B; break;
                        }
                        if (code >= MAP_CODE_KEY_A && code <= MAP_CODE_KEY_B)
                        {
                            var itemState = new ItemState();
                            itemState.mapX = x;
                            itemState.mapY = y;
                            itemState.code = code;
                            itemState.available = true;
                            itemStates.Add(itemState);
                        }
                    }

                    line.Add(code);
                }
            }

            var floorState = new FloorState { map = map, itemStates = itemStates, doorStates = doorStates };
            this.floors.Add(floorState);
        }
    }

    public MoveResult TestMove(int dx, int dy)
    {
        if ((Mathf.Abs(dx) + Mathf.Abs(dy) != 1)) return MoveResult.Invalid;

        var floorMap = this.floors[this.currentFloorIdx];

        int nx = this.axie.mapX + dx;
        int ny = this.axie.mapY + dy;
        if (nx < 0 || nx >= MAP_SIZE || ny < 0 || ny >= MAP_SIZE) return MoveResult.Invalid;
        int wallVal = MAP_CODE_CLEAR;
        int colMapX, colMapY;
        if (dx != 0)
        {
            colMapX = (this.axie.mapX + (dx == 1 ? 1 : 0)) * 2;
            colMapY = this.axie.mapY * 2 + 1;
            wallVal = floorMap.map[colMapY][colMapX];
        }
        else
        {
            colMapX = this.axie.mapX * 2 + 1;
            colMapY = (this.axie.mapY + (dy == 1 ? 1 : 0)) * 2;
            wallVal = floorMap.map[colMapY][colMapX];
        }

        if (wallVal != MAP_CODE_CLEAR)
        {
            if(wallVal == MAP_CODE_DOOR_A)
            {
                return MoveResult.Require_Key_A;
            }
            else if (wallVal == MAP_CODE_DOOR_B)
            {
                return MoveResult.Require_Key_B;
            }
            else
            {
                return MoveResult.Invalid;
            }
        }
        return MoveResult.Valid;
    }

    public int GetRoomValue(int x, int y)
    {
        //public const int MAP_CODE_CLEAR = 1001;
        //public const int MAP_CODE_START = 2000;
        //public const int MAP_CODE_END = 2001;
        //public const int MAP_CODE_KEY_A = 2100;
        //public const int MAP_CODE_KEY_B = 2101;

        if (x < 0 || x >= MAP_SIZE || y < 0 || y >= MAP_SIZE) return 0;
        var floorMap = this.floors[this.currentFloorIdx];
        int roomVal = floorMap.map[y * 2 + 1][x * 2 + 1];
        return roomVal;
    }

    public Dictionary<string, string> OnMove(int dx, int dy)
    {
        if ((Mathf.Abs(dx) + Mathf.Abs(dy) != 1)) return new Dictionary<string, string>{ { "action", "none" } };

        var floorMap = this.floors[this.currentFloorIdx];

        int nx = this.axie.mapX + dx;
        int ny = this.axie.mapY + dy;
        if (nx < 0 || nx >= MAP_SIZE || ny < 0 || ny >= MAP_SIZE) return new Dictionary<string, string> { { "action", "none" } };
        int wallVal = MAP_CODE_CLEAR;
        int colMapX, colMapY;
        if (dx != 0)
        {
            colMapX = (this.axie.mapX + (dx == 1 ? 1 : 0)) * 2;
            colMapY = this.axie.mapY * 2 + 1;
            wallVal = floorMap.map[colMapY][colMapX];
        }
        else
        {
            colMapX = this.axie.mapX * 2 + 1;
            colMapY = (this.axie.mapY + (dy == 1 ? 1 : 0)) * 2;
            wallVal = floorMap.map[colMapY][colMapX];
        }

        var logs = new Dictionary<string, string>();
        if (wallVal != MAP_CODE_CLEAR)
        {
            if (wallVal >= MAP_CODE_DOOR_A && wallVal <= MAP_CODE_DOOR_B)
            {
                this.UnlockDoor(floorMap, colMapX, colMapY, logs);
                return logs;
            }
            else
            {
                return new Dictionary<string, string> { { "action", "none" } };
            }
        }
        int newRoomVal = floorMap.map[ny * 2 + 1][nx * 2 + 1];

        //if (newRoomVal == MAP_CODE_START && this.currentFloorIdx > 0)
        //{
        //    this.currentFloorIdx -= 1;
        //    logs["action"] = "enterFloor";
        //}
        //else
        if (newRoomVal == MAP_CODE_END)
        {
            if ((this.currentFloorIdx < this.floors.Count - 1))
            {
                this.currentFloorIdx += 1;
                logs["action"] = "enterFloor";
            }
            else
            {
                this.isWon = true;
                logs["action"] = "enterFloor";
            }
        }
        else if (newRoomVal >= MAP_CODE_KEY_A && newRoomVal <= MAP_CODE_KEY_B)
        {
            this.GainKey(floorMap, nx, ny, logs);
        }
        else
        {
            logs["action"] = "move";
        }

        this.axie.mapX = nx;
        this.axie.mapY = ny;

        return logs;
    }

    private bool GainKey(FloorState floorState, int targetMapX, int targetMapY, Dictionary<string, string> logs)
    {
        var itemState = floorState.itemStates.Find(x => x.mapX == targetMapX && x.mapY == targetMapY);
        if (itemState == null)
        {
            logs["action"] = "none";
            return false;
        }
        logs["action"] = "gainKey";

        floorState.map[targetMapY * 2 + 1][targetMapX * 2 + 1] = MAP_CODE_CLEAR;
        itemState.available = false;
        string keyId = "key-" + System.Convert.ToChar(System.Convert.ToByte('a') + itemState.code - MAP_CODE_KEY_A);
        if (this.axie.consumableItems.ContainsKey(keyId))
        {
            this.axie.consumableItems[keyId] += 1;
        }
        else
        {
            this.axie.consumableItems[keyId] = 1;
        }
        return true;
    }

    private bool UnlockDoor(FloorState floorState, int colMapX, int colMapY, Dictionary<string, string> logs)
    {
        int doorLevel = floorState.map[colMapY][colMapX] - MAP_CODE_DOOR_A;
        string keyId = "key-" + System.Convert.ToChar(System.Convert.ToByte('a') + doorLevel);

        if (this.axie.consumableItems.ContainsKey(keyId))
        {
            if (this.axie.consumableItems[keyId] >= 1)
            {
                var doorState = floorState.doorStates.Find(x => x.colMapX == colMapX && x.colMapY == colMapY);
                if (doorState == null)
                {
                    logs["action"] = "none";
                    return false;
                }
                this.axie.consumableItems[keyId] -= 1;
                floorState.map[colMapY][colMapX] = MAP_CODE_CLEAR;
                doorState.locked = false;
                logs["action"] = "unlockDoor";
            }
        }
        else
        {
            logs["action"] = "none";
            return false;
        }
        return true;
    }
}
