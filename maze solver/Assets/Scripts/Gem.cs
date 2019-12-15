using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    void Start()
    {
        StopAllCoroutines();
        StartCoroutine(RotateAndStuff());
        StartCoroutine(MoveUpAndDown());
    }

    private IEnumerator RotateAndStuff()
    {
        Quaternion fromRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        Quaternion toRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);

        for (float t = 0.0f; t < 1.0f; t += Time.fixedDeltaTime * 0.25f)
        {
            transform.localRotation = Quaternion.Lerp(fromRotation, toRotation, t);
            yield return null;
        }

        StartCoroutine(RotateAndStuff());
    }

    private IEnumerator MoveUpAndDown()
    {
        Vector3 fromPosition = transform.position;
        Vector3 toPosition = new Vector3(fromPosition.x, fromPosition.y + 0.5f, fromPosition.z);

        for (float t = 0.0f; t < 1.0f; t += Time.fixedDeltaTime * 0.5f)
        {
            transform.position = Vector3.Lerp(fromPosition, toPosition, t);
            yield return null;
        }

        for (float t = 1.0f; t > 0.0f; t -= Time.fixedDeltaTime * 0.5f)
        {
            transform.position = Vector3.Lerp(fromPosition, toPosition, t);
            yield return null;
        }

        StartCoroutine(MoveUpAndDown());
    }
}
