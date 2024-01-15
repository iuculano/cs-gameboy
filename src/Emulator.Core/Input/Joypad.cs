using Emulator.Core.Input;

namespace Emulator.Core.Bus;

public class Joypad
{
    private readonly MemoryBus memory;

    // https://gbdev.io/pandocs/Joypad_Input.html
    // Masks for reading specific bits out of the JOYP register.
    private const byte InputBitmask    = 0b_00001111; // Bits 3-0 are the button/d-pad input state - whether it's pressed or not.
    private const byte SelectorBitmask = 0b_00110000; // Bits 4-5 determine whether we can read that state.
    private const byte UnusedBitmask   = 0b_11000000; // Bits 6-7 are unused, always 1.


    // These represent the key state of the JOYP register, the lower nibble
    // These are buffered and the joystick state is only truly updated in the
    // Update() function. Note that a button press is 0, not 1.
    private byte pendingDirection = InputBitmask;
    private byte pendingButton    = InputBitmask;


    public Joypad(MemoryBus memory)
    {
        this.memory = memory;
    }

    private byte GetButtonMask(JoypadButton button)
    {
        // Helper function to get the correct bit mask for the button
        // Directions overlay buttons on the lower nibble
        if (button == JoypadButton.Right || button == JoypadButton.A)      return 0b_00000001;
        if (button == JoypadButton.Left  || button == JoypadButton.B)      return 0b_00000010;
        if (button == JoypadButton.Up    || button == JoypadButton.Select) return 0b_00000100;
        if (button == JoypadButton.Down  || button == JoypadButton.Start)  return 0b_00001000;

        return 0;
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

    private void UpdateInputBuffer(JoypadButton button, bool isPressed)
    {
        // Map the keyboard state into the appropriate JOYP bits

        byte mask = GetButtonMask(button);
        if (IsDirection(button))
        {
            if (isPressed)
            {
                pendingDirection &= (byte)~mask;
            }

            else
            {
                pendingDirection |= mask;
            }
        }

        else if (IsButton(button))
        {
            if (isPressed)
            {
                pendingButton &= (byte)~mask;
            }

            else
            {
                pendingButton |= mask;
            }
        }
    }

    public void SetKeyDown(JoypadButton button)
    {
        UpdateInputBuffer(button, true);
    }

    public void SetKeyUp(JoypadButton button)
    {
        UpdateInputBuffer(button, false);
    }

    public void Update()
    {
        // Keep the selector - keeping the JOYP register in the same state but
        // splicing in the appropriate input depending on which is selected.
        // If neither is selected, then we treat all keys as unset.
        byte selector = (byte)(memory.JOYP & SelectorBitmask);
        if (selector == 0b_00110000)
        {
            // Remember, the input bits being 1 means it's not pressed
            memory.JOYP = 0xFF;
            return;
        }

        // Figure out which one is set, if any
        bool isDirectionSelected = (selector & 0b_00010000) == 0;
        bool isButtonSelected    = (selector & 0b_00100000) == 0;

        if (isDirectionSelected)
        {
            // Force the unused bits to 1, keep the same selector, set our input bits
            memory.JOYP |= (byte)(UnusedBitmask | selector | pendingDirection);

            // If any bit is unset, something's pressed and we can request and interrupt
            if (pendingDirection != InputBitmask)
            {
                memory.IF |= 0b_00010000;
            }
        }

        if (isButtonSelected)
        {
            memory.JOYP |= (byte)(UnusedBitmask | selector | pendingButton);

            if (pendingButton != InputBitmask)
            {
                memory.IF |= 0b_00010000;
            }
        }
    }
}
