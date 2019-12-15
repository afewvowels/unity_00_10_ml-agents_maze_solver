using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeController : MonoBehaviour
{
    private MazeGenerator generator;

    void Start()
    {
        generator = GetComponent<MazeGenerator>();
    }
}
