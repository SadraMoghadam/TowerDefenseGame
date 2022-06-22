using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBallController : MonoBehaviour
{
    public GameObject Explosion;
    
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Launcher")
            return;
        Destroy(Instantiate(Explosion, transform.position, transform.rotation), 2);
        Destroy(this.gameObject);
    }
}
