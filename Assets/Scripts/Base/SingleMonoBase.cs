using UnityEngine;

namespace Base
{
    /// <summary>
    /// 单例模式限定器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingleMonoBase<T> : MonoBehaviour where T : SingleMonoBase<T>
    {
        public static T INSTANCE;
        protected virtual void Awake()
        {
            if (INSTANCE != null)
            {
                Debug.LogError("单例模式限定器：" + typeof(T) + "已存在");
            }
            INSTANCE = (T)this;
        }

        private void OnDestroy()
        {
            INSTANCE = null;
        }
    }
}
