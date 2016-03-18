using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Standard interface for an instructor transition
// ----------------------------------------------------------------------------------------------------
interface ITransition
{
    // ----------------------------------------------------------------------------------------------------
    // For UnitTesting purposes, should never be used elsewhere
    // ----------------------------------------------------------------------------------------------------
    void __TestInit();
    void __TestSetPriority(int _priority);
    void __TestSetTime(float _time);
    // ----------------------------------------------------------------------------------------------------

    int Priority
    {
        get;
    }

    float Time
    {
        get;
    }
}
