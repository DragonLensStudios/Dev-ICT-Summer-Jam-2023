using System;
using System.Collections;
using System.Collections.Generic;
using DLS.Core.Input;
using DLS.Game.Managers;
using DLS.Game.PLayers;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootScript : MonoBehaviour
{
    public Transform spawnPosition; 
    public GameObject fireBallPrefab;
    public float timeDelay = 0.25f;
    public float fireballForce = 20f;
    private PlayerController player;
    private PlayerInputActions playerInput;
    private Coroutine shoot;
    void Awake()
    {
        playerInput = new PlayerInputActions();
        player = PlayerManager.Instance.Player;
    }

    private void OnEnable()
    {
        playerInput.Enable();
        playerInput.Player.Fire.performed += FireOnperformed;
    }

    private void OnDisable()
    {
        playerInput.Disable();
        playerInput.Player.Fire.performed += FireOnperformed;
    }

    IEnumerator Shoot()
    {
        if (player.lastMovementPosition == Vector2.right) 
        {
            GameObject bullet = Instantiate(fireBallPrefab, spawnPosition.transform.position,Quaternion.identity);   //spawn fireball
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.AddForce(Vector2.right * fireballForce, ForceMode2D.Impulse);    //project fireball
        }
        else if (player.lastMovementPosition == Vector2.up)
        {
            GameObject bullet = Instantiate(fireBallPrefab, spawnPosition.transform.position,Quaternion.identity);   //spawn fireball
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.AddForce(Vector2.up * fireballForce, ForceMode2D.Impulse);    //project fireball
        }
        else if (player.lastMovementPosition == Vector2.left)
        {
            GameObject bullet = Instantiate(fireBallPrefab, spawnPosition.transform.position,Quaternion.identity);   //spawn fireball
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.AddForce(Vector2.left * fireballForce, ForceMode2D.Impulse);    //project fireball
        }
        else if (player.lastMovementPosition == Vector2.down)
        {
            GameObject bullet = Instantiate(fireBallPrefab, spawnPosition.transform.position,Quaternion.identity);   //spawn fireball
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.AddForce(Vector2.down * fireballForce, ForceMode2D.Impulse);    //project fireball
        }
        yield return new WaitForSeconds(timeDelay);
        StopCoroutine(shoot);
        shoot = null;
    }
    
    private void FireOnperformed(InputAction.CallbackContext input)
    {
        shoot ??= StartCoroutine(Shoot());
    }

}
