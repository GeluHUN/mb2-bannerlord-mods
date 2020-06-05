using TaleWorlds.GauntletUI;

namespace Extension.Config.UI
{
    class OptionWidget : Widget
    {
        public string Name { get; set; }
        public string Hint { get; set; }
        public string HintTitleWidgetName { get; set; }
        public string HintTextWidgetName { get; set; }

        RichTextWidget _hintTitleWidget;
        RichTextWidget _hintTextWidget;

        public OptionWidget(UIContext context)
            : base(context)
        {
        }

        protected override void OnChildAdded(Widget child)
        {
            base.OnChildAdded(child);
            AddEventToChildren(child);
        }

        protected override void OnChildRemoved(Widget child)
        {
            base.OnChildRemoved(child);
            RemoveEventFromChildren(child);
        }

        protected override void OnHoverBegin()
        {
            base.OnHoverBegin();
            ShowHint();
        }

        protected override void OnHoverEnd()
        {
            base.OnHoverEnd();
            HideHint();
        }

        void AddEventToChildren(Widget child)
        {
            foreach (Widget w in child.AllChildrenAndThis)
            {
                w.EventFire += ChildEvent;
            }
        }

        void RemoveEventFromChildren(Widget child)
        {
            foreach (Widget w in child.AllChildrenAndThis)
            {
                w.EventFire -= ChildEvent;
            }
        }

        void ChildEvent(Widget child, string eventName, object[] args)
        {
            if (eventName == "HoverBegin")
            {
                ShowHint();
            }
            else if (eventName == "HoverEnd")
            {
                HideHint();
            }
            else if (eventName == "ItemAdd")
            {
                AddEventToChildren(child);
            }
            else if (eventName == "ItemRemove")
            {
                RemoveEventFromChildren(child);
            }
        }

        RichTextWidget GetHintTitleWidget()
        {
            if (_hintTitleWidget == null)
            {
                _hintTitleWidget = Context?.Root?.FindChild(HintTitleWidgetName, true) as RichTextWidget;
            }
            return _hintTitleWidget;
        }

        RichTextWidget GetHintTextWidget()
        {
            if (_hintTextWidget == null)
            {
                _hintTextWidget = Context?.Root?.FindChild(HintTextWidgetName, true) as RichTextWidget;
            }
            return _hintTextWidget;
        }

        void ShowHint()
        {
            if (GetHintTitleWidget() is RichTextWidget nameWidget)
            {
                nameWidget.Text = Name;
            }
            if (GetHintTextWidget() is RichTextWidget hintWidget)
            {
                hintWidget.Text = Hint;
            }
        }

        void HideHint()
        {
            if (GetHintTitleWidget() is RichTextWidget nameWidget)
            {
                nameWidget.Text = "";
            }
            if (GetHintTextWidget() is RichTextWidget hintWidget)
            {
                hintWidget.Text = "";
            }
        }
    }
}
