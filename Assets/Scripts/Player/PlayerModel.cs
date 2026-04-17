
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
        Hover,  // 悬空状态
        Aiming  // 瞄准状态
    }

    /// <summary>
    /// 角色模型类
    /// 负责管理玩家的所有数据、动画、状态机和物理相关参数
    /// 每个玩家实例独立拥有自己的所有数据
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
        
        #region 玩家输入数据（每个玩家独立）
        [HideInInspector] public Vector2 MoveInput;
        [HideInInspector] public bool IsSprint;
        [HideInInspector] public bool IsJumping;
        [HideInInspector] public bool IsAiming;
        [HideInInspector] public bool IsFire;
        #endregion
        
        #region 移动物理参数
        [Tooltip("重力加速度")]
        public float gravity = -9.81f;
        
        [Tooltip("跳跃高度")]
        public float jumpHeight = 1.5f;
        
        [Tooltip("转向速度")]
        public float rotationSpeed = 650f;
        
        [HideInInspector]
        public float verticalSpeed;
        
        [HideInInspector]
        public Vector3 horizontalVelocity;
        [HideInInspector]
        public float fallHeight = 0.2f;
        
        [HideInInspector]
        public Vector3 localMovement;//本地坐标系下的移动向量
        [HideInInspector]
        public Vector3 worldMovement;//世界坐标系下的移动向量
        #endregion
        
        #region 瞄准相关参数
        [Tooltip("瞄准目标")]
        public Transform aimTarget;
        [Tooltip("射线检测最大距离")]
        public float maxRayDistance = 650f;
        [Tooltip("射线检测的层级")]
        public LayerMask aimLayerMask = ~0;
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
        
        /// <summary>
        /// 接收Controller转发的输入，处理本地移动方向计算
        /// </summary>
        public void UpdateInput(Vector2 moveInput, bool isSprint, bool isJumping, bool isAiming, bool isFire, Transform cameraTransform)
        {
            // 更新输入数据
            MoveInput = moveInput;
            IsSprint = isSprint;
            IsJumping = isJumping;
            IsAiming = isAiming;
            IsFire = isFire;
            
            if (cameraTransform == null)
            {
                // 相机未初始化时，使用默认世界坐标系方向保证输入正常生效
                worldMovement = new Vector3(moveInput.x, 0, moveInput.y);
                localMovement = worldMovement;
                return;
            }
            
            #region 计算玩家的移动方向
            // 将相机的前向和右向投影到水平面上（忽略Y轴），并进行归一化
            Vector3 cameraForwardProjection = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
            Vector3 cameraRightProjection = new Vector3(cameraTransform.right.x, 0, cameraTransform.right.z).normalized;
            
            worldMovement = cameraRightProjection * moveInput.x + cameraForwardProjection * moveInput.y;
            // 将世界坐标系的移动方向转换为角色本地坐标系的方向
            localMovement = transform.InverseTransformDirection(worldMovement);
            #endregion
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
        /// 更新瞄准目标位置
        /// </summary>
        public void UpdateAimingTarget()
        {
            //发射射线
            if (Camera.main != null)
            {
                Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                RaycastHit hit;
                //如果射线击中了物体
                if (Physics.Raycast(ray,out hit,maxRayDistance,aimLayerMask))
                {
                    //更新瞄准目标的位置
                    aimTarget.position = hit.point;
                }
                else
                {
                    aimTarget.position =ray.origin+ray.direction*maxRayDistance;
                }
            }
        }
        
        /// <summary>
        /// 动画移动回调
        /// 处理 Root Motion 和空中惯性移动
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