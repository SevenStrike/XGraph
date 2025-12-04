namespace SevenStrikeModules.XGraph
{
    using System.Collections.Generic;
    using UnityEngine;

    public class xAction_Property : xAction_Base
    {
        [SerializeReference] public List<Variable> PropertyVariables = new List<Variable>();

        /// <summary>
        /// 初始化属性变量列表
        /// </summary>
        public virtual void Propertys_Initialize()
        {

        }

        /// <summary>
        /// 行为节点执行方法
        /// </summary>
        public override void Execute()
        {

        }

        /// <summary>
        /// 添加一个属性变量
        /// </summary>
        /// <param name="vare"></param>
        public void Propertys_Add(Variable vare)
        {
            PropertyVariables.Add(vare);
        }

        /// <summary>
        /// 获取属性变量
        /// </summary>
        /// <param name="name"></param>
        public Variable Propertys_Get(string name)
        {
            Variable vare = null;
            for (int i = 0; i < PropertyVariables.Count; i++)
            {
                if (PropertyVariables[i].name == name)
                {
                    vare = PropertyVariables[i];
                    break;
                }
            }
            return vare;
        }

        /// <summary>
        /// 更新属性
        /// </summary>
        public virtual void Propertys_Update()
        {

        }

        /// <summary>
        /// 克隆该节点时确保改脚本独立变量正确克隆
        /// </summary>
        /// <returns></returns>
        public override xAction_Base Clone()
        {
            // 调用基类的Clone方法
            xAction_Property clone = base.Clone() as xAction_Property;

            // 复制派生类特有的字段
            if (clone != null)
            {
                clone.PropertyVariables = this.PropertyVariables;
            }

            return clone;
        }
    }
}