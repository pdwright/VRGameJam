using UnityEngine;
using System.Collections;
using SharpUnit;

// ----------------------------------------------------------------------------------------------------
// Test runner for the linear network test cases
// ----------------------------------------------------------------------------------------------------
public class TestRunnerLinearNetwork : Unity3D_TestRunner
{
    // ----------------------------------------------------------------------------------------------------
    // Use this for initialization
    // ----------------------------------------------------------------------------------------------------
    void Start()
    {
        TestCategory(new TestLinearNetwork(), "Network test results - ");

        Debug.Break();
    }
}
