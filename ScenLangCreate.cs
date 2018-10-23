// Lic:
// 	ScenLang - Creation Wizard
// 	
// 	
// 	
// 	
// 	(c) Jeroen P. Broks, 2018, All rights reserved
// 	
// 		This program is free software: you can redistribute it and/or modify
// 		it under the terms of the GNU General Public License as published by
// 		the Free Software Foundation, either version 3 of the License, or
// 		(at your option) any later version.
// 		
// 		This program is distributed in the hope that it will be useful,
// 		but WITHOUT ANY WARRANTY; without even the implied warranty of
// 		MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// 		GNU General Public License for more details.
// 		You should have received a copy of the GNU General Public License
// 		along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 		
// 	Exceptions to the standard GNU license are available with Jeroen's written permission given prior 
// 	to the project the exceptions are needed for.
// Version: 18.10.23
// EndLic
﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Gtk;
using TrickyUnits;
using TrickyUnits.GTK;
using UseJCR6;

namespace ScenLangCreate
{
    class MainClass
    {
        static Dictionary<string, Entry> Data = new Dictionary<string, Entry>();
        static Dictionary<Entry, string> DataID = new Dictionary<Entry, string>();
        static VBox workbox;
        static TextView Languages;

        static void LoadMascot(Image m){
            Assembly asm = Assembly.GetExecutingAssembly();
            var stream = asm.GetManifestResourceStream("ScenLangCreate.Mascot.Mascot.png");
            var pb = new Gdk.Pixbuf(stream);
            stream.Dispose();
            m.Pixbuf = pb;
            m.SetAlignment(0, 1);

        }

        static void OnBrowse(object sender,EventArgs e){
            Data["Project"].Text = QuickGTK.RequestDir("Please select a directory to store this project in");
        }
        static void OnEntry(object sender,EventArgs e){}
        static void OnGoForIt(object sender,EventArgs e){
            var outdir = Data["Project"].Text;
            var prj = System.IO.Path.GetFileName(outdir);
            var langs = Languages.Buffer.Text.Split('\n');
            var gini = new TGINI();
            gini.D("Author", Data["Author"].Text);
            gini.D("Copyright", Data["Author"].Text);
            gini.D("Notes", Data["License"].Text);
            gini.D("License", Data["License"].Text);
            gini.D("lzma", "YES");
            var outt = "[rem]\nEmpty now\n[tags]\n\n[scenario]\n\n";
            var li = 0;
            if (langs.Length < 1) { QuickGTK.Error("At least one language is required to do the job!"); return; }
            foreach(string flang in langs){
                li++;
                Console.WriteLine($"Creating language #{li}: {flang}");
                var lang = flang.Trim(); gini.D($"Lang{li}.Name", lang);
                var outj = outdir + "/" + lang + ".jcr"; gini.D($"Lang{li}.File", outj);
                var jo = new TJCRCreate(outj, "lzma");
                jo.AddString(outt, "BASICENTRY", "lzma", Data["Author"].Text, Data["License"].Text);
                jo.Close();
            }
            Console.WriteLine("Creating project GINI");
            gini.SaveSource(outdir + "/" + prj + ".scenlang.gini");
            QuickGTK.Info("Project has been created.\nYou can now use the regular ScenLang tool and open project file:\n\n" + outdir + "/" + prj + ".scenlang.gini");
            Application.Quit();
        }

        public static void AddEntry(string id, string caption, bool browse=false){
            var label = new Label(caption); label.ModifyFg(StateType.Normal, new Gdk.Color(255, 255, 0));
            var entry = new Entry();
            Button browsebutton;
            entry.ModifyText(StateType.Normal, new Gdk.Color(0, 180, 255));
            entry.ModifyBase(StateType.Normal, new Gdk.Color(0, 18, 25));
            label.SetSizeRequest(300, 30);
            label.SetAlignment(0, 0);
            var mybox = new HBox();
            mybox.Add(label);
            mybox.Add(entry);
            if (browse)
            {
                entry.SetSizeRequest(250, 30);
                browsebutton = new Button("...");
                browsebutton.SetSizeRequest(50, 30);
                browsebutton.ModifyBg(StateType.Normal, new Gdk.Color(255, 255, 0));
                browsebutton.ModifyBg(StateType.Active, new Gdk.Color(255, 180, 0));
                browsebutton.ModifyBg(StateType.Prelight, new Gdk.Color(255, 255, 180));
                browsebutton.Clicked += OnBrowse;
                mybox.Add(browsebutton);
            }
            else
            {
                entry.SetSizeRequest(300, 30);
            }
            Data[id] = entry;
            DataID[entry] = id;
            entry.Changed += OnEntry;
            mybox.SetSizeRequest(600, 30);
            workbox.Add(mybox);
        }

        public static void Main(string[] args)
        {
            MKL.Version("ScenLang - Creation Wizard - ScenLangCreate.cs","18.10.23");
            MKL.Lic    ("ScenLang - Creation Wizard - ScenLangCreate.cs","GNU General Public License 3");
            JCR6_lzma.Init();
            Application.Init();
            MainWindow win = new MainWindow();
            win.ModifyBg(StateType.Normal, new Gdk.Color(0, 0, 25));
            win.Title = "ScenLang Creator - version " + MKL.Newest;
            win.SetSizeRequest(1000, 500);
            var mainbox = new HBox(); win.Add(mainbox);
            mainbox.SetSizeRequest(400, 500);
            var mascot = new Image();
            mascot.SetAlignment(0, 1);
            mainbox.Add(mascot);
            LoadMascot(mascot);
            mascot.SetSizeRequest(400, 500);
            workbox = new VBox(); mainbox.Add(workbox);
            workbox.SetSizeRequest(600, 500);
            AddEntry("Project", "Project directory:",true);
            AddEntry("Author", "Author:");
            AddEntry("License", "License:");
            var langbox = new HBox();
            langbox.SetSizeRequest(600, 380);
            var langlabel = new Label("Laguages:\n(Put every language on a new line)");
            langlabel.SetSizeRequest(300, 380);
            langlabel.SetAlignment(0, 0);
            langlabel.ModifyFg(StateType.Normal, new Gdk.Color(255, 255, 0));
            var scroll = new ScrolledWindow();
            Languages = new TextView();
            scroll.Add(Languages);
            scroll.SetSizeRequest(300, 380);
            Languages.ModifyBase(StateType.Normal, new Gdk.Color(0, 18, 25));
            Languages.ModifyText(StateType.Normal, new Gdk.Color(0, 180, 255));
            langbox.Add(langlabel);
            langbox.Add(scroll);
            workbox.Add(langbox);
            var finbox = new HBox();
            var goforit = new Button("Create Project");
            finbox.Add(new HBox());
            finbox.Add(goforit);
            goforit.Clicked += OnGoForIt;
            workbox.Add(finbox);
            win.ShowAll();
            Application.Run();
        }
    }
}
