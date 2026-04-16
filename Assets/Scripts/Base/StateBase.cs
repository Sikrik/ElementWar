using UnityEditor;
using Utils;

namespace Base
{
    /// <summary>
    /// 状态基类
    /// 所有游戏状态类的抽象基类，定义了状态的基本生命周期和行为
    /// </summary>
    public abstract class StateBase
    {
        /// <summary>
        /// 初始化方法
        /// 在状态创建时调用，用于设置状态所需的初始数据和引用
        /// </summary>
        /// <param name="owner">状态机所有者，提供状态访问上下文的接口</param>
        public abstract void Init(IStateMachineOwner owner);

        /// <summary>
        /// 进入状态
        /// 当状态机切换到此状态时调用，执行一次性的初始化逻辑
        /// 例如：播放进入动画、设置角色属性、启动音效等
        /// </summary>
        public abstract void Enter();

        /// <summary>
        /// 状态更新
        /// 在此状态激活期间每帧调用，处理状态的持续逻辑
        /// 例如：检测状态转换条件、更新角色行为、处理输入等
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// 离开状态
        /// 当状态机从此状态切换到其他状态时调用，执行清理逻辑
        /// 例如：停止动画、重置临时变量、清理资源等
        /// </summary>
        public abstract void Exit();

        /// <summary>
        /// 销毁方法
        /// 当角色在场景内被销毁时调用，确保相关资源得到正确释放
        /// 例如：销毁动画状态、清理事件监听、释放托管资源等
        /// </summary>
        public abstract void Destory();
    }
}