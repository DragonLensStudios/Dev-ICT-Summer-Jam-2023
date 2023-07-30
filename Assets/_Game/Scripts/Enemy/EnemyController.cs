using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using DLS.Core.Data_Persistence;
using DLS.Game.Enums;
using DLS.Game.GameStates;
using DLS.Game.Interfaces;
using DLS.Game.Managers;
using DLS.Game.Messages;
using DLS.Game.PLayers;
using DLS.Game.Utilities;
using DLS.Utilities;
using UnityEngine;

public class EnemyController : MonoBehaviour, IHealth, IID
{
    [field: SerializeField] public GameObject Target { get; set; }
    [field: SerializeField] public SerializableGuid PrefabID { get; set; }
    [field: SerializeField] public SerializableGuid LevelID { get; set; }
    [field: SerializeField] public SerializableGuid ID { get; set; }
    [field: SerializeField] public string ObjectName { get; set; }
    
    [field: SerializeField] public int CurrentHealth { get; set; }
    [field: SerializeField] public int MaxHealth { get; set; }
    [field: SerializeField] public bool IsAlive { get; set; }
    [field: SerializeField] public int ResourcesDropped { get; set; } = 1;
    
    [field: SerializeField] public int Damage { get; set; }
    
    public float MoveSpeed = 1f;

    public Vector2 movement;

    private PlayerController player;
    protected Animator animator;

    protected virtual void Awake()
    {
        //Initialize aiPath
        player = PlayerManager.Instance.Player;
        
        //Initialize animator
        animator = GetComponent<Animator>();

        Target = FindObjectOfType<SkullController>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Target == null)
        {
            Target = FindObjectOfType<SkullController>().gameObject;
        }
        if (GameManager.Instance.IsCurrentState<PausedState>()) return;
        // Determine the new position to move towards.
        movement = Vector2.MoveTowards(transform.position, Target.transform.position, MoveSpeed * Time.deltaTime);
        // Move the enemy towards the player.
        transform.position = movement;
        UpdateAnimator();

    }
    private void OnEnable()
    {
        MessageSystem.MessageManager.RegisterForChannel<PauseMessage>(MessageChannels.GameFlow, PauseMessageHandler);
        MessageSystem.MessageManager.RegisterForChannel<DamageTargetMessage>(MessageChannels.Gameplay, DamageTargetMessageHandler);
    }

    private void DamageTargetMessageHandler(MessageSystem.IMessageEnvelope message)
    {
        if(!message.Message<DamageTargetMessage>().HasValue) return;
        var data = message.Message<DamageTargetMessage>().Value;

        if (!data.ID.Equals(ID)) return;
        CurrentHealth -= data.Damage;
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void PauseMessageHandler(MessageSystem.IMessageEnvelope message)
    {
        if(!message.Message<PauseMessage>().HasValue) return;
        var data = message.Message<PauseMessage>().Value;
    }

    private void OnDisable()
    {
        MessageSystem.MessageManager.UnregisterForChannel<PauseMessage>(MessageChannels.GameFlow, PauseMessageHandler);
        MessageSystem.MessageManager.UnregisterForChannel<DamageTargetMessage>(MessageChannels.Gameplay, DamageTargetMessageHandler);
    }

    //Gets the direction the character is facing(Up, Down, Left, Right)
    public Vector2 GetDirectionFacing()
    {
        Vector2 direction = Vector2.zero;
        direction = player.transform.position - gameObject.transform.position;
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            direction = new Vector2(direction.x, 0);
            direction.Normalize();
        }
        else
        {
            direction = new Vector2(0, direction.y);
            direction.Normalize();
        }
        return direction;
    }

    //update the animator
    private void UpdateAnimator()
    {
        Vector2 Direction = GetDirectionFacing();
        animator.SetFloat("MoveX", Direction.x);
        animator.SetFloat("MoveY", Direction.y);
        animator.SetFloat("LastMoveX", Direction.x);
        animator.SetFloat("LastMoveY", Direction.y);
        animator.SetBool("isMoving", movement.normalized != Vector2.zero);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Target = collision.gameObject;
            var health = Target.GetComponent<IHealth>();
            if (health != null)
            {
                health.CurrentHealth -= Damage;
                if (health.CurrentHealth > 0) return;
                health.Die();;
            }
        }
    }

    public void SetHealth(int amount)
    {
        CurrentHealth = amount >= MaxHealth ? MaxHealth : amount;
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void AddHealth(int amount)
    {
        CurrentHealth = CurrentHealth + amount >= MaxHealth ? MaxHealth : CurrentHealth + amount;
    }

    public void RemoveHealth(int amount)
    {
        CurrentHealth = CurrentHealth - amount <= 0 ? 0 : CurrentHealth - amount;
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (player != null)
        {
            player.Resources += ResourcesDropped;
        }
        Destroy(gameObject);
        
    }
}
