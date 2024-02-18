using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Game.GUI.TextPanel;
using VRageMath;
using VRageRender;

namespace IngameScript
{
    internal class Drawing
    {

        /// <summary>
        /// Draws a background with the specified color onto the given sprite draw frame.
        /// </summary>
        /// <param name="color">The color of the background to draw.</param>
        /// <param name="frame">The sprite draw frame to draw the background onto. This parameter is passed by reference.</param>
        public static void DrawBackground(Color color, ref MySpriteDrawFrame frame)
        {
            // Set up the initial position - and remember to add our viewport offset
            var position = new Vector2(0, 256) + Program.viewport.Position;

            // Create a filled rectangle sprite covering the entire viewport
            var backgroundSprite = new MySprite()
            {
                Type = SpriteType.TEXTURE,
                Data = "SquareSimple",
                Position = position,
                Size = Program.viewport.Size,
                Color = color,
                Alignment = TextAlignment.LEFT
            };

            // Add the background sprite to the frame
            frame.Add(backgroundSprite);
        }

        /// <summary>
        /// Draws a rounded rectangle on the specified MySpriteDrawFrame at the given position and dimensions with the specified radius and color.
        /// </summary>
        /// <param name="frame">The MySpriteDrawFrame to draw on.</param>
        /// <param name="x">The x-coordinate of the top-left corner of the rectangle.</param>
        /// <param name="y">The y-coordinate of the top-left corner of the rectangle.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        /// <param name="radius">The radius of the rounded corners.</param>
        /// <param name="color">The color of the rectangle.</param>
        public static void DrawRoundedRect(ref MySpriteDrawFrame frame, int x, int y, int width, int height, int radius, Color color)
        {
            // Calculate the coordinates of the four corners of the rectangle
            int topLeftX = x;
            int topLeftY = y;
            int topRightX = x + width;
            int topRightY = y;
            int bottomLeftX = x;
            int bottomLeftY = y + height;
            int bottomRightX = x + width;
            int bottomRightY = y + height;

            // Draw the four lines of the rectangle
            DrawLine(ref frame, topLeftX + radius, topLeftY, topRightX - radius, topRightY + radius, color); // Top
            DrawLine(ref frame, topRightX - radius, topRightY + radius, bottomRightX, bottomRightY - radius, color); // Right
            DrawLine(ref frame, bottomLeftX + radius, bottomLeftY - radius, bottomRightX - radius, bottomRightY, color); // Bottom
            DrawLine(ref frame, topLeftX, topLeftY + radius, bottomLeftX + radius, bottomLeftY - radius, color); // Left

            // Draw the circles to create rounded corners
            DrawClippedCircle(ref frame, topRightX - radius, topRightY + radius, radius, radius, 0, color); // Top right
            DrawClippedCircle(ref frame, bottomRightX - radius, bottomRightY - radius, radius, radius, 1, color); // Bottom right
            DrawClippedCircle(ref frame, bottomLeftX + radius, bottomLeftY - radius, radius, radius, 2, color); // Bottom left
            DrawClippedCircle(ref frame, topLeftX + radius, topLeftY + radius, radius, radius, 3, color); // Top left
        }

        /// <summary>
        /// Draws a line on the specified sprite draw frame using the given coordinates, color, and viewport offset.
        /// </summary>
        /// <param name="frame">The sprite draw frame to add the line sprite to.</param>
        /// <param name="lx">The x-coordinate of the starting point of the line.</param>
        /// <param name="ly">The y-coordinate of the starting point of the line.</param>
        /// <param name="rx">The x-coordinate of the ending point of the line.</param>
        /// <param name="ry">The y-coordinate of the ending point of the line.</param>
        /// <param name="color">The color of the line.</param>
        public static void DrawLine(ref MySpriteDrawFrame frame, int lx, int ly, int rx, int ry, Color color)
        {
            // Set up the initial position - and remember to add our viewport offset
            var position = new Vector2(lx, ly) + Program.viewport.Position;
            var size = new Vector2(rx - lx, ry - ly);
            position += size / 2;

            // Create a filled rectangle sprite acting as the line
            var lineSprite = new MySprite()
            {
                Type = SpriteType.TEXTURE,
                Data = "SquareSimple",
                Position = position,
                Size = size,
                Color = color,
                Alignment = TextAlignment.CENTER,
            };

            frame.Add(lineSprite);
        }

