namespace BFIDE
{
    using System;
    using System.Reflection;
    using System.Windows;
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            AssemblyName asmname = Assembly.GetExecutingAssembly().GetName();
            Version v = asmname.Version;

            this.TBXVersion.Text = string.Format("Version {0}", v);
            this.TBXInfo.Text = "BFIDE is licensed under The MIT License. \n\n" + Properties.Resources.MITLicense +
                "\n\nBrainfuck IDE uses AvalonEdit as the code editor box, which is licensed under LGPL License:\n" +
                "https://github.com/icsharpcode/SharpDevelop/wiki/AvalonEdit \n\n";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
