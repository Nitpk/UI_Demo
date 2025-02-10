/*
 * 作者：阳贻凡
 * 描述：
 * 将2进制数据读取出来，生成对应的数据类对象
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

/// <summary>
/// 2进制数据管理器
/// </summary>
public class BinaryMgr
{
    #region 单例
    private static BinaryMgr instance=new BinaryMgr();
    public static BinaryMgr Instance=>instance;
    //存储表
    private Dictionary<string,object> tableDic=new Dictionary<string, object>();
    //2进制文件路径
    private string BINARY_PATH=Application.streamingAssetsPath+"/BinaryData/";
    //字节每次最大读取长度
    private int MAX_BYTES_LEN=256;
    //密钥
    private byte key=233;
    //是否加密
    private bool isEncrypt=true;
    private BinaryMgr(){
        Init();
    }
    #endregion

    /// <summary>
    /// 初始化数据
    /// </summary>
    private void Init(){
        LoadTable<CharacterInfo, CharacterInfoContainer>();
    }
    
    /// <summary>
    /// 加载表
    /// </summary>
    public void LoadTable<Tdata,Tcontainer>(){
        Type containerType=typeof(Tcontainer);
        //判断表是否已经加载，有则退出
        if(tableDic.ContainsKey(containerType.Name))return;

        //加载表
        Type dataType=typeof(Tdata);
        Tcontainer container=Activator.CreateInstance<Tcontainer>();
        Tdata data;

        byte[] bytes=new byte[MAX_BYTES_LEN];
        string keyName;
        FieldInfo[] dataFieldInfos=dataType.GetFields();
        FieldInfo dataDicFieldInfo=containerType.GetField("dataDic");
        int intValue;
        float floatValue;
        bool boolValue;
        string strValue;

        using(FileStream fs=new FileStream(BINARY_PATH+dataType.Name+".bd",FileMode.Open,FileAccess.Read)){
            //得到行数
            fs.Read(bytes,0,4);
            int rowNum=BitConverter.ToInt32(bytes,0);

            //得到主键变量名
            fs.Read(bytes,0,4);
            intValue=BitConverter.ToInt32(bytes,0);
            fs.Read(bytes,0,intValue);
            keyName=Encoding.UTF8.GetString(bytes,0,intValue);

            //创建容器类字典
            IDictionary dic=Activator.CreateInstance(dataDicFieldInfo.FieldType)as IDictionary;
            //字典的键
            object keyValue= Activator.CreateInstance(dataDicFieldInfo.FieldType.GetGenericArguments()[0]);

            //按行读取
            for(int i=0;i<rowNum;i++){
                //创建一个数据对象
                data=Activator.CreateInstance<Tdata>();
                
                //遍历该数据类的所有字段，并赋值
                for(int j=0;j<dataFieldInfos.Length;j++){
                    if(dataFieldInfos[j].FieldType==typeof(int)){

                        fs.Read(bytes,0,4);
                        intValue=BitConverter.ToInt32(bytes,0);
                        dataFieldInfos[j].SetValue(data,intValue);

                        if(dataFieldInfos[j].Name==keyName)
                            keyValue=intValue;

                    }else if(dataFieldInfos[j].FieldType==typeof(float)){

                        fs.Read(bytes,0,4);
                        floatValue=BitConverter.ToSingle(bytes,0);
                        dataFieldInfos[j].SetValue(data,floatValue);

                        if(dataFieldInfos[j].Name==keyName)
                            keyValue=floatValue;

                    }else if(dataFieldInfos[j].FieldType==typeof(bool)){

                        fs.Read(bytes,0,1);
                        boolValue=BitConverter.ToBoolean(bytes,0);
                        dataFieldInfos[j].SetValue(data,boolValue);

                        if(dataFieldInfos[j].Name==keyName)
                            keyValue=boolValue;

                    }else if(dataFieldInfos[j].FieldType==typeof(string)){

                        fs.Read(bytes,0,4);
                        intValue=BitConverter.ToInt32(bytes,0);
                        fs.Read(bytes,0,intValue);
                        strValue=Encoding.UTF8.GetString(bytes,0,intValue);
                        dataFieldInfos[j].SetValue(data,strValue);

                        if(dataFieldInfos[j].Name==keyName)
                            keyValue=strValue;

                    }else{
                        Debug.Log("[BinaryMgr]:数据类有暂不支持的字段类型");
                    }
                }

                //加入容器的字典中
                dic.Add(keyValue,data);
            }

            //将字典加入容器类对象中
            dataDicFieldInfo.SetValue(container,dic);

            //将容器类对象加入表字典中
            tableDic.Add(containerType.Name,container);

        }
    }

    /// <summary>
    /// 得到表
    /// </summary>
    public Tcontainer GetTable<Tcontainer>() where Tcontainer:class{
        Type containerType=typeof(Tcontainer);

        if(!tableDic.ContainsKey(containerType.Name)){
            Debug.Log("[BinaryMgr]:该表还没有被加载");
            return null;
        }
        
        return tableDic[containerType.Name] as Tcontainer;
    }
    /// <summary>
    /// 存储数据类对象
    /// </summary>
    /// <param name="dataObj">数据类对象(需要System.Serializable特性)</param>
    /// <param name="fileName">文件名（不含后缀）</param>
    public void Save(object dataObj,string fileName){
        string path=Application.persistentDataPath+"/BinaryData/"+fileName+".bd";
        //如果不存在该目录，则创建
        if(!Directory.Exists(Application.persistentDataPath+"/BinaryData/"))
            Directory.CreateDirectory(Application.persistentDataPath+"/BinaryData/");


        if(isEncrypt){
            using(MemoryStream ms=new MemoryStream()){
                BinaryFormatter binaryFormatter=new BinaryFormatter();
                binaryFormatter.Serialize(ms,dataObj);

                byte[] bytes=ms.GetBuffer();
                for(int i=0;i<bytes.Length;i++){
                    bytes[i]^=key;
                }
                File.WriteAllBytes(path,bytes);
            }
        }else{
            using(FileStream fs=new FileStream(path,FileMode.OpenOrCreate,FileAccess.Write)){
                BinaryFormatter binaryFormatter=new BinaryFormatter();
                binaryFormatter.Serialize(fs,dataObj);
                fs.Flush();
            }
        }
        

    }
    /// <summary>
    /// 加载数据类对象
    /// </summary>
    /// <param name="fileName">文件名（不含后缀）</param>
    public T Load<T>(string fileName)where T:class{
        T result=default;
        string path=Application.persistentDataPath+"/BinaryData/"+fileName+".bd";

        if(isEncrypt){
            byte[] bytes=File.ReadAllBytes(path);
            for(int i=0;i<bytes.Length;i++){
                bytes[i]^=key;
            }
            using(MemoryStream ms=new MemoryStream(bytes)){
                BinaryFormatter binaryFormatter=new BinaryFormatter();
                result = binaryFormatter.Deserialize(ms)as T;
            }
        }else{
            using(FileStream fs=new FileStream(path,FileMode.Open,FileAccess.Read)){
                BinaryFormatter binaryFormatter=new BinaryFormatter();
                result = binaryFormatter.Deserialize(fs) as T;
            }
        }

        return result;
    }
}
