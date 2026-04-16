using System;
using Base;
using Cinemachine;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// 玩家控制器，继承单例控制器
    /// 负责管理玩家的输入、状态和行为逻辑
    /// 作为单例存在，其他系统可通过实例访问玩家相关数据
    /// </summary>
    public class PlayerController : SingleMonoBase<PlayerController>
    {
        public PlayerModel currentPlayerModel;//当前玩家模型，用于显示和操作玩家角色
        private Transform _cameraTransform;//相机变换组件，用于获取相机方向和位置信息
        
        [Tooltip("正常视角的虚拟摄像机")]
        public CinemachineFreeLook freeLookCamera;
        [Tooltip("瞄准视角的虚拟摄像机")]
        public CinemachineFreeLook aimingCamera;

        #region 玩家输入
        private MyInputSystem _inputSystem;//输入系统，处理玩家的各种输入操作
        [HideInInspector]
        public Vector2 _moveInput;//移动输入向量，存储玩家的移动方向和强度（已归一化）
        public bool _isSprint;//是否冲刺，标记玩家当前是否处于冲刺状态
        public bool _isJumping;//是否跳跃，标记玩家当前是否按下跳跃键
        public bool _isAiming;//是否瞄准，标记玩家当前是否处于瞄准状态
        #endregion
        
        [Tooltip("转向速度")]
        public float rotationSpeed;//角色转向速度，控制角色旋转的快慢
        
        [HideInInspector]
        public Vector3 localMovement;//本地坐标系下的移动向量，用于角色自身的移动计算
        public Vector3 worldMovement;//世界坐标系下的移动向量，用于实际的角色位移
        
        /// <summary>
        /// 初始化方法，在对象创建时调用
        /// 用于设置初始化和一次性配置
        /// </summary>
        protected override void Awake()
        {
            base.Awake();//调用父类的Awake方法，确保单例模式正确初始化
            _inputSystem = new MyInputSystem();//初始化输入系统实例，准备接收玩家输入
        }
        
        /// <summary>
        /// 启动时调用，在所有Awake之后执行
        /// 用于获取场景中的相机引用，为后续的方向计算做准备
        /// </summary>
        private void Start()
        {
            // 如果场景中存在主相机，则获取其变换组件
            if (Camera.main != null) _cameraTransform = Camera.main.transform;
            Cursor.lockState = CursorLockMode.Locked;
            ExitAim();
        }

        /// <summary>
        /// 启用时调用
        /// 当脚本启用时激活输入系统，开始接收玩家输入
        /// </summary>
        private void OnEnable()
        {
            _inputSystem.Enable();//激活输入系统，使输入映射生效并开始监听输入事件
        }

        /// <summary>
        /// 禁用时调用
        /// 当脚本禁用时关闭输入系统，停止接收玩家输入
        /// </summary>
        private void OnDisable()
        {
            _inputSystem.Disable();//禁用输入系统，停止监听输入事件以节省资源
        }

        /// <summary>
        /// 每帧更新逻辑
        /// 读取并处理玩家的输入状态，更新相关的输入标志和移动方向
        /// </summary>
        private void Update()
        {
            #region 更新玩家输入
            // 读取移动输入并归一化，确保对角线移动速度一致
            _moveInput = _inputSystem.Player.Move.ReadValue<Vector2>().normalized;
            
            // 检测冲刺按键是否被按下
            _isSprint = _inputSystem.Player.IsSprint.IsPressed();
            
            // 检测瞄准按键是否被按下
            _isAiming = _inputSystem.Player.IsAiming.IsPressed();
            
            // 检测跳跃按键是否被按下
            _isJumping = _inputSystem.Player.IsJumping.IsPressed();
            #endregion

            #region 计算玩家的移动方向
            // 将相机的前向和右向投影到水平面上（忽略Y轴），并进行归一化（避免斜视时速度变慢）
            Vector3 cameraForwardProjection = new Vector3(_cameraTransform.forward.x, 0, _cameraTransform.forward.z).normalized;
            Vector3 cameraRightProjection = new Vector3(_cameraTransform.right.x, 0, _cameraTransform.right.z).normalized;

            // 正确的映射：
            // 水平输入 (x) -> 乘上相机的右方向 (左右移动)
            // 垂直输入 (y) -> 乘上相机的前方向 (前后移动)
            worldMovement = cameraRightProjection * _moveInput.x + cameraForwardProjection * _moveInput.y;

            // 将世界坐标系的移动方向转换为角色本地坐标系的方向，用于角色的朝向和动画
            localMovement = currentPlayerModel.transform.InverseTransformDirection(worldMovement);
            #endregion
        }

        public void ExitAim()
        {
            freeLookCamera.m_XAxis.Value = aimingCamera.m_XAxis.Value;
            freeLookCamera.m_YAxis.Value = aimingCamera.m_YAxis.Value;
            
            freeLookCamera.Priority = 100;
            aimingCamera.Priority = 0;
        }
        public void EnterAim()
        {
            //同步瞄准相机和自由相机的瞄准角度
            aimingCamera.m_XAxis.Value = freeLookCamera.m_XAxis.Value;
            aimingCamera.m_YAxis.Value = freeLookCamera.m_YAxis.Value;
            //设置优先级
            freeLookCamera.Priority = 0;
            aimingCamera.Priority = 100;
        }
    }
}
