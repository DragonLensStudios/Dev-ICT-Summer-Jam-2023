using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootScript : MonoBehaviour
{
    public Transform fireSource;
    public GameObject fireBallPrefab;

    public float fireballForce = 20f;
    private PlayerController player;
    void Awake()
    {
        player = PlayerManager.Instance.Player;
    }
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(fireBallPrefab, fireSource.position,fireSource.rotation);   //spawn fireball
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        if (lastMovementPosition == ) 
        {
            rb.AddForce(fireSource.right * fireballForce, ForceMode2D.Impulse);    //project fireball
        }
    }

}
