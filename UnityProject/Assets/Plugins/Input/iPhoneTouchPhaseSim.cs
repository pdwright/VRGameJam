using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Static class with properties to have the exact same interface as iPhone module for iPhoneTouchPhaseSim
// ----------------------------------------------------------------------------------------------------
public static class iPhoneTouchPhaseSim
{
    // ----------------------------------------------------------------------------------------------------
    // Status of a touch
    // ----------------------------------------------------------------------------------------------------
    public enum Phase
    {
        Began,
        Moved,
        Stationary,
        Ended,
        Canceled // Unused on PC
    }

    public static Phase Began
    {
        get { return Phase.Began; }
    }

    public static Phase Moved
    {
        get { return Phase.Moved; }
    }

    public static Phase Stationary
    {
        get { return Phase.Stationary; }
    }

    public static Phase Ended
    {
        get { return Phase.Ended; }
    }

    public static Phase Canceled
    {
        get { return Phase.Canceled; }
    }
}
