/**
 * @file TestRunner.cs
 * 
 * Unity3D unit test runner.
 * Sets up the unit testing suite and executes all unit tests.
 * Drag this onto an empty GameObject to run tests.
 */

using UnityEngine;
using System.Collections;
using SharpUnit;

public class Unity3D_TestRunner : MonoBehaviour 
{
    protected void TestCategory(TestCase _category, string _prefix)
    {
        TestSuite suite = new TestSuite();
        suite.AddAll(_category);

        TestResult res = suite.Run(null);

        Unity3D_TestReporter reporter = new Unity3D_TestReporter();
        reporter.LogResults(res, _prefix);
    }

	void Start()
    {
        Debug.Break();
	}
}
