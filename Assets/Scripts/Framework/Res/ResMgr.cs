using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace TFrameWork {
    /// <summary>
    /// 资源信息基类
    /// </summary>
    public abstract class ResInfoBase
    {
        //资源引用次数
        public int usedNum;
    }
    /// <summary>
    /// 资源信息
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    public class ResInfo<T> : ResInfoBase 
    {
        //资源
        public T asset;
        //异步加载的回调函数
        public UnityAction<T> callBack;
        //协程，便于可能的停止
        public Coroutine coroutine;
        //（在单个删除时）引用次数为0时是否立刻删除
        public bool isDel;

        /// <summary>
        /// 增加引用次数
        /// </summary>
        public void Add()
        {
            usedNum++;
        }
        /// <summary>
        /// 减少引用次数
        /// </summary>
        public void Sub()
        {
            usedNum--;
            if (usedNum < 0)
            {
                Debug.Log("引用次数低于0，请检查加载和卸载是否匹配");
            }
        }
    }
    /// <summary>
    /// Resources资源管理器
    /// </summary>
    public class ResMgr : BaseMgr<ResMgr>
    {
        private ResMgr() { }
        private Dictionary<string,ResInfoBase> resDic = new Dictionary<string,ResInfoBase>();

        /// <summary>
        /// 同步加载
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public T Load<T>(string path) where T : Object
        {
            string key=path + "_" + typeof(T).Name;

            ResInfo<T> resInfo;

            if (resDic.ContainsKey(key))
            {//如果字典中已经存在资源
                resInfo = resDic[key] as ResInfo<T>;

                if (resInfo.asset == null)
                {//如果异步还没加载完成

                    //停止协程
                    MonoUpdate.Instance.StopCoroutine(resInfo.coroutine);

                    //同步加载资源
                    resInfo.asset = Resources.Load<T>(path);

                    //执行回调
                    resInfo.callBack?.Invoke(resInfo.asset);

                    //执行完毕，清空
                    resInfo.coroutine = null;
                    resInfo.callBack = null;
                }    
            }
            else
            {//如果不存在
                resInfo = new ResInfo<T>();
                //同步加载资源
                resInfo.asset = Resources.Load<T>(path);
                //加入字典
                resDic.Add(key,resInfo);
            }
            //增加一次引用次数
            resInfo.Add();
            //返回资源
            return (resDic[key] as ResInfo<T> ).asset;
            
        }
        /// <summary>
        /// 异步加载
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="callBack"></param>
        public void LoadAsync<T>(string path,UnityAction<T> callBack) where T : Object
        {
            string key = path + "_" + typeof(T).Name;
            ResInfo<T> resInfo;

            if (resDic.ContainsKey(key))
            {//如果字典中存在
                resInfo = resDic[key] as ResInfo<T>;

                if(resInfo.asset == null)
                {//还没有加载完成
                    //将回调加入正在执行的协程
                    resInfo.callBack += callBack;
                }
                else
                {//已经加载完成
                    //直接使用
                    callBack?.Invoke(resInfo.asset);
                }
            }
            else
            {//字典中不存在

                resInfo= new ResInfo<T>();
                //开启协程，异步加载
                resInfo.callBack += callBack;
                resInfo.coroutine = MonoUpdate.Instance.StartCoroutine(CoroutineLoadAsync<T>(path));
                
                //加入字典
                resDic.Add(key, resInfo);
            }
            //引用次数加一 
            resInfo.Add();
        }

        private IEnumerator CoroutineLoadAsync<T>(string path) where T : Object
        {
            ResourceRequest rq = Resources.LoadAsync<T>(path);
            yield return rq;

            //异步加载完成
            string key = path + "_" + typeof(T).Name;

            if (resDic.ContainsKey(key))
            {
                ResInfo<T> resInfo = resDic[key] as ResInfo<T>;
                //保存资源
                resInfo.asset = rq.asset as T;

                if (resInfo.usedNum == 0)
                {//如果 引用计数为0 卸载资源
                    UnloadAsset<T>(path, resInfo.isDel, null, false);
                }
                else
                {
                    //执行回调
                    resInfo.callBack?.Invoke(resInfo.asset);
                    //清空
                    resInfo.coroutine = null;
                    resInfo.callBack = null;
                }
            }  
        }
        /// <summary>
        /// 卸载指定资源
        /// </summary>
        /// <param name="path"></param>
        public void UnloadAsset<T>(string path, bool isDel = false,UnityAction < T> callBack=null,bool isSub = true )
        {
            string key = path + "_"+typeof(T).Name;
            if (resDic.ContainsKey(key))
            {//如果字典中存在

                ResInfo<T> resInfo = resDic[key] as ResInfo<T>;

                //引用次数减一
                if(isSub)
                    resInfo.Sub();
                
                resInfo.isDel = isDel;

                if (resInfo.asset == null)
                {//如果资源还在异步加载
                    //把callBack移除
                    if(callBack != null)
                        resInfo.callBack -= callBack;
                }
                else if(resInfo.usedNum == 0 && resInfo.isDel)
                {//资源加载完毕   并且引用计数为0时，直接卸载
                    //移除出字典
                    resDic.Remove(key);
                    //卸载资源
                    Resources.UnloadAsset(resInfo.asset as Object);
                }
                
            } 
        }

        /// <summary>
        /// 卸载所有未使用资源
        /// </summary>
        public void UnloadUnusedAssets(UnityAction callBack = null)
        {
            List<string> removeList = new List<string>();
            foreach (string key in resDic.Keys)
            {
                if (resDic[key].usedNum == 0)
                {//找到引用次数为0的资源，加入待移除列表
                    removeList.Add(key);
                }
            }
            //遍历列表清除
            foreach (string key in removeList)
            {
                resDic.Remove(key);
            }

            //卸载所有未使用资源
            MonoUpdate.Instance.StartCoroutine(C_Clear(callBack));
        }

        /// <summary>
        /// 清空全部资源
        /// </summary>
        /// <param name="callBack"></param>
        public void Clear(UnityAction callBack = null)
        {
            resDic.Clear();
            //卸载所有未使用资源
            MonoUpdate.Instance.StartCoroutine(C_Clear(callBack));
        }
        private IEnumerator C_Clear(UnityAction callBack)
        {
            AsyncOperation ao = Resources.UnloadUnusedAssets();
            yield return ao;

            //卸载完毕，执行回调
            callBack?.Invoke();

        }

    }
}
