![](https://skymavis.com/images/branding.svg)

# UNITY TEST: AXIE GAME 01

## About Axie Infinity

Axie Infinity is a virtual world filled with cute, formidable creatures known as Axies. Axies can be battled, bred,
collected, and even used to earn resources & collectibles that can be traded on an open marketplace.
Axie was designed to introduce the world to an exciting new technology called Blockchain, through a fun, nostalgic, &
charming game.

## The Challenge Begins

### Guide our Axie to escape

One of our adorable Mystic Axies is trapped inside a maze, and we must guide it to escape by reaching the **escape point
**. Unfortunately, the **point** is hidden behind several **gates**, which can only be opened by **keys** of the _same
colors_.

So, in short: Grab key(s) --> Open gate(s) --> Escape!

> **Important**: A key can only be used **once** to open a Gate with the _same color_.

Using the given source code (in Unity), make a pathfinding algorithm to help our Axie get out of the maze.

### Samples

Some maze samples are illustrated in pictures below:

- **Axie's current positions** are shown as its **Axie figure images**.
- **Escape points** are shown as the **Gray squares**.
- **Keys** and **Gates** are shown as **Key images** and **Thick lines**.

![Sample map](images/01.png?raw=true)

### To control the Axie

Once per loop (0.25 second) , Axie can only perform at most **one** move action, which is one of functions below:

```csharp
  this.moveAxie(-1, 0);
  this.moveAxie(1, 0);
  this.moveAxie(0, -1);
  this.moveAxie(0, 1);
```

If our Axie steps on any key or gate, it automatically grabs or unlocks. The simulation continues until our axie reaches
the escape point (Gray square).

> Note: Axie can only move in four directions **(-1, 0)**, **(1, 0)**, **(0, -1)** and **(0, 1)**. Do not attempt to use
> other inputs, or our Axie may stumble somewhere 😔

### Implementation

Open file `GameMazeManager.cs` located in `Assets/Scripts/GameMaze/GameMazeManager.cs`. Implement your move algorithm in the function `onSimulateTurn()`.

```csharp
    //***************YOUR CODE HERE**************************/
    void OnSimulateTurn()
    {   
        //TODO: Discard randomness, make smart moves to reach the goal
        
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
```

### Requirements

- Our Axie should only be moved using provided functions with valid parameters.
- Your code must be able to solve mazes beyond the sample ones.
- You are allowed to create new files or functions, but ensure that you do not edit any existing files (except the allowed
  function).

## Submission

- Create your own git repository, so we can see your work process.

## Deadline

This test is designed for 5 - 10 hours of coding. We know you might be busy with your current work, the maximum deadline
is **7 days** after you received this test.

## Good luck!

![](https://pbs.twimg.com/media/FKm5bKbWUAEePGX.jpg:large)
