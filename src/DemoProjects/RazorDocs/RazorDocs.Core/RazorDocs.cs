﻿using System;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Word;
using Office.Contrib.RibbonFactory.Interfaces;
using Office.Contrib.RibbonFactory.Internal;
using Office.Contrib.Wpf;
using Office.Word.Contrib.RibbonFactory;

namespace RazorDocs.Core
{
    [WordRibbonViewModel]
    public class RazorDocs : OfficeViewModelBase, IRibbonViewModel, IRegisterCustomTaskPane
    {
        private bool _panelShown;
        private Document _document;
        private ICustomTaskPaneWrapper _razorDocsTaskPane;

        public void Initialised(object context)
        {
            _document = (Document) context;
        }

        public void CurrentViewChanged(object currentView)
        {
        }

        public IRibbonUI RibbonUi { get; set; }

        public bool PanelShown
        {
            get { return _panelShown; }
            set
            {
                if (_panelShown == value) return;
                _panelShown = value;
                _razorDocsTaskPane.Visible = value;
                RaisePropertyChanged(()=>PanelShown);
            }
        }

        public void RegisterTaskPanes(Register register)
        {
            _razorDocsTaskPane = register(
                ()=>new WpfPanelHost
                {
                    Child = new RazorDocsPanel
                    {
                        DataContext = new RazorDocsPanelViewModel(this)
                    }
                }, "RazorDocs");
            _razorDocsTaskPane.Visible = true;
            PanelShown = true;
            _razorDocsTaskPane.VisibleChanged += TwitterTaskPaneVisibleChanged;
            TwitterTaskPaneVisibleChanged(this, EventArgs.Empty);
        }

        public void Cleanup()
        {
            _razorDocsTaskPane.VisibleChanged -= TwitterTaskPaneVisibleChanged;
        }

        private void TwitterTaskPaneVisibleChanged(object sender, EventArgs e)
        {
            _panelShown = _razorDocsTaskPane.Visible;
            RaisePropertyChanged(()=>PanelShown);
        }
    }
}
