using SevenStrikeModules.XGraph;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSampleGraph", menuName = "XGraphAsset-Sample")]
public class mc_GraphAsset : xAction_Asset
{
    /// <summary>
    /// Sample场景控制器
    /// 此脚本用于给行为节点提供调用机制
    /// </summary>
    public Module_Controller ModuleController;
}
