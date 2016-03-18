using UnityEngine;
using System.Collections;
using SharpUnit;

// ----------------------------------------------------------------------------------------------------
// Intensive test on the camera using priority special cases
// ----------------------------------------------------------------------------------------------------
public class TestInstructorCameraHard : TestUtilitiesInstructorCamera
{
    // ----------------------------------------------------------------------------------------------------
    // Test a transition releasing priority 1 and a cut to priotity 2 interrupting, make sure priority 1 is
    // cleared from containers
    // ----------------------------------------------------------------------------------------------------
    // INITIAL STATE : {P0T2C01} {P1T1C00} {} {} -> [P0T2C01(1)] [] [] []
    // COMMAND       : Cut[P2T1C00]
    // FINAL STATE   : {P0T2C01} {} {P2T1C00} {} -> [] [] [] []
    // ----------------------------------------------------------------------------------------------------
    [UnitTest]
    public void TestInstructorCamera_CutDuringPriorityRelease()
    {
        SetInitialContainerState(P0T2C01, P1T1C00, null, null);
        SetInitialTransitionState(P1T1C00, true, null, false, null, false, null, false);
        m_Camera.AddCut(P2T1C00);
        Check.Containers(P0T2C01, null, P2T1C00, null); Check.Transitions(null, null, null, null);
    }

    // ----------------------------------------------------------------------------------------------------
    // Cancels a non-zero priority transition before it ends
    // ----------------------------------------------------------------------------------------------------
    // INITIAL STATE : {P0T2C01} {} {} {} -> [P1T1C00] [] [] []
    // COMMAND       : ReleaseT[P1T1C00]
    // INTER STATE   : {P0T2C01} {} {} {} -> [P1T1C00] [P0T2C01(1)] [] []
    // WAIT          : T1
    // FINAL STATE   : {P0T2C01} {} {} {} -> [] [] [] []
    // ----------------------------------------------------------------------------------------------------
    [UnitTest]
    public void TestInstructorCamera_CancelPriorityTransition()
    {
        SetInitialTransitionState(P1T1C00, false, null, false, null, false, null, false);
        m_Camera.ReleasePriority(P1T1C00);
        Assert.Equal(ContainerPriority.None, m_Camera.__TestPriority);
        Check.Containers(P0T2C01, null, null, null); Check.Transitions(P1T1C00, P0T2C01, null, null);
        Wait(m_FastTime);
        Check.Containers(P0T2C01, null, null, null); Check.Transitions(null, null, null, null);
    }

    // ----------------------------------------------------------------------------------------------------
    // Insert a priority release while a transition to higher priority is in progress
    // Expected behavior : The transition for the priority release must be inserted before the higher
    // priority transition(s)
    // ----------------------------------------------------------------------------------------------------
    // INITIAL STATE : {P0T2C01} {} {} {} -> [P1T1C00] [P2T1C00] [] []
    // COMMAND       : ReleaseT[P1T1C00]
    // INTER STATE   : {P0T2C01} {} {} {} -> [P1T1C00] [P0T2C01(1)] [P2T1C00] []
    // WAIT          : T1
    // FINAL STATE   : {P0T2C01} {} {P2T1C00} {} -> [] [] [] []
    // ----------------------------------------------------------------------------------------------------
    [UnitTest]
    public void TestInstructorCamera_InsertReleasePriorityWhileInProgress()
    {
        SetInitialTransitionState(P1T1C00, false, P2T1C00, false, null, false, null, false);
        m_Camera.ReleasePriority(P1T1C00);
        Assert.Equal(ContainerPriority.None, m_Camera.__TestPriority);
        Check.Containers(P0T2C01, null, null, null); Check.Transitions(P1T1C00, P0T2C01, P2T1C00, null);
        Wait(m_FastTime);
        Assert.Equal(ContainerPriority.Zone2, m_Camera.__TestPriority);
        Check.Containers(P0T2C01, null, P2T1C00, null); Check.Transitions(null, null, null, null);
    }

    // ----------------------------------------------------------------------------------------------------
    // Add a transition to a lower priority than the last transition
    // ----------------------------------------------------------------------------------------------------
    // INITIAL STATE : {P0T2C01} {} {} {} -> [P1T1C00] [] [] []
    // COMMAND       : Transition[P0T1C00]
    // INTER STATE   : {P0T2C01} {} {} {} -> [P0T1C00] [P1T1C00] [] []
    // WAIT          : T1
    // FINAL STATE   : {P0T1C00} {P1T1C00} {} {} -> [] [] [] []
    // ----------------------------------------------------------------------------------------------------
    [UnitTest]
    public void TestInstructorCamera_InsertTransition()
    {
        SetInitialTransitionState(P1T1C00, false, null, false, null, false, null, false);
        m_Camera.AddTransition(P0T1C00);
        Assert.Equal(ContainerPriority.None, m_Camera.__TestPriority);
        Check.Containers(P0T2C01, null, null, null); Check.Transitions(P0T1C00, P1T1C00, null, null);
        Wait(m_FastTime);
        Assert.Equal(ContainerPriority.Zone1, m_Camera.__TestPriority);
        Check.Containers(P0T1C00, P1T1C00, null, null); Check.Transitions(null, null, null, null);
    }

    // ----------------------------------------------------------------------------------------------------
    // Add a lower priority transition while a transition to higher priority is in progress, then release
    // the higher priority before the transition ends
    // ----------------------------------------------------------------------------------------------------
    // INITIAL STATE : {P0T2C01} {} {} {} -> [P1T1C00] [P0T1C00] [] []
    // COMMAND       : ReleaseT[P1T1C00]
    // INTER STATE   : {P0T2C01} {} {} {} -> [P1T1C00] [P0T1C00] [P1T1C00(1)] []
    // WAIT          : T1
    // FINAL STATE   : {P0T1C00} {} {} {} -> [] [] [] []
    // ----------------------------------------------------------------------------------------------------
    [UnitTest]
    public void TestInstructorCamera_AddLowerPriorityTransitionWhileGoingToHigherAndRelease()
    {
        SetInitialTransitionState(P1T1C00, false, P0T1C00, false, null, false, null, false);
        m_Camera.ReleasePriority(P1T1C00);
        Check.Containers(P0T2C01, null, null, null); Check.Transitions(P0T1C00, P1T1C00, P0T1C00, null);
        Wait(m_FastTime);
        Assert.Equal(ContainerPriority.None, m_Camera.__TestPriority);
        Check.Containers(P0T1C00, null, null, null); Check.Transitions(null, null, null, null);
    }

    // TODO : Fill the transition list and add an extra transition, first one must be cut from the list

    // TODO : Same but with a priority release in the first index
}
