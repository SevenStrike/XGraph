using UnityEngine;

public class ModuleController : MonoBehaviour
{
    [SerializeField] public Transform ModulesRoot;
    [SerializeField] public ModuleEffective[] ModulesEffective;
    [SerializeField] private float smoothValue = 0;
    [SerializeField] private bool smoothEnabled = false;
    [SerializeField] public float smoothSpeed = 0.05f;
    [SerializeField] public float LightIntensity = 0;
    [SerializeField] public Light MainLight;

    void Start()
    {
        Modules_Initialize();
    }

    void Update()
    {
        SmoothValue();
        MainLight_Update();
    }

    private void Modules_Initialize()
    {
        ModulesEffective = new ModuleEffective[ModulesRoot.childCount];
        for (int i = 0; i < ModulesRoot.childCount; i++)
        {
            ModulesEffective[i] = ModulesRoot.GetChild(i).GetComponent<ModuleEffective>();
        }

        LightIntensity = MainLight.intensity;
        MainLight_Control(false);
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
            }
        }
    }

    /// <summary>
    /// 模组激活 - 改变全部状态
    /// </summary>
    /// <param name="state"></param>
    public void Modules_Active(bool state)
    {
        for (int i = 0; i < ModulesEffective.Length; i++)
        {
            ModulesEffective[i].ActiveModule(state, true);
        }
    }

    /// <summary>
    /// 场景主灯光开关
    /// </summary>
    /// <param name="state"></param>
    public void MainLight_Control(bool state)
    {
        smoothEnabled = state;
    }

    /// <summary>
    /// 灯光强度刷新
    /// </summary>
    private void MainLight_Update()
    {
        MainLight.intensity = LightIntensity * smoothValue;
    }

    /// <summary>
    /// 平滑数值因子
    /// </summary>
    private void SmoothValue()
    {
        if (smoothEnabled)
        {
            smoothValue = Mathf.Lerp(smoothValue, 1, smoothSpeed);
        }
        else
        {
            smoothValue = Mathf.Lerp(smoothValue, 0, smoothSpeed);
        }
    }
}
