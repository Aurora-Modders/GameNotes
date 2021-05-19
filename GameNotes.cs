using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using HarmonyLib;

namespace GameNotes
{
    public class GameNotes : AuroraPatch.Patch
    {
        public override string Description => "Simple note taking patch";
        public override IEnumerable<string> Dependencies => new[] { "Lib" };
        private static bool TacticalMapButtonAdded = false;
        private static bool GalacticMapButtonAdded = false;
        private static ComboBox systems;
        private static GameNotesForm gameNotesForm;
        private static Lib.Lib lib;

        protected override void Loaded(Harmony harmony)
        {
            // Hooking into the ResumeLayout method in order to add our game notes button before layout panels are completed.
            HarmonyMethod resumeLayoutPrefixMethod = new HarmonyMethod(GetType().GetMethod("ResumeLayoutPrefix", AccessTools.all));
            foreach (var method in typeof(Control).GetMethods(AccessTools.all))
            {
                if (method.Name.Contains("ResumeLayout"))
                {
                    try
                    {
                        harmony.Patch(method, prefix: resumeLayoutPrefixMethod);
                    }
                    catch (Exception e)
                    {
                        LogError($"Failed to patch ResumeLayout for Control, exception: {e}");
                    }
                }
            }
            // Hook into DrawLine and DrawString methods so that we can move the distance indicator/text in order to make room for our game notes button.
            HarmonyMethod drawLinePrefixMethod = new HarmonyMethod(GetType().GetMethod("DrawLinePrefix", AccessTools.all));
            HarmonyMethod drawStringPrefixMethod = new HarmonyMethod(GetType().GetMethod("DrawStringPrefix", AccessTools.all));
            foreach (var method in typeof(Graphics).GetMethods(AccessTools.all))
            {
                if (method.Name == "DrawLine")
                {
                    var parameters = method.GetParameters();
                    if (parameters.Length == 5 && parameters[1].ParameterType == typeof(int))
                    {
                        try
                        {
                            harmony.Patch(method, prefix: drawLinePrefixMethod);
                        }
                        catch (Exception e)
                        {
                            LogError($"Failed to patch DrawLine for Graphics, exception: {e}");
                        }
                    }
                }
                else if (method.Name == "DrawString")
                {
                    var parameters = method.GetParameters();
                    if (parameters.Length == 5 && parameters[4].ParameterType == typeof(float))
                    {
                        try
                        {
                            harmony.Patch(method, prefix: drawStringPrefixMethod);
                        }
                        catch (Exception e)
                        {
                            LogError($"Failed to patch DrawString for Graphics, exception: {e}");
                        }
                    }
                }
            }
        }

        protected override void Started()
        {
            // We need our Lib depencency reference.
            lib = GetDependency<Lib.Lib>("Lib");
            // Keep track of the systems dropdown control on the TacticalMap.
            FindSystemsDropdown();
            // Prepare our notes form and load our previously saved notes.
            ReloadGameNotesForm();
        }

        /// <summary>
        /// Will iterate over all TacticalMap controls and store our systems dropdown control.
        /// This is needed to know which system notes to load up when a user opens up the
        /// game notes form from the TacticalMap.
        /// </summary>
        private void FindSystemsDropdown()
        {
            foreach (Control control in TacticalMap.Controls)
            {
                if (control.Name == "cboSystems")
                {
                    systems = control as ComboBox;
                    break;
                }
            }
        }

        /// <summary>
        /// Check if we're dealing with the distance indicator line and move it to the right
        /// to make room for our game notes button.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        private static void DrawLinePrefix(ref int x1, ref int x2)
        {
            if (x1 == 1306 && x2 == 1456)
            {
                x1 = 1354;
                x2 = 1504;
            }
        }

        /// <summary>
        /// Check if we're dealing with the distance indicator string and move it to the right
        /// to make room for our game notes button.
        /// </summary>
        /// <param name="x"></param>
        private static void DrawStringPrefix(ref float x)
        {
            if (x == 1306f)
            {
                x = 1354f;
            }
        }

        /// <summary>
        /// Our ResumeLayout prefix hook is used to add our game notes button.
        /// We need to find our main toolbar, expand it horizontally a bit, and add our button.
        /// This hook is called recursively - we prevent infinite loops by keeping track of
        /// it's execution for tactical map and galactic map. Both controls have the same name
        /// so they are identified using the tab index property.
        /// </summary>
        /// <param name="__instance"></param>
        private static void ResumeLayoutPrefix(Control __instance)
        {
            if (__instance.Name == "tlbMainToolbar")
            {
                if (__instance.TabIndex < 10 && !TacticalMapButtonAdded)
                {
                    TacticalMapButtonAdded = true;
                    __instance.Size = new Size(__instance.Size.Width + 48, __instance.Size.Height);
                    __instance.Controls.Add(GetGameNotesButton(true));
                }
                else if (__instance.TabIndex >= 10 && !GalacticMapButtonAdded)
                {
                    GalacticMapButtonAdded = true;
                    __instance.Size = new Size(__instance.Size.Width + 48, __instance.Size.Height);
                    __instance.Controls.Add(GetGameNotesButton(false));
                }
            }
        }

