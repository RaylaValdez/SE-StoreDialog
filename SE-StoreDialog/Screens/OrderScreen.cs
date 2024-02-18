using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace IngameScript.Screens
{
    internal class OrderScreen
    {

        public static void DrawScreen(MySpriteDrawFrame frame, Dictionary<string, int> stockListing, string newArg, ref int screen, ref bool fix)
        {
            Drawing.DrawBackground(Color.Black, ref frame);
            Drawing.DrawRoundedRect(ref frame, 20, 10, 512 - 40, 512 - 445, 4, Color.White);
            Drawing.DrawText(ref frame, 256, 15, "Please input", Color.White, 0.9f, TextAlignment.CENTER, "White");
            Drawing.DrawText(ref frame, 256, 40, "Your desired quantity of components", Color.White, 0.9f, TextAlignment.CENTER, "White");


            Drawing.DrawRoundedRect(ref frame, 20, 512 - 427, 512 - 40, 512 - 330, 4, Color.White);

            Drawing.DrawRoundedRect(ref frame, 512 - 170, 95, 140, 512 - 410, 4, Color.White);

            int drawnOrder = 0;
            int quantDrawnOrder = 0;

            // Draw the labels for the stocks.
            foreach (string i in Program.stock)
            {

                if (Program.selected == i)
                {
                    Drawing.DrawText(ref frame, 120, 105 + drawnOrder, ">" + i, Color.White, 0.7f, TextAlignment.CENTER, "White");
                }
                else
                {
                    Drawing.DrawText(ref frame, 120, 105 + drawnOrder, i, Color.White, 0.7f, TextAlignment.CENTER, "White");
                }
                drawnOrder += 25;
            }


            // Draw the Desired order, should be 0,0,0 on beginning
            foreach (var item in Program.desiredStock)
            {
                if (Program.selected == item.Key.ToString())
                {
                    Drawing.DrawText(ref frame, 512 - 102, 105 + quantDrawnOrder, ">" + item.Value.ToString(), Color.White, 0.7f, TextAlignment.CENTER, "White");
                }
                else
                {
                    Drawing.DrawText(ref frame, 512 - 102, 105 + quantDrawnOrder, item.Value.ToString(), Color.White, 0.7f, TextAlignment.CENTER, "White");
                }
                quantDrawnOrder += 25;
            }

            // Switch statement for argument handling before selecting a value to change


            // GRRR WHY THE FUCK ISNT THIS UPDATING THE SELECTED VALUE WHEN U INCREMENT IT IANJOISDNGAOIKSNGMOIKSNGMAOKGN

            switch (newArg) // Check the value of newArg
            {
                case "next": // If newArg is "next"
                    if (!Program.shouldEdit) // If shouldEdit is false
                    {
                        int newIndex = Program.stock.IndexOf(Program.selected) + 1; // Get the index of the next item in stock
                        if (newIndex >= Program.stock.Count) // If the new index is out of bounds
                            newIndex = 0; // Wrap around to the beginning
                        Program.selected = Program.stock[newIndex]; // Update the selected item
                    }
                    else // If shouldEdit is true
                    {
                        switch (Program.selected) // Check the value of selected
                        {
                            case "EmagnetAWE":
                                Program.desiredStock["EmagnetAWE"] += 1000;

                                break;

                            case "MilPlateAWE":
                                Program.desiredStock["MilPlateAWE"] += 1000;
                                break;

                            case "SchargerAWE":
                                Program.desiredStock["SchargerAWE"] += 1000;
                                break;
                        }
                    }
                    break;
                case "prev": // If newArg is "prev"
                    if (!Program.shouldEdit) // If shouldEdit is false
                    {
                        int newIndex = Program.stock.IndexOf(Program.selected) - 1; // Get the index of the previous item in stock
                        if (newIndex <= -1) // If the new index is out of bounds
                            newIndex = Program.stock.Count - 1; // Wrap around to the end
                        Program.selected = Program.stock[newIndex]; // Update the selected item
                    }
                    else // If shouldEdit is true
                    {
                        switch (Program.selected) // Check the value of selected
                        {
                            case "EmagnetAWE":
                                Program.desiredStock["EmagnetAWE"] -= 1000;
                                break;

                            case "MilPlateAWE":
                                Program.desiredStock["MilPlateAWE"] -= 1000;
                                break;

                            case "SchargerAWE":
                                Program.desiredStock["SchargerAWE"] -= 1000;
                                break;
                        }
                    }
                    break;
                case "back": // If newArg is "back"
                    if (!Program.shouldEdit) // If shouldEdit is false
                    {
                        screen = 0; // Set the screen to 0
                    }
                    else // If shouldEdit is true
                    {
                        Program.selected = Program.stock[0]; // Reset selected to the first item in stock
                        Program.shouldEdit = false; // Set shouldEdit to false
                    }
                    break;
                case "sel": // If newArg is "sel"
                    if (!Program.shouldEdit) // If shouldEdit is false
                    {
                        switch (Program.selected) // Check the value of selected
                        {
                            case "ElectromagnetAWE": // If selected is "ElectromagnetAWE"

                                Program.shouldEdit = true; // Set shouldEdit to true
                                Program.selected = "EmagnetAWE";
                                break;
                            case "MilitaryPlateAWE": // If selected is "MilitaryPlateAWE"
                                Program.selected = "MilPlateAWE";
                                Program.shouldEdit = true; // Set shouldEdit to true
                                break;
                            case "SuperchargerAWE": // If selected is "SuperchargerAWE"
                                Program.selected = "SchargerAWE";
                                Program.shouldEdit = true; // Set shouldEdit to true
                                break;
                                // Add more cases as needed
                        }
                    }
                    break;
            }
        }

    }
}