        /// Draws a clipped circle on the specified sprite draw frame at the given position and dimensions, with a specified sector and color.
        /// </summary>
        /// <param name="frame">The sprite draw frame to draw the circle on.</param>
        /// <param name="lx">The x-coordinate of the circle's center.</param>
        /// <param name="ly">The y-coordinate of the circle's center.</param>
        /// <param name="width">The width of the circle.</param>
        /// <param name="height">The height of the circle.</param>
        /// <param name="sector">The sector of the circle (0: Top right, 1: Bottom right, 2: Bottom left, 3: Top left).</param>
        /// <param name="color">The color of the circle.</param>
        public static void DrawClippedCircle(ref MySpriteDrawFrame frame, int lx, int ly, int width, int height, int sector, Color color)
        {
            // Set up the initial position - and remember to add our viewport offset
            var clipOffset = Vector2.Zero;
            var position = new Vector2(lx, ly) + Program.viewport.Position;
            var size = new Vector2(width * 2, height * 2);

            switch (sector)
            {
                // Top right
                default:
                case 0:
                    clipOffset = new Vector2(0, height);
                    break;
                // Bottom Right
                case 1:
                    clipOffset = new Vector2(0, 0);
                    break;
                // Bottom Left
                case 2:
                    clipOffset = new Vector2(width, 0);
                    break;
                // Top Left
                case 3:
                    clipOffset = new Vector2(width, height);
                    break;
            }

            using (frame.Clip(lx - (int)clipOffset.X, ly - (int)clipOffset.Y, width, height))
            {
                // Create a filled circle
                var circleSprite = new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Data = "Circle",
                    Position = position,
                    Size = size,
                    Color = color,
                    Alignment = TextAlignment.CENTER
                };
                frame.Add(circleSprite);
            }
        }

        /// <summary>
        /// Draws text on a sprite draw frame at the specified position with the given text, color, size, alignment, and font.
        /// </summary>
        /// <param name="frame">The sprite draw frame to add the text sprite to.</param>
        /// <param name="x">The x-coordinate of the text position.</param>
        /// <param name="y">The y-coordinate of the text position.</param>
        /// <param name="text">The text to be displayed.</param>
        /// <param name="color">The color of the text.</param>
        /// <param name="size">The size of the text.</param>
        /// <param name="alignment">The alignment of the text.</param>
        /// <param name="font">The font to be used for the text.</param>
        public static void DrawText(ref MySpriteDrawFrame frame, int x, int y, string text, Color color, float size, TextAlignment alignment, string font)
        {
            // Set up the initial position - and remember to add our viewport offset
            var position = new Vector2(x, y) + Program.viewport.Position;

            var sprite = new MySprite()
            {
                Type = SpriteType.TEXT,
                Data = text,
                Position = position,
                RotationOrScale = size,
                Color = color,
                Alignment = alignment,
                FontId = font
            };
            // Add the sprite to the frame
            frame.Add(sprite);
        }


        // Drawing Sprites
        /// <summary>
        /// Draws two lines of text sprites on a given frame with specified properties such as position, rotation/scale, color, alignment, and font.
        /// </summary>
        public static void DrawSprites(ref MySpriteDrawFrame frame)
        {
            // Set up the initial position - and remember to add our viewport offset
            var position = new Vector2(256, 20) + Program.viewport.Position;

            // Create our first line
            var sprite = new MySprite()
            {
                Type = SpriteType.TEXT,
                Data = "Line 1",
                Position = position,
                RotationOrScale = 0.8f /* 80 % of the font's default size */,
                Color = Color.Red,
                Alignment = TextAlignment.CENTER /* Center the text on the position */,
                FontId = "White"
            };
            // Add the sprite to the frame
            frame.Add(sprite);

            // Move our position 20 pixels down in the viewport for the next line
            position += new Vector2(0, 20);

            // Create our second line, we'll just reuse our previous sprite variable - this is not necessary, just
            // a simplification in this case.
            sprite = new MySprite()
            {
                Type = SpriteType.TEXT,
                Data = "Line 2",
                Position = position,
                RotationOrScale = 0.8f,
                Color = Color.Blue,
                Alignment = TextAlignment.CENTER,
                FontId = "White"
            };
            // Add the sprite to the frame
            frame.Add(sprite);

        }

    }
}
