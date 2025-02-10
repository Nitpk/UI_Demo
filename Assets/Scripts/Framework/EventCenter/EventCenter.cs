/*
 * 作者：阳贻凡
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TFrameWork {
    /// <summary>
    /// 事件管理中心
    /// </summary>
    public class EventCenter : BaseMgr<EventCenter>
    {
        private Dictionary<E_EventType, EventContentBase> eventDic = new Dictionary<E_EventType, EventContentBase>();
    
        private EventCenter() { }

        /// <summary>
        /// 触发事件 无参数
        /// </summary>
        public void TriggerEvent(E_EventType e_EventType)
        {
            if (eventDic.ContainsKey(e_EventType))
            {//如果有该事件，则触发
                (eventDic[e_EventType] as EventContent).actions?.Invoke();
            }
        }

        /// <summary>
        /// 触发事件 带参数
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        public void TriggerEvent<T>(E_EventType e_EventType,T arg)
        {
            if (eventDic.ContainsKey(e_EventType))
            {//如果有该事件，则触发
                (eventDic[e_EventType] as EventContent<T>).actions?.Invoke(arg);
            }
        }
        /// <summary>
        /// 添加事件监听 无参数
        /// </summary>
        public void AddEventListener(E_EventType e_EventType,UnityAction action)
        {
            if (eventDic.ContainsKey(e_EventType))
            {
                (eventDic[e_EventType] as EventContent).actions += action;
            }
            else
            {
                eventDic.Add(e_EventType, new EventContent(action));
            }
        }

        /// <summary>
        /// 添加事件监听 带参数
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        public void AddEventListener<T>(E_EventType e_EventType, UnityAction<T> action)
        {
            if (eventDic.ContainsKey(e_EventType))
            {
                (eventDic[e_EventType] as EventContent<T>).actions += action;
            }
            else
            {
                eventDic.Add(e_EventType, new EventContent<T>(action));
            }
        }
        /// <summary>
        /// 移除监听 无参数
        /// </summary>
        public void RemoveEventListener(E_EventType e_EventType,UnityAction action)
        {
            if (eventDic.ContainsKey(e_EventType))
            {
                (eventDic[e_EventType] as EventContent).actions -= action;
            }
        }
        /// <summary>
        /// 移除监听 带参数
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        public void RemoveEventListener<T>(E_EventType e_EventType, UnityAction<T> action)
        {
            if (eventDic.ContainsKey(e_EventType))
            {
                (eventDic[e_EventType] as EventContent<T>).actions -= action;
            }
        }
        /// <summary>
        /// 清空事件监听
        /// </summary>
        public void Clear()
        {
            eventDic.Clear();
        }
        /// <summary>
        /// 清空某个事件的所有监听
        /// </summary>
        public void Clear(E_EventType e_EventType)
        {
            eventDic.Remove(e_EventType);
        }
    }
    /// <summary>
    /// 事件内容基类
    /// </summary>
    public abstract class EventContentBase {}

    /// <summary>
    /// 带参数的委托
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    public class EventContent<T> : EventContentBase
    {
        public UnityAction<T> actions;
        public EventContent(UnityAction<T> action)
        {
            actions += action;
        }
    }

    /// <summary>
    /// 无参数的委托
    /// </summary>
    public class EventContent : EventContentBase
    {
        public UnityAction actions;
        public EventContent(UnityAction action)
        {
            actions += action;
        }
    }
}
