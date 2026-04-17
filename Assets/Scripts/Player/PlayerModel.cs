using System;
using Player.States;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Utils;

namespace Player
{
    /// <summary>
    /// 玩家状态枚举
    /// </summary>
    public enum PlayerState
    {
        Idle,   // 待机状态
        Move,   // 移动状态
        Hover,
        Aiming// 悬空状态
    }

    /// <summary>
    /// 角色模型类
    /// 负责管理玩家的动画、状态机和物理相关参数
    /// </summary>
    public class PlayerModel : MonoBehaviour, IStateMachineOwner
    {
        [Tooltip("角色武器")]
        public PlayerWeapon weapon;
        
        [HideInInspector]
        public Animator animator;
        public CharacterController cc;
        
        private StateMachine _stateMachine;
        private PlayerState _currentState;
        
        public TwoBoneIKConstraint rightHandConstraint;//正常状态下的右手约束
        public MultiAimConstraint rightHandAimConstraint;
        public MultiAimConstraint bodyAimConstraint;
        #region 垂直速度参数
        [Tooltip("重力加速度")]
        public float gravity = -9.81f;
        
        [Tooltip("跳跃高度")]
        public float jumpHeight = 1.5f;
        
        [HideInInspector]
        public float verticalSpeed;
        
        [HideInInspector]
        public Vector3 horizontalVelocity;

        [HideInInspector]
        public float fallHeight = 0.2f;
        #endregion

        #region 速度缓存
        private static readonly int CACHE_SIZE = 3;
        private Vector3[] _speedCache = new Vector3[CACHE_SIZE];
        private int _speedCacheIndex;
        private Vector3 _averageDeltaMovement;
        #endregion
        
        void Awake()
        {
            _stateMachine = new StateMachine(this);
            animator = GetComponent<Animator>();
            cc = GetComponent<CharacterController>();
        }

        void Start()
        {
            SwitchState(PlayerState.Idle);
        }

        void Update()
        {
        }
        
        /// <summary>
        /// 切换到指定状态
        /// </summary>
        public void SwitchState(PlayerState state)
        {
            switch (state)
            {
                case PlayerState.Idle:
                    _stateMachine.EnterState<PlayerIdleState>();
                    break;
                case PlayerState.Move:
                    _stateMachine.EnterState<PlayerMoveState>();
                    break;
                case PlayerState.Hover:
                    _stateMachine.EnterState<PlayerHoverState>();
                    break;
                case PlayerState.Aiming:
                    _stateMachine.EnterState<PlayerAimingState>();
                    break;
            }
            _currentState = state;
        }

        /// <summary>
        /// 更新速度缓存并计算平均速度
        /// </summary>
        private void UpdateAverageCacheSpeed(Vector3 newSpeed)
        {
            _speedCache[_speedCacheIndex++] = newSpeed;
            _speedCacheIndex %= CACHE_SIZE;
            
            Vector3 sum = Vector3.zero;
            foreach (Vector3 cache in _speedCache)
                sum += cache;
            
            _averageDeltaMovement = sum / CACHE_SIZE;
        }

        /// <summary>
        /// 播放状态动画，使用平滑过渡
        /// </summary>
        /// <param name="animationName">动画名称</param>
        /// <param name="transition">过渡时间（秒）</param>
        /// <param name="layer">动画层级</param>
        public void PlayerStateAnimation(string animationName, float transition = 0.25f, int layer = 0)
        {
            animator.CrossFadeInFixedTime(animationName, transition, layer);
        }

        /// <summary>
        /// 检测是否处于悬空状态
        /// 通过向下射线检测判断是否着地
        /// </summary>
        public bool IsHover()
        {
            return !Physics.Raycast(transform.position, Vector3.down, fallHeight);
        }
        
        /// <summary>
        /// 动画移动回调
        /// 处理 Root Motion 和空中惯性移动
        /// 地面时使用动画位移，空中时使用惯性速度
        /// </summary>
        private void OnAnimatorMove()
        {
            Vector3 playerDeltaMovement;

            if (cc.isGrounded)
            {
                // 地面：使用动画 Root Motion 位移
                playerDeltaMovement = animator.deltaPosition;
                UpdateAverageCacheSpeed(animator.velocity);
                // 记录水平速度用于起跳时的惯性保持
                if (Time.deltaTime > 0)
                {
                    horizontalVelocity = playerDeltaMovement / Time.deltaTime;
                    horizontalVelocity.y = 0;
                }
            }
            else
            {
                // 空中：使用记录的惯性速度
                playerDeltaMovement = _averageDeltaMovement * Time.deltaTime;
            }

            // 垂直方向由代码控制
            playerDeltaMovement.y = verticalSpeed * Time.deltaTime;
    
            cc.Move(playerDeltaMovement);
        }
    }
}
