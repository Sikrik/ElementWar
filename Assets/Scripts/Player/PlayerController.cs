using Base;
using Cinemachine;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// 玩家控制器，继承单例控制器
    /// 负责管理玩家的输入转发、全局相机系统
    /// </summary>
    public class PlayerController : SingleMonoBase<PlayerController>
    {
        public PlayerModel currentPlayerModel;//当前玩家模型，用于显示和操作玩家角色
        private Transform _cameraTransform;//相机变换组件，用于获取相机方向和位置信息
        
        [Tooltip("正常视角的虚拟摄像机")]
        public CinemachineFreeLook freeLookCamera;
        [Tooltip("瞄准视角的虚拟摄像机")]
        public CinemachineFreeLook aimingCamera;
        
        #region 瞄准开火相关
        private CinemachineImpulseSource _impulseSource;
        #endregion
        
        private MyInputSystem _inputSystem;//输入系统，处理玩家的各种输入操作

        /// <summary>
        /// 初始化方法，在对象创建时调用
        /// </summary>
        protected override void Awake()
        {
            base.Awake();//调用父类的Awake方法，确保单例模式正确初始化
            _inputSystem = new MyInputSystem();//初始化输入系统实例，准备接收玩家输入
        }
        
        /// <summary>
        /// 启动时调用，在所有Awake之后执行
        /// </summary>
        private void Start()
        {
            // 如果场景中存在主相机，则获取其变换组件
            if (Camera.main != null) _cameraTransform = Camera.main.transform;
            Cursor.lockState = CursorLockMode.Locked;
            ExitAim();
            _impulseSource = aimingCamera.GetComponent<CinemachineImpulseSource>();
        }
        
        /// <summary>
        /// 启用时调用
        /// </summary>
        private void OnEnable()
        {
            _inputSystem.Enable();//激活输入系统，使输入映射生效并开始监听输入事件
        }
        
        /// <summary>
        /// 禁用时调用
        /// </summary>
        private void OnDisable()
        {
            _inputSystem.Disable();//禁用输入系统，停止监听输入事件以节省资源
        }
        
        /// <summary>
        /// 每帧更新逻辑
        /// 读取并处理玩家的输入状态，转发给当前玩家Model
        /// </summary>
        private void Update()
        {
            if (currentPlayerModel == null) return;
            
            #region 更新玩家输入并转发给Model
            // 读取移动输入并归一化，确保对角线移动速度一致
            Vector2 moveInput = _inputSystem.Player.Move.ReadValue<Vector2>().normalized;
            
            // 读取各个功能按键状态
            bool isSprint = _inputSystem.Player.IsSprint.IsPressed();
            bool isJumping = _inputSystem.Player.IsJumping.IsPressed();
            bool isAiming = _inputSystem.Player.IsAiming.IsPressed();
            bool isFire = _inputSystem.Player.Fire.IsPressed();
            
            // 将输入转发给当前玩家Model，由Model处理本地逻辑
            currentPlayerModel.UpdateInput(
                moveInput, 
                isSprint, 
                isJumping, 
                isAiming, 
                isFire,
                _cameraTransform
            );
            #endregion
        }

        /// <summary>
        /// 退出瞄准状态，恢复正常相机
        /// </summary>
        public void ExitAim()
        {
            if(currentPlayerModel == null) return;
            
            freeLookCamera.m_XAxis.Value = aimingCamera.m_XAxis.Value;
            freeLookCamera.m_YAxis.Value = aimingCamera.m_YAxis.Value;
            //关闭瞄准约束
            currentPlayerModel.rightHandAimConstraint.weight = 0;
            currentPlayerModel.bodyAimConstraint.weight = 0;
            currentPlayerModel.rightHandConstraint.weight = 1;
            freeLookCamera.Priority = 100;
            aimingCamera.Priority = 0;
        }

        /// <summary>
        /// 进入瞄准状态，切换到瞄准相机
        /// </summary>
        public void EnterAim()
        {
            if(currentPlayerModel == null) return;
            
            //同步瞄准相机和自由相机的瞄准角度
            aimingCamera.m_XAxis.Value = freeLookCamera.m_XAxis.Value;
            aimingCamera.m_YAxis.Value = freeLookCamera.m_YAxis.Value;
            //启用瞄准约束
            currentPlayerModel.rightHandAimConstraint.weight = 1;
            currentPlayerModel.bodyAimConstraint.weight = 1;
            currentPlayerModel.rightHandConstraint.weight = 0;
            //设置优先级
            freeLookCamera.Priority = 0;
            aimingCamera.Priority = 100;
        }

        /// <summary>
        /// 抖动屏幕
        /// </summary>
        public void ShakeCamera()
        {
           _impulseSource.GenerateImpulse(); 
        }
    }
}