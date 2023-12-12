using System;

namespace axGB.System
{
    public enum JoypadButton
    {
        Unknown,
        Left,
        Right,
        Up,
        Down,
        A,
        B,
        Select,
        Start
    }

    public class Joypad
    {
        private readonly MemoryBus memory;

        // https://gbdev.io/pandocs/Joypad_Input.html
        public Joypad(MemoryBus memory)
        {
            this.memory = memory;
        }

        private byte GetButtonMask(JoypadButton button)
        {
            // Helper function to get the correct bit mask for the button
            // Directions overlay buttons on the lower nibble

            byte mask = 0;
            if (button == JoypadButton.Right || button == JoypadButton.A)
            {
                mask = 0b_00000001;
            }

            if (button == JoypadButton.Left || button == JoypadButton.B)
            {
                mask = 0b_00000010;
            }

            if (button == JoypadButton.Up || button == JoypadButton.Select)
            {
                mask = 0b_00000100;
            }

            if (button == JoypadButton.Down || button == JoypadButton.Start)
            {
                mask = 0b_00001000;
            }

            return mask;
        }

        private bool IsDirection(JoypadButton button)
        {
            return button == JoypadButton.Right || button == JoypadButton.Left ||
                   button == JoypadButton.Up    || button == JoypadButton.Down;
        }

        private bool IsButton(JoypadButton button)
        {
            return button == JoypadButton.A      || button == JoypadButton.B ||
                   button == JoypadButton.Select || button == JoypadButton.Start;
        }

        private void UpdateJoypadRegister(JoypadButton button, bool isPressed)
        {
            // Something's wrong here and these bits are _always_ getting set
            // This causes the inputs to overlap each other, for example
            // pressing Down will also press Start
            var isDirectionSelected = (memory.JOYP & 0b_00010000) == 0;
            var isButtonSelected    = (memory.JOYP & 0b_00100000) == 0;

            byte mask = GetButtonMask(button);

            if ((isDirectionSelected && IsDirection(button)) || (isButtonSelected && IsButton(button)))
            {
                if (isPressed)
                {
                    memory.JOYP &= (byte)~mask;
                }

                else
                {
                    memory.JOYP |= mask;
                }
            }
        }


        public void SetKeyDown(JoypadButton button)
        {
            UpdateJoypadRegister(button, true);
        }


        public void SetKeyUp(JoypadButton button)
        {
            UpdateJoypadRegister(button, false);
        }

        public void Update()
        {


        }
    }
}
