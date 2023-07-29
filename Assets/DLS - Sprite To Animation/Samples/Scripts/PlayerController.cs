using UnityEngine;

namespace DLS.Sprite_To_Animation.Samples
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 3f;
    
        private Vector2 movement;
        private Vector2 lastMovement;

        private Animator anim;


        private void Awake()
        {
            anim = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            if (movement != Vector2.zero)
            {
                lastMovement = movement;
                anim.SetBool("isMoving", true);
            }
            else
            {
                anim.SetBool("isMoving", false);
            }

            anim.SetFloat("move_x", lastMovement.x);
            anim.SetFloat("move_y", lastMovement.y);
            transform.position += (Vector3)movement * (Time.deltaTime * moveSpeed);
        }
    }
}