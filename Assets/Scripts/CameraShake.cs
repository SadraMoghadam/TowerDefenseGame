using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraShake : MonoBehaviour
{
    private Transform _transform;
    private void Start()
    {
        _transform = transform;
    }

    // turret / Launcher / Cannonball
    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = _transform.localPosition;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float x = Random.Range(originalPos.x - 1f * magnitude, originalPos.x + 1f * magnitude);
            float y = Random.Range(originalPos.y - 1f * magnitude, originalPos.y + 1f * magnitude);
            _transform.localPosition = new Vector3(x, y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}
