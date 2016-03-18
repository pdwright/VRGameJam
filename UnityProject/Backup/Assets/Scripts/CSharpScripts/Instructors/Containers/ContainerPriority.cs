using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Class matching an int with an enum for instructor container priority
// ----------------------------------------------------------------------------------------------------
public static class ContainerPriority
{
    private enum Priority
    {
        None,
        Zone1,
        Zone2,
        Zone3,
        Count
    }

    public static int None
    {
        get { return (int)Priority.None; }
    }

    public static int Zone1
    {
        get { return (int)Priority.Zone1; }
    }

    public static int Zone2
    {
        get { return (int)Priority.Zone2; }
    }

    public static int Zone3
    {
        get { return (int)Priority.Zone3; }
    }

    public static int Count
    {
        get { return (int)Priority.Count; }
    }
}
