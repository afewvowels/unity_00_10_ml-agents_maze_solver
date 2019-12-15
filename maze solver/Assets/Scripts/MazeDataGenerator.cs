using UnityEngine;

public class MazeDataGenerator
{
    public float placementThreshold;

    public MazeDataGenerator()
    {
        placementThreshold = 0.1f;
    }

    public int[,] FromDimensions(int rows, int cols, bool fillMaze = true)
    {
        int[,] maze = new int[rows, cols];

        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        for (int i = 0; i <= rMax; i++)
        {
            for (int j = 0; j <= cMax; j++)
            {
                if (i == 0 || j == 0 || i == rMax || j == cMax)
                {
                    maze[i, j] = 1;
                }
                else if (fillMaze)
                {
                    if (i % 2 == 0 && j % 2 == 0)
                    {
                        if (Random.value > placementThreshold)
                        {
                            maze[i, j] = 1;

                            int a = Random.value < 0.5f ? 0 : (Random.value < 0.5f ? -1 : 1);
                            int b = a != 0 ? 0 : (Random.value < 0.5f ? -1 : 1);
                            maze[i + a, j + b] = 1;
                        }
                    }
                }
                else
                {
                    maze[i, j] = 0;
                }
            }
        }

        return maze;
    }
}
