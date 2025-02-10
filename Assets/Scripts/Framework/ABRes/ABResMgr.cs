using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



namespace TFrameWork
{
    /// <summary>
    /// AB包资源加载器
    /// </summary>
    public class ABResMgr : AutoMonoMgr<ABResMgr>
    {
        //是否使用WebUnityRequest加载ab包
        private bool useWUR = false;
        //AB包是否附加哈希值
        private bool withHash = false;
        //哈希值和ab包名对应字典
        private Dictionary<string,string> abHashDic = new Dictionary<string,string>();
        //主包
        private AssetBundle mainAB = null;
        //主包依赖获取的配置文件
        private AssetBundleManifest manifest = null;

        //ab包资源字典
        private Dictionary<string,AssetBundle> abDic = new Dictionary<string,AssetBundle>();
        
        //AB包加载路径
        private string PathUrl
        {
            get
            {
                if (useWUR)
                    return "https://6e69-nitpk-3gdacahbe85b7526-1329319779.tcb.qcloud.la/webgl/StreamingAssets/WEBGL/";
                else
                    return Application.streamingAssetsPath + "/";
            }
        }
        //主包名
        private string MainABName
        {
            get
            {
#if UNITY_IOS
                return "IOS";
#elif UNITY_ANDROID
                return "Android";

#elif UNITY_WEBGL
                return "WEBGL";
#else
                return "PC";
#endif
            }
        }

        public enum E_UnloadResult
        {
            /// <summary>
            /// 成功卸载
            /// </summary>
            Success,
            /// <summary>
            /// 加载中，卸载失败
            /// </summary>
            Loading,
            /// <summary>
            /// 不存在，卸载失败
            /// </summary>
            Null,
        }

        /// <summary>
        /// 加载AB包
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="callBack">回调</param>
        /// <param name="isSync">是否同步加载</param>
        private IEnumerator LoadAB(string path, UnityAction<AssetBundle> callBack, bool isSync = true)
        {
            
            if(isSync)
                callBack.Invoke(AssetBundle.LoadFromFile(path));
            else
            {
                //异步加载
                AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(path);
                yield return req;

                if (req.assetBundle != null)
                {
                    callBack.Invoke(req.assetBundle);
                }
                else
                    Debug.LogError("AB包加载失败");

            }
            
        }


        /// <summary>
        /// 预先加载 加载主包和依赖包
        /// </summary>
        private IEnumerator LoadAhead(string abName,bool isSync)
        {
            //加载主包
            if(mainAB == null)
            {
                yield return LoadAB(PathUrl + MainABName, (ab) =>
                {
                    mainAB = ab;
                });
                manifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

                if (withHash)
                {
                    //如果ab包名附加了哈希值
                    foreach (string abNameWithHash in manifest.GetAllAssetBundles())
                    {
                        abHashDic.Add(abNameWithHash.Substring(0, abNameWithHash.IndexOf("_")), abNameWithHash);
                    }
                }
                

            }

            //加载依赖
            if(withHash)
                abName = abHashDic[abName];

            string[] d_abNames = manifest.GetAllDependencies(abName);
            for(int i = 0; i < d_abNames.Length; i++)
            {
                if (!abDic.ContainsKey(d_abNames[i]))
                {//如果字典中不存在

                    if (isSync)
                    {//同步加载
                        yield return LoadAB(PathUrl + d_abNames[i], (ab) =>
                        {
                            abDic.Add(d_abNames[i], ab);
                        });
                        
                    }
                    else
                    {//异步加载

                        //先添加一条记录
                        abDic.Add(d_abNames[i], null);
                        
                        //异步加载
                        yield return LoadAB(PathUrl + d_abNames[i], (ab) =>
                        {
                            //替换
                            abDic[d_abNames[i]] = ab;
                        }, false);

                    }
                }
                else
                {//字典中已经存在

                    //等待加载完成
                    while (abDic[d_abNames[i]] == null)
                    {
                        yield return null;
                    }
                }
            }
        }

        /// <summary>
        /// 加载AB包资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="abName">ab包名</param>
        /// <param name="resName">资源名</param>
        /// <param name="callBack">回调</param
        /// /// <param name="isSync">是否同步加载</param>
        public void LoadABRes<T>(string abName, string resName,UnityAction<T> callBack,bool isSync = false)where T : UnityEngine.Object
        {
            StartCoroutine(C_LoadABRes<T>(abName,resName,callBack,isSync,false,null,false));
        }

        /// <summary>
        /// 加载AB包资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="abName">ab包名</param>
        /// <param name="resName">资源名</param>
        /// <param name="callBack">回调</param
        /// /// <param name="isSync">是否同步加载</param>
        public void LoadABRes(string abName, string resName,Type type,UnityAction<UnityEngine.Object> callBack, bool isSync = false)
        {
            StartCoroutine(C_LoadABRes<UnityEngine.Object>(abName, resName, callBack, isSync,true,type,false));
        }

