using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public List<GameObject> scenes;
    private int activeIndex;
    public MazeAgent activeAgent;
    public Camera mainCamera;
    public bool isOrthographic;
    // Start is called before the first frame update
    void Start()
    {
        isOrthographic = true;
        activeIndex = 0;
        foreach (GameObject scene in GameObject.FindGameObjectsWithTag("mazescene"))
        {
            scenes.Add(scene);
        }
        GetActiveAgent();
        GetMazeOrigin();
        SetCameraBounds();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            activeIndex++;
            if (activeIndex > scenes.Count - 1)
            {
                activeIndex = 0;
            }
            GetMazeOrigin();
            GetActiveAgent();
            if (isOrthographic)
            {
                SetCameraBounds();
            }
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            GetMazeOrigin();
            if (isOrthographic)
            {
                SetCameraBounds();
            }
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            isOrthographic = !isOrthographic;
            MazeGenerator.ChangeCameraProjection(isOrthographic);
            GetMazeOrigin();
            if (isOrthographic)
            {
                SetToOrthographic();
            }
            else
            {
                SetToPerspective();
            }
        }

        if (!isOrthographic)
        {
            if (Input.GetKey(KeyCode.W))
            {
                MoveUp();
            }
            if (Input.GetKey(KeyCode.S))
            {
                MoveDown();
            }
            if (Input.GetKey(KeyCode.A))
            {
                MoveLeft();
            }
            if (Input.GetKey(KeyCode.D))
            {
                MoveRight();
            }
            if (Input.GetKey(KeyCode.Q))
            {
                RotateLeft();
            }
            if (Input.GetKey(KeyCode.E))
            {
                RotateRight();
            }
        }
    }

    private void MoveUp()
    {
        transform.position += transform.forward;
    }

    private void MoveDown()
    {
        transform.position -= transform.forward;
    }

    private void MoveLeft()
    {
        transform.position -= transform.right;
    }

    private void MoveRight()
    {
        transform.position += transform.right;
    }

    private void RotateLeft()
    {
        transform.Rotate(0.0f, -5.0f, 0.0f);
    }

    private void RotateRight()
    {
        transform.Rotate(0.0f, 5.0f, 0.0f);
    }

    private void GetMazeOrigin()
    {
        for (int i = 0; i < scenes[activeIndex].transform.childCount; i++)
        {
            if (scenes[activeIndex].transform.GetChild(i).CompareTag("mazemesh"))
            {
                transform.position = scenes[activeIndex].transform.GetChild(i).GetComponent<MeshRenderer>().bounds.center;
                transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            }
        }
    }

    private void GetActiveAgent()
    {
        for (int i = 0; i < scenes[activeIndex].transform.childCount; i++)
        {
            if (scenes[activeIndex].transform.GetChild(i).CompareTag("mazeagent"))
            {
                activeAgent = scenes[activeIndex].transform.GetChild(i).GetComponent<MazeAgent>();
                return;
            }
        }
    }

    private void SetCameraBounds()
    {
        if (activeAgent.currentMazeWidth < 10)
        {
            mainCamera.orthographicSize = activeAgent.currentMazeWidth + 10;
        }
        else
        {
            mainCamera.orthographicSize = activeAgent.currentMazeWidth * 2;
        }
    }

    private void SetToOrthographic()
    {
        foreach (GameObject mazeController in GameObject.FindGameObjectsWithTag("mazecontroller"))
        {
            mazeController.GetComponent<MazeGenerator>().PlaceUICanvas();
        }
        Camera.main.orthographic = true;
        mainCamera.transform.localPosition = new Vector3(0.0f, 15.0f, 0.0f);
        mainCamera.transform.localRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
        SetCameraBounds();
    }

    private void SetToPerspective()
    {
        foreach (GameObject mazeController in GameObject.FindGameObjectsWithTag("mazecontroller"))
        {
            mazeController.GetComponent<MazeGenerator>().PlaceUICanvas();
        }
        Camera.main.orthographic = false;
        mainCamera.transform.localPosition = new Vector3(0.0f, 30.0f, -35.0f);
        mainCamera.transform.localRotation = Quaternion.Euler(40.0f, 0.0f, 0.0f);
    }
}
