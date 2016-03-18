using UnityEngine;
using System.Collections;
using SharpUnit;

// ----------------------------------------------------------------------------------------------------
// The hardest set of tests on complex transitions/cuts interaction
// ----------------------------------------------------------------------------------------------------
public class TestInstructorCameraInsane : TestUtilitiesInstructorCamera
{
    // ----------------------------------------------------------------------------------------------------
    // Adds a transition to a priority that is being released to
    // ----------------------------------------------------------------------------------------------------
    // INITIAL STATE : {P0T2C01} {} {} {} -> [P1T1C00] [P0T1C00] [P1T1C00(1)] []
    // COMMAND       : Transition[P0T2C01]
    // INTER STATE   : {P0T2C01} {} {} {} -> [P0T1C00] [P1T1C00] [P0T1C00] [P0T2C01]
    // WAIT          : T2
    // FINAL STATE   : {P0T2C01} {} {} {} -> [] [] [] []
    // ----------------------------------------------------------------------------------------------------
    [UnitTest]
    public void TestInstructorCamera_LowPriorityTransitionWhileReleasingAPriority()
    {
        SetInitialTransitionState(P1T1C00, false, P0T1C00, false, P1T1C00, true, null, false);
        m_Camera.AddTransition(P0T2C01);
        Check.Containers(P0T2C01, null, null, null); Check.Transitions(P0T1C00, P1T1C00, P0T1C00, P0T2C01);
        Wait(m_SlowTime);
        Assert.Equal(ContainerPriority.None, m_Camera.__TestPriority);
        Check.Containers(P0T2C01, null, null, null); Check.Transitions(null, null, null, null);
    }

    // ----------------------------------------------------------------------------------------------------
    // Release a priority while two low priority transitions exist in the list
    // ----------------------------------------------------------------------------------------------------
    // INITIAL STATE : {P0T2C01} {} {} {} -> [P1T1C00] [P0T1C00] [P0T2C01] []
    // COMMAND       : ReleaseT[P1T1C00]
    // INTER STATE   : {P0T2C01} {} {} {} -> [P0T1C00] [P0T2C01] [P1T1C00] [P0T2C01(1)]
    // WAIT          : T2
    // FINAL STATE   : {P0T2C01} {} {} {} -> [] [] [] []
    // ----------------------------------------------------------------------------------------------------
    [UnitTest]
    public void TestInstructorCamera_ReleasePriorityAfterDoubleLowPriorityTransitions()
    {
        SetInitialTransitionState(P1T1C00, false, P0T1C00, false, P0T2C01, false, null, false);
        m_Camera.ReleasePriority(P1T1C00);
        Check.Containers(P0T2C01, null, null, null); Check.Transitions(P0T1C00, P0T2C01, P1T1C00, P0T2C01);
        Wait(m_SlowTime);
        Assert.Equal(ContainerPriority.None, m_Camera.__TestPriority);
        Check.Containers(P0T2C01, null, null, null); Check.Transitions(null, null, null, null);
    }

    // ----------------------------------------------------------------------------------------------------
    // Overrides a transition made from a priority release by another transition of the same priority
    // ----------------------------------------------------------------------------------------------------
    // INITIAL STATE : {P0T2C01} {P1T1C00} {} {} -> [P0T2C01(1)] [] [] []
    // COMMAND       : Transition[P0T1C00]
    // INTER STATE   : {P0T2C01} {P1T1C00} {} {} -> [P0T2C01(1)] [P0T1C00] [] []
    // WAIT          : T1
    // FINAL STATE   : {P0T1C00} {} {} {} -> [] [] [] []
    // ----------------------------------------------------------------------------------------------------
    [UnitTest]
    public void TestInstructorCamera_OverridePriorityReleaseWithSamePriorityTransition()
    {
        SetInitialContainerState(P0T2C01, P1T1C00, null, null);
        SetInitialTransitionState(P1T1C00, true, null, false, null, false, null, false);
        m_Camera.AddTransition(P0T1C00);
        Check.Containers(P0T2C01, P1T1C00, null, null); Check.Transitions(P0T2C01, P0T1C00, null, null);
        Wait(m_FastTime);
        Check.Containers(P0T1C00, null, null, null); Check.Transitions(null, null, null, null);
    }

    // TODO : Cancel priority release with transition having same priority as the one being released, validate after transition 1 ends, priority goes down to 0, then up to 1
}
