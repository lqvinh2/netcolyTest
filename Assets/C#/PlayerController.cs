﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public bool isLocalPlayer = true;

    Vector3 oldPosition;
    Vector3 currentPosition;
    Quaternion oldRotation;
    Quaternion currentRotation;

    // Use this for initialization
    void Start()
    {
        oldPosition = transform.position;
        currentPosition = oldPosition;
        oldRotation = transform.rotation;
        currentRotation = oldRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer || netColy.instance == null)
        {
            return;
        }



        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, z);

        currentPosition = transform.position;
        currentRotation = transform.rotation;

        if (currentPosition != oldPosition)
        {
            netColy.instance.CommandMove(gameObject.transform.position);
            oldPosition = currentPosition;
        }

        if (currentRotation != oldRotation)
        {
            //NetworkManager.instance.GetComponent<NetworkManager>().CommandTurn(transform.rotation);
            oldRotation = currentRotation;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //NetworkManager n = NetworkManager.instance.GetComponent<NetworkManager>();
            //n.CommandShoot();
            CmdFire();
        }
    }

    public void CmdFire()
    {
        var bullet = Instantiate(bulletPrefab,
                                 bulletSpawn.position,
                                 bulletSpawn.rotation) as GameObject;
        Bullet b = bullet.GetComponent<Bullet>();
        b.playerFrom = this.gameObject;
        print("setting the velocity");
        print(bullet.transform.up);
        bullet.GetComponent<Rigidbody>().isKinematic = false;
        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.up * 6, ForceMode.VelocityChange);
        Destroy(bullet, 2.0f);
    }


}