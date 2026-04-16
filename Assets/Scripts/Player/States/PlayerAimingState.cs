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
            playerModel.PlayerStateAnimation("Aiming");
            if (IsBeControl())
            {
                playerController.EnterAim();
            }
            playerController.EnterAim();
        }
        public override void Exit()
        {
            base.Exit();
            if(IsBeControl())
                playerController.ExitAim(); // <--- 加上这句，退出状态时恢复正常相机
        }
        public override void Update()
        {
            base.Update();
            if(IsBeControl()){
                //让模型旋转至摄像机方向
                if (Camera.main != null)
                    playerModel.transform.rotation =
                        Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);

                #region 玩家松开鼠标右键后恢复正常状态

            if (!playerController._isAiming)
            {
                playerModel.SwitchState(PlayerState.Idle);
            }
            

                #endregion
            
            #region 处理角色的移动输入

            _aimingX = Mathf.Lerp(_aimingX, playerController._moveInput.x, _transitionSpeed * Time.deltaTime);
            _aimingY = Mathf.Lerp(_aimingY, playerController._moveInput.y, _transitionSpeed * Time.deltaTime);
            playerModel.animator.SetFloat(_aimingXHash, _aimingX);
            playerModel.animator.SetFloat(_aimingYHash, _aimingY);
            #endregion
            }
        }
    }
}