using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TFrameWork
{
#if UNITY_EDITOR
    /// <summary>
    /// 编辑器资源管理器
    /// </summary>
    public class EditorResMgr : BaseMgr<EditorResMgr>
    {
        //资源路径
        private string resPath = "Assets/Editor/";
        private EditorResMgr() { }

        /// <summary>
        /// 加载单个资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="path">资源文件路径（去掉Assets/Editor/）</param>
        /// <returns></returns>
        public T LoadEditorRes<T>(string path) where T : Object
        {
            string suffixName = "";
            //确定后缀名
            if (typeof(T) == typeof(GameObject))
                suffixName = ".prefab";
            else if (typeof(T) == typeof(Material))
                suffixName = ".mat";
            else if (typeof(T) == typeof(Texture)||typeof(T) == typeof(Sprite))
                suffixName = ".png";
            else if (typeof(T) == typeof(AudioClip))
                suffixName = ".mp3";
            //加载资源
            T res = AssetDatabase.LoadAssetAtPath<T>(resPath+path+suffixName);

            return res; 
        }
        /// <summary>
        /// 获得图集中的某一张图
        /// </summary>
        /// <param name="path">图集路径</param>
        /// <param name="spriteName">图名</param>
        /// <returns></returns>
        public Sprite LoadSprite(string path,string spriteName)
        {
            Object[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(resPath + path);

            foreach (var sprite in sprites)
            {
                if(sprite.name == spriteName)
                    return sprite as Sprite;
            }
            return null;
        }

        /// <summary>
        /// 获得图集字典
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Dictionary<string,Sprite> LoadAllSprites(string path)
        {
            Dictionary<string, Sprite> spritesDic = new Dictionary<string, Sprite>();
            Object[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(resPath + path);

            foreach (var sprite in sprites)
            {
                spritesDic.Add(sprite.name, sprite as Sprite);
            }

            return spritesDic;
        }
    }
#endif
}
