using Base;

namespace Enemy.States
{
    public class ZombieMoveState: EnemyStateBase
    {
        public override void Enter()
        {
            base.Enter();
            enemyModel.PlayStateAnimation("Move");
            enemyModel.navMeshAgent.enabled = true;
        }

        public override void Update()
        {
            base.Update();
            if (!enemyModel.IsInAttackRange())
            {
                enemyModel.ChaseTarget();
            }
            else
            {
                enemyModel.SwitchState(EnemyState.Idle);
            }
        }
    }
}