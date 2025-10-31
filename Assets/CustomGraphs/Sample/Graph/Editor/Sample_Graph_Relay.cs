namespace SevenStrikeModules.XGraph
{
    using System.Collections.Generic;
    using UnityEngine;

    public class Sample_Graph_Relay : VNode_Relay
    {
        public override void Initialize(xg_GraphView graphView, Vector2 pos = default, ActionNode_Base data = null)
        {
            base.Initialize(graphView, pos, data);
        }

        public override void CheckConnected()
        {
            base.CheckConnected();
        }

        public override void Connected()
        {
            base.Connected();
        }

        public override void Disconnected()
        {
            base.Disconnected();
        }

        public override VNode_Base Draw()
        {
            return base.Draw();
        }

        public override void Draw_Extension()
        {
            base.Draw_Extension();
        }

        public override void Draw_Input()
        {
            base.Draw_Input();
        }

        public override void Draw_Main()
        {
            base.Draw_Main();
        }

        public override void Draw_Output()
        {
            base.Draw_Output();
        }

        public override void Draw_Title()
        {
            base.Draw_Title();
        }

        public override void Draw_TitleButton()
        {
            base.Draw_TitleButton();
        }

        public override void Draw_Top()
        {
            base.Draw_Top();
        }

        public override void OnDuplicatedNode(List<DuplicateNodeData> list)
        {
            base.OnDuplicatedNode(list);
        }
    }
}