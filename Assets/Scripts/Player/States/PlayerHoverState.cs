namespace Player.States
{
    /// <summary>
    /// 玩家悬空状态
    /// 当玩家跳跃或处于空中时处于此状态
    /// </summary>
    public class PlayerHoverState:PlayerStateBase
    {
        /// <summary>
        /// 进入悬空状态时调用
        /// </summary>
        public override void Enter()
        {
            base.Enter();
            PlayerModel.PlayerStateAnimation("Hover");
        }
        
        /// <summary>
        /// 每帧更新悬空状态逻辑
        /// </summary>
        public override void Update()
        {
            base.Update();
            #region 检测玩家是否在地面上
            if(PlayerModel.cc.isGrounded)
                PlayerModel.SwitchState(PlayerState.Idle);
            #endregion
        }
    }
}