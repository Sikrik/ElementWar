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
        private float _transitionSpeed = 5;
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
        }

        public override void Update()
        {
            base.Update();

            #region 处理角色的移动输入

            _aimingX = Mathf.Lerp(_aimingX, playerController._moveInput.x, _transitionSpeed);
            _aimingY = Mathf.Lerp(_aimingY, playerController._moveInput.y, _transitionSpeed);
            playerModel.animator.SetFloat(_aimingXHash, _aimingX);
            playerModel.animator.SetFloat(_aimingYHash, _aimingY);
            #endregion
        }
    }
}