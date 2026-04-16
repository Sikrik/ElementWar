using Base;
using UnityEngine;
using Utils;

namespace Player.States
{
    /// <summary>
    /// 移动状态
    /// </summary>
    public class PlayerMoveState: PlayerStateBase
    {
        #region 动画器相关参数

        private int _moveBlendHash;
        private float _moveBlend;//移动混合
        private float _runThreshold = 0;
        private float _sprintThreshold = 1;
        private float _transitionSpeed = 5;
        
        #endregion

        public override void Init(IStateMachineOwner owner)
        {
            base.Init(owner);
            _moveBlendHash = Animator.StringToHash("MoveBlend");
        }

        public override void Enter()
        {
            base.Enter();
            playerModel.PlayerStateAnimation("Move");
        }
        
        public override void Update()
        {
            base.Update();
            if (IsBeControl())
            {
                #region 监听待机状态
                if (playerController._moveInput.magnitude == 0)
                {
                    playerModel.SwitchState(PlayerState.Idle);
                    return;
                }
                #endregion
                
                #region 处理跳跃状态
                if (playerController._isJumping)
                {
                    SwitchToHover();
                    return;
                }
                #endregion
                
                #region 处理移动速度

                if (playerController._isSprint)
                {
                    _moveBlend = Mathf.Lerp(_moveBlend, _sprintThreshold, _transitionSpeed * Time.deltaTime);
                }
                else
                {
                    _moveBlend = Mathf.Lerp(_moveBlend, _runThreshold, _transitionSpeed * Time.deltaTime);
                }
                playerModel.animator.SetFloat(_moveBlendHash,_moveBlend);

                #endregion

                #region 处理方向

                float rad = Mathf.Atan2(playerController.localMovement.x, playerController.localMovement.z);
                playerModel.transform.Rotate(0,rad *playerController.rotationSpeed * Time.deltaTime,0);

                #endregion
            }
            
        }
    }
}