using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TFrameWork
{
    /// <summary>
    /// 整合的AB包资源管理器
    /// </summary>
    public class ABMgr : BaseMgr<ABMgr>
    {
        //在编辑器模式下使用ab包方式加载资源
        private bool useABResWhenEditor = false;
        private ABMgr() { }

        /// <summary>
        /// 加载AB包资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="abName">ab包名</param>
        /// <param name="resName">资源名</param>
        /// <param name="callBack">回调</param
        /// /// <param name="isSync">是否同步加载，默认异步</param>
        public void LoadABRes<T>(string abName, string resName, UnityAction<T> callBack, bool isSync = false) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            if (!useABResWhenEditor)
            {
                T res = EditorResMgr.Instance.LoadEditorRes<T>($"{abName}/{resName}");
                callBack?.Invoke(res);
            }
            else
            {
                ABResMgr.Instance.LoadABRes<T>(abName, resName, callBack, isSync);
            }

#else
            ABResMgr.Instance.LoadABRes<T>(abName, resName, callBack, isSync);
#endif
        }

    }
}

