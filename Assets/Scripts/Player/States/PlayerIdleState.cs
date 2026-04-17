using Base;
using UnityEngine;

namespace Player.States
{
    /// <summary>
    /// 玩家待机状态
    /// 当玩家静止不动时处于此状态
    /// 负责播放待机动画并监听移动输入以切换到移动状态
    /// </summary>
    public class PlayerIdleState : PlayerStateBase
    {
        /// <summary>
        /// 进入待机状态时调用
        /// </summary>
        public override void Enter()
        {
            base.Enter();
            playerModel.PlayerStateAnimation("Idle");//播放待机动画
        }
        
        /// <summary>
        /// 每帧更新待机状态逻辑
        /// </summary>
        public override void Update()
        {
            base.Update();
            
            if (IsBeControl())
            {
                #region 移动状态监听
                if (playerModel.MoveInput.magnitude != 0)
                    playerModel.SwitchState(PlayerState.Move);//切换到移动状态
                #endregion
                
                #region 悬空状态监听
                if (playerModel.IsJumping)
                    SwitchToHover();//切换到悬空状态
                #endregion
            }
        }
    }
}