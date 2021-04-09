using RectanglePackerWindow.Utilities;
using System;
using System.Reflection;
using System.Windows;

namespace RectanglePackerWindow
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public string Version { get; private set; }
        public string TaggedVersion { get; private set; }

        public App()
        {
            WindowUtils.SetMenuAlignment();

            Assembly assembly = Assembly.GetExecutingAssembly();
            Version v = assembly.GetName().Version;
            Version = string.Format("{0}.{1}.{2}", v.Major, v.Minor, v.Build);

            object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            if (attributes.Length > 0)
            {
                TaggedVersion = ((AssemblyProductAttribute)attributes[0]).Product.Trim();
                if (TaggedVersion.Contains(" "))
                {
                    string[] tagArr = TaggedVersion.Split(' ');
                    TaggedVersion = tagArr[tagArr.Length - 1];
                }
            }
            else
            {
                TaggedVersion = "v" + Version;
            }
        }
    }
}