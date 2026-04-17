using Base;
using Enemy.States;

namespace Enemy
{
    /// <summary>
    /// 僵尸类敌人
    /// </summary>
    public class ZombieEnemy: EnemyBase
    {
        public override void SwitchState(EnemyState state)
        {
            switch (state)
            {
                case EnemyState.Idle:
                    StateMachine.EnterState<ZombieIdleState>();
                    break;
                case EnemyState.Move:
                    StateMachine.EnterState<ZombieMoveState>();
                    break;
                case EnemyState.Attack:
                    StateMachine.EnterState<ZombieAttackState>();
                    break;
                case EnemyState.Dead:
                    StateMachine.EnterState<ZombieDeadState>();
                    break;
            }
        }
    }
}