# axie-game-unity-test-01

Using the given source code (unity), implement a pathfinding algorithm to help the Axie get out of the maze.

![Sample map](images/01.png?raw=true "Axie Labyrinth")

Gameplay:

- Axie only can do move action
- There are 2 kind objects: Key and Gate. The Gate can be unlocked if the Axie has the key with the same color
- The simulation ends when the Axie reaches and unlocks the Gray Gate

Requirement:

- Use these functions to move the Axie
    - this.moveAxie(-1, 0);
    - this.moveAxie(1, 0);
    - this.moveAxie(0, -1);
    - this.moveAxie(0, 1);
- Code can solve other map
- DO NOT change the game logic code. You can create new files or functions.
- Your algorithm code should be placed in this function, this function is called each 0.25 second:
    
    ```cs
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
    ```
    

## Resources

- Base project repository:
    - [https://github.com/axieinfinity/game-unity-test-01](https://github.com/axieinfinity/game-unity-test-01)

## Submission

- Create your own git repository so we can see your work process.

## Deadline

This test is designed for 5 - 10 hours of coding. We know you might be busy with your current work, the maximum deadline is **7 days** after you received this test.
