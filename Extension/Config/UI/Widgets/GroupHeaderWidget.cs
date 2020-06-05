using TaleWorlds.GauntletUI;

namespace Extension.Config.UI
{
    class GroupHeaderWidget : ButtonWidget
    {
        public string HintTitleWidgetName { get; set; }
        public string HintTextWidgetName { get; set; }

        RichTextWidget _hintTitleWidget;
        RichTextWidget _hintTextWidget;
        ListPanel _listPanel;
        ButtonWidget _collapseButton;
        TextWidget _groupNameWidget;
        string _hintTitle;
        string _hintText;
        bool _isGroupEnabled;
        bool HintActive;
        bool HasItems => ListPanel != null ? ListPanel.ChildCount > 0 && IsGroupEnabled : false;

        public GroupHeaderWidget(UIContext context)
            : base(context)
        {
            ClickEventHandlers.Add((Widget widget) => ToggleListPanelVisible());
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
            HintActive = true;
            if (GetHintTitleWidget() is RichTextWidget nameWidget)
            {
                nameWidget.Text = HintTitle;
            }
            if (GetHintTextWidget() is RichTextWidget hintWidget)
            {
                hintWidget.Text = HintText;
            }
        }

        void HideHint()
        {
            HintActive = false;
            if (GetHintTitleWidget() is RichTextWidget nameWidget)
            {
                nameWidget.Text = "";
            }
            if (GetHintTextWidget() is RichTextWidget hintWidget)
            {
                hintWidget.Text = "";
            }
        }

        void ToggleListPanelVisible()
        {
            if (HasItems)
            {
                ListPanel.IsVisible = !ListPanel.IsVisible;
            }
            UpdateStatus();
        }

        void ListPanelSizeChanged()
        {
            ListPanel.IsVisible = ListPanel.ChildCount > 0;
            UpdateStatus();
        }

        void UpdateStatus()
        {
            if (HasItems)
            {
                IsSelected = ListPanel.IsVisible;
                if (CollapseButton != null)
                {
                    CollapseButton.IsSelected = IsSelected;
                    CollapseButton.IsEnabled = true;
                }
                if (GroupNameWidget != null)
                {
                    GroupNameWidget.IsEnabled = true;
                }
            }
            else
            {
                IsSelected = ListPanel.IsVisible = false;
                if (CollapseButton != null)
                {
                    CollapseButton.IsEnabled = CollapseButton.IsSelected = false;
                }
                if (GroupNameWidget != null)
                {
                    GroupNameWidget.IsEnabled = false;
                }
            }
        }

        public string HintTitle
        {
            get => _hintTitle;
            set
            {
                if (_hintTitle != value)
                {
                    _hintTitle = value;
                    if (HintActive)
                    {
                        ShowHint();
                    }
                }
            }
        }

        public string HintText
        {
            get => _hintText;
            set
            {
                if (_hintText != value)
                {
                    _hintText = value;
                    if (HintActive)
                    {
                        ShowHint();
                    }
                }
            }
        }

        public bool IsGroupEnabled
        {
            get => _isGroupEnabled;
            set
            {
                if (_isGroupEnabled != value)
                {
                    _isGroupEnabled = value;
                    if (value)
                    {
                        ToggleListPanelVisible();
                    }
                    else
                    {
                        UpdateStatus();
                    }
                }
            }
        }

        public TextWidget GroupNameWidget
        {
            get => _groupNameWidget;
            set
            {
                if (_groupNameWidget != value)
                {
                    _groupNameWidget = value;
                    UpdateStatus();
                }
            }
        }

        public ListPanel ListPanel
        {
            get => _listPanel;
            set
            {
                if (_listPanel != value)
                {
                    _listPanel = value;
                    if (_listPanel != null)
                    {
                        ListPanel.ItemAfterRemoveEventHandlers.Add((Widget widget) => ListPanelSizeChanged());
                        ListPanel.ItemAddEventHandlers.Add((Widget parentWidget, Widget addedWidget) => ListPanelSizeChanged());
                        ListPanelSizeChanged();
                    }
                    else
                    {
                        UpdateStatus();
                    }
                }
            }
        }

        public ButtonWidget CollapseButton
        {
            get => _collapseButton;
            set
            {
                if (_collapseButton != value)
                {
                    _collapseButton = value;
                    if (_collapseButton != null)
                    {
                        CollapseButton.ClickEventHandlers.Add((Widget widget) => ToggleListPanelVisible());
                    }
                    UpdateStatus();
                }
            }
        }
    }
}
