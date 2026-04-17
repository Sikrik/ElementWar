using System;
using Base;

namespace Utils
{
    /// <summary>
    /// 任务处理管理器
    /// </summary>
    public class MonoManager:SingleMonoBase<MonoManager>
    {
        private Action _updateAction;

        public void AddUpdateAction(Action task)
        {
            _updateAction += task;
        }

        public void RemoveUpdateAction(Action  task)
        {
            _updateAction -= task;
        }
        private void Update()
        {
            _updateAction?.Invoke();
        }
    }
}