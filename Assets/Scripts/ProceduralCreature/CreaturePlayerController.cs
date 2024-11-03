using System;
using UnityEngine;

namespace ProceduralCreature
{
    public class CreaturePlayerController : MonoBehaviour
    {
        public float moveSpeed = 3f;
        public float rotSpeed = 300f;
        public float jumpHeight = 1.25f;
        public float jumpDuration = 0.5f; 
        
        private bool isJumping = false;
        private float jumpStartTime;
        private float initialY;
        
        private void Awake()
        {
            Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z - 4);
            Camera.main.transform.LookAt(transform, Vector3.up);
            Camera.main.transform.SetParent(transform);
        }

        void Update()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;

            if (moveDirection.magnitude >= 0.1f)
            {
                moveDirection.Normalize();
                Vector3 newPosition = transform.position + moveDirection * moveSpeed * Time.deltaTime;
                transform.position = newPosition;

                Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotSpeed * Time.deltaTime);
            }

            if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
            {
                isJumping = true;
                jumpStartTime = Time.time;
                initialY = transform.position.y;
            }

            if (isJumping)
            {
                float t = (Time.time - jumpStartTime) / jumpDuration;
                if (t < 1f)
                {
                    float jumpY = initialY + Mathf.Sin(t * Mathf.PI) * jumpHeight;
                    transform.position = new Vector3(transform.position.x, jumpY, transform.position.z);
                }
                else
                {
                    isJumping = false;
                }
            }
        }
    }
}