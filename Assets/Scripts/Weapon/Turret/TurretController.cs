using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour, IWeapon
{
    public string name;
    public List<Transform> shotPoints;
    public GameObject hitParticle;
    [SerializeField] private Transform TurretBodyTransform;
    [SerializeField] private GameObject bullet;
    [HideInInspector] public float blastPower = 2;
    [HideInInspector] public AudioSource audioSource;
    private float rotationSpeed = 1.5f;
    private float startRotationTime = 0;
    private bool startToSpeedUp = false;
    private Joystick joystick;
    private WeaponController weaponController;
    private float RPM = .2f;
    private float minX = 4;
    private float maxX = 345;

    private void Start()
    {
        weaponController = transform.parent.GetComponent<WeaponController>();
        weaponController.ableToShot = true;
        startRotationTime = 0;
        startToSpeedUp = false;
        joystick = GameUIController.instance.joystick;
        audioSource = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        Move();
    }
    
    

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
        }
    }

    protected void Move()
    {
        #if UNITY_EDITOR
        float HorizontalRotation = Input.GetAxis("Horizontal");
        float VericalRotation = Input.GetAxis("Vertical");
        
        TurretBodyTransform.rotation = Quaternion.Euler(TurretBodyTransform.rotation.eulerAngles + 
                                                        new Vector3(0, HorizontalRotation * rotationSpeed, 0));
        if (TurretBodyTransform.rotation.eulerAngles.x > maxX || TurretBodyTransform.rotation.eulerAngles.x < minX)
        {
            var newx = TurretBodyTransform.rotation.eulerAngles.x + -VericalRotation * rotationSpeed;
            if (newx > minX && newx < minX + 2)
            {
                TurretBodyTransform.rotation = Quaternion.Euler(minX - .1f, TurretBodyTransform.rotation.eulerAngles.y, TurretBodyTransform.rotation.eulerAngles.z);
            }
            else if (newx < maxX && newx > maxX - 2)
            {
                TurretBodyTransform.rotation = Quaternion.Euler(maxX + .1f, TurretBodyTransform.rotation.eulerAngles.y, TurretBodyTransform.rotation.eulerAngles.z);
            }
            else
            {
                TurretBodyTransform.rotation = Quaternion.Euler(TurretBodyTransform.rotation.eulerAngles +
                                                                new Vector3(-VericalRotation * rotationSpeed, 0, 0));
            }
        }
        if (Input.GetKeyDown(KeyCode.Space) && weaponController.ableToShot)
        {
            Shot();
        }
        #endif
        
        float HorizontalJoystickRotation = joystick.Horizontal;
        float VericalJoystickRotation = joystick.Vertical;
        var turretBodyTransformRotation = TurretBodyTransform.rotation;
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
                    TurretBodyTransform.rotation = Quaternion.Euler(turretBodyTransformRotation.eulerAngles + 
                                                                    new Vector3(0, HorizontalJoystickRotation * rotationSpeed * timeOut, 0));
                }
                else
                {
                    TurretBodyTransform.rotation = Quaternion.Euler(turretBodyTransformRotation.eulerAngles + 
                                                                    new Vector3(0, HorizontalJoystickRotation * rotationSpeed, 0));
                }
            }
            else
            {
                TurretBodyTransform.rotation = Quaternion.Euler(turretBodyTransformRotation.eulerAngles + 
                                                                new Vector3(0, HorizontalJoystickRotation * rotationSpeed, 0));
                startToSpeedUp = false;
            }
        }
        if (TurretBodyTransform.rotation.eulerAngles.x > maxX || TurretBodyTransform.rotation.eulerAngles.x < minX)
        {
            if (VericalJoystickRotation > .3f || VericalJoystickRotation < -.3f)
            {
                var newx = TurretBodyTransform.rotation.eulerAngles.x - VericalJoystickRotation * rotationSpeed;
                if (newx > minX && newx < minX + 2)
                {
                    TurretBodyTransform.rotation = Quaternion.Euler(minX - .1f, turretBodyTransformRotation.eulerAngles.y, turretBodyTransformRotation.eulerAngles.z);
                }
                else if (newx < maxX && newx > maxX - 2)
                {
                    TurretBodyTransform.rotation = Quaternion.Euler(maxX + .1f, turretBodyTransformRotation.eulerAngles.y, turretBodyTransformRotation.eulerAngles.z);
                }
                else
                {
                    TurretBodyTransform.rotation = Quaternion.Euler(turretBodyTransformRotation.eulerAngles +
                                                                    new Vector3(-VericalJoystickRotation * rotationSpeed, 0, 0));
                }
            }
        }
    }

    public void Shot()
    {
        StartCoroutine(ShotProcess());
    }

    private IEnumerator ShotProcess()
    {
        weaponController.ableToShot = false;
        GameManager.instance.audioController.PlaySfx(audioSource, AudioController.SFXType.Cannon);
        yield return new WaitForSeconds(RPM);
        weaponController.ableToShot = true;
        shotABullet(shotPoints[0]);
        if (shotPoints.Count > 1)
        {
            yield return new WaitForSeconds(RPM);
            shotABullet(shotPoints[1]);
        }
        
    }

    private void shotABullet(Transform shotPoint)
    {
        GameObject createdBullet = Instantiate(bullet, shotPoint.position, shotPoint.rotation);
        createdBullet.GetComponent<Rigidbody>().velocity = shotPoint.transform.up * blastPower;
        GameUIController.instance.ammoController.DecreaseAmmo();
        StartCoroutine(weaponController.mainCamera.gameObject.GetComponent<CameraShake>().Shake(.05f, .1f));
    }
}
