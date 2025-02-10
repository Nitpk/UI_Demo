using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TFrameWork
{
    /// <summary>
    /// 缓存池管理器
    /// GameObject类型的对象的初始化或结束时的操作需要在取出或放入后进行处理，不能直接用onEnable等方法，否则在放入取出中可能出现问题
    /// </summary>
    public class PoolMgr : BaseMgr<PoolMgr>
    {
        //开启布局
        public static bool isLayout = false;

        //缓存池字典(继承Mono的GameObject)
        private Dictionary<string, PoolDate> poolDic = new Dictionary<string, PoolDate>();

        //池根物体
        private GameObject rootObj;

        //缓存池字典(不继承Mono的类)
        private Dictionary<string,BasePoolDataClass> poolClassDic = new Dictionary<string,BasePoolDataClass>();

        private PoolMgr() { }

        /// <summary>
        /// 获取对象（GameObj）
        /// 同步加载（从Resources中加载）
        /// </summary>
        /// <param name="name">资源名</param>
        public GameObject GetObj(string name)
        {
            if (isLayout && rootObj == null) rootObj = new GameObject("Pool");

            if (!poolDic.ContainsKey(name))
                //如果没有该种类型的数据池，则创建
                poolDic.Add(name, new PoolDate(name, rootObj));

            return poolDic[name].Pop();
        }
        /// <summary>
        /// 获取对象（GameObj）
        /// 异步加载（从AB包“pool”中加载）
        /// </summary>
        /// <param name="name">资源名</param>
        /// <param name="callBack">回调</param>
        public void GetObj(string name,UnityAction<GameObject> callBack)
        {
            if (isLayout && rootObj == null) rootObj = new GameObject("Pool");

            if (!poolDic.ContainsKey(name))
                //如果没有该种类型的数据池，则创建
                poolDic.Add(name, new PoolDate(name, rootObj));

             poolDic[name].Pop(callBack);

        }

        /// <summary>
        /// 存入对象（GameObj）
        /// </summary>
        /// <param name="obj">对象</param>
        public void PushObj(GameObject obj)
        {
            string name = obj.name;

            poolDic[name].Push(obj);
        }

        /// <summary>
        /// 清空所有缓存池（GameObj 和 非继承Mono）
        /// </summary>
        public void Clear()
        {
            poolDic.Clear();
            rootObj = null;
            poolClassDic.Clear();
        }

        /// <summary>
        /// 得到对象（非继承Mono）
        /// </summary>
        /// <typeparam name="T">数据或逻辑类的类型</typeparam>
        /// <param name="nameSpace">类型的命名空间</param>
        /// <param name="typeName">对于泛型类，需要用这个来进行区分字典的键</param>
        /// <returns></returns>
        public T GetPoolData<T>(string nameSpace = "",string typeName = "") where T : class,IPoolData,new()
        {
            string key = nameSpace + "_" + typeof(T).Name+"_"+typeName;
            
            T result;

            //如果字典中不存在  直接创建对应的空池 并加入字典
            if (!poolClassDic.ContainsKey(key))
                poolClassDic.Add(key, new PoolDataClass<T>());

            //获得对应的池
            PoolDataClass<T> poolDataClass = poolClassDic[key] as PoolDataClass<T>;  

            if (poolDataClass.leisureList.Count > 0)
            {//如果有空闲对象，直接取出
                result = poolDataClass.leisureList.Pop();
            }
            else
            {//如果没有空闲对象
                //实例化一个新对象
                result = new T();
            }
            
            return result;
        }

        /// <summary>
        /// 放入对象（非继承Mono）
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="data">对象</param>
        /// <param name="typeName">对于泛型类，需要用这个来进行区分字典的键</param>
        public void PushPoolData<T>(T data,string nameSpace="", string typeName = "") where T : class, IPoolData
        {
            string key = nameSpace+"_" + typeof(T).Name + "_" + typeName;

            if (poolClassDic.ContainsKey(key))
            {//如果字典中存在

                //重置对象数据
                data.ResetData();

                //存入
                (poolClassDic[key] as PoolDataClass<T>).leisureList.Push(data);
            }
        }

    }

    public class PoolDate
    {
        //空闲对象
        public Stack<GameObject> leisureList = new Stack<GameObject>();
        //正在使用中的对象
        public List<GameObject> usedList = new List<GameObject>();
        //数据根物体
        private GameObject rootObj;
        //游戏对象名
        private string name;
        //最大数量上限
        public int maxNum;

        public PoolDate(string name, GameObject root)
        {
            this.name = name;

            if (PoolMgr.isLayout)
            {//开启布局，挂载到根物体下
                rootObj = new GameObject(name);
                rootObj.transform.SetParent(root.transform);
            }

            //创建对象
            TextAsset text = Resources.Load<TextAsset>(name);

            if (text != null)
            {
                //获得上限数量
                maxNum = Int32.Parse(text.text);

            }
            else Debug.LogError("对象缺少PoolNum文本文件");
        }


        //同步加载（从Resources中加载）
        public GameObject Pop()
        {
            GameObject obj;
            if (leisureList.Count > 0)
            {//有空闲对象
                obj = leisureList.Pop();
            }
            else if (usedList.Count < maxNum)
            {//没有空闲对象，且没有过上限
             //创建新对象

                obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
                obj.name = name;
            }
            else
            {//超过上限

                //抢一个使用最久的对象
                obj = usedList[0];
                usedList.RemoveAt(0);
            }
            //放入使用列表
            usedList.Add(obj);

            obj.transform.SetParent(null);
            obj.SetActive(true);

            return obj;
        }

        //异步加载（从AB包“pool”中加载）
        public void Pop(UnityAction<GameObject> callBack)
        {
            GameObject obj;
            if (leisureList.Count > 0)
            {//有空闲对象
                obj = leisureList.Pop();
            }
            else if (usedList.Count < maxNum)
            {//没有空闲对象，且没有过上限
             //创建新对象

                ABMgr.Instance.LoadABRes<GameObject>("pool",name, (gameObj) =>
                {
                    obj = UnityEngine.Object.Instantiate(gameObj);
                    obj.name = name;
                    //放入使用列表
                    usedList.Add(obj);

                    obj.transform.SetParent(null);
                    obj.SetActive(true);

                    callBack.Invoke(obj);
                });

                return;
            }
            else
            {//超过上限

                //抢一个使用最久的对象
                obj = usedList[0];
                usedList.RemoveAt(0);
            }
            //放入使用列表
            usedList.Add(obj);

            obj.transform.SetParent(null);
            obj.SetActive(true);

            callBack.Invoke(obj);
        }

        public void Push(GameObject obj)
        {
            obj.SetActive(false);
            if(PoolMgr.isLayout)
                obj.transform.SetParent(rootObj.transform);

            leisureList.Push(obj);

            usedList.Remove(obj);

        }
    }
    /// <summary>
    /// 逻辑数据类的池基类
    /// </summary>
    public abstract class BasePoolDataClass { }

    /// <summary>
    /// 逻辑数据类的池
    /// </summary>
    public class PoolDataClass<T>: BasePoolDataClass where T : class,IPoolData
    {
        /// <summary>
        /// 空闲对象的栈
        /// </summary>
        public Stack<T> leisureList =  new Stack<T>();
    }
    /// <summary>
    /// 非继承Mono的对象接口
    /// </summary>
    public interface IPoolData
    {
        void ResetData();
    }

}
