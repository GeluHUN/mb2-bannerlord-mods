using TaleWorlds.GauntletUI;

namespace Extension.Config.UI
{
    class ExtTabToggleWidget : TabToggleWidget
    {
        public string CategoryName { get; set; }
        public string CategoryHint { get; set; }
        public string HintTitleWidgetName { get; set; }
        public string HintTextWidgetName { get; set; }

        RichTextWidget _hintTitleWidget;
        RichTextWidget _hintTextWidget;

        public ExtTabToggleWidget(UIContext context)
            : base(context)
        {
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
                nameWidget.Text = CategoryName;
            }
            if (GetHintTextWidget() is RichTextWidget hintWidget)
            {
                hintWidget.Text = CategoryHint;
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
