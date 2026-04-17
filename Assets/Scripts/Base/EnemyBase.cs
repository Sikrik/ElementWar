using System;
using Managers;
using Player;
using UnityEngine;
using UnityEngine.AI;
using Utils;

public enum EnemyState
{
    Idle,
    Attack,
    Dead,
    Move
}
namespace Base
{
    public abstract class EnemyBase: MonoBehaviour,IStateMachineOwner
    {
        [HideInInspector]
        public Animator animator;
        protected StateMachine StateMachine;
        public GameObject bloodSmashPrefab;
        public GameObject bloodDrippingPrefab;
        #region 寻路相关组件
        [HideInInspector]
        public NavMeshAgent navMeshAgent;

        [Tooltip("转向速度")]
        public float rotationSpeed = 650;

        [Tooltip("最小攻击距离")] public float minAttackDistance = 1;
        [Tooltip("追击目标")] public PlayerModel attackTarget;
        #endregion
        protected virtual void Awake()
        {
            StateMachine = new StateMachine(this);
            animator = GetComponent<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.stoppingDistance = minAttackDistance;
            navMeshAgent.angularSpeed = rotationSpeed;
        }

        private void Start()
        {
            SwitchState(EnemyState.Idle);
            FindAttackTarget();
        }

        public virtual void FindAttackTarget()
        {
            PlayerModel[] playerModels = GameManager.INSTANCE.playerModels;
            if (playerModels != null && playerModels.Length > 0)
            {
                PlayerModel closestPlayer = null;
                float minDistance = float.MaxValue;
                foreach (PlayerModel playerModel in playerModels)
                {
                    if (playerModel!= null)
                    {
                        float distance = Vector3.Distance(transform.position, playerModel.transform.position);
                        if (distance< minDistance)
                        {
                            minDistance = distance;
                            closestPlayer = playerModel;
                        }
                    }
                }
                attackTarget = closestPlayer;
            }
        }

        public virtual bool HasAttackTarget()
        {
            return attackTarget!= null;
        }

        public virtual bool IsInAttackRange()
        {
            if (HasAttackTarget())
            {
                return Vector3.Distance(transform.position,attackTarget.transform.position)< minAttackDistance;
            }

            return false;
        }

        public virtual void ChaseTarget()
        {
            if(HasAttackTarget())
                navMeshAgent.SetDestination(attackTarget.transform.position);
        }

        public abstract void SwitchState(EnemyState state);

        public virtual void PlayStateAnimation(string animationName,float transition=0.25f,int layer=0)
        {
            animator.CrossFadeInFixedTime(animationName,transition, layer);           
        }

        public virtual void Hurt(PlayerWeaponBullet bullet, float damageMultiplier=1)
        {
            Vector3 bulletDir = bullet.transform.forward;
            Quaternion rotation = Quaternion.LookRotation(-bulletDir);
            Destroy(Instantiate(bloodSmashPrefab,bullet.transform.position,rotation),3f);

            Destroy(Instantiate(bloodDrippingPrefab, transform.position+Vector3.up*0.1f, Quaternion.Euler(0,0,0)));
        }
    }
}