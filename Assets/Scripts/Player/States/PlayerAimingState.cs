using UnityEngine;
using Utils;

namespace Player.States
{
    public class PlayerAimingState: PlayerStateBase
    {
        #region 动画器相关属性
        private int _aimingXHash;
        private int _aimingYHash;
        private float _aimingX = 0;
        private float _aimingY = 0;
        private float _transitionSpeed = 8;
        #endregion
        
        public override void Init(IStateMachineOwner owner)
        {
            base.Init(owner);
            _aimingXHash = Animator.StringToHash("AimingX");
            _aimingYHash = Animator.StringToHash("AimingY");
        }
        
        public override void Enter()
        {
            base.Enter();
            PlayerModel.PlayerStateAnimation("Aiming");
            if (IsBeControl())
            {
                PlayerModel.UpdateAimingTarget();
                PlayerController.EnterAim();
            }
        }
        
        /// <summary>
        /// 退出瞄准状态
        /// </summary>
        public override void Exit()
        {
            base.Exit();
            if(IsBeControl())
                PlayerController.ExitAim();
        }
        
        public override void Update()
        {
            base.Update();
            if(IsBeControl()){
                //让模型旋转至摄像机方向
                if (Camera.main != null)
                {
                    PlayerModel.transform.rotation =
                        Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);
                    PlayerModel.UpdateAimingTarget();
                }
                
                #region 玩家松开鼠标右键后恢复正常状态
                if (!PlayerModel.IsAiming&& !PlayerModel.IsFire)
                {
                    PlayerModel.SwitchState(PlayerState.Idle);
                    return;
                }
                #endregion
                
                #region 开火监听
                if (PlayerModel.IsFire)
                {
                    PlayerModel.weapon.Fire(PlayerModel.aimTarget.position);
                    PlayerController.ShakeCamera();//屏幕抖动
                }
                #endregion
            
                #region 处理角色的移动输入
                _aimingX = Mathf.Lerp(_aimingX, PlayerModel.MoveInput.x, _transitionSpeed * Time.deltaTime);
                _aimingY = Mathf.Lerp(_aimingY, PlayerModel.MoveInput.y, _transitionSpeed * Time.deltaTime);
                PlayerModel.animator.SetFloat(_aimingXHash, _aimingX);
                PlayerModel.animator.SetFloat(_aimingYHash, _aimingY);
                #endregion
            }
        }
    }
}