using UnityEngine;

public class Sample_Module_Controller : MonoBehaviour
{
    public Transform ModulesRoot;
    public GameObject[] Modules;
    public string CurrentActiveModule;

    void Start()
    {
        modules_initialize();
        modules_active(false);
    }

    private void modules_initialize()
    {
        Modules = new GameObject[ModulesRoot.childCount];
        for (int i = 0; i < ModulesRoot.childCount; i++)
        {
            Modules[i] = ModulesRoot.GetChild(i).gameObject;
        }
    }

    /// <summary>
    /// 模组激活 - 改变名称为 name 的模组的状态为 state
    /// </summary>
    /// <param name="name"></param>
    /// <param name="state"></param>
    public void module_active(string name, bool state)
    {
        for (int i = 0; i < Modules.Length; i++)
        {
            if (Modules[i].name == name)
            {
                if (state)
                    CurrentActiveModule = name;
                Modules[i].SetActive(state);
            }
        }
    }

    /// <summary>
    /// 模组激活 - 改变全部状态
    /// </summary>
    /// <param name="state"></param>
    public void modules_active(bool state)
    {
        if (!state)
            return;
        for (int i = 0; i < Modules.Length; i++)
        {
            Modules[i].SetActive(state);
        }
        CurrentActiveModule = null;
    }


}
