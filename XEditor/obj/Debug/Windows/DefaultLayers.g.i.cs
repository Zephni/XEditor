﻿#pragma checksum "..\..\..\Windows\DefaultLayers.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "DF2C5F5A5A58EEAC28F0E8CE4AECCDC860782B60"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

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
using XEditor;


namespace XEditor {
    
    
    /// <summary>
    /// DefaultLayers
    /// </summary>
    public partial class DefaultLayers : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 21 "..\..\..\Windows\DefaultLayers.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox Layers;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\..\Windows\DefaultLayers.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox DefaultLayer;
        
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
            System.Uri resourceLocater = new System.Uri("/XEditor;component/windows/defaultlayers.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Windows\DefaultLayers.xaml"
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
            this.Layers = ((System.Windows.Controls.ListBox)(target));
            
            #line 21 "..\..\..\Windows\DefaultLayers.xaml"
            this.Layers.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.Layers_MouseDown);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 26 "..\..\..\Windows\DefaultLayers.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.ContextMenu_Add);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 27 "..\..\..\Windows\DefaultLayers.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.ContextMenu_Edit);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 28 "..\..\..\Windows\DefaultLayers.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.ContextMenu_Delete);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 29 "..\..\..\Windows\DefaultLayers.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.ContextMenu_MoveUp);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 30 "..\..\..\Windows\DefaultLayers.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.ContextMenu_MoveDown);
            
            #line default
            #line hidden
            return;
            case 7:
            this.DefaultLayer = ((System.Windows.Controls.ComboBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

