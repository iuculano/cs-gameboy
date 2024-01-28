using Emulator.Core.Input;
using Enulator.Core.Bus;

namespace Emulator.Core.Bus;

public class Joypad
{
    // https://gbdev.io/pandocs/Joypad_Input.html

    // Masks for reading specific bits out of the JOYP register.
    private const byte InputBitmask    = 0b_00001111; // Bits 3-0 are the button/d-pad input state - whether it's pressed or not.
    private const byte SelectorBitmask = 0b_00110000; // Bits 4-5 determine whether we can read that state.
    private const byte UnusedBitmask   = 0b_11000000; // Bits 6-7 are unused, always 1.

    private readonly MemoryBus        memory;
    private readonly InterruptHandler interruptHandler;

    // These represent the key state of the JOYP register.
    // These are buffered and the joystick state is only truly updated in the
    // Update() function. Note that a button press is 0, not 1.
    private byte pendingDirection = InputBitmask;
    private byte pendingButton    = InputBitmask;

     public Joypad(MemoryBus memory, InterruptHandler interruptHandler)
    {
        this.memory           = memory;
        this.interruptHandler = interruptHandler;
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
        byte selector = (byte)(memory.JOYP & SelectorBitmask);

        // If neither is selected, then we treat all keys as unset.
        if (selector == SelectorBitmask)
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
            // Force unused bits to 1, keep the same selector, set input bits
            memory.JOYP |= (byte)(UnusedBitmask | selector | pendingDirection);

            // If any bit is 0, something's pressed
            if (pendingDirection != InputBitmask)
            {
                interruptHandler.Request(InterruptType.Joypad);
            }
        }

        if (isButtonSelected)
        {
            memory.JOYP |= (byte)(UnusedBitmask | selector | pendingButton);

            if (pendingButton != InputBitmask)
            {
                interruptHandler.Request(InterruptType.Joypad);
            }
        }
    }
}
