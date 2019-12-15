using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MazeGenerator : MonoBehaviour
{
    public Material matFloor;
    public Material matCeiling;
    public Material matWall;
    public Material matTop;
    public Material highValueGem;
    public Material mediumValueGem;
    public Material lowValueGem;

    public GameObject mazeAgent;
    public GameObject treasurePrefab;
    public GameObject obstaclePrefab;
    public GameObject[] corners = new GameObject[4];

    public GameObject start;
    public GameObject end;
    public GameObject guide;
    public GameObject sceneRoot;
    public GameObject mazeObject;
    public GameObject instantiatedContainer;
    public GameObject uiCanvas;

    public Text displayText;

    public int[] startTuple = new int[2];
    public int[] endTuple = new int[2];

    public List<GameObject> deadEnds;
    public List<GameObject> instantiatedTreasures;
    public List<GameObject> instantiatedObstacles;
    public List<GameObject> instantiatedGuides;

    private List<int[]> pathToTravel;
    private List<int[]> treasureLocations;

    private MazeDataGenerator mazeDataGenerator;
    private MazeMeshGenerator mazeMeshGenerator;
    public bool flag = true;

    private int[,] solvedMaze;

    public int[,] data
    {
        get; private set;
    }

    private void Awake()
    {
        mazeDataGenerator = new MazeDataGenerator();
        mazeMeshGenerator = new MazeMeshGenerator();
    }

    private void Start()
    {
        pathToTravel = new List<int[]>();
        treasureLocations = new List<int[]>();

        deadEnds = new List<GameObject>();
        instantiatedTreasures = new List<GameObject>();
        instantiatedObstacles = new List<GameObject>();
        instantiatedGuides = new List<GameObject>();
    }

    public void CreateMaze(int rows, int cols, bool makeInterior, bool obstacles)
    {
        GenerateNewMaze(rows, cols, makeInterior);
        GenerateCorners(rows, cols);
        DisplayMaze();
        ChooseStartAndEnd(rows, cols);
        SolveMaze();
        PlaceStartEndAndGuides(makeInterior, obstacles);
        if (makeInterior)
        {
            GetTreasureLocations();
            PlaceTreasure();
        }
        PrintMaze();
        PlaceUICanvas();
    }

    public void DestroyMaze()
    {
        Destroy(start);
        Destroy(end);
        Destroy(mazeObject);
        foreach (GameObject go in deadEnds)
        {
            Destroy(go);
        }
        deadEnds.Clear();

        foreach (GameObject go in instantiatedGuides)
        {
            Destroy(go);
        }
        instantiatedGuides.Clear();

        foreach (GameObject go in instantiatedTreasures)
        {
            Destroy(go);
        }
        instantiatedTreasures.Clear();

        foreach (GameObject go in instantiatedObstacles)
        {
            Destroy(go);
        }
        instantiatedObstacles.Clear();

        for (int i = 0; i < instantiatedContainer.transform.childCount; i++)
        {
            Destroy(instantiatedContainer.transform.GetChild(i).gameObject);
        }

        pathToTravel.Clear();
        treasureLocations.Clear();
    }

    public void GenerateNewMaze(int rows, int cols, bool makeInterior)
    {
        if (rows % 2 == 0 || cols % 2 == 0)
        {
            Debug.Log("Works better with odd row & col counts");
            return;
        }

        data = mazeDataGenerator.FromDimensions(rows, cols, makeInterior);
    }

    public void PrintMaze()
    {
        flag = !flag;

        int[,] maze;

        if (flag)
        {
            maze = solvedMaze;
        }
        else
        {
            maze = data;
        }

        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        string msg = "";

        for (int i = rMax; i >= 0; i--)
        {
            for (int j = 0; j <= cMax; j++)
            {
                if (maze[i, j] == 0)
                {
                    msg += ". ";
                }
                else if (maze[i, j] == 2)
                {
                    msg += "# ";
                }
                else if (maze[i, j] == 3)
                {
                    msg += "X ";
                }
                else if (maze[i, j] == 4)
                {
                    msg += "S ";
                }
                else if (maze[i, j] == 5)
                {
                    msg += "E ";
                }
                else
                {
                    msg += "= ";
                }
            }
            if (i != 0)
            {
                msg += "\n";
            }
        }

        displayText.text = msg;
    }

    private void DisplayMaze()
    {
        GameObject go = new GameObject();
        //go.transform.position = Vector3.zero;
        go.name = "Maze";
        go.tag = "mazemesh";
        go.transform.SetParent(sceneRoot.transform, false);
        go.transform.localPosition = sceneRoot.transform.position;

        MeshFilter mf = go.AddComponent<MeshFilter>();
        mf.mesh = mazeMeshGenerator.FromData(data);

        MeshCollider mc = go.AddComponent<MeshCollider>();
        mc.sharedMesh = mf.mesh;

        MeshRenderer mr = go.AddComponent<MeshRenderer>();
        mr.materials = new Material[4] { matFloor, matCeiling, matWall, matTop };

        mazeObject = go;
    }

    private void GenerateCorners(int rows, int cols)
    {
        float cellWidth = mazeMeshGenerator.width;

        corners[0].transform.position = new Vector3(cellWidth, 0.0f, cellWidth);
        corners[1].transform.position = new Vector3(cellWidth * (cols - 2), 0.0f, cellWidth);
        corners[2].transform.position = new Vector3(cellWidth * (cols - 2), 0.0f, cellWidth * (rows - 2));
        corners[3].transform.position = new Vector3(cellWidth, 0.0f, cellWidth * (rows - 2));
    }

    private void PlaceStartEndAndGuides(bool makeInterior, bool obstacles = false)
    {
        if (obstacles)
        {
            instantiatedObstacles = new List<GameObject>();
        }

        float multiplier = mazeMeshGenerator.width;
        Vector3 newPosition = new Vector3(startTuple[1] * multiplier, 0.0f, startTuple[0] * multiplier);

        start = (GameObject)Instantiate(start);
        start.transform.SetParent(sceneRoot.transform, false);
        start.transform.localPosition = newPosition;
        mazeAgent.transform.localPosition = newPosition;
        mazeAgent.transform.localRotation = Quaternion.Euler(0.0f, 360.0f * UnityEngine.Random.value, 0.0f);

        newPosition = new Vector3(endTuple[1] * multiplier, 0.0f, endTuple[0] * multiplier);

        end = (GameObject)Instantiate(end);
        end.transform.SetParent(sceneRoot.transform, false);
        end.transform.localPosition = newPosition;

        if (makeInterior)
        {
            int[] previousTuple = new int[2];
            instantiatedGuides = new List<GameObject>();
            if(obstacles)
            {
                instantiatedObstacles = new List<GameObject>();
            }

            for (int i = 0; i <= solvedMaze.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= solvedMaze.GetUpperBound(1); j++)
                { 
                    if (solvedMaze[i, j] == 3)
                    {
                        if (previousTuple != null && obstacles && UnityEngine.Random.value < 0.1f)
                        {
                            if (solvedMaze[i - 1, j] == 3 && solvedMaze[i + 1, j] == 3 ||
                                solvedMaze[i, j - 1] == 3 && solvedMaze[i, j + 1] == 3)
                            {
                                float rotateModifier;
                                GameObject obstacle = (GameObject)Instantiate(obstaclePrefab);
                                Vector3 positionToPlace = new Vector3(j * multiplier, 0.0f, i * multiplier);
                                obstacle.transform.SetParent(instantiatedContainer.transform, false);
                                obstacle.transform.localPosition = positionToPlace;

                                instantiatedObstacles.Add(obstacle);

                                if (solvedMaze[i - 1, j] == 3)
                                {
                                    rotateModifier = 0.0f;
                                }
                                else
                                {
                                    rotateModifier = 90.0f;
                                }

                                obstacle.transform.localRotation = Quaternion.Euler(0.0f, rotateModifier, 0.0f);

                                if (Mathf.Abs(obstacle.transform.position.x - start.transform.position.x) < 0.1f && Mathf.Abs(obstacle.transform.position.z - start.transform.position.z) < 0.1f ||
                                    Mathf.Abs(obstacle.transform.position.x - end.transform.position.x) < 0.1f && Mathf.Abs(obstacle.transform.position.z - end.transform.position.z) < 0.1f)
                                {
                                    instantiatedObstacles.Remove(obstacle);
                                    Destroy(obstacle);
                                }
                            }
                        }
                        else
                        {
                            GameObject graphic = (GameObject)Instantiate(guide);
                            Vector3 positionToPlace = new Vector3(j * multiplier, 0.0f, i * multiplier);
                            graphic.transform.SetParent(instantiatedContainer.transform, false);
                            graphic.transform.localPosition = positionToPlace;

                            instantiatedGuides.Add(graphic);

                            if (Mathf.Abs(graphic.transform.position.x - start.transform.position.x) < 0.1f && Mathf.Abs(graphic.transform.position.z - start.transform.position.z) < 0.1f ||
                                Mathf.Abs(graphic.transform.position.x - end.transform.position.x) < 0.1f && Mathf.Abs(graphic.transform.position.z - end.transform.position.z) < 0.1f)
                            {
                                instantiatedGuides.Remove(graphic);
                                Destroy(graphic);
                            }
                        }

                    // previousTuple = tuple;
                    }
                }
            }
        }
        else
        {
            int treasureCount = 0;
            List<int[]> treasurePositions = new List<int[]>();
            while (treasureCount < 4)
            {
                int row = UnityEngine.Random.Range(1, data.GetUpperBound(0) - 1);
                int col = UnityEngine.Random.Range(1, data.GetUpperBound(1) - 1);
                int[] pos = new int[2];

                pos[0] = row;
                pos[1] = col;

                if (row == 1 && col == 1 ||
                    row == 1 && col == data.GetUpperBound(1) - 1 ||
                    row == data.GetUpperBound(0) - 1 && col == data.GetUpperBound(1) - 1 ||
                    row == data.GetUpperBound(0) - 1 && col == 1)
                {
                    continue;   
                }
                else if (!treasurePositions.Contains(pos))
                {
                    if (UnityEngine.Random.value > 0.8f)
                    {
                        float chance = UnityEngine.Random.value;
                        GameObject treasure = (GameObject)Instantiate(treasurePrefab);
                        treasure.transform.SetParent(instantiatedContainer.transform, false);
                        Vector3 treasurePosition = new Vector3(row * multiplier, 0.0f, col * multiplier);
                        treasure.transform.localPosition = treasurePosition;

                        if (chance > 0.9f)
                        {
                            treasure.transform.GetChild(0).gameObject.AddComponent<GemHigh>();
                            treasure.transform.GetChild(0).GetComponent<MeshRenderer>().material = highValueGem;
                        }
                        else if (chance > 0.6f)
                        {
                            treasure.transform.GetChild(0).gameObject.AddComponent<GemMedium>();
                            treasure.transform.GetChild(0).GetComponent<MeshRenderer>().material = mediumValueGem;
                        }
                        else
                        {
                            treasure.transform.GetChild(0).gameObject.AddComponent<GemLow>();
                            treasure.transform.GetChild(0).GetComponent<MeshRenderer>().material = lowValueGem;
                        }

                        instantiatedTreasures.Add(treasure);
                        treasurePositions.Add(pos);

                        treasureCount++;

                        if (Mathf.Abs(treasurePosition.x - start.transform.position.x) < 0.1f && Mathf.Abs(treasurePosition.z - start.transform.position.z) < 0.1f ||
                            Mathf.Abs(treasurePosition.x - end.transform.position.x) < 0.1f && Mathf.Abs(treasurePosition.z - end.transform.position.z) < 0.1f)
                        {
                            treasureCount--;
                            treasurePositions.Remove(pos);
                            instantiatedTreasures.Remove(treasure);
                            Destroy(treasure);
                        }
                    }
                }
                treasurePositions.Clear();
            }
        }
    }

    private void ChooseStartAndEnd(int rows, int cols)
    {
        int startRandomIndex = UnityEngine.Random.Range(0, 3);
        int endRandomIndex = UnityEngine.Random.Range(0, 3);

        while (startRandomIndex == endRandomIndex)
        {
            endRandomIndex = UnityEngine.Random.Range(0, 3);
        }

        //start.transform.position = corners[startRandomIndex].transform.position;
        //end.transform.position = corners[endRandomIndex].transform.position;

        switch (startRandomIndex)
        {
            case 0:
                startTuple[0] = 1;
                startTuple[1] = 1;
                break;
            case 1:
                startTuple[0] = cols - 2;
                startTuple[1] = 1;
                break;
            case 2:
                startTuple[0] = rows - 2;
                startTuple[1] = cols - 2;
                break;
            case 3:
                startTuple[0] = 1;
                startTuple[1] = rows - 2;
                break;
        }

        switch (endRandomIndex)
        {
            case 0:
                endTuple[0] = 1;
                endTuple[1] = 1;
                break;
            case 1:
                endTuple[0] = cols - 2;
                endTuple[1] = 1;
                break;
            case 2:
                endTuple[0] = cols - 2;
                endTuple[1] = rows - 2;
                break;
            case 3:
                endTuple[0] = 1;
                endTuple[1] = rows - 2;
                break;
        }
    }

    private void GetTreasureLocations()
    {
        treasureLocations = new List<int[]>();

        for (int i = 0; i <= data.GetUpperBound(0); i++)
        {
            for (int j = 0; j <= data.GetUpperBound(1); j++)
            {
                if (data[i, j] == 0)
                {
                    int wallCount = 0;

                    if (data[i - 1, j] == 1)
                    {
                        wallCount++;
                    }
                    if (data[i + 1, j] == 1)
                    {
                        wallCount++;
                    }
                    if (data[i, j - 1] == 1)
                    {
                        wallCount++;
                    }
                    if (data[i, j + 1] == 1)
                    {
                        wallCount++;
                    }

                    if (wallCount == 3)
                    {
                        int[] treasureLocation = new int[2];
                        treasureLocation[0] = i;
                        treasureLocation[1] = j;

                        treasureLocations.Add(treasureLocation);
                    }
                }
            }
        }
    }

    private void PlaceTreasure()
    {
        instantiatedTreasures = new List<GameObject>();
        float multiplier = mazeMeshGenerator.width;

        foreach (int[] coordinate in treasureLocations)
        {
            GameObject treasure = (GameObject)Instantiate(treasurePrefab);
            treasure.transform.SetParent(instantiatedContainer.transform, false);
            Vector3 treasurePosition = new Vector3(coordinate[1] * multiplier, 0.0f, coordinate[0] * multiplier);
            treasure.transform.localPosition = treasurePosition;

            float chance = UnityEngine.Random.value;

            if (chance > 0.9f)
            {
                treasure.transform.GetChild(0).gameObject.AddComponent<GemHigh>();
                treasure.transform.GetChild(0).GetComponent<MeshRenderer>().material = highValueGem;
            }
            else if (chance > 0.6f)
            {
                treasure.transform.GetChild(0).gameObject.AddComponent<GemMedium>();
                treasure.transform.GetChild(0).GetComponent<MeshRenderer>().material = mediumValueGem;
            }
            else
            {
                treasure.transform.GetChild(0).gameObject.AddComponent<GemLow>();
                treasure.transform.GetChild(0).GetComponent<MeshRenderer>().material = lowValueGem;
            }

            if (Mathf.Abs(treasurePosition.x - start.transform.position.x) < 0.1f && Mathf.Abs(treasurePosition.z - start.transform.position.z) < 0.1f ||
                Mathf.Abs(treasurePosition.x - end.transform.position.x) < 0.1f && Mathf.Abs(treasurePosition.z - end.transform.position.z) < 0.1f)
            {
                Destroy(treasure);
            }
            instantiatedTreasures.Add(treasure);
        }
    }

    private void SolveMaze()
    {
        int[,] localMaze = new int[data.GetUpperBound(0) + 1, data.GetUpperBound(1) + 1];

        for (int i = 0; i <= localMaze.GetUpperBound(0); i++)
        {
            for (int j = 0; j <= localMaze.GetUpperBound(1); j++)
            {
                localMaze[i, j] = data[i, j];
            }
        }

        Stack<int[]> path = new Stack<int[]>();

        bool finished = false;

        path.Push(startTuple);

        while (!finished)
        {
            int[] topItem = path.Peek();
            int currentRow = topItem[0];
            int currentCol = topItem[1];

            int[] newLocation = new int[2];

            if (localMaze[currentRow + 1, currentCol] == 0)
            {
                newLocation[0] = currentRow + 1;
                newLocation[1] = currentCol;
                if (newLocation[0] == endTuple[0] && newLocation[1] == endTuple[1])
                {
                    finished = true;
                }
                else
                {
                    localMaze[newLocation[0], newLocation[1]] = 3;
                    path.Push(newLocation);
                }
            }
            else if (localMaze[currentRow - 1, currentCol] == 0)
            {
                newLocation[0] = currentRow - 1;
                newLocation[1] = currentCol;
                if (newLocation[0] == endTuple[0] && newLocation[1] == endTuple[1])
                {
                    finished = true;
                }
                else
                {
                    localMaze[newLocation[0], newLocation[1]] = 3;
                    path.Push(newLocation);
                }
            }
            else if (localMaze[currentRow, currentCol - 1] == 0)
            {
                newLocation[0] = currentRow;
                newLocation[1] = currentCol - 1;
                if (newLocation[0] == endTuple[0] && newLocation[1] == endTuple[1])
                {
                    finished = true;
                }
                else
                {
                    localMaze[newLocation[0], newLocation[1]] = 3;
                    path.Push(newLocation);
                }
            }
            else if (localMaze[currentRow, currentCol + 1] == 0)
            {
                newLocation[0] = currentRow;
                newLocation[1] = currentCol + 1;
                if (newLocation[0] == endTuple[0] && newLocation[1] == endTuple[1])
                {
                    finished = true;
                }
                else
                {
                    localMaze[newLocation[0], newLocation[1]] = 3;
                    path.Push(newLocation);
                }
            }
            else
            {
                localMaze[currentRow, currentCol] = 2;
                path.Pop();
            }

            if (path.Count < 1)
            {
                finished = true;
            }
        }

        pathToTravel = new List<int[]>();

        for (int i = 0; i <= localMaze.GetUpperBound(0); i++)
        {
            for (int j = 0; j <= localMaze.GetUpperBound(1); j++)
            {
                if (localMaze[i, j] == 3)
                {
                    int[] point = new int[2];
                    point[0] = i;
                    point[1] = j;
                    pathToTravel.Add(point);
                }
            }
        }

        localMaze[startTuple[0], startTuple[1]] = 4;
        localMaze[endTuple[0], endTuple[1]] = 5;

        //PrintMaze();

        solvedMaze = localMaze;
    }

    private void PlaceUICanvas()
    {
        float offset = 0.0f;

        for (int i = 0; i < sceneRoot.transform.childCount; i++)
        {
            if (sceneRoot.transform.GetChild(i).gameObject.CompareTag("mazemesh"))
            {
                offset = sceneRoot.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().bounds.extents.x;
                break;
            }
        }
        Vector3 uiPos = new Vector3(-offset, 10.0f, 0.0f);
        uiCanvas.transform.localPosition = uiPos;
    }
}
