using SevenStrikeModules.XGraph;
using UnityEngine;

[CreateAssetMenu(fileName = "MC-Graph", menuName = "XGraphAsset-ModuleCotroller")]
public class mc_GraphAsset : xAction_Asset
{
    /// <summary>
    /// MC场景控制脚本（自定义目标脚本）
    /// 此脚本用于给行为节点提供调用通道
    /// </summary>
    public ModuleController ModuleController;
}
