using System;


namespace ConsoleUI
{
    public class Layout
    {
        public enum Alignment
        {
            Left,
            Center,
            Right,
            Justify
        }

        public enum Direction
        {
            Horizontal,
            Vertical
        }
        
        public float Percentage { get; set; }
    }
}
