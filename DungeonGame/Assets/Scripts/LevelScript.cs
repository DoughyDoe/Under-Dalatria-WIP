using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScript : MonoBehaviour {
    enum gridSpace { empty, floor, wall, slime};
    gridSpace[,] grid;
    int roomHeight, roomWidth;
    Vector2 roomSizeWorldUnits = new Vector2(60, 40);
    float worldUnitsInOneGridCell = 1;
    struct walker
    {
        public Vector2 dir;
        public Vector2 pos;
    }
    List<walker> walkers;
    public float chanceWalkerChangeDir = .01f;
    public float chanceWalkerCreate = .05f;
    public float chanceWalkerDestroy = .01f;
    public int MAX_WALKERS = 10;
    public float percentToFill = .9f;
    public GameObject wallObj, floorObj, slimeObj;
   
    
    
    
    // Use this for initialization



    void Start () {
        Setup();
        CreateFloors();
        CreateWalls();
        CreateSlime();
        SpawnLevel();
	}


    void Setup()
    {
        //find grid size
        roomHeight = Mathf.RoundToInt(roomSizeWorldUnits.x / worldUnitsInOneGridCell);
        roomWidth = Mathf.RoundToInt(roomSizeWorldUnits.y / worldUnitsInOneGridCell);
        //create the grid
        grid = new gridSpace[roomWidth, roomHeight];
        //set grids default size
        for(int x = 0; x < roomWidth-1; x++)
        {
            for(int y = 0; y < roomHeight-1; y++)
            {
                grid[x, y] = gridSpace.empty;
            }
        }
        //set first walker
        //init list
        walkers = new List<walker>();
        walker newWalker = new walker();
        newWalker.dir = RandomDirection();
        //  find center of the grid
        Vector2 spawnPos = new Vector2(Mathf.RoundToInt(roomWidth / 2.0f), Mathf.RoundToInt(roomHeight / 2.0f));
        newWalker.pos = spawnPos;
        walkers.Add(newWalker);

    }


    void CreateFloors()
    {
        int iterations = 0;//sentinel value for loop
        do
        {
            foreach (walker myWalker in walkers)
            {
                grid[(int)myWalker.pos.x, (int)myWalker.pos.y] = gridSpace.floor;
            }

            //chance:: to destroy walker
            int numberChecks = walkers.Count; //might modify count while in this loop
            for (int i = 0; i < numberChecks; i++)
            {
                if (Random.value < chanceWalkerDestroy && walkers.Count > 1)
                {
                    walkers.RemoveAt(i);
                    break; // ensures you only destroy one per interation
                }
            }

            //chance:: to change direction
            for (int i = 0; i < walkers.Count; i++)
            {
                if (Random.value < chanceWalkerChangeDir)
                {
                    walker thisWalker = walkers[i];
                    thisWalker.dir = RandomDirection();
                    walkers[i] = thisWalker;
                }
            }

            //chance:: spawn new walker
            numberChecks = walkers.Count;// Might modify count while in this loop
            for (int i = 0; i < numberChecks; i++)
            {
                //only if# of walkers < max, and at a low chance
                if (Random.value < chanceWalkerCreate && walkers.Count < MAX_WALKERS)
                {
                    walker newWalker = new walker();
                    newWalker.dir = RandomDirection();
                    newWalker.pos = walkers[i].pos;
                    walkers.Add(newWalker);
                }
            }

            //move the walkers
            for (int i = 0; i < walkers.Count; i++)
            {
                walker thisWalker = walkers[i];
                thisWalker.pos += thisWalker.dir; // uses the random direction function
                walkers[i] = thisWalker;
            }

            //avoid end of map
            for (int i = 0; i < walkers.Count; i++)
            {
                walker thisWalker = walkers[i];
                //clamp x,y to leave 1 space available for walls
                thisWalker.pos.x = Mathf.Clamp(thisWalker.pos.x, 1, roomWidth - 2);
                thisWalker.pos.y = Mathf.Clamp(thisWalker.pos.y, 1, roomHeight - 2);
                walkers[i] = thisWalker;
            }

            //check to exit the loop
            if ((float)NumberOfFloors() / (float)grid.Length > percentToFill)
            {
                break;
            }
            iterations++;
        } while (iterations < 100000);
    }

    void CreateWalls()
    {//loops through every grid space
        for(int x =0; x < roomWidth-1; x++)
        {
            for(int y = 0; y < roomHeight-1; y++)
            {
                //check if there is floor then if there is space surrounding it
                if(grid[x,y]== gridSpace.floor)
                {
                    if(grid[x, y+1] == gridSpace.empty)
                    {
                        grid[x, y+1] = gridSpace.wall;
                    }
                    if(grid[x, y-1] == gridSpace.empty)
                    {
                        grid[x, y - 1] = gridSpace.wall;
                    }
                    if(grid[x+1, y] == gridSpace.empty)
                    {
                        grid[x + 1, y] = gridSpace.wall;
                    }
                    if(grid[x-1,y] == gridSpace.empty)
                    {
                       grid[x - 1, y] = gridSpace.wall;
                    }

                }
            }
        }
    }


    void CreateSlime()
    {
        for (int x = 0; x < roomWidth - 1; x++)
        {
            for (int y = 0; y < roomHeight - 1; y++)
            {
                if (grid[x, y] == gridSpace.wall)
                {
                    bool allFloors = true; //bool for if all surrounding tiles are floor tiles
                    for (int checkX = -1; checkX <= 1; checkX++)// gets left and right of the Tile being checked
                    {
                        for(int checkY = -1; checkY <= 1; checkY++)//gets above and below the Tile being checked
                        {
                            if( x + checkX < 0 || x + checkX > roomWidth -1 || y + checkY < 0 || y + checkY > roomHeight -1) //skips out of bounds tiles
                            {
                                continue;
                            }
                            if ((checkX != 0 && checkY !=0 ) || (checkX == 0 && checkY == 0))//skips corners and center
                            {
                                continue; 
                            }
                            if(grid[x + checkX, y + checkY] != gridSpace.floor)//checks the space to make sure it is not a floor
                            {
                                allFloors = false;
                            }
                        }
                    }
                    if(allFloors)
                    {
                        grid[x, y] = gridSpace.slime;
                    }
                }
            }
        }
    }


    void SpawnLevel()
    {
        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                switch (grid[x, y])
                {
                    case gridSpace.empty:
                        break;
                    case gridSpace.floor:
                        Spawn(x, y, floorObj);
                        break;
                    case gridSpace.wall:
                        Spawn(x, y, wallObj);
                        break;
                    case gridSpace.slime:
                        Spawn(x, y, slimeObj);
                        break;
                }


            }
        }
    }


    int NumberOfFloors()// used in CreateFloors()
    {
        int count = 0;
        foreach(gridSpace space in grid)// goes through every entry in the grid
        {
            if(space == gridSpace.floor)// if the area on a grid = a floor it increases count
            {
                count++;
            }
        }
        return count;
    }


    void Spawn(float x, float y, GameObject toSpawn)// used in SpawnLevel()
    {
        //find the location to spawn the object at
        Vector2 offset = roomSizeWorldUnits / 2.0f;
        Vector2 spawnPos = new Vector2(x, y) * worldUnitsInOneGridCell - offset;
        //spawn object
        Instantiate(toSpawn, spawnPos, Quaternion.identity);
    }


    Vector2 RandomDirection()
    {
        int choice = Mathf.FloorToInt(Random.value * 3.99f);
        switch(choice)
        {
            case 0:
                return Vector2.down;
            case 1:
                return Vector2.up;
            case 2:
                return Vector2.left;
            default:
                return Vector2.right;
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
