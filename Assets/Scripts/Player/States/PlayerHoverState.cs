
namespace Player.States
{
    /// <summary>
    /// 玩家悬空状态
    /// 当玩家跳跃或处于空中时处于此状态
    /// 负责播放悬空动画并检测落地以切换到待机状态
    /// </summary>
    public class PlayerHoverState:PlayerStateBase
    {
        /// <summary>
        /// 进入悬空状态时调用
        /// 播放悬空/跳跃动画，让玩家角色进入 hover 状态
        /// 同时调用基类的 Enter 方法注册 Update 回调
        /// </summary>
        public override void Enter()
        {
            base.Enter();
            playerModel.PlayerStateAnimation("Hover");
        }

        /// <summary>
        /// 每帧更新悬空状态逻辑
        /// 持续检测玩家是否已经落地
        /// 如果检测到玩家接触地面，则切换回待机状态
        /// </summary>
        public override void Update()
        {
            base.Update();

            #region 检测玩家是否在地面上
            // 通过 CharacterController 的 isGrounded 属性检测玩家是否已接触地面
            // isGrounded 为 true 表示玩家已落地，可以切换回待机状态
            if(playerModel.cc.isGrounded)
                playerModel.SwitchState(PlayerState.Idle);

            #endregion
        }
    }
}