using Base;
using UnityEngine;

namespace Enemy.States
{
    public class ZombieIdleState:EnemyStateBase
    {
        public override void Enter()
        {
            base.Enter();
            enemyModel.PlayStateAnimation("Idle");
            enemyModel.navMeshAgent.velocity = Vector3.zero;
        }

        public override void Update()
        {
            base.Update();
            if (!enemyModel.IsInAttackRange())
            {
                enemyModel.SwitchState(EnemyState.Move);
            }
        }
    }
}