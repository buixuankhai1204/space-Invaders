
using System;
using UnityEngine;

namespace RayWenderlich.SpaceInvadersUnity
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float speed = 200f;

        [SerializeField] private float liveTime = 5f;

        internal void DestroySelf()
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
            GameManager.Instance.CreateExPlosion(transform.position);
        }

        private void Awake()
        {
            Invoke("DestroySelf", liveTime);
        }

        private void Update()
        {
            Move();
        }

        private void Move()
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            DestroySelf();
        }
    }
}