        /// <summary>
        /// Helper method to create a game notes button.
        /// </summary>
        /// <param name="tacticalMap"></param>
        /// <returns></returns>
        private static Button GetGameNotesButton(bool tacticalMap)
        {
            var button = new Button();
            button.Name = "cmdGameNotes";
            button.AccessibleName = "Game Notes";
            button.Size = new Size(48, 48);
            button.Location = new Point(1296, 0);
            button.BackgroundImage = Image.FromFile(@"Patches\GameNotes\notes.png");
            button.BackgroundImageLayout = ImageLayout.Stretch;
            button.Margin = new Padding(0);
            button.TabIndex = 26;
            button.UseVisualStyleBackColor = true;
            if (tacticalMap)
            {
                button.Click += TacticalGameNotesClick;
            }
            else
            {
                button.Click += GalacticGameNotesClick;
            }
            return button;
        }

        /// <summary>
        /// Helper method to ensure the game notes form object exists and is visible.
        /// </summary>
        private static void ReloadGameNotesForm()
        {
            if (gameNotesForm != null && !gameNotesForm.IsDisposed)
            {
                gameNotesForm.WindowState = FormWindowState.Normal;
                gameNotesForm.BringToFront();
            }
            else if (lib != null)
            {
                var gameState = lib.KnowledgeBase.GetGameState(lib.TacticalMap);
                var gameName = GetGameName(gameState);
                if (gameName != null)
                {
                    gameNotesForm = new GameNotesForm(gameName, GetSystemNames(gameName));
                }
            }
        }

        /// <summary>
        /// Helper method that returns a list of system names for the game name specified.
        /// Will query the DB for the list of systems discovered by the player race in the game.
        /// </summary>
        /// <param name="gameName"></param>
        /// <returns></returns>
        private static List<string> GetSystemNames(string gameName)
        {
            List<string> systems = new List<string>();
            try
            {
                var query = lib.DatabaseManager.ExecuteQuery(
                    "SELECT FCT_RaceSysSurvey.Name " +
                    "FROM FCT_Game LEFT JOIN FCT_RaceSysSurvey ON FCT_Game.GameID = FCT_RaceSysSurvey.GameID " +
                    "              LEFT JOIN FCT_Race ON FCT_RaceSysSurvey.RaceID = FCT_Race.RaceID " +
                    "WHERE FCT_Race.NPR = 0 AND FCT_Game.GameName = '" + gameName + "'"
                );
                foreach (System.Data.DataRow system in query.Rows)
                {
                    systems.Add(system[0] as string);
                }
            }
            catch (Exception ex)
            {
                // Can't use logging statements in static method.
                MessageBox.Show("GameNotes Patch - Failed to load system names from the DB for the current game: " + ex.ToString());
            }
            return systems;
        }

        /// <summary>
        /// Helper method to get the current game name.
        /// Requires the game state object.
        /// </summary>
        /// <param name="gameState"></param>
        /// <returns></returns>
        private static string GetGameName(object gameState)
        {
            var gameNameField = gameState.GetType().GetFields(AccessTools.all).SingleOrDefault(f => f.Name == "fw");
            if (gameNameField != null)
            {
                string value = gameNameField.GetValue(gameState) as string;
                return gameNameField.GetValue(gameState) as string;
            }
            else
            {
                var potentialGameNameFields = gameState.GetType().GetFields(AccessTools.all).Where(f => f.GetType() == typeof(string));
                foreach (var potentialGameNameField in potentialGameNameFields)
                {
                    string value = potentialGameNameField.GetValue(gameState) as string;
                    if (!string.IsNullOrEmpty(value) && value != "Aurora.exe")
                    {
                        return value;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// TacticalMap click callback for our game notes button.
        /// Will load the appropriate system notes and display them.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void TacticalGameNotesClick(object sender, EventArgs e)
        {
            ReloadGameNotesForm();
            string system = null;
            if (systems != null) system = systems.SelectedItem as string;
            gameNotesForm.ShowSystemTab(system);
        }

        /// <summary>
        /// GalacticMap click callback for our game notes button.
        /// Will load the galaxy notes and display them.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void GalacticGameNotesClick(object sender, EventArgs e)
        {
            ReloadGameNotesForm();
            gameNotesForm.ShowGalaxyTab();
        }
    }
}
