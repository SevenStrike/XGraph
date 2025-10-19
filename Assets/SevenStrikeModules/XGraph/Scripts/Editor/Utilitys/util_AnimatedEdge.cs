namespace SevenStrikeModules.XGraph
{
    using System;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class util_AnimatedEdge : Edge
    {
        #region 字段和属性
        /// <summary>
        /// 是否启用流动效果
        /// </summary>
        private bool _isFlowEnabled;

        /// <summary>
        /// 流动点大小
        /// </summary>
        private float _flowSize = 8f;

        /// <summary>
        /// 是否已注册更新
        /// </summary>
        private bool _isUpdateRegistered = false;

        /// <summary>
        /// 连线总长度
        /// </summary>
        private float _totalEdgeLength;

        /// <summary>
        /// 动画开始时间
        /// </summary>
        private double _animationStartTime;

        /// <summary>
        /// 控制点数组
        /// </summary>
        private Vector2[] _controlPoints;

        /// <summary>
        /// 线段长度数组
        /// </summary>
        private float[] _segmentLengths;

        /// <summary>
        /// 累积长度数组
        /// </summary>
        private float[] _cumulativeLengths;

        /// <summary>
        /// 流动点图像
        /// </summary>
        private readonly Image _flowImage;

        /// <summary>
        /// 是否启用流动效果
        /// </summary>
        public bool EnableFlow
        {
            get => _isFlowEnabled;
            set
            {
                if (_isFlowEnabled == value)
                {
                    return;
                }

                _isFlowEnabled = value;
                if (_isFlowEnabled)
                {
                    Add(_flowImage);
                    RegisterToUpdate();
                }
                else
                {
                    Remove(_flowImage);
                    UnregisterFromUpdate();
                }
            }
        }

        /// <summary>
        /// 流动点大小
        /// </summary>
        public float FlowSize
        {
            get => _flowSize;
            set
            {
                _flowSize = value;
                _flowImage.style.width = new Length(_flowSize, LengthUnit.Pixel);
                _flowImage.style.height = new Length(_flowSize, LengthUnit.Pixel);
            }
        }

        /// <summary>
        /// 流动速度
        /// </summary>
        public float FlowSpeed { get; set; } = 100f;
        #endregion

        /// <summary>
        /// 当选中连线时的委托事件
        /// </summary>
        public Action OnSelectedEdge;
        /// <summary>
        /// 当选中连线时的委托事件
        /// </summary>
        public Action OnUnSelectedEdge;

        #region 构造函数

        /// <summary>
        /// 动画连线构造器
        /// </summary>
        public util_AnimatedEdge()
        {
            _flowImage = new Image
            {
                name = "flow-image",
                style =
                {
                    width = new Length(FlowSize, LengthUnit.Pixel),
                    height = new Length(FlowSize, LengthUnit.Pixel),
                    borderTopLeftRadius = new Length(FlowSize / 2, LengthUnit.Pixel),
                    borderTopRightRadius = new Length(FlowSize / 2, LengthUnit.Pixel),
                    borderBottomLeftRadius = new Length(FlowSize / 2, LengthUnit.Pixel),
                    borderBottomRightRadius = new Length(FlowSize / 2, LengthUnit.Pixel),
                }
            };

            edgeControl.RegisterCallback<GeometryChangedEvent>(OnEdgeControlGeometryChanged);
        }

        #endregion

        #region 更新注册管理

        /// <summary>
        /// 注册到编辑器更新
        /// </summary>
        private void RegisterToUpdate()
        {
            if (!_isUpdateRegistered)
            {
                EditorApplication.update += OnEditorUpdate;
                _isUpdateRegistered = true;
            }
        }

        /// <summary>
        /// 从编辑器更新中取消注册
        /// </summary>
        private void UnregisterFromUpdate()
        {
            if (_isUpdateRegistered)
            {
                EditorApplication.update -= OnEditorUpdate;
                _isUpdateRegistered = false;
            }
        }

        #endregion

        #region 更新方法

        /// <summary>
        /// 编辑器更新
        /// </summary>
        private void OnEditorUpdate()
        {
            if (EnableFlow)
            {
                UpdateFlow();
                MarkDirtyRepaint();
            }
        }

        /// <summary>
        /// 更新连线控制
        /// </summary>
        public override bool UpdateEdgeControl()
        {
            if (!base.UpdateEdgeControl())
            {
                return false;
            }

            UpdateFlow();
            return true;
        }

        #endregion

        #region 流动动画效果

        /// <summary>
        /// 更新流动效果
        /// </summary>
        public void UpdateFlow()
        {
            if (!EnableFlow || _controlPoints == null || _controlPoints.Length < 2)
                return;

            // 计算基于总长度的动画进度
            double elapsedTime = EditorApplication.timeSinceStartup - _animationStartTime;
            float totalProgress = (float)(elapsedTime * FlowSpeed / _totalEdgeLength);

            // 循环动画
            totalProgress %= 1.0f;

            // 应用缓动
            float easedProgress = EaseInOut(totalProgress);

            // 根据总进度计算当前在整条路径上的位置
            float targetLength = easedProgress * _totalEdgeLength;

            // 找到目标线段和在线段上的位置
            int segmentIndex = 0;
            float segmentProgress = 0f;

            FindSegmentAndProgress(targetLength, out segmentIndex, out segmentProgress);

            // 计算当前位置
            Vector2 currentPosition = GetPointOnSegment(segmentIndex, segmentProgress);
            _flowImage.transform.position = currentPosition - Vector2.one * FlowSize / 2;

            // 计算颜色：透明→实心→透明
            float alpha = CalculateAlphaSharp(totalProgress);
            var startColor = edgeControl.outputColor;
            var endColor = edgeControl.inputColor;
            var flowColor = Color.Lerp(startColor, endColor, totalProgress);
            flowColor.a = alpha; // 应用透明度
            _flowImage.style.backgroundColor = flowColor;
        }

        /// <summary>
        /// 初始化流动参数
        /// </summary>
        private void InitializeFlowParameters()
        {
            if (edgeControl.controlPoints == null || edgeControl.controlPoints.Length < 2)
                return;

            // 复制控制点
            _controlPoints = new Vector2[edgeControl.controlPoints.Length];
            for (int i = 0; i < edgeControl.controlPoints.Length; i++)
            {
                _controlPoints[i] = edgeControl.controlPoints[i];
            }

            // 计算线段长度和累积长度
            _segmentLengths = new float[_controlPoints.Length - 1];
            _cumulativeLengths = new float[_controlPoints.Length];

            _totalEdgeLength = 0f;
            _cumulativeLengths[0] = 0f;

            for (int i = 0; i < _controlPoints.Length - 1; i++)
            {
                float segmentLength = Vector2.Distance(_controlPoints[i], _controlPoints[i + 1]);
                _segmentLengths[i] = segmentLength;
                _totalEdgeLength += segmentLength;
                _cumulativeLengths[i + 1] = _totalEdgeLength;
            }

            // 重置动画时间
            _animationStartTime = EditorApplication.timeSinceStartup;
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 根据目标长度找到对应的线段和在线段上的进度
        /// </summary>
        private void FindSegmentAndProgress(float targetLength, out int segmentIndex, out float segmentProgress)
        {
            segmentIndex = 0;
            segmentProgress = 0f;

            // 找到目标长度所在的线段
            for (int i = 0; i < _cumulativeLengths.Length - 1; i++)
            {
                if (targetLength <= _cumulativeLengths[i + 1])
                {
                    segmentIndex = i;
                    float segmentStartLength = _cumulativeLengths[i];
                    float segmentEndLength = _cumulativeLengths[i + 1];
                    float segmentLength = segmentEndLength - segmentStartLength;
                    segmentProgress = (targetLength - segmentStartLength) / segmentLength;
                    break;
                }
            }
        }

        /// <summary>
        /// 获取线段上的点
        /// </summary>
        private Vector2 GetPointOnSegment(int segmentIndex, float progress)
        {
            if (segmentIndex < 0 || segmentIndex >= _controlPoints.Length - 1)
                return Vector2.zero;

            Vector2 start = _controlPoints[segmentIndex];
            Vector2 end = _controlPoints[segmentIndex + 1];
            return Vector2.Lerp(start, end, progress);
        }

        #endregion

        #region 缓动和透明度计算

        /// <summary>
        /// EaseInOut 缓动函数
        /// </summary>
        private float EaseInOut(float t)
        {
            t = Mathf.Clamp01(t);
            return t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t;
        }

        /// <summary>
        /// 计算透明度：开始透明(0) → 中间实心(1) → 结束透明(0)
        /// </summary>
        private float CalculateAlpha(float progress)
        {
            // 使用三角函数实现平滑的透明度变化
            // sin(π * progress) 会在 0→1→0 之间变化
            return Mathf.Sin(Mathf.PI * progress);
        }

        /// <summary>
        /// 计算透明度（更平缓的变化）
        /// </summary>
        private float CalculateAlphaSoft(float progress)
        {
            return Mathf.Pow(Mathf.Sin(Mathf.PI * progress), 2f);
        }

        /// <summary>
        /// 计算透明度（更尖锐的变化，在中间更实心）
        /// </summary>
        private float CalculateAlphaSharp(float progress)
        {
            return Mathf.Pow(Mathf.Sin(Mathf.PI * progress), 0.65f);
        }

        /// <summary>
        /// 计算透明度（使用二次函数）
        /// </summary>
        private float CalculateAlphaQuadratic(float progress)
        {
            // 二次函数实现：在0和1处为0，在0.5处为1
            return 4 * progress * (1 - progress);
        }

        #endregion

        #region 事件处理

        /// <summary>
        /// 连线控制几何变化事件处理
        /// </summary>
        private void OnEdgeControlGeometryChanged(GeometryChangedEvent evt)
        {
            InitializeFlowParameters();
        }
        #endregion

        public override void OnSelected()
        {
            base.OnSelected();

            if (OnSelectedEdge != null)
                OnSelectedEdge();
        }

        public override void OnUnselected()
        {
            base.OnUnselected();

            if (OnUnSelectedEdge != null)
                OnUnSelectedEdge();
        }
    }
}