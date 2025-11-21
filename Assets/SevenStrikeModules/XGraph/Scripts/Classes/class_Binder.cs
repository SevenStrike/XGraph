namespace SevenStrikeModules.XGraph
{
    using System;
    using UnityEngine;

    [Serializable]
    /// <summary>
    /// 绑定器 - 变量
    /// </summary>
    public class Binder_Varialble
    {
        [SerializeField] public string TargetPortName;
        [SerializeField] public string VariableNodeGuid;
        [SerializeReference] public Variable variable;

        public Binder_Varialble() { }

        public Binder_Varialble(string guid, string name, Variable variable)
        {
            this.VariableNodeGuid = guid;
            this.TargetPortName = name;
            this.variable = variable.Clone(false);
        }
    }
    [Serializable]
    /// <summary>
    /// 绑定器 - 属性
    /// </summary>
    public class Binder_Property
    {
        [SerializeField] public string Property_NodeName;
        [SerializeField] public string Property_GUID;
        [SerializeField] public string Property_PortName;
        [SerializeField] public string Property_PortType;
        [SerializeField] public string Action_PortName;
        [SerializeField] public string Action_PortType;

        public Binder_Property() { }

        public Binder_Property(string name, string guid, string property_port_name, string property_port_type, string action_port_name, string action_port_type)
        {
            this.Property_NodeName = name;
            this.Property_GUID = guid;
            this.Property_PortName = property_port_name;
            this.Property_PortType = property_port_type;
            this.Action_PortName = action_port_name;
            this.Action_PortType = action_port_type;
        }
    }
}