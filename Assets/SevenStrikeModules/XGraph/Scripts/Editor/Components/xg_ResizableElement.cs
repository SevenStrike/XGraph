namespace SevenStrikeModules.XGraph
{
    using System;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;
    using Cursor = UnityEngine.UIElements.Cursor;

    [Serializable]
    public class ResizeHandleInfo
    {
        public string h_name;
        public VisualElement handle;
        public float size_normal;
        public float offset_normal;
        public Vector2 offset_corner_normal;
        public Vector2 size_corner_normal;
        public float size_focused;
    }

    [UxmlElement]
    public partial class xg_ResizableElement : VisualElement
    {
        private const float HandleSize = 12;
        private const float HandleSize_Focused = 275;
        private ResizeHandleInfo[] resizeHandleInfos = new ResizeHandleInfo[8];
        private bool isDragging = false;
        private int activeHandleIndex = -1;
        private Vector2 dragStartMousePosition;
        private Rect dragStartLayout;
        /// <summary>
        /// 记录控制柄在元素局部坐标系中的初始位置
        /// </summary>
        private Vector2 dragStartHandleLocalPos;
        // 应用最小尺寸限制
        public Vector2 minSize = new Vector2(100, 100);

        public xg_ResizableElement()
        {
            style.position = Position.Absolute;
            CreateResizeHandles();

            RegisterCallback<MouseDownEvent>(OnMouseDown);
            RegisterCallback<MouseUpEvent>(OnMouseUp);
            RegisterCallback<MouseMoveEvent>(OnMouseMove);
            RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
        }

        /// <summary>
        /// 创建尺寸控制柄
        /// </summary>
        private void CreateResizeHandles()
        {
            for (int i = 0; i < resizeHandleInfos.Length; i++)
            {
                resizeHandleInfos[i] = new ResizeHandleInfo();
                ResizeHandleInfo info = resizeHandleInfos[i];
                if (i == 0)
                {
                    info.h_name = "上";
                    info.handle = CreateEdgeHandle("top", left: HandleSize * 0.75f, top: -10, right: HandleSize * 0.75f, height: HandleSize, cursorstyle: MouseCursor.ResizeVertical);
                    info.size_normal = HandleSize;
                    info.offset_normal = info.handle.style.top.value.value;
                    info.size_focused = HandleSize_Focused + 70;
                }
                if (i == 1)
                {
                    info.h_name = "下";
                    info.handle = CreateEdgeHandle("bottom", left: HandleSize * 0.75f, bottom: -10, right: HandleSize * 0.75f, height: HandleSize, cursorstyle: MouseCursor.ResizeVertical);
                    info.size_normal = HandleSize;
                    info.offset_normal = info.handle.style.bottom.value.value;
                    info.size_focused = HandleSize_Focused + 70;
                }
                if (i == 2)
                {
                    info.h_name = "左";
                    info.handle = CreateEdgeHandle("top", left: -10, top: HandleSize * 0.75f, width: HandleSize, bottom: HandleSize * 0.75f, cursorstyle: MouseCursor.ResizeHorizontal);
                    info.size_normal = HandleSize;
                    info.size_focused = HandleSize_Focused;
                    info.offset_normal = info.handle.style.left.value.value;
                }
                if (i == 3)
                {
                    info.h_name = "右";
                    info.handle = CreateEdgeHandle("top", right: -10, top: HandleSize * 0.75f, width: HandleSize, bottom: HandleSize * 0.75f, cursorstyle: MouseCursor.ResizeHorizontal);
                    info.size_normal = HandleSize;
                    info.size_focused = HandleSize_Focused;
                    info.offset_normal = info.handle.style.right.value.value;
                }
                if (i == 4)
                {
                    info.h_name = "左上";
                    info.handle = CreateCornerHandle("top-left", 0, 0, cursorstyle: MouseCursor.ResizeUpLeft);
                    info.offset_corner_normal.x = info.handle.style.left.value.value;
                    info.offset_corner_normal.y = info.handle.style.top.value.value;
                    info.size_corner_normal.x = info.handle.style.width.value.value;
                    info.size_corner_normal.y = info.handle.style.height.value.value;
                }
                if (i == 5)
                {
                    info.h_name = "右上";
                    info.handle = CreateCornerHandle("top-right", 1, 0, cursorstyle: MouseCursor.ResizeUpRight);
                    info.offset_corner_normal.x = info.handle.style.right.value.value;
                    info.offset_corner_normal.y = info.handle.style.top.value.value;
                    info.size_corner_normal.x = info.handle.style.width.value.value;
                    info.size_corner_normal.y = info.handle.style.height.value.value;
                }
                if (i == 6)
                {
                    info.h_name = "左下";
                    info.handle = CreateCornerHandle("bottom-left", 0, 1, cursorstyle: MouseCursor.ResizeUpRight);
                    info.offset_corner_normal.x = info.handle.style.left.value.value;
                    info.offset_corner_normal.y = info.handle.style.bottom.value.value;
                    info.size_corner_normal.x = info.handle.style.width.value.value;
                    info.size_corner_normal.y = info.handle.style.height.value.value;
                }
                if (i == 7)
                {
                    info.h_name = "右下";
                    info.handle = CreateCornerHandle("bottom-right", 1, 1, cursorstyle: MouseCursor.ResizeUpLeft);
                    info.offset_corner_normal.x = info.handle.style.right.value.value;
                    info.offset_corner_normal.y = info.handle.style.bottom.value.value;
                    info.size_corner_normal.x = info.handle.style.width.value.value;
                    info.size_corner_normal.y = info.handle.style.height.value.value;
                }
            }

            foreach (var h_info in resizeHandleInfos)
                Add(h_info.handle);
        }

        /// <summary>
        /// 创建控制点 - 角点
        /// </summary>
        /// <param h_name="solution"></param>
        /// <param h_name="horizontalPos"></param>
        /// <param h_name="verticalPos"></param>
        /// <param h_name="cursorstyle"></param>
        /// <returns></returns>
        private VisualElement CreateCornerHandle(string name, int horizontalPos, int verticalPos, MouseCursor cursorstyle = MouseCursor.Arrow)
        {
            var handle = new VisualElement
            {
                name = $"resize-handle-{name}",
                style = {
                position = Position.Absolute,
                width = HandleSize*1.5f,
                height = HandleSize*1.5f,
                opacity = 0,
                cursor =SetCursor(cursorstyle),
                backgroundColor = new Color(0.25f, 0.5f, 0.8f, 0.7f),
            }
            };

            if (horizontalPos == 0) handle.style.left = -5;
            else handle.style.right = -5;

            if (verticalPos == 0) handle.style.top = -5;
            else handle.style.bottom = -5;

            return handle;
        }
        /// <summary>
        /// 创建控制点 - 边缘
        /// </summary>
        /// <param h_name="solution"></param>
        /// <param h_name="horizontalPos"></param>
        /// <param h_name="verticalPos"></param>
        /// <param h_name="cursorstyle"></param>
        /// <returns></returns>
        private VisualElement CreateEdgeHandle(string name, float left = float.NaN, float top = float.NaN, float right = float.NaN, float bottom = float.NaN, float width = float.NaN, float height = float.NaN, MouseCursor cursorstyle = MouseCursor.Arrow)
        {
            var handle = new VisualElement
            {
                name = $"resize-handle-{name}",
                style = {
                position = Position.Absolute,
                opacity = 0,
                cursor =SetCursor(cursorstyle),
                backgroundColor = new Color(1, 1, 1, 0.7f),
            }
            };

            if (!float.IsNaN(left)) handle.style.left = left;
            if (!float.IsNaN(top)) handle.style.top = top;
            if (!float.IsNaN(right)) handle.style.right = right;
            if (!float.IsNaN(bottom)) handle.style.bottom = bottom;
            if (!float.IsNaN(width)) handle.style.width = width;
            if (!float.IsNaN(height)) handle.style.height = height;

            if (name == "top" || name == "bottom")
            {
                handle.style.borderTopWidth = 1f;
                handle.style.borderBottomWidth = 1f;
            }
            else
            {
                handle.style.borderLeftWidth = 1f;
                handle.style.borderRightWidth = 1f;
            }
            //h_info.style.borderColor = Color.black;

            return handle;
        }

        /// <summary>
        /// 设置最小尺寸
        /// </summary>
        public void SetMinSize(Vector2 size)
        {
            minSize = size;
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            // 只响应左键
            if (evt.button != 0) return;

            // 放大拖动区域尺寸提高命中率
            for (int i = 0; i < resizeHandleInfos.Length; i++)
            {
                ResizeHandleInfo info = resizeHandleInfos[i];
                if (evt.target == info.handle)
                {
                    if (info.h_name == "上")
                    {
                        info.handle.style.height = info.size_focused;
                        info.handle.style.top = new StyleLength(info.offset_normal - 15);
                    }
                    if (info.h_name == "下")
                    {
                        info.handle.style.height = info.size_focused;
                        info.handle.style.bottom = new StyleLength(info.offset_normal - 15);
                    }
                    if (info.h_name == "左")
                    {
                        info.handle.style.width = info.size_focused;
                        info.handle.style.left = new StyleLength(info.offset_normal - 15);
                    }
                    if (info.h_name == "右")
                    {
                        info.handle.style.width = info.size_focused;
                        info.handle.style.right = new StyleLength(info.offset_normal - 15);
                    }
                    if (info.h_name == "左上")
                    {
                        info.handle.style.width = info.size_corner_normal.x + 255;
                        info.handle.style.height = info.size_corner_normal.y + 325;
                        info.handle.style.top = new StyleLength(info.offset_corner_normal.y - 15);
                        info.handle.style.left = new StyleLength(info.offset_corner_normal.x - 15);
                    }
                    if (info.h_name == "右上")
                    {
                        info.handle.style.width = info.size_corner_normal.x + 255;
                        info.handle.style.height = info.size_corner_normal.y + 325;
                        info.handle.style.top = new StyleLength(info.offset_corner_normal.y - 15);
                        info.handle.style.right = new StyleLength(info.offset_corner_normal.x - 15);
                    }
                    if (info.h_name == "左下")
                    {
                        info.handle.style.width = info.size_corner_normal.x + 255;
                        info.handle.style.height = info.size_corner_normal.y + 325;
                        info.handle.style.left = new StyleLength(info.offset_corner_normal.x - 15);
                        info.handle.style.bottom = new StyleLength(info.offset_corner_normal.y - 15);
                    }
                    if (info.h_name == "右下")
                    {
                        info.handle.style.width = info.size_corner_normal.x + 255;
                        info.handle.style.height = info.size_corner_normal.y + 325;
                        info.handle.style.right = new StyleLength(info.offset_corner_normal.x - 15);
                        info.handle.style.bottom = new StyleLength(info.offset_corner_normal.y - 15);
                    }
                }

                if (resizeHandleInfos[i].handle.worldBound.Contains(evt.mousePosition))
                {
                    isDragging = true;
                    activeHandleIndex = i;
                    dragStartMousePosition = evt.mousePosition;
                    dragStartLayout = new Rect(resolvedStyle.left, resolvedStyle.top, resolvedStyle.width, resolvedStyle.height);

                    // 记录控制柄在元素局部坐标系中的初始位置
                    var handleCenter = new Vector2(
                        resizeHandleInfos[i].handle.layout.x + resizeHandleInfos[i].handle.layout.width / 2,
                        resizeHandleInfos[i].handle.layout.y + resizeHandleInfos[i].handle.layout.height / 2);
                    dragStartHandleLocalPos = handleCenter;

                    this.CaptureMouse();
                    evt.StopPropagation();
                    return;
                }
            }
        }

        private void OnMouseUp(MouseUpEvent evt)
        {
            // 只响应左键
            if (evt.button != 0) return;

            // 恢复拖动区域尺寸提高命中率
            for (int i = 0; i < resizeHandleInfos.Length; i++)
            {
                ResizeHandleInfo info = resizeHandleInfos[i];

                if (info.h_name == "上")
                {
                    info.handle.style.height = info.size_normal;
                    info.handle.style.top = new StyleLength(info.offset_normal);
                }
                if (info.h_name == "下")
                {
                    info.handle.style.height = info.size_normal;
                    info.handle.style.bottom = new StyleLength(info.offset_normal);
                }
                if (info.h_name == "左")
                {
                    info.handle.style.width = info.size_normal;
                    info.handle.style.left = new StyleLength(info.offset_normal);
                }
                if (info.h_name == "右")
                {
                    info.handle.style.width = info.size_normal;
                    info.handle.style.right = new StyleLength(info.offset_normal);
                }
                if (info.h_name == "左上")
                {
                    info.handle.style.width = info.size_corner_normal.x;
                    info.handle.style.height = info.size_corner_normal.y;
                    info.handle.style.top = new StyleLength(info.offset_corner_normal.y);
                    info.handle.style.left = new StyleLength(info.offset_corner_normal.x);
                }
                if (info.h_name == "右上")
                {
                    info.handle.style.width = info.size_corner_normal.x;
                    info.handle.style.height = info.size_corner_normal.y;
                    info.handle.style.top = new StyleLength(info.offset_corner_normal.y);
                    info.handle.style.right = new StyleLength(info.offset_corner_normal.x);
                }
                if (info.h_name == "左下")
                {
                    info.handle.style.width = info.size_corner_normal.x;
                    info.handle.style.height = info.size_corner_normal.y;
                    info.handle.style.left = new StyleLength(info.offset_corner_normal.x);
                    info.handle.style.bottom = new StyleLength(info.offset_corner_normal.y);
                }
                if (info.h_name == "右下")
                {
                    info.handle.style.width = info.size_corner_normal.x;
                    info.handle.style.height = info.size_corner_normal.y;
                    info.handle.style.right = new StyleLength(info.offset_corner_normal.x);
                    info.handle.style.bottom = new StyleLength(info.offset_corner_normal.y);
                }
            }

            if (isDragging)
            {
                isDragging = false;
                activeHandleIndex = -1;
                this.ReleaseMouse();
                evt.StopPropagation();
            }

            StickeyWindowRegion(evt);
        }

        private void OnMouseMove(MouseMoveEvent evt)
        {
            // 只响应左键
            if (evt.button != 0) return;

            if (!isDragging || activeHandleIndex < 0) return;

            // 计算鼠标在元素局部坐标系的当前位置
            Vector2 currentMouseLocal = this.WorldToLocal(evt.mousePosition);

            // 计算控制柄应该移动到的理论位置（基于初始局部位置+鼠标移动量）
            Vector2 targetHandlePos = dragStartHandleLocalPos + (currentMouseLocal - this.WorldToLocal(dragStartMousePosition));

            Rect newLayout = new Rect(dragStartLayout);

            // 根据控制柄类型调整布局
            switch (activeHandleIndex)
            {
                case 0: // 上边缘
                    newLayout.y = dragStartLayout.y + (targetHandlePos.y - dragStartHandleLocalPos.y);
                    newLayout.height = dragStartLayout.height - (targetHandlePos.y - dragStartHandleLocalPos.y);
                    break;

                case 1: // 下边缘
                    newLayout.height = dragStartLayout.height + (targetHandlePos.y - dragStartHandleLocalPos.y);
                    break;

                case 2: // 左边缘
                    newLayout.x = dragStartLayout.x + (targetHandlePos.x - dragStartHandleLocalPos.x);
                    newLayout.width = dragStartLayout.width - (targetHandlePos.x - dragStartHandleLocalPos.x);
                    break;

                case 3: // 右边缘
                    newLayout.width = dragStartLayout.width + (targetHandlePos.x - dragStartHandleLocalPos.x);
                    break;
                case 4: // 左上角
                    newLayout.x = dragStartLayout.x + (targetHandlePos.x - dragStartHandleLocalPos.x);
                    newLayout.width = dragStartLayout.width - (targetHandlePos.x - dragStartHandleLocalPos.x);
                    newLayout.y = dragStartLayout.y + (targetHandlePos.y - dragStartHandleLocalPos.y);
                    newLayout.height = dragStartLayout.height - (targetHandlePos.y - dragStartHandleLocalPos.y);
                    break;

                case 5: // 右上角
                    newLayout.width = dragStartLayout.width + (targetHandlePos.x - dragStartHandleLocalPos.x);
                    newLayout.y = dragStartLayout.y + (targetHandlePos.y - dragStartHandleLocalPos.y);
                    newLayout.height = dragStartLayout.height - (targetHandlePos.y - dragStartHandleLocalPos.y);
                    break;

                case 6: // 左下角
                    newLayout.x = dragStartLayout.x + (targetHandlePos.x - dragStartHandleLocalPos.x);
                    newLayout.width = dragStartLayout.width - (targetHandlePos.x - dragStartHandleLocalPos.x);
                    newLayout.height = dragStartLayout.height + (targetHandlePos.y - dragStartHandleLocalPos.y);
                    break;

                case 7: // 右下角
                    newLayout.width = dragStartLayout.width + (targetHandlePos.x - dragStartHandleLocalPos.x);
                    newLayout.height = dragStartLayout.height + (targetHandlePos.y - dragStartHandleLocalPos.y);
                    break;
            }

            if (newLayout.width < minSize.x)
            {
                if (activeHandleIndex == 4 || activeHandleIndex == 6 || activeHandleIndex == 2)
                    newLayout.x = dragStartLayout.x - (minSize.x - dragStartLayout.width);
                newLayout.width = minSize.x;
            }

            if (newLayout.height < minSize.y)
            {
                if (activeHandleIndex == 4 || activeHandleIndex == 5 || activeHandleIndex == 0)
                    newLayout.y = dragStartLayout.y - (minSize.y - dragStartLayout.height);
                newLayout.height = minSize.y;
            }

            // 原子性更新布局
            style.left = newLayout.x;
            style.top = newLayout.y;
            style.width = newLayout.width;
            style.height = newLayout.height;

            // 强制立即重绘
            this.MarkDirtyRepaint();

            evt.StopPropagation();
        }

        private void OnMouseLeave(MouseLeaveEvent evt)
        {
            // 只响应左键
            if (evt.button != 0) return;

            if (isDragging) this.CaptureMouse();
        }

        public StyleCursor SetCursor(MouseCursor cursor)
        {
            object objCursor = new Cursor();
            PropertyInfo fields = typeof(Cursor).GetProperty("defaultCursorId", BindingFlags.NonPublic | BindingFlags.Instance);
            fields.SetValue(objCursor, (int)cursor);
            return new StyleCursor((Cursor)objCursor);
        }

        private void StickeyWindowRegion(MouseUpEvent evt)
        {
            // 自动吸附到最近的象限
            SnapToNearestQuadrant();

            evt.StopPropagation();
        }

        /// <summary>
        /// 根据当前样式值自动设置面板到最近的象限
        /// </summary>
        public void SnapToNearestQuadrant()
        {
            if (parent == null) return;

            // 获取当前有效的定位值
            float left = this.style.left.keyword == StyleKeyword.Auto ?
                parent.layout.width - this.layout.width - this.style.right.value.value :
                this.style.left.value.value;

            float top = this.style.top.keyword == StyleKeyword.Auto ?
                parent.layout.height - this.layout.height - this.style.bottom.value.value :
                this.style.top.value.value;

            // 使用这些值进行象限判断
            SetPanelByStyleValues(left, top);
        }

        /// <summary>
        /// 根据传入的left/top基础值自动设置面板到最近的象限
        /// </summary>
        private void SetPanelByStyleValues(float baseLeft, float baseTop)
        {
            Vector2 parentSize = new Vector2(parent.layout.width, parent.layout.height);
            Vector2 panelSize = new Vector2(this.layout.width, this.layout.height);

            // 计算面板中心点
            Vector2 panelCenter = new Vector2(
                baseLeft + panelSize.x * 0.5f,
                baseTop + panelSize.y * 0.5f);

            // 确定象限
            bool isRight = panelCenter.x > parentSize.x * 0.5f;
            bool isBottom = panelCenter.y > parentSize.y * 0.5f;

            // 重置所有定位属性
            this.style.left = StyleKeyword.Auto;
            this.style.right = StyleKeyword.Auto;
            this.style.top = StyleKeyword.Auto;
            this.style.bottom = StyleKeyword.Auto;

            // 根据象限设置定位
            if (isRight && !isBottom) // 右上象限
            {
                this.style.right = parentSize.x - (baseLeft + panelSize.x);
                this.style.top = baseTop;
            }
            else if (!isRight && !isBottom) // 左上象限
            {
                this.style.left = baseLeft;
                this.style.top = baseTop;
            }
            else if (!isRight && isBottom) // 左下象限
            {
                this.style.left = baseLeft;
                this.style.bottom = parentSize.y - (baseTop + panelSize.y);
            }
            else // 右下象限
            {
                this.style.right = parentSize.x - (baseLeft + panelSize.x);
                this.style.bottom = parentSize.y - (baseTop + panelSize.y);
            }
        }

    }
}