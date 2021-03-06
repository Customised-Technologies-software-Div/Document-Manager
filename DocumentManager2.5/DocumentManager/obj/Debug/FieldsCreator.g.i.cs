﻿#pragma checksum "..\..\FieldsCreator.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "7694CE3F255AED3A12B8797CB79FF4F03D97122FB18F7F3B98C398412DAEC13A"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using DocumentManager;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using unvell.ReoGrid;


namespace DocumentManager {
    
    
    /// <summary>
    /// FieldsCreator
    /// </summary>
    public partial class FieldsCreator : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 36 "..\..\FieldsCreator.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtTemplateName;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\FieldsCreator.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtSelectTemplate;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\FieldsCreator.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal unvell.ReoGrid.ReoGridControl reoGridControl;
        
        #line default
        #line hidden
        
        
        #line 56 "..\..\FieldsCreator.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cmbFieldNames;
        
        #line default
        #line hidden
        
        
        #line 59 "..\..\FieldsCreator.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtRow;
        
        #line default
        #line hidden
        
        
        #line 61 "..\..\FieldsCreator.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtColumn;
        
        #line default
        #line hidden
        
        
        #line 63 "..\..\FieldsCreator.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtSheet;
        
        #line default
        #line hidden
        
        
        #line 65 "..\..\FieldsCreator.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock lblFieldError;
        
        #line default
        #line hidden
        
        
        #line 76 "..\..\FieldsCreator.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid fieldDataGrid;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/DocumentManager;component/fieldscreator.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\FieldsCreator.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.txtTemplateName = ((System.Windows.Controls.TextBox)(target));
            return;
            case 2:
            this.txtSelectTemplate = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            
            #line 39 "..\..\FieldsCreator.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.btnSelectTemplateFile_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.reoGridControl = ((unvell.ReoGrid.ReoGridControl)(target));
            
            #line 42 "..\..\FieldsCreator.xaml"
            this.reoGridControl.CurrentWorksheetChanged += new System.EventHandler(this.reoGridControl_CurrentWorksheetChanged);
            
            #line default
            #line hidden
            return;
            case 5:
            this.cmbFieldNames = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 6:
            this.txtRow = ((System.Windows.Controls.TextBox)(target));
            return;
            case 7:
            this.txtColumn = ((System.Windows.Controls.TextBox)(target));
            return;
            case 8:
            this.txtSheet = ((System.Windows.Controls.TextBox)(target));
            return;
            case 9:
            this.lblFieldError = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 10:
            
            #line 66 "..\..\FieldsCreator.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.btnAddField_Click);
            
            #line default
            #line hidden
            return;
            case 11:
            
            #line 67 "..\..\FieldsCreator.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.btnResetFieldsInfo_Click);
            
            #line default
            #line hidden
            return;
            case 12:
            this.fieldDataGrid = ((System.Windows.Controls.DataGrid)(target));
            
            #line 76 "..\..\FieldsCreator.xaml"
            this.fieldDataGrid.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.fieldDataGrid_MouseLeftButtonUp);
            
            #line default
            #line hidden
            
            #line 76 "..\..\FieldsCreator.xaml"
            this.fieldDataGrid.Loaded += new System.Windows.RoutedEventHandler(this.fieldDataGrid_Loaded);
            
            #line default
            #line hidden
            return;
            case 13:
            
            #line 78 "..\..\FieldsCreator.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.saveButton_Click);
            
            #line default
            #line hidden
            return;
            case 14:
            
            #line 79 "..\..\FieldsCreator.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.cancelButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

