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
    public Camera mainCamera;
    public float blastPower;
    public GameObject filterFire;
    [HideInInspector] public bool ableToShot;
    private float rotationSpeed = 1.5f;
    private float startRotationTime = 0;
    private bool startToSpeedUp = false;
    private Joystick joystick;
    // private float yAxisTurnSpeed = 2f;
    // private float xAxisTurnSpeed = 25f;

    private void Start()
    {
        ableToShot = true;
        filterFire.SetActive(false);
        startRotationTime = 0;
        startToSpeedUp = false;
        joystick = GameUIController.instance.joystick;
    }

    private void FixedUpdate()
    {
        #if UNITY_EDITOR
        float HorizontalRotation = Input.GetAxis("Horizontal");
        float VericalRotation = Input.GetAxis("Vertical");
        
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + 
                                              new Vector3(0, HorizontalRotation * rotationSpeed, 0));
        if (launcherBodyTransform.rotation.eulerAngles.x <= 335 && launcherBodyTransform.rotation.eulerAngles.x >= 272)
        {
            launcherBodyTransform.rotation = Quaternion.Euler(launcherBodyTransform.rotation.eulerAngles +
                                                              new Vector3(VericalRotation * rotationSpeed, 0, 0));
        }
        if (Input.GetKeyDown(KeyCode.Space) && ableToShot)
        {
            Shot();
        }
        #endif
        
        float HorizontalJoystickRotation = joystick.Horizontal;
        float VericalJoystickRotation = joystick.Vertical;
        var launcherBodyTransformRotation = launcherBodyTransform.rotation;
        var transformRotation = transform.rotation;
        if (HorizontalJoystickRotation > .3f || HorizontalJoystickRotation < -.3f)
        {
            if (HorizontalJoystickRotation >= .8f || HorizontalJoystickRotation <= -.8f)
            {
                if (!startToSpeedUp)
                {
                    startToSpeedUp = true;
                    startRotationTime = Time.time;
                }

                float timeOut = Time.time - startRotationTime;
                
                if (timeOut > 1f)
                {
                    if (timeOut > 2f)
                    {
                        timeOut = 2f;
                    }
                    transform.rotation = Quaternion.Euler(transformRotation.eulerAngles + 
                                                          new Vector3(0, HorizontalJoystickRotation * rotationSpeed * timeOut, 0));
                }
                else
                {
                    transform.rotation = Quaternion.Euler(transformRotation.eulerAngles + 
                                                          new Vector3(0, HorizontalJoystickRotation * rotationSpeed, 0));
                }
            }
            else
            {
                transform.rotation = Quaternion.Euler(transformRotation.eulerAngles + 
                                                      new Vector3(0, HorizontalJoystickRotation * rotationSpeed, 0));
                startToSpeedUp = false;
            }
        }
        if (launcherBodyTransformRotation.x <= 335 && launcherBodyTransformRotation.eulerAngles.x >= 272)
        {
            if (VericalJoystickRotation > .3f || VericalJoystickRotation < -.3f)
            {
                launcherBodyTransform.rotation = Quaternion.Euler(launcherBodyTransform.rotation.eulerAngles + 
                                                                  new Vector3(VericalJoystickRotation * rotationSpeed * .5f, 0, 0));
            }
        }
        if (launcherBodyTransformRotation.eulerAngles.x > 334)
        {
            launcherBodyTransform.rotation = Quaternion.Euler(new Vector3(334,
                launcherBodyTransform.rotation.eulerAngles.y, launcherBodyTransform.rotation.eulerAngles.z));
        }
        else if (launcherBodyTransformRotation.eulerAngles.x < 273)
        {
            launcherBodyTransform.rotation = Quaternion.Euler(new Vector3(273,
                launcherBodyTransform.rotation.eulerAngles.y, launcherBodyTransform.rotation.eulerAngles.z));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
        }
    }

    public void Shot()
    {
        StartCoroutine(ShotProcess());
    }

    private IEnumerator ShotProcess()
    {
        ableToShot = false;
        filterFire.SetActive(true);
        yield return new WaitForSeconds(1f);
        filterFire.SetActive(false);
        ableToShot = true;
        GameObject CreatedCannonball = Instantiate(Cannonball, ShotPoint.position, ShotPoint.rotation);
        CreatedCannonball.GetComponent<Rigidbody>().velocity = ShotPoint.transform.up * blastPower;
        GameUIController.instance.ammoController.DecreaseAmmo();
        Destroy(Instantiate(Explosion, ShotPoint.position, ShotPoint.rotation), 2);
        StartCoroutine(mainCamera.gameObject.GetComponent<CameraShake>().Shake(.1f, .2f));
    }

    
}
