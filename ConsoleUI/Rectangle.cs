using System;


namespace ConsoleUI
{
    using Pair = Tuple<Rectangle, Rectangle>;


    public class Rectangle
    {
        public int X      { get; set; }
        public int Y      { get; set; }
        public int Width  { get; set; }
        public int Height { get; set; }

        
        Rectangle()
        {

        }

        Rectangle(int x, int y, int width, int height)
        {
            this.X      = x;
            this.Y      = y;
            this.Width  = width;
            this.Height = height;
        }

        public Pair Split(float percentage, bool vertical)
        {
            var one = new Rectangle
            {
                X      = this.X,
                Y      = this.Y,
                Width  = vertical ? (int)Math.Round((percentage / 100.0f) * this.Width) : this.Width,
                Height = vertical ? this.Height : (int)Math.Round((percentage / 100.0f) * this.Height)
            };

            var two = new Rectangle
            {
                // Just use the difference to determine the leftover space and
                // shift the starting point depending on slice orientation.
                X      = vertical ? one.Width : one.X,
                Y      = vertical ? one.Y     : one.Height,
                Width  = vertical ? this.Width  - one.Width : this.Width,
                Height = vertical ? this.Height : this.Height - one.Height
            };
            
            return new Pair(one, two);
        }
    }
}
