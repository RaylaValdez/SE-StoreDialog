using IngameScript.Screens;
using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;
using VRageRender;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {

        // This script uses 4 arguments
        // All arguments are as follows
        // back - return to previous selection or screen if selection has no path further back
        // prev - scroll backward thru available selections && decrease active selection incrementally
        // sel - make a selection based on the highlighted item (signified with '>')
        // next - scroll forward thru available selections && increase active selection incrementally


        // The LCD We're going to be drawing to
        public static IMyTextPanel drawingPanel;
        // The lcd we're getting information of stock availability from 
        public static IMyTextPanel infoPanel;
        // The cargo container we're going to be editing the custom data of
        public static IMyCargoContainer cargoContainer;
        // The buttons for buttoninging
        public static IMyButtonPanel buttonPanel;
        public static IMyTextSurface bP1;
        public static IMyTextSurface bP2;
        public static IMyTextSurface bP3;
        public static IMyTextSurface bP4;
        // It's Text Surface Provider
        public static IMyTextSurfaceProvider textSurfaceProvider;

        // drawingPanel's viewport, available drawing space in the drawingPanel
        public static RectangleF viewport;
        // int value declaring the 'screen' we're on, used for a switch statement to change screens
        public static int screen;
        // string value storing what we're currently selecting
        public static string selected;
        // List declaring which stocks we're selling, used for GetAvailableStockForComp()
        public static List<string> stock = new List<string>();
        // Bool for localising next/prev actions when selecting different things
        public static bool shouldEdit;
        // Dictionary for availableStock and price
        public static Dictionary<string, int> stockListing;
        // Dictionary for setting desired quantities depending on their key
        public static Dictionary<string, int> desiredStock = new Dictionary<string, int>();
        // Custom Data of pb
        public static string customData;
        // frame
        public static MySpriteDrawFrame frame;


        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update10;

            // PB has ran for first time since compile, set defaults
            screen = 0;
            selected = string.Empty;

            shouldEdit = false;
            stock.Add("ElectromagnetAWE");
            stock.Add("MilitaryPlateAWE");
            stock.Add("SuperchargerAWE");

            desiredStock.Add("EmagnetAWE", 0);
            desiredStock.Add("MilPlateAWE", 0);
            desiredStock.Add("SchargerAWE", 0);

            selected = stock[0];

            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();

            GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(blocks);

            List<IMyButtonPanel> buttons = new List<IMyButtonPanel>();
            GridTerminalSystem.GetBlocksOfType(buttons);

            List<IMyTextPanel> drawingPanels = new List<IMyTextPanel>();

            // Initialize the dictionary for stock and prices
            stockListing = new Dictionary<string, int>();

            // Get the custom data of the Programmable Block
            customData = Me.CustomData;

            // Parse the custom data to populate the stockListing dictionary
            ParseCustomData(customData);

            #region Drawing Panels
            foreach (IMyTextPanel block in blocks)
            {
                // Check if the block reference is not null
                if (block != null)
                {
                    // Check if the block is functional and accessible
                    if (block.IsFunctional && block.IsWorking)
                    {
                        // Check if the custom name of the block contains the string "[StoreLCD]"
                        if (block.CustomName.ToLower().Contains("[StoreLCD]".ToLower()))
                        {
                            // Add the block to the list of drawing panels
                            drawingPanels.Add(block);
                        }
                    }
                }
            }

            // Check if any drawing panels were found
            if (drawingPanels.Count == 0)
            {
                Echo("No drawing panels found with the specified criteria.");
            }
            else
            {
                // Assign the first drawing panel from the list to drawingPanel
                drawingPanel = drawingPanels[0];
                // Now you have a drawing panel to work with
            }

            // Calculate the viewport based on the texture size and surface size of the drawingPanel
            if (drawingPanel != null)
            {
                viewport = new RectangleF((drawingPanel.TextureSize - drawingPanel.SurfaceSize) / 2f, drawingPanel.SurfaceSize);
            }

            #endregion

            #region Info Panels
            List<IMyTextPanel> textPanels = new List<IMyTextPanel>();

            foreach (IMyTextPanel block in blocks)
            {
                // Check if the block reference is not null
                if (block != null)
                {
                    // Check if the block is functional and accessible
                    if (block.IsFunctional && block.IsWorking)
                    {
                        // Check if the custom name of the block contains the string "[StoreLCD]"
                        if (block.CustomName.ToLower().Contains("Autocrafting".ToLower()))
                        {
                            // Add the block to the list of drawing panels
                            textPanels.Add(block);
                        }
                    }
                }
            }


            // Check if any text panels were found
            if (textPanels.Count == 0)
            {
                Echo("No text panels found with the specified criteria.");
            }
            else
            {
                infoPanel = textPanels[0];
            }
            #endregion

            #region Button Panel
            List<IMyButtonPanel> buttonPanels = new List<IMyButtonPanel>();

            foreach (IMyButtonPanel button in buttons)
            {
                // Check if the block reference is not null
                if (button != null)
                {
                    // Check if the block is functional and accessible
                    if (button.IsFunctional && button.IsWorking)
                    {
                        // Check if the custom name of the block contains the string "[StoreLCD]"
                        if (button.CustomName.ToLower().Contains("[StoreButtons]".ToLower()))
                        {
                            // Add the block to the list of drawing panels
                            buttonPanels.Add(button);
                        }
                    }
                }
            }


            // Check if any text panels were found
            if (buttonPanels.Count == 0)
            {
                Echo("No text panels found with the specified criteria.");
            }
            else
            {
                buttonPanel = buttonPanels[0];
                textSurfaceProvider = buttonPanel as IMyTextSurfaceProvider;
                bP1 = textSurfaceProvider.GetSurface(0);
                bP2 = textSurfaceProvider.GetSurface(1);
                bP3 = textSurfaceProvider.GetSurface(2);
                bP4 = textSurfaceProvider.GetSurface(3);
            }
            #endregion
        }


        public void Save()
        {

        }

        bool fix;
        public void Main(string argument, UpdateType updateSource)
        {
            // Create a new frame object using the DrawFrame method from the drawingPanel
            frame = drawingPanel.DrawFrame();

            // Check if the fix variable is true
            if (fix)
            {
                // Add a new MySprite object to the frame
                frame.Add(new MySprite());
            }

            // Initialize a newArg variable with an empty string
            string newArg = string.Empty;

            // Check if the argument string is not null or empty
            if (!string.IsNullOrEmpty(argument.ToLower()))
            {
                // Assign the value of the argument to the newArg variable
                newArg = argument.ToLower();
            }


            switch (screen)
            {
                // Home Screen
                case 0:
                    {
                        HomeScreen.DrawScreen(frame,stockListing,newArg, ref screen,ref fix);
                        // Let's do stuff to the button panel!
                        // Change the text of each button
                        buttonPanel.SetCustomButtonName(0, "");
                        buttonPanel.SetCustomButtonName(1, "");
                        buttonPanel.SetCustomButtonName(2, "Begin");
                        buttonPanel.SetCustomButtonName(3, "");
                        bP1.Alignment = TextAlignment.CENTER;
                        bP1.TextPadding = 20;
                        bP1.FontSize = 6;
                        bP1.WriteText("");
                        bP2.Alignment = TextAlignment.CENTER;
                        bP2.TextPadding = 20;
                        bP2.FontSize = 5;
                        bP2.WriteText("");
                        bP3.Alignment = TextAlignment.CENTER;
                        bP3.TextPadding = 20;
                        bP3.FontSize = 6;
                        bP3.WriteText("Begin");
                        bP4.Alignment = TextAlignment.CENTER;
                        bP4.TextPadding = 20;
                        bP4.FontSize = 6;
                        bP4.WriteText("");

                        break;
                    }
                // New Order Screen
                case 1:
                    {
                        OrderScreen.DrawScreen(frame, stockListing, newArg, ref screen, ref fix);
                        buttonPanel.SetCustomButtonName(0, "Home");
                        buttonPanel.SetCustomButtonName(1, "Previous");
                        buttonPanel.SetCustomButtonName(2, "Select");
                        buttonPanel.SetCustomButtonName(3, "Next");
                        bP1.Alignment = TextAlignment.CENTER;
                        bP1.TextPadding = 20;
                        bP1.FontSize = 6;
                        bP1.WriteText("Home");
                        bP2.Alignment = TextAlignment.CENTER;
                        bP2.TextPadding = 27;
                        bP2.FontSize = 4.9f;
                        bP2.WriteText("Previous");
                        bP3.Alignment = TextAlignment.CENTER;
                        bP3.TextPadding = 20;
                        bP3.FontSize = 6;
                        bP3.WriteText("Select");
                        bP4.Alignment = TextAlignment.CENTER;
                        bP4.TextPadding = 20;
                        bP4.FontSize = 6;
                        bP4.WriteText("Next");

                        if (selected == "EmagnetAWE" || selected == "MilPlateAWE" || selected == "SchargerAWE")
                        {
                            bP1.WriteText("Back");
                            bP2.WriteText("+ 1k");
                            bP4.WriteText("- 1k");
                        }

                        break;

                    }
            }
            frame.Dispose();
        }

        #region Backend

        // This method takes in a string parameter compName and returns an integer value
        /// <summary>
        /// Retrieves the available stock quantity for a given component name.
        /// </summary>
        /// <param name="compName">The name of the component.</param>
        /// <returns>The available stock quantity for the component.</returns>
        public static int GetAvailableStockForComp(string compName)
        {
            // Get the text from the infoPanel and store it in the string variable allStock
            string allStock = infoPanel.GetText();

            // Initialize the componentCount variable to 0
            int componentCount = 0;

            // Split the allStock string by newline character ('\n') and iterate over each line
            foreach (var line in allStock.Split('\n'))
            {
                // Check if the line starts with the compName parameter
                if (line.StartsWith(compName))
                {
                    // Split the line by space character (' ') and remove any empty or whitespace strings
                    var lineSplit = line.Split(' ').Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

                    // Check if the lineSplit list has more than 1 element
                    if (lineSplit.Count > 1)
                    {
                        // Get the component count string from the second element of the lineSplit list
                        string componentCountString = lineSplit[1];

                        // Try to parse the componentCountString to an integer and store the result in the componentCount variable
                        if (int.TryParse(componentCountString, out componentCount))
                        {
                            // If successful, break out of the foreach loop
                            break;
                        }
                    }
                }
            }

            // Return the componentCount value
            return componentCount;
        }


        /// <summary>
        /// Parses the custom data string to extract stock names and prices, then adds them to a dictionary.
        /// </summary>
        private void ParseCustomData(string customData)
        {
            // Split the custom data string by newline character ('\n')
            string[] lines = customData.Split('\n');

            // Iterate over each line in the custom data
            foreach (string line in lines)
            {
                // Split the line by '@' character
                string[] parts = line.Split('@');

                // Check if the line contains exactly two parts (stock name and price)
                if (parts.Length == 2)
                {
                    // Extract the stock name and price from the parts
                    string stockName = parts[0].Trim();
                    int price;

                    // Try to parse the price as an integer
                    if (int.TryParse(parts[1], out price))
                    {
                        // Add the stock name and price to the stockListing dictionary
                        stockListing[stockName] = price;
                    }
                }
            }
        }


        #endregion
    }
}
