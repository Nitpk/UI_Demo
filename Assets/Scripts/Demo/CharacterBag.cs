/*
 * 作者：阳贻凡
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TFrameWork;

namespace Demo
{
    /// <summary>
    /// 角色背包
    /// </summary>
    public class CharacterBag
    {
        //背包容量
        public int MaxNum { get; private set; }
        public int CurrentNum=>characterList.Count;

        //角色列表
        private List<CharacterInfo> characterList;

        public CharacterBag(int bagMaxNum)
        {
            //初始化背包

            MaxNum = bagMaxNum;
            characterList = new List<CharacterInfo>(MaxNum);

            //获得角色信息并加入背包
            foreach (CharacterInfo cInfo in BinaryMgr.Instance.GetTable<CharacterInfoContainer>().dataDic.Values)
            {
                AddCharacter(cInfo);
            }
        }
        /// <summary>
        /// 得到角色背包
        /// </summary>
        public List<CharacterInfo> GetList(E_CharacterType type)
        {
            List<CharacterInfo> list = new List<CharacterInfo>();

            foreach (CharacterInfo cInfo in characterList)
            {
                if((int)type==0||(int)type==cInfo.type)
                    list.Add(cInfo); 
            }
            return list;
        }

        /// <summary>
        /// 往背包放入角色
        /// </summary>
        /// <param name="character"></param>
        public void AddCharacter(CharacterInfo character)
        {
            if (characterList.Count == MaxNum)
            {
                Debug.LogError("背包已满");
                return;
            }

            //有序放入列表
            bool flag = false;
            for (int i=characterList.Count;i>0 ;i--)
            {
                //根据品质和等级排序
                if (characterList[i - 1].quality > character.quality 
                    || characterList[i - 1].quality == character.quality && characterList[i - 1].level > character.level)
                {
                    //放在当前位置
                    if (i >= characterList.Count)
                        characterList.Add(character);
                    else characterList[i] = character;
                    
                    flag = true;
                    break;
                }

                //交换并考虑下一个位置
                if (i >= characterList.Count)
                    characterList.Add(characterList[i - 1]);
                else characterList[i] = characterList[i - 1];
            }

            if (!flag)
            {
                //如果还没有放入，放在最开头
                if(characterList.Count==0)characterList.Add(character);
                else characterList[0]=character;
            }
        }


        /// <summary>
        /// 增加背包容量
        /// </summary>
        /// <param name="delta">增量</param>
        public void AddBagNum(int delta)
        {
            //TODO 背包扩容
        }
    }
}

