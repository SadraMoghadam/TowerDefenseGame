using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherController : MonoBehaviour
{
    public Transform launcherBodyTransform;
    public GameObject rotateAroundObjectY;
    public GameObject Cannonball;
    public Transform ShotPoint;
    public GameObject Explosion;
    public float BlastPower = 20;
    
    private float rotationSpeed = 1;
    // private float yAxisTurnSpeed = 2f;
    // private float xAxisTurnSpeed = 25f;
    
    private void Update()
    {
        float HorizontalRotation = Input.GetAxis("Horizontal");
        float VericalRotation = Input.GetAxis("Vertical");

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + 
                                              new Vector3(0, HorizontalRotation * rotationSpeed, 0));
        if (launcherBodyTransform.rotation.eulerAngles.x <= 335 && launcherBodyTransform.rotation.eulerAngles.x >= 272)
        {
            launcherBodyTransform.rotation = Quaternion.Euler(launcherBodyTransform.rotation.eulerAngles + 
                                                              new Vector3(VericalRotation * rotationSpeed, 0, 0));   
        }
        else if (launcherBodyTransform.rotation.eulerAngles.x > 334)
        {
            launcherBodyTransform.rotation = Quaternion.Euler(new Vector3(334,
                launcherBodyTransform.rotation.eulerAngles.y, launcherBodyTransform.rotation.eulerAngles.z));
        }
        else if (launcherBodyTransform.rotation.eulerAngles.x < 273)
        {
            launcherBodyTransform.rotation = Quaternion.Euler(new Vector3(273,
                launcherBodyTransform.rotation.eulerAngles.y, launcherBodyTransform.rotation.eulerAngles.z));
        }
        // if (Input.GetKey(KeyCode.RightArrow))
        // {
        //     transform.RotateAround(rotateAroundObjectY.transform.position, Vector3.up, yAxisTurnSpeed * Time.deltaTime);
        // }
        // else if (Input.GetKey(KeyCode.LeftArrow))
        // {
        //     transform.RotateAround(rotateAroundObjectY.transform.position, Vector3.up, -1 * yAxisTurnSpeed * Time.deltaTime);
        // }
        // if (Input.GetKey(KeyCode.UpArrow) && launcherBodyTransform.localEulerAngles.x > 275)
        // {
        //     launcherBodyTransform.localEulerAngles = new Vector3(launcherBodyTransform.localEulerAngles.x - xAxisTurnSpeed * Time.deltaTime, launcherBodyTransform.localEulerAngles.y, launcherBodyTransform.localEulerAngles.z);
        // }
        // else if (Input.GetKey(KeyCode.DownArrow) && launcherBodyTransform.localEulerAngles.x < 335)
        // {
        //     launcherBodyTransform.localEulerAngles = new Vector3(launcherBodyTransform.localEulerAngles.x + xAxisTurnSpeed * Time.deltaTime, launcherBodyTransform.localEulerAngles.y, launcherBodyTransform.localEulerAngles.z);
        // }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shot();
        }
    }

    private void Shot()
    {
        GameObject CreatedCannonball = Instantiate(Cannonball, ShotPoint.position, ShotPoint.rotation);
        CreatedCannonball.GetComponent<Rigidbody>().velocity = ShotPoint.transform.up * BlastPower;
        Destroy(Instantiate(Explosion, ShotPoint.position, ShotPoint.rotation), 2);
    }

    
}