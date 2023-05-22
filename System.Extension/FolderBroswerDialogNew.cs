﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace System
{
    public class FolderBrowserDialogNew
    {
        System.Windows.Forms.OpenFileDialog openFileDialog = null;
        public FolderBrowserDialogNew()
        {
            openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "Folders|\n";
            openFileDialog.AddExtension = false;
            openFileDialog.CheckFileExists = false;
            openFileDialog.DereferenceLinks = true;
            openFileDialog.Multiselect = Multiselect;
        }

        public string InitialDirectory
        {
            get { return openFileDialog.InitialDirectory; }
            set { openFileDialog.InitialDirectory = value == null || value.Length == 0 ? Environment.CurrentDirectory : value; }
        }

        public string Title
        {
            get { return openFileDialog.Title; }
            set { openFileDialog.Title = value == null ? "Select a folder" : value; }
        }

        public bool Multiselect
        {
            get { return openFileDialog.Multiselect; }
            set { openFileDialog.Multiselect = value == false ? false : value; }
        }

        public string FolderName
        {
            get { return openFileDialog.FileName; }
        }

        public string[] FolderNames
        {
            get { return openFileDialog.FileNames; }
        }

        public bool ShowDialog()
        {
            return ShowDialog(IntPtr.Zero);
        }

        public bool ShowDialog(IntPtr hWndOwner)
        {
            bool flag = false;

            if (Environment.OSVersion.Version.Major >= 6)
            {
                var r = new Reflector("System.Windows.Forms");
                uint num = 0;
                Type typeIFileDialog = r.GetType("FileDialogNative.IFileDialog");
                object dialog = r.Call(openFileDialog, "CreateVistaDialog");
                r.Call(openFileDialog, "OnBeforeVistaDialog", dialog);
                uint options = (uint)r.CallAs(typeof(System.Windows.Forms.FileDialog), openFileDialog, "GetOptions");
                options |= (uint)r.GetEnum("FileDialogNative.FOS", "FOS_PICKFOLDERS");
                r.CallAs(typeIFileDialog, dialog, "SetOptions", options);
                object pfde = r.New("FileDialog.VistaDialogEvents", openFileDialog);
                object[] parameters = new object[] { pfde, num };
                r.CallAs2(typeIFileDialog, dialog, "Advise", parameters);
                num = (uint)parameters[1];
                try
                {
                    int num2 = (int)r.CallAs(typeIFileDialog, dialog, "Show", hWndOwner);
                    flag = 0 == num2;
                }
                finally
                {
                    r.CallAs(typeIFileDialog, dialog, "Unadvise", num);
                    GC.KeepAlive(pfde);
                }
            }
            else
            {
                var fbd = new FolderBrowserDialog();
                fbd.Description = this.Title;
                fbd.SelectedPath = this.InitialDirectory;
                fbd.ShowNewFolderButton = false;
                if (fbd.ShowDialog(new WindowWrapper(hWndOwner)) != DialogResult.OK)
                    return false;
                openFileDialog.FileName = fbd.SelectedPath;
                flag = true;
            }
            return flag;
        }

        public class WindowWrapper : System.Windows.Forms.IWin32Window
        {
            public WindowWrapper(IntPtr handle)
            {
                _hwnd = handle;
            }
            public IntPtr Handle
            {
                get { return _hwnd; }
            }

            private IntPtr _hwnd;
        }

        public class Reflector
        {
            string m_ns;
            Assembly m_asmb;
            public Reflector(string ns)
                : this(ns, ns)
            { }
            public Reflector(string an, string ns)
            {
                m_ns = ns;
                m_asmb = null;
                foreach (AssemblyName aN in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
                {
                    if (aN.FullName.StartsWith(an))
                    {
                        m_asmb = Assembly.Load(aN);
                        break;
                    }
                }
            }
            public Type GetType(string typeName)
            {
                Type type = null;
                string[] names = typeName.Split('.');

                if (names.Length > 0)
                    type = m_asmb.GetType(m_ns + "." + names[0]);

                for (int i = 1; i < names.Length; ++i)
                {
                    type = type.GetNestedType(names[i], BindingFlags.NonPublic);
                }
                return type;
            }
            public object New(string name, params object[] parameters)
            {
                Type type = GetType(name);
                ConstructorInfo[] ctorInfos = type.GetConstructors();
                foreach (ConstructorInfo ci in ctorInfos)
                {
                    try
                    {
                        return ci.Invoke(parameters);
                    }
                    catch { }
                }

                return null;
            }
            public object Call(object obj, string func, params object[] parameters)
            {
                return Call2(obj, func, parameters);
            }
            public object Call2(object obj, string func, object[] parameters)
            {
                return CallAs2(obj.GetType(), obj, func, parameters);
            }
            public object CallAs(Type type, object obj, string func, params object[] parameters)
            {
                return CallAs2(type, obj, func, parameters);
            }
            public object CallAs2(Type type, object obj, string func, object[] parameters)
            {
                MethodInfo methInfo = type.GetMethod(func, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                return methInfo.Invoke(obj, parameters);
            }
            public object Get(object obj, string prop)
            {
                return GetAs(obj.GetType(), obj, prop);
            }
            public object GetAs(Type type, object obj, string prop)
            {
                PropertyInfo propInfo = type.GetProperty(prop, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                return propInfo.GetValue(obj, null);
            }
            public object GetEnum(string typeName, string name)
            {
                Type type = GetType(typeName);
                FieldInfo fieldInfo = type.GetField(name);
                return fieldInfo.GetValue(null);
            }
        }
    }
}
