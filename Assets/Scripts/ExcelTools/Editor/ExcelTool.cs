/*
 * 作者：阳贻凡
 * 描述：
 * 一个简单的工具类
 * 将excel表中的数据自动生成对应的数据类和2进制数据文件
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ExcelDataReader;
using System.IO;
using System.Data;
using System;
using System.Text;

public class ExcelTool
{   
    //Excel存放目录
    private static string EXCEL_PATH=Application.dataPath+"/Excel/";
    //数据类存放目录
    private static string DATA_PATH=Application.dataPath+"/Scripts/DataScripts/Data/";
    //容器类存放目录
    private static string CONTAINER_PATH=Application.dataPath+"/Scripts/DataScripts/DataContainer/";
    //2进制存放目录
    private static string BINARY_PATH=Application.streamingAssetsPath+"/BinaryData/";
    //变量名行号
    private static int variableNamesRowIndex=0;
    //变量类型行号
    private static int variableTypesRowIndex=1;
    //主键行号
    private static int keyRowIndex=2;
    //数据内容开始行号
    private static int dataRowStartIndex=4;

    [MenuItem("GameTools/DataTool/ExcelToBinary")]
    public static void ToBinary(){

        //检测路径文件夹是否存在，不存在自动创建
        if(!Directory.Exists(EXCEL_PATH)){
            Debug.Log($"[ExcelToJson]：不存在目录{EXCEL_PATH},自动创建,终止生成");
            Directory.CreateDirectory(EXCEL_PATH);
        }
        if(!Directory.Exists(DATA_PATH)){
            Debug.Log($"[ExcelToJson]：不存在目录{DATA_PATH},自动创建");
            Directory.CreateDirectory(DATA_PATH);
        }
        if(!Directory.Exists(CONTAINER_PATH)){
            Debug.Log($"[ExcelToJson]：不存在目录{CONTAINER_PATH},自动创建");
            Directory.CreateDirectory(CONTAINER_PATH);
        }
        if(!Directory.Exists(BINARY_PATH)){
            Debug.Log($"[ExcelToJson]：不存在目录{BINARY_PATH},自动创建");
            Directory.CreateDirectory(BINARY_PATH);
        }


        DirectoryInfo dirInfo=new DirectoryInfo(EXCEL_PATH);
        FileInfo[] fileInfos=dirInfo.GetFiles();

        IExcelDataReader reader;
        DataSet dataSet;
        DataTableCollection tableCollection;

        //遍历Excel目录下的所有文件
        for(int i=0;i<fileInfos.Length;i++){
            //如果不是excel文件 则跳过
            if(fileInfos[i].Extension != ".xlsx" && fileInfos[i].Extension != ".xls") continue;

            //得到excel中所有表的数据
            using(reader = ExcelReaderFactory.CreateReader(fileInfos[i].Open(FileMode.Open,FileAccess.Read))){
                dataSet = reader.AsDataSet();
                tableCollection=dataSet.Tables;
                dataSet.Dispose();
            }   
            
            //遍历每一张表
            foreach(DataTable table in tableCollection){             
                //生成数据类
                GenerateData(table);            
                //生成容器类
                GenerateDataContainer(table);
                //生成2进制文件
                GenerateBinary(table);

                AssetDatabase.Refresh();
            }
        }
    }

    /// <summary>
    /// 生成数据类
    /// </summary>
    private static void GenerateData(DataTable dataTable){

        string path=DATA_PATH+dataTable.TableName+".cs";
        if(!File.Exists(path))
            File.Create(path).Dispose();

        File.WriteAllText(path,$"public class {dataTable.TableName}{{\n");
        
        for(int i=0;i<dataTable.Columns.Count;i++){
            File.AppendAllText(path,
            $"  public {dataTable.Rows[variableTypesRowIndex][i]} {dataTable.Rows[variableNamesRowIndex][i]};\n");
        }

        File.AppendAllText(path,"}");

    }
    /// <summary>
    /// 生成容器类
    /// </summary>
    private static void GenerateDataContainer(DataTable dataTable){
        //找主键所在的列
        int keyPos=-1;
        for(int j=0;j<dataTable.Columns.Count;j++){
            if(dataTable.Rows[keyRowIndex][j].ToString()=="key"){
                keyPos=j;
                break;
            }
        }
        if(keyPos==-1){//没有找到主键
            Debug.Log($"[ExcelToJson]：在{dataTable.TableName}中没有找到主键，终止生成容器类");
            return;
        }

        string path=CONTAINER_PATH+dataTable.TableName+"Container.cs";
        if(!File.Exists(path))
            File.Create(path).Dispose();

        File.WriteAllText(path,"using System.Collections.Generic;\n");

        File.AppendAllText(path,$"public class {dataTable.TableName}Container{{\n");

        File.AppendAllText(path,
        $"  public Dictionary<{dataTable.Rows[variableTypesRowIndex][keyPos]},{dataTable.TableName}> dataDic;\n");
        //+$"=new Dictionary<{dataTable.Rows[variableTypesRowIndex][keyPos]},{dataTable.TableName}>();\n");
        
        File.AppendAllText(path,"}");
    }

    /// <summary>
    /// 生成2进制文件
    /// </summary>
    /// <param name="dataTable"></param>
    private static void GenerateBinary(DataTable dataTable){
        //找主键所在的列
        int keyPos=-1;
        for(int j=0;j<dataTable.Columns.Count;j++){
            if(dataTable.Rows[keyRowIndex][j].ToString()=="key"){
                keyPos=j;
                break;
            }
        }
        if(keyPos==-1){//没有找到主键
            Debug.Log($"[ExcelToJson]：在{dataTable.TableName}中没有找到主键，终止生成2进制文件");
            return;
        }

        string path=BINARY_PATH+dataTable.TableName+".bd";

        using(FileStream fs=File.Create(path)){
            //存储数据的总行数
            fs.Write(BitConverter.GetBytes(dataTable.Rows.Count-dataRowStartIndex),0,4);

            //存储主键变量名
            byte[] bytes=Encoding.UTF8.GetBytes(dataTable.Rows[variableNamesRowIndex][keyPos].ToString());
            fs.Write(BitConverter.GetBytes(bytes.Length),0,4);
            fs.Write(bytes,0,bytes.Length);

            //存储每行数据
            DataRow row;
            for(int i=dataRowStartIndex;i<dataTable.Rows.Count;i++){

                row=dataTable.Rows[i];
                //存储该行内容
                for(int j=0;j<dataTable.Columns.Count;j++){
                    
                    switch(dataTable.Rows[variableTypesRowIndex][j]){
                        case "string":
                            bytes=Encoding.UTF8.GetBytes(row[j].ToString());
                            fs.Write(BitConverter.GetBytes(bytes.Length),0,4);
                            fs.Write(bytes,0,bytes.Length);
                            break;
                        case "int":
                            fs.Write(BitConverter.GetBytes(int.Parse(row[j].ToString()) ),0,4);
                            break;
                        case "float":
                            fs.Write(BitConverter.GetBytes(float.Parse(row[j].ToString()) ),0,4);
                            break;
                        case "bool":
                            fs.Write(BitConverter.GetBytes(bool.Parse(row[j].ToString()) ),0,1);
                            break;
                    }
                }

            }
            fs.Flush();
        }
    }
}
