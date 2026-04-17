using Base;

namespace Enemy.States
{
    public class ZombieDeadState: EnemyStateBase
    {
        public override void Enter()
        {
            base.Enter();
            enemyModel.PlayStateAnimation("Dead");
        }
    }
}