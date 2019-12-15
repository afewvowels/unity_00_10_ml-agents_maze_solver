using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFPV : MonoBehaviour
{
    public Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            MoveForwards();
        }
        if (Input.GetKey(KeyCode.S))
        {
            MoveBackwards();
        }
        if (Input.GetKey(KeyCode.A))
        {
            RotateLeft();
        }
        if (Input.GetKey(KeyCode.D))
        {
            RotateRight();
        }
    }

    public void MoveForwards()
    {
        rb.AddRelativeForce(new Vector3(0.0f, 0.0f, 50.0f), ForceMode.Force);
    }

    public void MoveBackwards()
    {
        rb.AddRelativeForce(new Vector3(0.0f, 0.0f, -50.0f), ForceMode.Force);
    }

    public void RotateRight()
    {
        transform.Rotate(0.0f, 5.0f, 0.0f);
    }

    public void RotateLeft()
    {
        transform.Rotate(0.0f, -5.0f, 0.0f);
    }
}
