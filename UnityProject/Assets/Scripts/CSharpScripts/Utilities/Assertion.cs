using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Static class for assert utility
// ----------------------------------------------------------------------------------------------------
public static class Assertion
{
    // ----------------------------------------------------------------------------------------------------
    // Asserts on a condition, breaks the engine if it is false and displays the error
    // ----------------------------------------------------------------------------------------------------
    public static void Assert(bool _condition, string _message)
    {
        if (!_condition)
        {
            Debug.LogError(_message);
            Debug.Break();
        }
    }
}
