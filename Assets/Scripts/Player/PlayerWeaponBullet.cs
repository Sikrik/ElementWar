using System;
using Base;
using UnityEngine;

namespace Player
{
    public class PlayerWeaponBullet: MonoBehaviour
    {
        [Tooltip("子弹伤害")]
        public int damage = 10;
        [HideInInspector]
        public Rigidbody rb;
        [Tooltip("推力")]
        public float flyPower = 30f;
        
        [Tooltip("存活时间")]
        public float lifeTime = 10f;
private Vector3 prevPosition;
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            prevPosition = transform.position;
            rb.velocity = transform.forward * flyPower;
            Destroy(gameObject, lifeTime);
        }


        private void Update()
        {
            CheckCillision();
            prevPosition = transform.position;
        }

        private void CheckCillision()
        {
            RaycastHit hit;
            Vector3 dir = transform.position - prevPosition;
            float distance = Vector3.Distance(transform.position, prevPosition);

            if (Physics.Raycast(prevPosition,dir.normalized,out hit,distance))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    EnemyBase enemyBase = hit.collider.GetComponent<EnemyBase>();
                    enemyBase.Hurt(this, 1);
                }
            }
        }
    }
}