using Base;

namespace Enemy.States
{
    public class ZombieAttackState: EnemyStateBase
    {
        public override void Enter()
        {
            base.Enter();
            enemyModel.PlayStateAnimation("Attack");
        }
    }
}