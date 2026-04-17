using Base;
using UnityEngine;
using Utils;

namespace Player
{
    /// <summary>
    /// 玩家状态基类
    /// 所有玩家状态的父类，提供通用的状态管理功能
    /// 包括重力计算、状态切换、输入控制等基础逻辑
    /// </summary>
    public class PlayerStateBase : StateBase
    {
        protected PlayerController playerController;
        protected PlayerModel playerModel;
        
        /// <summary>
        /// 初始化状态，获取玩家控制器和模型引用
        /// </summary>
        public override void Init(IStateMachineOwner owner)
        {
            playerController = PlayerController.INSTANCE;
            playerModel = (PlayerModel)owner;
        }
        
        /// <summary>
        /// 进入状态时调用，注册 Update 到 MonoManager 实现每帧更新
        /// </summary>
        public override void Enter()
        {
            MonoManager.INSTANCE.AddUpdateAction(Update);
        }
        
        /// <summary>
        /// 每帧更新状态逻辑，处理重力计算和悬空状态检测
        /// </summary>
        public override void Update()
        {
            #region 重力计算
            if (!playerModel.cc.isGrounded)
            {
                // 空中状态：累加重力加速度
                playerModel.verticalSpeed += playerModel.gravity * Time.deltaTime;
                if (playerModel.IsHover())
                    playerModel.SwitchState(PlayerState.Hover);
            }
            else
            {
                // 地面状态：重置垂直速度
                playerModel.verticalSpeed = playerModel.gravity * Time.deltaTime;
            }
            #endregion
            
            #region 瞄准监听
            // 添加条件：如果你已经处于瞄准状态，就不要再重复请求切换了
            if (playerModel.IsAiming && !(this is Player.States.PlayerAimingState) || playerModel.IsFire)
            {
                playerModel.SwitchState(PlayerState.Aiming);
            }
            #endregion
        }
        
        /// <summary>
        /// 退出状态时调用，从 MonoManager 移除 Update 注册
        /// </summary>
        public override void Exit()
        {
            MonoManager.INSTANCE.RemoveUpdateAction(Update);
        }
        
        /// <summary>
        /// 销毁状态时调用，清理资源并移除 Update 注册
        /// </summary>
        public override void Destory()
        {
            MonoManager.INSTANCE.RemoveUpdateAction(Update);
        }
        
        /// <summary>
        /// 判断当前模型是否被玩家所控制
        /// </summary>
        /// <returns>如果当前模型是控制器管理的模型则返回 true</returns>
        public bool IsBeControl()
        {
            return playerModel == playerController.currentPlayerModel;
        }
        
        /// <summary>
        /// 切换到悬空状态
        /// </summary>
        public void SwitchToHover()
        {
            playerModel.verticalSpeed = Mathf.Sqrt(-2 * playerModel.gravity * playerModel.jumpHeight);
            playerModel.SwitchState(PlayerState.Hover);
        }
    }
}