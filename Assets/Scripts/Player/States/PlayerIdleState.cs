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
        /// 播放待机动画，让玩家角色进入 idle 状态
        /// 同时调用基类的 Enter 方法注册 Update 回调
        /// </summary>
        public override void Enter()
        {
            base.Enter();
            playerModel.PlayerStateAnimation("Idle");//播放待机动画
        }

        /// <summary>
        /// 每帧更新待机状态逻辑
        /// 检测玩家是否有移动输入，如果有则切换到移动状态
        /// 检测玩家是否处于悬空状态，如果是则切换到悬空状态
        /// 仅在玩家可被控制时响应输入
        /// </summary>
        public override void Update()
        {
            base.Update();
            
            // 检查玩家是否可以被控制（未被眩晕、冰冻等控制效果影响）
            // 只有当前模型是控制器管理的模型时才响应输入
            if (IsBeControl())
            {
                #region 移动状态监听
                // 检测移动输入的大小，如果不为零说明玩家有移动意图
                // _moveInput 是 Vector2 类型，magnitude 表示输入的向量长度
                if (playerController._moveInput.magnitude != 0)
                    playerModel.SwitchState(PlayerState.Move);//切换到移动状态
                #endregion
                
                #region 悬空状态监听
                // 检测玩家是否处于悬空状态（跳跃或下落中）
                // _isJumping 为 true 时表示玩家正在空中
                if (playerController._isJumping)
                    SwitchToHover();//切换到悬空状态，同时设置垂直速度
                #endregion
            }
        }
    }
}