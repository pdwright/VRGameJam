using UnityEngine;
using System.Collections;
using SharpUnit;

// ----------------------------------------------------------------------------------------------------
// Runs all categories of tests for the camera
// ----------------------------------------------------------------------------------------------------
public class TestRunnerInstructorCamera : Unity3D_TestRunner
{
    // ----------------------------------------------------------------------------------------------------
    // Use this for initialization
    // ----------------------------------------------------------------------------------------------------
	void Start()
    {
        TestCategory(new TestInstructorCameraTrivial(), "Camera test results - Difficulty level : Trivial - ");
        TestCategory(new TestInstructorCameraEasy(), "Camera test results - Difficulty level : Easy - ");
        TestCategory(new TestInstructorCameraMedium(), "Camera test results - Difficulty level : Medium - ");
        TestCategory(new TestInstructorCameraHard(), "Camera test results - Difficulty level : Hard - ");
        TestCategory(new TestInstructorCameraInsane(), "Camera test results - Difficulty level : Insane - ");

        Debug.Break();
	}
}
