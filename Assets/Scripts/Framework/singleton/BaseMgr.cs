using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace TFrameWork
{
    /// <summary>
    /// 单例基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseMgr<T> where T : class
    {
        private static T instance;

        private static readonly object lockObj = new object();

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObj)
                    {
                        if (instance == null)
                        {
                            ConstructorInfo info = typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
                            if (info != null)
                            {
                                instance = info.Invoke(null) as T;
                            }
                            else
                            {
                                Debug.Log("没有私有的无参构造函数:" + typeof(T).ToString());
                            }
                        }
                    }
                }

                return instance;
            }
        }
    }
}
