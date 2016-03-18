using UnityEngine;
using System.Collections;
using SharpUnit;

// ----------------------------------------------------------------------------------------------------
// Test runner for the Math module test cases
// ----------------------------------------------------------------------------------------------------
public class TestRunnerMath : Unity3D_TestRunner
{
    // ----------------------------------------------------------------------------------------------------
    // Use this for initialization
    // ----------------------------------------------------------------------------------------------------
    void Start()
    {
        TestCategory(new TestMath(), "Math module test results - ");

        Debug.Break();
    }
}
