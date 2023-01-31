using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GameMazeManager : MonoBehaviour
{
    [SerializeField] UILayer uiLayer;
    [SerializeField] GameObject[] sampleObjects;
    [SerializeField] AxieObject axie;
    Mesh mesh;
    MazeState mazeState;
    bool isPlaying;

    float turnDelay;
    float autoMoveDelay;

    Dictionary<string, List<IMazeObject>> pools = new Dictionary<string, List<IMazeObject>>();

    private void Awake()
    {
        mesh = new Mesh
        {
            name = "Procedural Mesh"
        };
        GetComponent<MeshFilter>().mesh = mesh;

        AxieMixer.Unity.Mixer.Init();
        string axieId = PlayerPrefs.GetString("selectingId", "2727");
        string genes = PlayerPrefs.GetString("selectingGenes", "0x2000000000000300008100e08308000000010010088081040001000010a043020000009008004106000100100860c40200010000084081060001001410a04406");
        axie.figure.SetGenes(axieId, genes);

        mazeState = new MazeState();
        ResetGame();
    }

    IMazeObject GetGameObject(string key)
    {
        var sample = sampleObjects.FirstOrDefault(x => x.name == key);
        if (sample == null) return null;

        if (!pools.ContainsKey(key))
        {
            pools.Add(key, new List<IMazeObject>());
        }
        var lst = pools[key];

        var mazeGO = lst.FirstOrDefault(x => !x.gameObject.activeSelf);
        if(mazeGO == null)
        {
            var go = Instantiate(sample);
            mazeGO = (IMazeObject)go.GetComponent(typeof(IMazeObject));
            lst.Add(mazeGO);
        }
        else
        {
            mazeGO.gameObject.SetActive(true);
        }
        return mazeGO;
    }

    void ResetGame()
    {
        this.uiLayer.SetResultFrame(false);

        mazeState.LoadMaps(MapPool.FLOOR_MAPS);

        this.uiLayer.SetInventoryStates(this.mazeState.axie.consumableItems);

        this.axie.SetMapPos(this.mazeState.axie.mapX, this.mazeState.axie.mapY);

        this.EnterFloor(this.mazeState.currentFloorIdx);
        this.isPlaying = true;
        this.turnDelay = 0.1f;
        this.autoMoveDelay = 5;
    }

    void EnterFloor(int idx)
    {
        this.mazeState.currentFloorIdx = idx;
        var floorMap = this.mazeState.floors[this.mazeState.currentFloorIdx];

        foreach (var p in pools)
        {
            foreach (var q in p.Value)
            {
                q.gameObject.SetActive(false);
            }
        }

        mesh.Clear();
        List<Vector3> vertices = new List<Vector3>();
        List<Color> colors = new List<Color>();
        List<int> triangles = new List<int>();
        for (int i = 0; i < MazeState.MAP_SIZE * 2 + 1; i++)
        {
            for (int j = 0; j < MazeState.MAP_SIZE * 2 + 1; j++)
            {
                int x = (int)(j / 2);
                int y = (int)(i / 2);
                int val = floorMap.map[i][j];
                if (i % 2 == 1 && j % 2 == 0)
                {
                    if (val == MazeState.MAP_CODE_WALL)
                    {
                        DrawRect((x - 6), (y - 6), 0.05f, 1f, vertices, colors, triangles);
                    }
                }
                else if (i % 2 == 0 && j % 2 == 1)
                {
                    if (val == MazeState.MAP_CODE_WALL)
                    {
                        DrawRect((x - 6), (y - 6), 1f, 0.05f, vertices, colors, triangles);
                    }
                }
                else if (i % 2 == 1 && j % 2 == 1)
                {
                    if (val == MazeState.MAP_CODE_END)
                    {
                        DrawRect((x - 6 + 0.2f), (y - 6 + 0.2f), 0.6f, 0.6f, vertices, colors, triangles);
                    }
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.colors = colors.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateBounds();

        foreach (var itemState in floorMap.itemStates){
            if (!itemState.available) continue;
            if (itemState.code >= MazeState.MAP_CODE_KEY_A && itemState.code <= MazeState.MAP_CODE_KEY_B)
            {
                var key = GetGameObject("KeyObject") as KeyObject;
                if (key != null)
                {
                    key.SetMapPos(itemState.mapX, itemState.mapY);
                    key.Setup(itemState.code - MazeState.MAP_CODE_KEY_A);
                }
            }
        }

        foreach (var doorState in floorMap.doorStates){
            if (!doorState.locked) continue;
            var door = GetGameObject("DoorObject") as DoorObject;
            if (door != null)
            {
                door.SetMapPos((int)(doorState.colMapX / 2),(int)(doorState.colMapY / 2));
                door.Setup(doorState.level, doorState.colMapX, doorState.colMapY);
            }
        }

        this.axie.SetMapPos(this.mazeState.axie.mapX, this.mazeState.axie.mapY);
    }

    void DrawRect(float x, float y, float w, float h, List<Vector3> vertices, List<Color> colors, List<int> triangles)
    {
        int off = vertices.Count;

        vertices.Add(new Vector3(x, y, 0f));
        vertices.Add(new Vector3(x + w, y, 0f));
        vertices.Add(new Vector3(x, y + h, 0f));
        vertices.Add(new Vector3(x + w, y + h, 0f));

        colors.Add(Color.gray);
        colors.Add(Color.gray);
        colors.Add(Color.gray);
        colors.Add(Color.gray);

        triangles.Add(off);
        triangles.Add(off + 1);
        triangles.Add(off + 2);

        triangles.Add(off + 3);
        triangles.Add(off + 1);
        triangles.Add(off + 2);
    }

    private void MoveAxie(int dx, int dy)
    {
        //if (!this.isPlaying || this.axie == null) return;
        int nx = this.mazeState.axie.mapX + dx;
        int ny = this.mazeState.axie.mapY + dy;
        int colMapX, colMapY;
        if (dx != 0)
        {
            colMapX = (this.mazeState.axie.mapX + (dx == 1 ? 1 : 0)) * 2;
            colMapY = this.mazeState.axie.mapY * 2 + 1;
        }
        else
        {
            colMapX = this.mazeState.axie.mapX * 2 + 1;
            colMapY = (this.mazeState.axie.mapY + (dy == 1 ? 1 : 0)) * 2;
        }
        var logs = this.mazeState.OnMove(dx, dy);
        if (!logs.ContainsKey("action")) return;

        string action = logs["action"];

        switch (action)
        {
            case "move":
                this.axie.SetMapPos(this.mazeState.axie.mapX, this.mazeState.axie.mapY);
                break;
            case "enterFloor":
                if (this.mazeState.isWon)
                {
                    this.GameOver(true);
                }
                else
                {
                    this.EnterFloor(this.mazeState.currentFloorIdx);
                }
                this.axie.SetMapPos(this.mazeState.axie.mapX, this.mazeState.axie.mapY);
                break;
            case "gainKey":
                this.SyncKey(nx, ny);
                this.axie.SetMapPos(this.mazeState.axie.mapX, this.mazeState.axie.mapY);
                break;
            case "unlockDoor":
                this.SyncDoor(colMapX, colMapY);
                break;

            default:
                break;
        }
    }

    private void SyncKey(int mapX, int mapY)
    {
        var floorMap = this.mazeState.floors[this.mazeState.currentFloorIdx];
        var key = this.pools["KeyObject"].Find(x => x.mapPos.x == mapX && x.mapPos.y == mapY);
        var itemState = floorMap.itemStates.Find(x => x.mapX == mapX && x.mapY == mapY);
        if (key == null || itemState == null) return;
        if (!itemState.available)
        {
            key.gameObject.SetActive(false);
        }
        this.uiLayer.SetInventoryStates(this.mazeState.axie.consumableItems);
    }

    private void SyncDoor(int colMapX, int colMapY)
    {
        var door = this.pools["DoorObject"].Find(x => (x as DoorObject).colMapPos.x == colMapX && (x as DoorObject).colMapPos.y == colMapY);
        if (door == null) return;

        var floorMap = this.mazeState.floors[this.mazeState.currentFloorIdx];
        if (floorMap.map[colMapY][colMapX] == MazeState.MAP_CODE_CLEAR)
        {
            door.gameObject.SetActive(false);
        }
        this.uiLayer.SetInventoryStates(this.mazeState.axie.consumableItems);
    }

    private void GameOver(bool isWon)
    {
        this.isPlaying = false;
        this.uiLayer.SetResultFrame(true);
        this.uiLayer.SetResultText(isWon);
    }

    private void Update()
    {
        if (!isPlaying)
        {
            if (Input.anyKeyDown)
            {
                this.ResetGame();
            }
            return;
        }

        if (Input.GetKeyDown("1"))
        {
            EnterFloor(0);
        }
        else if (Input.GetKeyDown("2"))
        {
            EnterFloor(1);
        }
        else if (Input.GetKeyDown("3"))
        {
            EnterFloor(2);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            this.autoMoveDelay = 5;
            MoveAxie(-1, 0);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            this.autoMoveDelay = 5;
            MoveAxie(1, 0);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            this.autoMoveDelay = 5;
            MoveAxie(0, 1);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            this.autoMoveDelay = 5;
            MoveAxie(0, -1);
        }

        this.autoMoveDelay -= Time.deltaTime;
        if (this.autoMoveDelay <= 0)
        {
            this.turnDelay -= Time.deltaTime;
            if (this.turnDelay <= 0)
            {
                this.turnDelay = 0.25f;
                this.OnSimulateTurn();
            }
        }
    }

    //***************YOUR CODE HERE**************************/
    void OnSimulateTurn()
    {
        //Do you check and give the action to reach the goal
        var floorMap = this.mazeState.floors[this.mazeState.currentFloorIdx];
        Vector2Int targetPos = Vector2Int.zero;
        for (int y = 0; y < MazeState.MAP_SIZE; y++)
        {
            for (int x = 0; x < MazeState.MAP_SIZE; x++)
            {
                int roomVal = this.mazeState.GetRoomValue(x, y);
                if (roomVal == MazeState.MAP_CODE_END)
                {
                    targetPos = new Vector2Int(x, y); 
                }
            }
        }

        Debug.Log($"curPos: {mazeState.axie.mapX},{mazeState.axie.mapY} targetPos: {targetPos.x},{targetPos.y} Item remain: {floorMap.itemStates.Count}");
        int ranVal = Random.Range(0, 4);
        if (ranVal == 0 && mazeState.TestMove(-1, 0) == MoveResult.Valid)
        {
            this.MoveAxie(-1, 0);
        }
        else if (ranVal == 1 && mazeState.TestMove(1, 0) == MoveResult.Valid)
        {
            this.MoveAxie(1, 0);
        }
        else if (ranVal == 2 && mazeState.TestMove(0, -1) == MoveResult.Valid)
        {
            this.MoveAxie(0, -1);
        }
        else if (ranVal == 3 && mazeState.TestMove(0, 1) == MoveResult.Valid)
        {
            this.MoveAxie(0, 1);
        }
    }
}
