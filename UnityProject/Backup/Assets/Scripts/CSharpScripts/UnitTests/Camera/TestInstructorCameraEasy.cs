using UnityEngine;
using System.Collections;
using SharpUnit;

// ----------------------------------------------------------------------------------------------------
// Some simple tests for camera cuts and transitions
// ----------------------------------------------------------------------------------------------------
public class TestInstructorCameraEasy : TestUtilitiesInstructorCamera
{
    // ----------------------------------------------------------------------------------------------------
    // Overrides an added transition of same priority
    // ----------------------------------------------------------------------------------------------------
    // INITIAL STATE : {P0T2C01} {} {} {} -> [P0T1C10] [] [] []
    // COMMAND       : Transition[P0T2C11]
    // INTER STATE   : {P0T2C01} {} {} {} -> [P0T1C10] [P0T2C11] [] []
    // WAIT          : T2
    // FINAL STATE   : {P0T2C11} {} {} {} -> [] [] [] []
    // ----------------------------------------------------------------------------------------------------
    [UnitTest]
    public void TestInstructorCamera_OverrideTransition()
    {
        SetInitialTransitionState(P0T1C10, false, null, false, null, false, null, false);
        m_Camera.AddTransition(P0T2C11);
        Check.Containers(P0T2C01, null, null, null); Check.Transitions(P0T1C10, P0T2C11, null, null);
        Wait(m_SlowTime);
        Check.Containers(P0T2C11, null, null, null); Check.Transitions(null, null, null, null);
    }

    // ----------------------------------------------------------------------------------------------------
    // Adds a slow transition, then a fast transition. After the transition time of the fast transition,
    // the system should remove both transitions since the fast transition is 100% finished and the slow
    // one has no more impact on camera position/orientation
    // ----------------------------------------------------------------------------------------------------
    // INITIAL STATE : {P0T2C01} {} {} {} -> [P0T2C11] [] [] []
    // COMMAND       : Transition[P0T1C00]
    // INTER STATE   : {P0T2C01} {} {} {} -> [P0T2C11] [P0T1C00] [] []
    // WAIT          : T1
    // FINAL STATE   : {P0T1C00} {} {} {} -> [] [] [] []
    // ----------------------------------------------------------------------------------------------------
    [UnitTest]
    public void TestInstructorCamera_AddSlowAndFastTransitions()
    {
        SetInitialTransitionState(P0T2C11, false, null, false, null, false, null, false);
        m_Camera.AddTransition(P0T1C00);
        Check.Containers(P0T2C01, null, null, null); Check.Transitions(P0T2C11, P0T1C00, null, null);
        Wait(m_FastTime);
        Check.Containers(P0T1C00, null, null, null); Check.Transitions(null, null, null, null);
    }

    // ----------------------------------------------------------------------------------------------------
    // Adds a fast transition, then a slow one
    // ----------------------------------------------------------------------------------------------------
    // INITIAL STATE : {P0T2C01} {} {} {} -> [P0T1C10] [] [] []
    // COMMAND       : Transition[P0T2C11]
    // INTER STATE   : {P0T2C01} {} {} {} -> [P0T1C10] [P0T2C11] [] []
    // WAIT          : T1
    // INNER STATE   : {P0T1C10} {} {} {} -> [P0T2C11] [] [] []
    // WAIT          : T1
    // FINAL STATE   : {P0T2C11} {} {} {} -> [] [] [] []
    // ----------------------------------------------------------------------------------------------------
    [UnitTest]
    public void TestInstructorCamera_AddFastAndSlowTransitions()
    {
        SetInitialTransitionState(P0T1C10, false, null, false, null, false, null, false);
        m_Camera.AddTransition(P0T2C11);
        Check.Containers(P0T2C01, null, null, null); Check.Transitions(P0T1C10, P0T2C11, null, null);
        Wait(m_FastTime);
        Check.Containers(P0T1C10, null, null, null); Check.Transitions(P0T2C11, null, null, null);
        Wait(m_FastTime);
        Check.Containers(P0T2C11, null, null, null); Check.Transitions(null, null, null, null);
    }

    // ----------------------------------------------------------------------------------------------------
    // Interrupts a transition with a cut
    // ----------------------------------------------------------------------------------------------------
    // INITIAL STATE : {P0T2C01} {} {} {} -> [P0T2C11] [] [] []
    // COMMAND       : Cut[P0T1C10]
    // FINAL STATE   : {} {} {} {} -> [] [] [] []
    // ----------------------------------------------------------------------------------------------------
    [UnitTest]
    public void TestInstructorCamera_InterruptTransitionWithCut()
    {
        SetInitialTransitionState(P0T2C11, false, null, false, null, false, null, false);
        m_Camera.AddCut(P0T1C10);
        Check.Containers(P0T1C10, null, null, null); Check.Transitions(null, null, null, null);
    }

