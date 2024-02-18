using System;
using System.Collections.Generic;
using VRage.Game.GUI.TextPanel;
using VRageMath;
using VRageRender;

namespace IngameScript
{
    internal class HomeScreen 
    {
        public static void DrawScreen(MySpriteDrawFrame frame, Dictionary<string, int> stockListing, string newArg, ref int screen, ref bool fix)
        {
            // All sprites must be added to the frame here
            // Welcome Header
            Drawing.DrawBackground(Color.Black, ref frame);
            Drawing.DrawRoundedRect(ref frame, 20, 10, 512 - 40, 512 - 420, 4, Color.White);
            Drawing.DrawText(ref frame, 256, 15, "Welcome To", Color.White, 0.9f, TextAlignment.CENTER, "White");
            Drawing.DrawText(ref frame, 256, 40, "Triquetra Trade Tower Terminal", Color.White, 0.9f, TextAlignment.CENTER, "White");
            Drawing.DrawText(ref frame, 256, 65, "Enjoy Your Stay", Color.White, 0.9f, TextAlignment.CENTER, "White");

            // Current Stock
            Drawing.DrawRoundedRect(ref frame, 20, 110, 512 - 40, 512 - 350, 4, Color.White);
            Drawing.DrawText(ref frame, 256, 115, "Stock Availability and Price Per Unit", Color.White, 0.75f, TextAlignment.CENTER, "White");
            Drawing.DrawRoundedRect(ref frame, 512 - 220, 157, 190, 512 - 410, 4, Color.White);
            Drawing.DrawLine(ref frame, 512 - 110, 175, 512 - 107, 240, Color.White);

            int drawnStock = 0;
            foreach (var item in stockListing)
            {
                Drawing.DrawText(ref frame, 120, 170 + drawnStock, item.Key.ToString(), Color.White, 0.7f, TextAlignment.CENTER, "White");
                Drawing.DrawText(ref frame, 512 - 160, 170 + drawnStock, Program.GetAvailableStockForComp(item.Key.ToString()).ToString(), Color.White, 0.7f, TextAlignment.CENTER, "White");
                Drawing.DrawText(ref frame, 512 - 75, 170 + drawnStock, item.Value.ToString(), Color.White, 0.7f, TextAlignment.CENTER, "White");
                drawnStock += 25;
            }

            if (newArg.ToLower() == "sel".ToLower())
            {
                screen = 1;
                fix = !fix;
            }

            // Create Order 'Button'
            Drawing.DrawRoundedRect(ref frame, 165, 512 - 58, 180, 40, 4, Color.White);
            Drawing.DrawText(ref frame, 256, 512 - 54, "> Create Order", Color.White, 1.0f, TextAlignment.CENTER, "White");
        }

    }
}
