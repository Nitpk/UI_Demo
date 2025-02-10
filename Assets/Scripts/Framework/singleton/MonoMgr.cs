using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFrameWork
{
    /// <summary>
    /// Mono单例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DisallowMultipleComponent]
    public class MonoMgr<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                return instance;
            }
        }

        protected virtual void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }

            instance = this as T;
            DontDestroyOnLoad(this.gameObject);

        }
    }
}
