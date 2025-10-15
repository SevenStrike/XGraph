namespace SevenStrikeModules.XGraph
{
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;

    public static class util_XGraphInspectorGUI
    {
        /// <summary>
        /// 初始化样式
        /// </summary>
        public static StyleSheet InitializeStyle(this VisualElement element, string path = null)
        {
            // 指定样式
            return util_XGraphEditorUtility.ElementStyle_Add(element, path);
        }
        /// <summary>
        /// 创建容器
        /// </summary>
        /// <param name="root"></param>
        /// <param name="styles"></param>
        /// <returns></returns>
        public static VisualElement GUI_Container(this VisualElement root, string[] styles = null)
        {
            VisualElement container = new VisualElement();
            container.name = "container";
            for (int i = 0; i < styles.Length; i++)
            {
                container.AddToClassList(styles[i]);
            }
            root.Add(container);

            return container;
        }
        /// <summary>
        /// 创建标题
        /// </summary>
        /// <param name="root"></param>
        /// <param name="theme"></param>
        /// <param name="title"></param>
        /// <param name="sub"></param>
        /// <param name="styles_group"></param>
        /// <param name="styles_mark"></param>
        /// <param name="styles_title"></param>
        /// <param name="styles_sub"></param>
        /// <returns></returns>
        public static VisualElement GUI_Title(this VisualElement root, Color theme, string title, string sub, string[] styles_group = null, string[] styles_mark = null, string[] styles_title = null, string[] styles_sub = null)
        {
            // 标题组
            VisualElement vm_group = new VisualElement();
            vm_group.name = "group";
            for (int i = 0; i < styles_group.Length; i++)
            {
                vm_group.AddToClassList(styles_group[i]);
            }
            root.Add(vm_group);

            // 标记
            VisualElement vm_mark = new VisualElement();
            vm_mark.name = "mark";
            for (int i = 0; i < styles_mark.Length; i++)
            {
                vm_mark.AddToClassList(styles_mark[i]);
            }
            vm_mark.style.backgroundColor = theme;
            vm_group.Add(vm_mark);

            // 标题
            Label lab_title = new Label(title);
            lab_title.name = "title";
            for (int i = 0; i < styles_title.Length; i++)
            {
                lab_title.AddToClassList(styles_title[i]);
            }
            lab_title.RegisterCallback<PointerDownEvent>((evt) =>
            {
                if (evt.button == (int)MouseButton.LeftMouse && evt.clickCount == 2)
                {
                    GUIUtility.systemCopyBuffer = lab_title.text;
                }
            });
            vm_group.Add(lab_title);

            // 类型
            Label lab_sub = new Label(sub);
            lab_sub.name = "sub";
            for (int i = 0; i < styles_sub.Length; i++)
            {
                lab_sub.AddToClassList(styles_sub[i]);
            }
            vm_group.Add(lab_sub);

            return vm_group;
        }
        /// <summary>
        /// 创建标题
        /// </summary>
        /// <param name="root"></param>
        /// <param name="nodedata"></param>
        /// <param name="title"></param>
        /// <param name="sub"></param>
        /// <param name="styles_group"></param>
        /// <param name="styles_mark"></param>
        /// <param name="styles_title"></param>
        /// <param name="styles_sub"></param>
        /// <returns></returns>
        public static VisualElement GUI_IconTitle(this VisualElement root, ActionNode_Base nodedata, string title, string sub, string[] styles_group = null, string[] styles_mark = null, string[] styles_title = null, string[] styles_sub = null)
        {
            // 标题组
            VisualElement vm_group = new VisualElement();
            vm_group.name = "group";
            for (int i = 0; i < styles_group.Length; i++)
            {
                vm_group.AddToClassList(styles_group[i]);
            }
            root.Add(vm_group);

            // 标记
            VisualElement vm_mark = new VisualElement();
            vm_mark.name = "icon";
            for (int i = 0; i < styles_mark.Length; i++)
            {
                vm_mark.AddToClassList(styles_mark[i]);
            }
            if (nodedata.NodeIcon == null)
                vm_mark.style.backgroundImage = util_XGraphEditorUtility.AssetLoad<Texture2D>($"{util_Dashboard.GetPath_GUI()}Icons/GraphIcon/{nodedata.icon}.png");
            else
                vm_mark.style.backgroundImage = nodedata.NodeIcon;
            vm_group.Add(vm_mark);

            // 标题
            Label lab_title = new Label(title);
            lab_title.name = "title";
            for (int i = 0; i < styles_title.Length; i++)
            {
                lab_title.AddToClassList(styles_title[i]);
            }
            lab_title.RegisterCallback<PointerDownEvent>((evt) =>
            {
                if (evt.button == (int)MouseButton.LeftMouse && evt.clickCount == 2)
                {
                    GUIUtility.systemCopyBuffer = lab_title.text;
                }
            });
            vm_group.Add(lab_title);

            // 类型
            Label lab_sub = new Label(sub);
            lab_sub.name = "sub";
            for (int i = 0; i < styles_sub.Length; i++)
            {
                lab_sub.AddToClassList(styles_sub[i]);
            }
            vm_group.Add(lab_sub);

            return vm_group;
        }
        /// <summary>
        /// 创建标签
        /// </summary>
        /// <param name="root"></param>
        /// <param name="content"></param>
        /// <param name="styles"></param>
        /// <returns></returns>
        public static Label GUI_Label(this VisualElement root, string content, string[] styles = null)
        {
            Label label = new Label(content);
            for (int i = 0; i < styles.Length; i++)
            {
                label.AddToClassList(styles[i]);
            }
            root.Add(label);

            return label;
        }
        /// <summary>
        /// 创建属性框 - 字符串
        /// </summary>
        /// <param name="root"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="styles"></param>
        /// <returns></returns>
        public static TextField GUI_Field_String(this VisualElement root, string name, string value, string[] styles = null)
        {
            TextField field = new TextField(name);
            field.value = value;
            for (int i = 0; i < styles.Length; i++)
            {
                field.AddToClassList(styles[i]);
            }
            root.Add(field);
            return field;
        }
        /// <summary>
        /// 创建属性框 - 浮点数
        /// </summary>
        /// <param name="root"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="styles"></param>
        /// <returns></returns>
        public static FloatField GUI_Field_Float(this VisualElement root, string name, float value, string[] styles = null)
        {
            FloatField field = new FloatField(name);
            field.value = value;
            for (int i = 0; i < styles.Length; i++)
            {
                field.AddToClassList(styles[i]);
            }
            root.Add(field);
            return field;
        }
        /// <summary>
        /// 创建属性框 - 整数
        /// </summary>
        /// <param name="root"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="styles"></param>
        /// <returns></returns>
        public static IntegerField GUI_Field_Int(this VisualElement root, string name, int value, string[] styles = null)
        {
            IntegerField field = new IntegerField(name);
            field.value = value;
            for (int i = 0; i < styles.Length; i++)
            {
                field.AddToClassList(styles[i]);
            }
            root.Add(field);
            return field;
        }
        /// <summary>
        /// 创建属性框 - 布尔
        /// </summary>
        /// <param name="root"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="styles"></param>
        /// <returns></returns>
        public static Toggle GUI_Field_Bool(this VisualElement root, string name, bool value, string[] styles = null)
        {
            Toggle field = new Toggle(name);
            field.value = value;
            for (int i = 0; i < styles.Length; i++)
            {
                field.AddToClassList(styles[i]);
            }
            root.Add(field);
            return field;
        }
        /// <summary>
        /// 创建属性框 - 2维向量
        /// </summary>
        /// <param name="root"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="styles"></param>
        /// <returns></returns>
        public static Vector2Field GUI_Field_Vector2(this VisualElement root, string name, Vector2 value, string[] styles = null)
        {
            Vector2Field field = new Vector2Field(name);
            field.value = value;
            for (int i = 0; i < styles.Length; i++)
            {
                field.AddToClassList(styles[i]);
            }
            root.Add(field);
            return field;
        }
        /// <summary>
        /// 创建属性框 - 3维向量
        /// </summary>
        /// <param name="root"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="styles"></param>
        /// <returns></returns>
        public static Vector3Field GUI_Field_Vector3(this VisualElement root, string name, Vector3 value, string[] styles = null)
        {
            Vector3Field field = new Vector3Field(name);
            field.value = value;
            for (int i = 0; i < styles.Length; i++)
            {
                field.AddToClassList(styles[i]);
            }
            root.Add(field);
            return field;
        }
        /// <summary>
        /// 创建属性框 - 4维向量
        /// </summary>
        /// <param name="root"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="styles"></param>
        /// <returns></returns>
        public static Vector4Field GUI_Field_Vector4(this VisualElement root, string name, Vector4 value, string[] styles = null)
        {
            Vector4Field field = new Vector4Field(name);
            field.value = value;
            for (int i = 0; i < styles.Length; i++)
            {
                field.AddToClassList(styles[i]);
            }
            root.Add(field);
            return field;
        }
        /// <summary>
        /// 创建属性框 - 颜色
        /// </summary>
        /// <param name="root"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="styles"></param>
        /// <returns></returns>
        public static ColorField GUI_Field_Color(this VisualElement root, string name, Color value, string[] styles = null)
        {
            ColorField field = new ColorField(name);
            field.value = value;
            for (int i = 0; i < styles.Length; i++)
            {
                field.AddToClassList(styles[i]);
            }
            root.Add(field);
            return field;
        }
    }
}