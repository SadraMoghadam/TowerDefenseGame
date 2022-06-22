using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherMovement : MonoBehaviour
{
    public Transform launcherBodyTransform;
    public GameObject rotateAroundObjectY;
    public GameObject rotateAroundObjectX;
    private float yAxisTurnSpeed = 30f;
    private float zAxisTurnSpeed = 20f;
    
    private void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.RotateAround(rotateAroundObjectY.transform.position, Vector3.up, yAxisTurnSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.RotateAround(rotateAroundObjectY.transform.position, Vector3.up, -1 * yAxisTurnSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.UpArrow) && launcherBodyTransform.localEulerAngles.x > 275)
        {
            // launcherBodyTransform.localRotation = Quaternion.Lerp(launcherBodyTransform.localRotation, new Quaternion(launcherBodyTransform.localRotation.x + zAxisTurnSpeed * Time.deltaTime, launcherBodyTransform.localRotation.y, launcherBodyTransform.localRotation.z, launcherBodyTransform.rotation.w), Time.deltaTime);
            // launcherBodyTransform.Rotate(zAxisTurnSpeed * Time.deltaTime, 0, 0, Space.World);
            launcherBodyTransform.localEulerAngles = new Vector3(launcherBodyTransform.localEulerAngles.x - zAxisTurnSpeed * Time.deltaTime, launcherBodyTransform.localEulerAngles.y, launcherBodyTransform.localEulerAngles.z);
        }
        if (Input.GetKey(KeyCode.DownArrow) && launcherBodyTransform.localEulerAngles.x < 335)
        {
            launcherBodyTransform.localEulerAngles = new Vector3(launcherBodyTransform.localEulerAngles.x + zAxisTurnSpeed * Time.deltaTime, launcherBodyTransform.localEulerAngles.y, launcherBodyTransform.localEulerAngles.z);
        }
    }
}
