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
    [HideInInspector] public AudioSource audioSource;
    private float rotationSpeed = 1f;
    private float range = 100;
    private float startRotationTime = 0;
    private bool startToSpeedUp = false;
    private Joystick joystick;
    private WeaponController weaponController;
    private float RPM = .1f;
    private float minX = 25;
    private float maxX = 345;
    private GameManager gameManager;
    private GameUIController gameUIController;
    private Camera camera;
    private Transform _transform;
    

    private void Start()
    {
        _transform = transform;
        gameManager = GameManager.instance;
        gameUIController = GameUIController.instance;
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
        if (TurretBodyTransform.rotation.eulerAngles.x > maxX || TurretBodyTransform.rotation.eulerAngles.x < minX)
        {
            float timeOut = Time.time - startRotationTime;
            float newRotationSpeed = rotationSpeed;
            if (HorizontalJoystickRotation < .3f || HorizontalJoystickRotation > -.3f ||
                VericalJoystickRotation < .3f || VericalJoystickRotation > -.3f)
            {
                if (timeOut < 1f)
                {
                    newRotationSpeed *= timeOut;
                }
                else
                {
                    newRotationSpeed = rotationSpeed;
                }
            }
            if (HorizontalJoystickRotation >= .8f || HorizontalJoystickRotation <= -.8f)
            {
                if (!startToSpeedUp)
                { 
                    startToSpeedUp = true;
                    startRotationTime = Time.time;
                }
                if (timeOut > 1f)
                { 
                    if (timeOut > 2f)
                    {
                        timeOut = 2f;
                    }

                    newRotationSpeed *= timeOut;
                }
                else
                {
                    newRotationSpeed = rotationSpeed;
                }
            }
            else
            {
                newRotationSpeed = rotationSpeed;
                startToSpeedUp = false;
            }
            var newx = TurretBodyTransform.rotation.eulerAngles.x - VericalJoystickRotation * rotationSpeed;
            if (newx > minX && newx < minX + 2)
            {
                TurretBodyTransform.rotation = Quaternion.Euler(minX - .1f, turretBodyTransformRotation.eulerAngles.y + HorizontalJoystickRotation * newRotationSpeed, turretBodyTransformRotation.eulerAngles.z);
            }
            else if (newx < maxX && newx > maxX - 2)
            {
                TurretBodyTransform.rotation = Quaternion.Euler(maxX + .1f, turretBodyTransformRotation.eulerAngles.y + HorizontalJoystickRotation * newRotationSpeed, turretBodyTransformRotation.eulerAngles.z);
            }
            else
            {
                TurretBodyTransform.rotation = Quaternion.Euler(turretBodyTransformRotation.eulerAngles +
                                                                new Vector3(-VericalJoystickRotation * rotationSpeed, HorizontalJoystickRotation * newRotationSpeed, 0));
            }
        }
    }

    public void Shot()
    {
        if (gameUIController.ammoController.isInReload)
            return;
        StartCoroutine(ShotProcess());
    }

    private IEnumerator ShotProcess()
    {
        weaponController.ableToShot = false;
        yield return new WaitForSeconds(RPM);
        gameManager.audioController.PlaySfx(audioSource, AudioController.SFXType.TurretShot);
        shotABullet(shotPoints[0]);
        if (shotPoints.Count > 1)
        {
            yield return new WaitForSeconds(RPM);
            gameManager.audioController.PlaySfx(audioSource, AudioController.SFXType.TurretShot);
            shotABullet(shotPoints[1]);
            weaponController.ableToShot = true;
        }
    }

    private void shotABullet(Transform shotPoint)
    {
        RaycastHit hit;
        Destroy(Instantiate(bullet, shotPoint.position, Quaternion.LookRotation(shotPoint.up)), .04f);
        if (Physics.Raycast(shotPoint.position, shotPoint.transform.up, out hit, range))
        {
            if (hit.transform.gameObject.CompareTag("Enemy"))
            {
                var enemyAI = hit.transform.GetComponent<EnemyAI>();
                if(enemyAI != null)
                    enemyAI.Damage(20);
            }
            Destroy(Instantiate(hitParticle, hit.point, Quaternion.LookRotation(hit.normal)), .5f);
        }
        gameUIController.ammoController.DecreaseAmmo();
        // StartCoroutine(weaponController.mainCamera.gameObject.GetComponent<CameraShake>().Shake(.1f, .1f));
    }
}
