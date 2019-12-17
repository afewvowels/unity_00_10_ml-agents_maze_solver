using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    void Start()
    {
        transform.rotation = Quaternion.Euler(0.0f, 360.0f * Random.value, 0.0f);
        StopAllCoroutines();
        StartCoroutine(RotateAndStuff());
        StartCoroutine(MoveUpAndDown());
    }

    private IEnumerator RotateAndStuff()
    {
        for (int i = 0; i < 72; i++)
        {
            transform.Rotate(new Vector3(0.0f, 5.0f, 0.0f));
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
