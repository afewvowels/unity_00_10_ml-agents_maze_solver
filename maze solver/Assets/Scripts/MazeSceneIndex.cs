using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeSceneIndex : MonoBehaviour
{
    public int xIndex;
    public int zIndex;
    public static float offset;

    private void Start()
    {
        offset = 100.0f;
    }
}
