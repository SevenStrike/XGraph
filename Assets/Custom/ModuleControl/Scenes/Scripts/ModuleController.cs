using System;
using UnityEngine;

[Serializable]
public class SmoothParams
{
    [SerializeField] public Light light;
    [SerializeField] public float smoothValue;
    [SerializeField] public float value;
    [SerializeField] public bool state;
    [SerializeField] public float speed;
}

public class ModuleController : MonoBehaviour
{
    [SerializeField] public Camera cam;
    [SerializeField] public Transform ModulesRoot;
    [SerializeField] public ModuleEffective[] ModulesEffective;

    public SmoothParams sp_MainLight;
    public SmoothParams sp_AnchorLight;

    void Start()
    {
        Modules_Initialize();
    }

    void Update()
    {
        SmoothValue();
        UpdateLightItensity();
    }

    private void Modules_Initialize()
    {
        // 收集模组
        ModulesEffective = new ModuleEffective[ModulesRoot.childCount];
        for (int i = 0; i < ModulesRoot.childCount; i++)
        {
            ModulesEffective[i] = ModulesRoot.GetChild(i).GetComponent<ModuleEffective>();
        }

        // 记录初始灯光强度
        sp_MainLight.value = sp_MainLight.light.intensity;
        sp_AnchorLight.value = sp_AnchorLight.light.intensity;

        // 关闭灯光
        MainLight_Control(false);
        AnchorLight_Control(false);
    }

    /// <summary>
    /// 模组激活 - 改变名称为 name 的模组的状态为 state
    /// </summary>
    /// <param name="name"></param>
    /// <param name="state"></param>
    public void Module_Active(string name, bool state)
    {
        for (int i = 0; i < ModulesEffective.Length; i++)
        {
            if (ModulesEffective[i].name == name)
            {
                ModulesEffective[i].ActiveModule(state);
                sp_AnchorLight.light.transform.position = ModulesEffective[i].anchor.position;
                AnchorLight_Control(state ? true : false);
                if (state)
                    AnchorLight_ItensityRelight();
            }
        }
    }

    /// <summary>
    /// 模组激活 - 改变全部状态
    /// </summary>
    /// <param name="state"></param>
    public void Modules_Active(bool state, bool fastmode = false)
    {
        for (int i = 0; i < ModulesEffective.Length; i++)
        {
            ModulesEffective[i].ActiveModule(state, fastmode);
            if (!state)
                AnchorLight_Control(false);
        }
    }

    /// <summary>
    /// 场景主灯光开关
    /// </summary>
    /// <param name="state"></param>
    public void MainLight_Control(bool state)
    {
        sp_MainLight.state = state;
    }

    /// <summary>
    /// 场景主灯光开关
    /// </summary>
    /// <param name="state"></param>
    public void AnchorLight_Control(bool state)
    {
        sp_AnchorLight.state = state;
    }

    /// <summary>
    /// 场景锚点灯光亮度刷新
    /// </summary>
    /// <param name="state"></param>
    public void AnchorLight_ItensityRelight()
    {
        sp_AnchorLight.smoothValue = 0;
    }

    /// <summary>
    /// 灯光强度刷新
    /// </summary>
    private void UpdateLightItensity()
    {
        // 主灯光强度更新
        sp_MainLight.light.intensity = sp_MainLight.smoothValue * sp_MainLight.value;
        // 锚点灯光强度更新
        sp_AnchorLight.light.intensity = sp_AnchorLight.smoothValue * sp_AnchorLight.value;
    }

    /// <summary>
    /// 平滑数值因子
    /// </summary>
    private void SmoothValue()
    {
        #region 平滑主灯光平滑值
        if (sp_MainLight.state)
        {
            if (sp_MainLight.smoothValue >= 0.9999f)
                sp_MainLight.smoothValue = 1;
            else
                sp_MainLight.smoothValue = Mathf.Lerp(sp_MainLight.smoothValue, 1, sp_MainLight.speed);
        }
        else
        {
            if (sp_MainLight.smoothValue <= 0.0001f)
                sp_MainLight.smoothValue = 0;
            else
                sp_MainLight.smoothValue = Mathf.Lerp(sp_MainLight.smoothValue, 0, sp_MainLight.speed);
        }
        #endregion

        #region 平滑锚点灯光平滑值
        if (sp_AnchorLight.state)
        {
            if (sp_AnchorLight.smoothValue >= 0.9999f)
                sp_AnchorLight.smoothValue = 1;
            else
                sp_AnchorLight.smoothValue = Mathf.Lerp(sp_AnchorLight.smoothValue, 1, sp_AnchorLight.speed);
        }
        else
        {
            if (sp_AnchorLight.smoothValue <= 0.0001f)
                sp_AnchorLight.smoothValue = 0;
            else
                sp_AnchorLight.smoothValue = Mathf.Lerp(sp_AnchorLight.smoothValue, 0, sp_AnchorLight.speed);
        }
        #endregion
    }
}
