using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class modulescale
{
    public Transform block;
    public Vector3 scale;
}

public class ModuleEffective : MonoBehaviour
{
    [SerializeField] public Transform blocksRoot;
    [SerializeField] public modulescale[] blocks;
    [SerializeField] public bool module_activate = true;
    [SerializeField] public float smoothValue = 1;
    [SerializeField] public float smoothSpeed = 0.05f;
    [SerializeField] public LineRenderer line;
    [SerializeField] public Transform anchor;
    [SerializeField] public float alpha = 0f;
    [SerializeField] public Material BlockMaterial;

    void Start()
    {
        InitializeBlocks();
        ActiveModule(false, true);

    }

    void Update()
    {
        SmoothValue();
        SetBlocksScale(smoothValue);
    }

    private void SetBlocksEmissive(float value)
    {
        BlockMaterial.SetFloat("_EmissiveHeight", value);
    }

    /// <summary>
    /// 初始化块类
    /// </summary>
    private void InitializeBlocks()
    {
        if (blocksRoot != null)
            return;

        // 初始化 Bocks
        blocksRoot = transform.Find("Blocks");
        blocks = new modulescale[blocksRoot.childCount];
        for (int i = 0; i < blocks.Length; i++)
        {
            modulescale ms = blocks[i] = new modulescale();
            ms.block = blocksRoot.GetChild(i);
            if (i == 0)
                BlockMaterial = new Material(ms.block.GetComponent<MeshRenderer>().material);
            ms.block.GetComponent<MeshRenderer>().sharedMaterial = BlockMaterial;
            ms.scale = ms.block.localScale;
        }

        // 初始化线
        line = transform.Find("Line").GetComponent<LineRenderer>();
        line.sharedMaterial = new Material(line.material);

        // 初始化块锚点
        anchor = transform.Find("Anchor");

        SetBlocksScale(smoothValue);
        SetLineOpacity(0);
    }
    /// <summary>
    /// 激活模组
    /// </summary>
    /// <param name="state"></param>
    /// <param name="fastmode"></param>
    internal void ActiveModule(bool state, bool fastmode = false)
    {
        module_activate = state;
        if (fastmode)
            smoothValue = state ? 1 : 0;
        if (!fastmode)
        {
            StartCoroutine(FlashLine(state));
        }
        else
        {
            SetLineOpacity(0f);
        }

        if (!fastmode)
            StartCoroutine(PulseEmissive(state));
        else
            SetBlocksEmissive(0);
    }
    /// <summary>
    /// 获取块锚点坐标
    /// </summary>
    /// <returns></returns>
    internal Vector3 GetAnchorPosition()
    {
        return anchor.position;
    }
    /// <summary>
    /// 平滑数值因子
    /// </summary>
    private void SmoothValue()
    {
        if (module_activate)
        {
            smoothValue = Mathf.Lerp(smoothValue, 1, smoothSpeed);
        }
        else
        {
            smoothValue = Mathf.Lerp(smoothValue, 0, smoothSpeed);
        }
    }
    /// <summary>
    /// 设置块尺寸
    /// </summary>
    /// <param name="scalefactor"></param>
    private void SetBlocksScale(float scalefactor)
    {
        for (int i = 0; i < blocks.Length; i++)
        {
            ScaleBlock(blocks[i].block, blocks[i].scale * scalefactor);
        }
    }
    /// <summary>
    /// 设置线条颜色
    /// </summary>
    /// <param name="alpha"></param>
    private void SetLineOpacity(float alpha)
    {
        Color color = line.sharedMaterial.color;
        color.a = alpha;
        line.sharedMaterial.color = color;
    }
    /// <summary>
    /// 缩放块物体
    /// </summary>
    /// <param name="block"></param>
    /// <param name="scale"></param>
    private void ScaleBlock(Transform block, Vector3 scale)
    {
        block.transform.localScale = scale;
    }
    /// <summary>
    /// 闪烁 alpha 值
    /// </summary>
    private IEnumerator FlashLine(bool state = true)
    {
        if (!state)
        {
            SetLineOpacity(0f);
            yield return null;
        }
        else
        {
            // 第一次闪烁：0 → 1 → 0
            SetLineOpacity(1f);
            yield return new WaitForSeconds(0.045f);
            SetLineOpacity(0f);
            yield return new WaitForSeconds(0.045f);

            // 第二次闪烁：0 → 0.35
            SetLineOpacity(0.35f);
            yield return new WaitForSeconds(0.045f);
            SetLineOpacity(0f);
            yield return new WaitForSeconds(0.045f);

            SetLineOpacity(1);
        }
    }
    /// <summary>
    /// 自发光脉动
    /// </summary>
    private IEnumerator PulseEmissive(bool state)
    {
        yield return new WaitForSeconds(state ? 0.5f : 0);
        float value = state ? 0 : 1;
        while (true)
        {
            value = Mathf.Lerp(value, state ? 1 : 0, smoothSpeed);
            SetBlocksEmissive(value);
            yield return new WaitForEndOfFrame();
        }
    }
}
