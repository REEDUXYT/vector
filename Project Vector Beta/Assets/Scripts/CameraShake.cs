using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float magnitude = 1;

    public IEnumerator Shake()
    {
        Vector3 originalPos = transform.position;

        while (Input.GetKey(KeyCode.LeftShift))
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(x, y, originalPos.z),2f);
            yield return null;
        }
        transform.localPosition = originalPos;
    }
}