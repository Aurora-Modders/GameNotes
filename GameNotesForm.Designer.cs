
namespace GameNotes
{
    partial class GameNotesForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gameNotesTabs = new System.Windows.Forms.TabControl();
            this.galaxyTab = new System.Windows.Forms.TabPage();
            this.systemTab = new System.Windows.Forms.TabPage();
            this.systemsDropdown = new System.Windows.Forms.ComboBox();
            this.gameNotesTabs.SuspendLayout();
            this.systemTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // gameNotesTabs
            // 
            this.gameNotesTabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gameNotesTabs.Controls.Add(this.galaxyTab);
            this.gameNotesTabs.Controls.Add(this.systemTab);
            this.gameNotesTabs.Location = new System.Drawing.Point(0, 0);
            this.gameNotesTabs.Margin = new System.Windows.Forms.Padding(0);
            this.gameNotesTabs.Name = "gameNotesTabs";
            this.gameNotesTabs.SelectedIndex = 0;
            this.gameNotesTabs.Size = new System.Drawing.Size(624, 441);
            this.gameNotesTabs.TabIndex = 0;
            // 
            // galaxyTab
            // 
            this.galaxyTab.Location = new System.Drawing.Point(4, 22);
            this.galaxyTab.Name = "galaxyTab";
            this.galaxyTab.Padding = new System.Windows.Forms.Padding(3);
            this.galaxyTab.Size = new System.Drawing.Size(616, 415);
            this.galaxyTab.TabIndex = 0;
            this.galaxyTab.Text = "Galaxy";
            this.galaxyTab.UseVisualStyleBackColor = true;
            // 
            // systemTab
            // 
            this.systemTab.Controls.Add(this.systemsDropdown);
            this.systemTab.Location = new System.Drawing.Point(4, 22);
            this.systemTab.Name = "systemTab";
            this.systemTab.Padding = new System.Windows.Forms.Padding(3);
            this.systemTab.Size = new System.Drawing.Size(616, 415);
            this.systemTab.TabIndex = 1;
            this.systemTab.Text = "System";
            this.systemTab.UseVisualStyleBackColor = true;
            // 
            // systemsDropdown
            // 
            this.systemsDropdown.Dock = System.Windows.Forms.DockStyle.Top;
            this.systemsDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.systemsDropdown.FormattingEnabled = true;
            this.systemsDropdown.ItemHeight = 13;
            this.systemsDropdown.Location = new System.Drawing.Point(3, 3);
            this.systemsDropdown.MaxDropDownItems = 25;
            this.systemsDropdown.Name = "systemsDropdown";
            this.systemsDropdown.Size = new System.Drawing.Size(610, 21);
            this.systemsDropdown.TabIndex = 0;
            this.systemsDropdown.SelectedIndexChanged += new System.EventHandler(this.systemsDropdown_SelectedIndexChanged);
            // 
            // GameNotesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 441);
            this.Controls.Add(this.gameNotesTabs);
            this.Name = "GameNotesForm";
            this.Text = "Game Notes";
            this.gameNotesTabs.ResumeLayout(false);
            this.systemTab.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl gameNotesTabs;
        private System.Windows.Forms.TabPage systemTab;
        private System.Windows.Forms.TabPage galaxyTab;
        public System.Windows.Forms.ComboBox systemsDropdown;
    }
}