    // ----------------------------------------------------------------------------------------------------
    // Interrupts multiple transitions with a cut
    // ----------------------------------------------------------------------------------------------------
    // INITIAL STATE : {P0T2C01} {} {} {} -> [P0T2C11] [P0T1C10] [P0T1C00] [P0T2C01]
    // COMMAND       : Cut[P0T1C00]
    // FINAL STATE   : {P0T1C00} {} {} {} -> [] [] [] []
    // ----------------------------------------------------------------------------------------------------
    [UnitTest]
    public void TestInstructorCamera_InterruptMultipleTransitionsWithCut()
    {
        SetInitialTransitionState(P0T2C11, false, P0T1C10, false, P0T1C00, false, P0T2C01, false);
        m_Camera.AddCut(P0T1C00);
        Check.Containers(P0T1C00, null, null, null); Check.Transitions(null, null, null, null);
    }

    // ----------------------------------------------------------------------------------------------------
    // Tests the camera matrix during multiple transitions
    // ----------------------------------------------------------------------------------------------------
    [UnitTest]
    public void TestInstructorCamera_MultipleTransitionsMatrix()
    {
        SetInitialTransitionState(P0T2C11, false, P0T2C01, false, null, false, null, false);
        FixedWait(m_SlowTime / 2);
        Check.Position(0.25f, 1.0f);
        Check.OrientationCloseTo((Vector3.up + Vector3.back) / 2);
    }

    // ----------------------------------------------------------------------------------------------------
    // Test adding a transition that is currently the active container
    // ----------------------------------------------------------------------------------------------------
    // INITIAL STATE : {P0T2C01} {} {} {} -> [] [] [] []
    // COMMAND       : Transition[P0T2C01]
    // FINAL STATE   : {P0T2C01} {} {} {} -> [] [] [] []
    // ----------------------------------------------------------------------------------------------------
    [UnitTest]
    public void TestInstructorCamera_TransitionToItself()
    {
        m_Camera.AddTransition(P0T2C01);
        Check.Containers(P0T2C01, null, null, null); Check.Transitions(null, null, null, null);
    }

    // ----------------------------------------------------------------------------------------------------
    // Test adding a transition that is currently the active container but with another transition before
    // it
    // ----------------------------------------------------------------------------------------------------
    // INITIAL STATE : {P0T2C01} {} {} {} -> [P0T2C11] [] [] []
    // COMMAND       : Transition[P0T2C01]
    // FINAL STATE   : {P0T2C01} {} {} {} -> [P0T2C11] [P0T2C01] [] []
    // ----------------------------------------------------------------------------------------------------
    [UnitTest]
    public void TestInstructorCamera_TransitionInSandwich()
    {
        SetInitialTransitionState(P0T2C11, false, null, false, null, false, null, false);
        m_Camera.AddTransition(P0T2C01);
        Check.Containers(P0T2C01, null, null, null); Check.Transitions(P0T2C11, P0T2C01, null, null);
    }

    // ----------------------------------------------------------------------------------------------------
    // Test adding a transition while already having it last in the transition list
    // ----------------------------------------------------------------------------------------------------
    // INITIAL STATE : {P0T2C01} {} {} {} -> [P0T2C11] [] [] []
    // COMMAND       : Transition[P0T2C11]
    // FINAL STATE   : {P0T2C01} {} {} {} -> [P0T2C11] [] [] []
    // ----------------------------------------------------------------------------------------------------
    [UnitTest]
    public void TestInstructorCamera_AddingSameTransitionTwice()
    {
        SetInitialTransitionState(P0T2C11, false, null, false, null, false, null, false);
        m_Camera.AddTransition(P0T2C11);
        Check.Containers(P0T2C01, null, null, null); Check.Transitions(P0T2C11, null, null, null);
    }

    // ----------------------------------------------------------------------------------------------------
    // Test adding a transition that exists in the transition list but another transition is in between
    // ----------------------------------------------------------------------------------------------------
    // INITIAL STATE : {P0T2C01} {} {} {} -> [P0T2C11] [P0T2C01] [] []
    // COMMAND       : Transition[P0T2C11]
    // FINAL STATE   : {P0T2C01} {} {} {} -> [P0T2C11] [P0T2C01] [P0T2C11] []
    // ----------------------------------------------------------------------------------------------------
    [UnitTest]
    public void TestInstructorCamera_TransitionInATransitionSandwich()
    {
        SetInitialTransitionState(P0T2C11, false, P0T2C01, false, null, false, null, false);
        m_Camera.AddTransition(P0T2C11);
        Check.Containers(P0T2C01, null, null, null); Check.Transitions(P0T2C11, P0T2C01, P0T2C11, null);
    }
}
