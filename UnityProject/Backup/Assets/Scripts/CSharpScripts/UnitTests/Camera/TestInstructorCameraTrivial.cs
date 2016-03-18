using UnityEngine;
using System.Collections;
using SharpUnit;

// ----------------------------------------------------------------------------------------------------
// The easiest set of tests, checks basic functionality
// ----------------------------------------------------------------------------------------------------
public class TestInstructorCameraTrivial : TestUtilitiesInstructorCamera
{
    // ----------------------------------------------------------------------------------------------------
    // Verifies that Init functions work and do not assert
    // ----------------------------------------------------------------------------------------------------
    [UnitTest]
    public void TestInstructorCamera_Init()
    {
        m_Camera.__TestInit();
        Assert.NotNull(m_Camera.__TestThisTransform);
    }

    // ----------------------------------------------------------------------------------------------------
    // Adds a simple camera cut
    // ----------------------------------------------------------------------------------------------------
    // INITIAL STATE : {P0T2C01} {} {} {} -> [] [] [] []
    // COMMAND       : Cut[P0T1C00]
    // FINAL STATE   : {P0T1C00} {} {} {} -> [] [] [] []
    // ----------------------------------------------------------------------------------------------------
    [UnitTest]
    public void TestInstructorCamera_AddCut()
    {
        m_Camera.AddCut(P0T1C00);
        Check.Containers(P0T1C00, null, null, null); Check.Transitions(null, null, null, null);
    }

    // ----------------------------------------------------------------------------------------------------
    // Adds a simple camera transition
    // ----------------------------------------------------------------------------------------------------
    // INITIAL STATE : {P0T2C01} {} {} {} -> [] [] [] []
    // COMMAND       : Transition[P0T1C00]
    // INTER STATE   : {P0T2C01} {} {} {} -> [P0T1C00] [] [] []
    // WAIT          : T1
    // FINAL STATE   : {P0T1C00} {} {} {} -> [] [] [] []
    // ----------------------------------------------------------------------------------------------------
    [UnitTest]
    public void TestInstructorCamera_AddTransition()
    {
        m_Camera.AddTransition(P0T1C00);
        Check.Containers(P0T2C01, null, null, null); Check.Transitions(P0T1C00, null, null, null);
        Wait(m_FastTime);
        Check.Containers(P0T1C00, null, null, null); Check.Transitions(null, null, null, null);
    }

    // ----------------------------------------------------------------------------------------------------
    // Tests the appropriate matrix of the camera
    // ----------------------------------------------------------------------------------------------------
    [UnitTest]
    public void TestInstructorCamera_Matrix()
    {
        Check.Position(0.0f, 1.0f);
        Check.OrientationCloseTo(Vector3.up);
    }

    // ----------------------------------------------------------------------------------------------------
    // Tests the appropriate matrix of the camera during a transition
    // ----------------------------------------------------------------------------------------------------
    [UnitTest]
    public void TestInstructorCamera_TransitionMatrix()
    {
        m_Camera.AddTransition(P0T1C00);
        FixedWait(m_FastTime / 2);
        Check.Position(0.0f, 0.5f);
        Check.OrientationCloseTo((Vector3.up + Vector3.forward) / 2);
    }

    // ----------------------------------------------------------------------------------------------------
    // Resets the state of the camera
    // ----------------------------------------------------------------------------------------------------
    [UnitTest]
    public void TestInstructorCamera_Reset()
    {
        // Reset is called every TearDown so everything should be fully initialized to default values at the end
        // except for the Container[0] instructors which must always be set
        Check.Containers(P0T2C01, null, null, null);
        Check.Transitions(null, null, null, null);
        Check.Position(0.0f, 1.0f);
        Assert.True(Vector3.up == m_Camera.__TestOrientation);
        Assert.True(Vector3.up == m_Camera.__TestBanking);
        Assert.Equal(m_Camera.__TestPriority, ContainerPriority.None);
        Assert.Equal(0, m_Camera.__TestTransitionCount);
    }
}
