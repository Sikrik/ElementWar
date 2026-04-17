using Utils;

namespace Base
{
    public class EnemyStateBase: StateBase
    {
        protected EnemyBase enemyModel;
        public override void Init(IStateMachineOwner owner)
        {
            enemyModel = (EnemyBase) owner;
        }

        public override void Enter()
        {
            MonoManager.INSTANCE.AddUpdateAction(Update);
        }

        public override void Update()
        {
           
        }

        public override void Exit()
        {
            MonoManager.INSTANCE.RemoveUpdateAction(Update);
        }

        public override void Destory()
        {
            
        }
    }
}