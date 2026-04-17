
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
            PlayerModel.PlayerStateAnimation("Move");
        }
        
        public override void Update()
        {
            base.Update();
            if (IsBeControl())
            {
                #region 监听待机状态
                if (PlayerModel.MoveInput.magnitude == 0)
                {
                    PlayerModel.SwitchState(PlayerState.Idle);
                    return;
                }
                #endregion
                
                #region 处理跳跃状态
                if (PlayerModel.IsJumping)
                {
                    SwitchToHover();
                    return;
                }
                #endregion
                
                #region 处理移动速度
                if (PlayerModel.IsSprint)
                {
                    _moveBlend = Mathf.Lerp(_moveBlend, _sprintThreshold, _transitionSpeed * Time.deltaTime);
                }
                else
                {
                    _moveBlend = Mathf.Lerp(_moveBlend, _runThreshold, _transitionSpeed * Time.deltaTime);
                }
                PlayerModel.animator.SetFloat(_moveBlendHash,_moveBlend);
                #endregion
                
                #region 处理方向
                float rad = Mathf.Atan2(PlayerModel.localMovement.x, PlayerModel.localMovement.z);
                PlayerModel.transform.Rotate(0,rad * PlayerModel.rotationSpeed * Time.deltaTime,0);
                #endregion
            }
        }
    }
}