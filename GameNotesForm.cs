using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;

using WinFormHtmlEditor;

namespace GameNotes
{
    public partial class GameNotesForm : Form
    {
        public static string PatchDirectory = AppDomain.CurrentDomain.BaseDirectory + @"Patches\GameNotes\";
        public static string NotesDirectory = PatchDirectory + @"Notes\";
        private List<string> Systems;
        private string GameName;
        private string SystemName;
        private TinyMCE TinyMceEditorGalaxy;
        private TinyMCE TinyMceEditorSystem;
        public GameNotesForm(string gameName, List<string> systems)
        {
            GameName = gameName;
            Systems = systems;
            if (systems.Count > 0) SystemName = systems[0];
            InitializeComponent();
            InitializeGameNotesFormComponent();
        }

        private void InitializeGameNotesFormComponent()
        {
            Name = "Game Notes for " + GameName;
            TinyMceEditorGalaxy = new TinyMCE(PatchDirectory + @"tinymce\");
            TinyMceEditorGalaxy.Dock = DockStyle.Fill;
            TinyMceEditorGalaxy.Name = "Aurora Galaxy Notes";
            TinyMceEditorGalaxy.HtmlContent = "";
            TinyMceEditorGalaxy.CreateEditor();
            galaxyTab.Controls.Add(TinyMceEditorGalaxy);
            TinyMceEditorSystem = new TinyMCE(PatchDirectory + @"tinymce\");
            TinyMceEditorSystem.Dock = DockStyle.Fill;
            TinyMceEditorSystem.Name = "Aurora System Notes";
            TinyMceEditorSystem.Padding = new Padding(0, 25, 0, 0);
            TinyMceEditorSystem.HtmlContent = "";
            TinyMceEditorSystem.CreateEditor();
            systemTab.Controls.Add(TinyMceEditorSystem);
            systemsDropdown.Items.AddRange(Systems.ToArray());
            Shown += GameNotesForm_Shown;
            FormClosing += GameNotesForm_FormClosing;
            gameNotesTabs.Selected += gameNotesTab_Selected;
        }

        private void GameNotesForm_FormClosing(object sender, EventArgs e)
        {
            SaveGalaxyNotes();
            SaveSystemNotes();
        }

        private void systemsDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveSystemNotes();
            SystemName = systemsDropdown.SelectedItem as string;
            LoadSystemNotes();
        }

        private void gameNotesTab_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPage.Name == "systemTab")
            {
                SaveGalaxyNotes();
                LoadSystemNotes();
            }
            else if (e.TabPage.Name == "galaxyTab")
            {
                SaveSystemNotes();
                LoadGalaxyNotes();
            }
        }

        private void GameNotesForm_Shown(object sender, EventArgs e)
        {
            LoadGalaxyNotes();
        }

        public void ShowGalaxyTab()
        {
            Show();
            gameNotesTabs.SelectTab(0);
        }

        public void ShowSystemTab(string systemName = null)
        {
            Show();
            gameNotesTabs.SelectTab(1);
            if (systemName != null)
            {
                systemsDropdown.SelectedItem = systemName;
                LoadSystemNotes();
            }
        }

        public void SaveGalaxyNotes()
        {
            if (string.IsNullOrEmpty(TinyMceEditorGalaxy.HtmlContent) || string.IsNullOrEmpty(GameName)) return;
            string gameNotesDirectory = NotesDirectory + GameName + "\\";
            if (!Directory.Exists(gameNotesDirectory)) Directory.CreateDirectory(gameNotesDirectory);
            File.WriteAllText(gameNotesDirectory + "galaxy.html", TinyMceEditorGalaxy.HtmlContent);
        }

        public void SaveSystemNotes()
        {
            if (string.IsNullOrEmpty(TinyMceEditorSystem.HtmlContent) ||
                string.IsNullOrEmpty(GameName)||
                string.IsNullOrEmpty(SystemName)) return;
            string gameNotesDirectory = NotesDirectory + GameName + "\\";
            if (!Directory.Exists(gameNotesDirectory)) Directory.CreateDirectory(gameNotesDirectory);
            File.WriteAllText(gameNotesDirectory + SystemName + ".html", TinyMceEditorSystem.HtmlContent);
        }

        public void LoadGalaxyNotes()
        {
            if (string.IsNullOrEmpty(GameName)) return;
            string gameNotesDirectory = NotesDirectory + GameName + "\\";
            if (!Directory.Exists(gameNotesDirectory)) Directory.CreateDirectory(gameNotesDirectory);
            if (File.Exists(gameNotesDirectory + "galaxy.html"))
            {
                TinyMceEditorGalaxy.HtmlContent = File.ReadAllText(gameNotesDirectory + "galaxy.html");
            }
        }

        public void LoadSystemNotes()
        {
            if (string.IsNullOrEmpty(GameName) || string.IsNullOrEmpty(SystemName)) return;
            string gameNotesDirectory = NotesDirectory + GameName + "\\";
            if (!Directory.Exists(gameNotesDirectory)) Directory.CreateDirectory(gameNotesDirectory);
            if (File.Exists(gameNotesDirectory + SystemName + ".html"))
            {
                systemsDropdown.SelectedItem = SystemName;
                TinyMceEditorSystem.HtmlContent = File.ReadAllText(gameNotesDirectory + SystemName + ".html");
                systemTab.Show();
            }
        }
    }
}
