using System;
using System.Collections.Generic;
using Base;
namespace Utils
{
    public interface IStateMachineOwner { }
    /// <summary>
    /// 角色状态机
    /// </summary>
    public class StateMachine
    {
        private StateBase _currentSate;//当前状态
        private IStateMachineOwner _owner;//状态机拥有者
        private Dictionary<Type, StateBase> _stateDic = new Dictionary<Type, StateBase>();//状态字典
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        public StateMachine(IStateMachineOwner owner)
        {
            _owner = owner;
        }
        /// <summary>
        /// 进入动画状态
        /// </summary>
        /// <typeparam name="T">状态的实例</typeparam>
        public void EnterState<T>() where T : StateBase , new()
        {   //判断当前状态是否和要进入的状态一致
            if(_currentSate != null && _currentSate.GetType() == typeof(T))
                return;
            //离开当前状态
            if (_currentSate != null)
                _currentSate.Exit();
            _currentSate = LoadState<T>();
            _currentSate.Enter();
        }
        
        /// <summary>
        /// 加载状态
        /// </summary>
        /// <typeparam name="T">状态类</typeparam>
        /// <returns>状态实例</returns>
        private StateBase LoadState<T>() where T : StateBase, new()
        {
            Type stateTtpe = typeof(T);
            //如果状态字典里没有这个状态
            if (!_stateDic.TryGetValue(stateTtpe, out StateBase state))
            {
                state = (StateBase) new T();//创建状态
                state.Init(_owner);//初始化状态
                _stateDic.Add(stateTtpe,state);//添加进字典
            }

            return state;
        }
        /// <summary>
        /// 停止状态机
        /// </summary>
        public void Stop()
        {
            //离开当前状态
            if (_currentSate!= null)
                _currentSate.Exit();
            //销毁状态
            foreach (var state in _stateDic.Values)
                state.Destory();
            _stateDic.Clear();
        }
    }
}
