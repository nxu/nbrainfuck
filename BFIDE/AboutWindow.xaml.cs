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
            this.TBXInfo.Text = 
                "nBrainfuck is a collection of freeware " +
                "and partly open source softwares written by nXu. Brainfuck Developer (a Brainfuck IDE) is a " +
                "part of it, so all licenses and terms of nBrainfuck apply to it.\n" +
                "TLDR: Feel free to use and share the software - for free. Have fun with it! ;)\n\n" +
                "Brainfuck Developer uses AvalonEdit for code editor box:\n" +
                "http://www.codeproject.com/Articles/42490/Using-AvalonEdit-WPF-Text-Editor \n\n" + 
                "(c) 2012 nXu - http://brainfuck.nXu.hu";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
