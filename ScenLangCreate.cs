using System;
using Gtk;

namespace ScenLangCreate
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Application.Init();
            MainWindow win = new MainWindow();
            win.ModifyBg(StateType.Normal, new Gdk.Color(0, 0, 25));
            win.Title = "ScenLang Creator";
            win.Show();
            Application.Run();
        }
    }
}
