﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MicroManager {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"
          <WindowPlacement xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
            <length>50</length>
            <flags>0</flags>
            <showCmd>1</showCmd>
            <minPosition>
              <X>0</X>
              <Y>0</Y>
            </minPosition>
            <maxPosition>
              <X>0</X>
              <Y>0</Y>
            </maxPosition>
            <normalPosition>
              <Left>800</Left>
              <Top>200</Top>
              <Right>1200</Right>
              <Bottom>800</Bottom>
            </normalPosition>
          </WindowPlacement>
        ")]
        public global::MicroManager.WindowPlacement WindowPlacement {
            get {
                return ((global::MicroManager.WindowPlacement)(this["WindowPlacement"]));
            }
            set {
                this["WindowPlacement"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public double Scale {
            get {
                return ((double)(this["Scale"]));
            }
            set {
                this["Scale"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool TopMost {
            get {
                return ((bool)(this["TopMost"]));
            }
            set {
                this["TopMost"] = value;
            }
        }
    }
}