        /// <summary>
        /// 加载AB包资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="abName">ab包名</param>
        /// <param name="resName">资源名</param>
        /// <param name="callBack">回调</param
        /// /// <param name="isSync">是否同步加载</param>
        public void LoadABRes(string abName, string resName, UnityAction<UnityEngine.Object> callBack, bool isSync = false)
        {
            StartCoroutine(C_LoadABRes<UnityEngine.Object>(abName, resName, callBack, isSync, false, null, true));
        }

        private IEnumerator C_LoadABRes<T>(string abName, string resName, UnityAction<T> callBack, bool isSync, bool typeMode, Type type, bool nameMode) where T : UnityEngine.Object
        {
            //预先加载
            yield return StartCoroutine( LoadAhead(abName, isSync));


            if(withHash)
                abName = abHashDic[abName];


            //加载目标包
            if (!abDic.ContainsKey(abName))
            {
                if (isSync)
                {//同步加载
                    yield return LoadAB(PathUrl + abName, (ab) =>
                    {
                        if (ab != null)
                            abDic.Add(abName, ab);
                        else
                            Debug.Log("没有找到AB包[" + abName + "]");
                    }); 
                }
                else
                {//异步加载
                    //先添加一条记录
                    abDic.Add(abName, null);
                    
                    //异步加载
                    yield return LoadAB(PathUrl + abName, (ab) =>
                    {
                        //替换
                        abDic[abName] = ab;

                    }, false);

                }
            }
            else
            {
                //如果已经存在

                //等待加载
                while (abDic[abName] == null)
                {
                    yield return null;
                }

            }

            //加载ab包中的资源
            if (isSync)
            {
                //同步加载
                T res;

                if (typeMode)
                {
                    res = abDic[abName].LoadAsset(resName, type) as T;
                }
                else if (nameMode)
                {
                    res = abDic[abName].LoadAsset(resName) as T;
                }
                else
                {
                    res = abDic[abName].LoadAsset<T>(resName);
                }

                callBack?.Invoke(res);
            }
            else
            {
                //异步加载

                AssetBundleRequest req;

                if (typeMode)
                {
                    req = abDic[abName].LoadAssetAsync(resName, type);
                }
                else if (nameMode)
                {
                    req = abDic[abName].LoadAssetAsync(resName);
                }
                else
                {
                    req = abDic[abName].LoadAssetAsync<T>(resName);
                }

                yield return req;
                //加载完成

                callBack?.Invoke(req.asset as T);
            }
        }


        /// <summary>
        /// 卸载AB包
        /// </summary>
        /// <param name="abName">包名</param>
        /// <param name="callBackResult">卸载结果回调</param>
        /// <param name="isDeleteRes">在删除ab包时，是否删除游戏内加载好的ab包的资源</param>
        public void UnloadABRes(string abName,UnityAction<E_UnloadResult> callBackResult = null,bool isDeleteRes = false)
        {
            if(withHash)
                abName = abHashDic[abName];


            if (abDic.ContainsKey(abName))
            {
                if (abDic[abName] == null)
                {
                    //还在加载中，卸载失败
                    callBackResult?.Invoke(E_UnloadResult.Loading);
                }
                else
                {
                    abDic[abName].Unload(isDeleteRes);
                    abDic.Remove(abName);
                    //卸载成功
                    callBackResult?.Invoke(E_UnloadResult.Success);
                }
            }
            else
            {
                //不存在，卸载失败
                callBackResult?.Invoke(E_UnloadResult.Null);
            }
        }

        /// <summary>
        /// 清空所有ab包
        /// </summary>
        /// <param name="isDeleteRes">在删除ab包时，是否删除游戏内加载好的ab包的资源</param>
        public void ClearAB(bool isDeleteRes = false)
        {
            //停止所有ab包加载
            StopAllCoroutines();
            //清空所有ab包
            AssetBundle.UnloadAllAssetBundles(isDeleteRes);
            abDic.Clear();
            //卸载主包
            mainAB = null;
            manifest = null;
        }

        /// <summary>
        /// 用AB包加载lua脚本（同步加载）
        /// </summary>
        /// <param name="abName">lua包名</param>
        /// <param name="luaName">lua脚本名（不带后缀）</param>
        /// <returns>字节数组</returns>
        public byte[] LoadLuaAB(string abName,string luaName){
            byte[] ret = null;

            //加载主包和依赖
            LoadAhead(abName,true);

            if (withHash)
                abName = abHashDic[abName];

            if (!abDic.ContainsKey(abName))
            {
                //还没加载lua包
                //则同步加载
                AssetBundle ab = AssetBundle.LoadFromFile(PathUrl+abName);
                if(ab != null)
                    abDic.Add(abName,ab);
                else{
                    Debug.Log("没有找到AB包["+abName+"]");
                    return ret;
                }     
            }

            //同步加载需要的lua脚本           
            TextAsset textAsset = abDic[abName].LoadAsset<TextAsset>(luaName+".lua");
            if (textAsset == null)
            {
                Debug.Log("["+luaName+"]lua加载失败");
                return ret;
            }
            ret = textAsset.bytes;
            
            return ret;
        }
    }
}
