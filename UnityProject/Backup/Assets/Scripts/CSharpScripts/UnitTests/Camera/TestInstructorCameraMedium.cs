using UnityEngine;
using System.Collections;
using SharpUnit;

// ----------------------------------------------------------------------------------------------------
// Regular tests using transition priorities
// ----------------------------------------------------------------------------------------------------
public class TestInstructorCameraMedium : TestUtilitiesInstructorCamera
{
    // ----------------------------------------------------------------------------------------------------
    // Adds a cut that has a non-zero priority
    // ----------------------------------------------------------------------------------------------------
    // INITIAL STATE : {P0T2C01} {} {} {} -> [] [] [] []
    // COMMAND       : Cut[P1T1C00]
    // FINAL STATE   : {P0T2C01} {P1T1C00} {} {} -> [] [] [] []
    // ----------------------------------------------------------------------------------------------------
    [UnitTest]
    public void TestInstructorCamera_AddPriorityCut()
    {
        m_Camera.AddCut(P1T1C00);
        Assert.Equal(ContainerPriority.Zone1, m_Camera.__TestPriority);
        Check.Containers(P0T2C01, P1T1C00, null, null); Check.Transitions(null, null, null, null);
    }

    // ----------------------------------------------------------------------------------------------------
    // Adds a transition that has a non-zero priority
    // ----------------------------------------------------------------------------------------------------
    // INITIAL STATE : {P0T2C01} {} {} {} -> [] [] [] []
    // COMMAND       : Transition[P1T1C00]
    // INTER STATE   : {P0T2C01} {} {} {} -> [P1T1C00] [] [] []
    // WAIT          : T1
    // FINAL STATE   : {P0T2C01} {P1T1C00} {} {} -> [] [] [] []
    // ----------------------------------------------------------------------------------------------------
    [UnitTest]
    public void TestInstructorCamera_AddPriorityTransition()
    {
        m_Camera.AddTransition(P1T1C00);
        Assert.Equal(ContainerPriority.None, m_Camera.__TestPriority);
        Check.Containers(P0T2C01, null, null, null); Check.Transitions(P1T1C00, null, null, null);
        Wait(m_FastTime);
        Check.Containers(P0T2C01, P1T1C00, null, null); Check.Transitions(null, null, null, null);
    }

    // ----------------------------------------------------------------------------------------------------
    // Changes an inactive container
    // ----------------------------------------------------------------------------------------------------
    // INITIAL STATE : {P1T1C00} {} {} {} -> [] [] [] []
    // COMMAND       : Transition[P0T2C01]
    // FINAL STATE   : {P0T2C01} {P1T1C00} {} {} -> [] [] [] []
    // ----------------------------------------------------------------------------------------------------
    [UnitTest]
    public void TestInstructorCamera_ChangeInactivePriority()
    {
        SetInitialContainerState(P1T1C00, null, null, null);
        m_Camera.AddTransition(P0T2C01);
        Assert.Equal(ContainerPriority.Zone1, m_Camera.__TestPriority);
        Check.Containers(P0T2C01, P1T1C00, null, null); Check.Transitions(null, null, null, null);
    }

    // ----------------------------------------------------------------------------------------------------
    // Releases an active priority
    // ----------------------------------------------------------------------------------------------------
    // INITIAL STATE : {P0T2C01} {P1T1C00} {} {} -> [] [] [] []
    // COMMAND       : ReleaseT[P1T1C00]
    // INTER STATE   : {P0T2C01} {P1T1C00} {} {} -> [P0T2C01(1)] [] [] []
    // WAIT          : T1
    // FINAL STATE   : {P0T2C01} {} {} {} -> [] [] [] []
    // ----------------------------------------------------------------------------------------------------
    [UnitTest]
    public void TestInstructorCamera_ReleasePriority()
    {
        SetInitialContainerState(P0T2C01, P1T1C00, null, null);
        m_Camera.ReleasePriority(P1T1C00);
        Check.Containers(P0T2C01, P1T1C00, null, null); Check.Transitions(P0T2C01, null, null, null);
        Wait(m_FastTime);
        Check.Containers(P0T2C01, null, null, null); Check.Transitions(null, null, null, null);
    }

    // ----------------------------------------------------------------------------------------------------
    // Releases an inactive priority
    // ----------------------------------------------------------------------------------------------------
    // INITIAL STATE : {P0T2C01} {P1T1C00} {P2T1C00} {} -> [] [] [] []
    // COMMAND       : ReleaseT[P1T1C00]
    // FINAL STATE   : {P0T2C01} {} {P2T1C00} {} -> [] [] [] []
    // ----------------------------------------------------------------------------------------------------
    [UnitTest]
    public void TestInstructorCamera_ReleaseInactivePriority()
    {
        SetInitialContainerState(P0T2C01, P1T1C00, P2T1C00, null);
        m_Camera.ReleasePriority(P1T1C00);
        Check.Containers(P0T2C01, null, P2T1C00, null); Check.Transitions(null, null, null, null);
    }

    // ----------------------------------------------------------------------------------------------------
    // Releases an active priority using a cut
    // ----------------------------------------------------------------------------------------------------
    // INITIAL STATE : {P0T2C01} {P1T1C00} {} {} -> [] [] [] []
    // COMMAND       : ReleaseC[P1T1C00]
    // FINAL STATE   : {P0T2C01} {} {} {} -> [] [] [] []
    // ----------------------------------------------------------------------------------------------------
    [UnitTest]
    public void TestInstructorCamera_ReleasePriorityCut()
    {
        SetInitialContainerState(P0T2C01, P1T1C00, null, null);
        m_Camera.ReleasePriorityCut(P1T1C00);
        Check.Containers(P0T2C01, null, null, null); Check.Transitions(null, null, null, null);
    }

    // ----------------------------------------------------------------------------------------------------
    // Releases an inactive priority using a cut
    // ----------------------------------------------------------------------------------------------------
    // INITIAL STATE : {P0T2C01} {P1T1C00} {P2T1C00} {} -> [] [] [] []
    // COMMAND       : ReleaseC[P1T1C00]
    // FINAL STATE   : {P0T2C01} {} {P2T1C00} {} -> [] [] [] []
    // ----------------------------------------------------------------------------------------------------
    [UnitTest]
    public void TestInstructorCamera_ReleaseInactivePriorityCut()
    {
        SetInitialContainerState(P0T2C01, P1T1C00, P2T1C00, null);
        m_Camera.ReleasePriorityCut(P1T1C00);
        Check.Containers(P0T2C01, null, P2T1C00, null); Check.Transitions(null, null, null, null);
    }

    // ----------------------------------------------------------------------------------------------------
    // Tests the appropriate matrix of the camera during a priority transition
    // ----------------------------------------------------------------------------------------------------
    [UnitTest]
    public void TestInstructorCamera_PriorityTransitionMatrix()
    {
        m_Camera.AddTransition(P1T1C00);
        FixedWait(m_FastTime / 2);
        Check.Position(0.0f, 0.5f);
        Check.OrientationCloseTo((Vector3.up + Vector3.forward) / 2);
    }

    // ----------------------------------------------------------------------------------------------------
    // Tests the appropriate matrix of the camera having a non-zero priority container
    // ----------------------------------------------------------------------------------------------------
    [UnitTest]
    public void TestInstructorCamera_PriorityMatrix()
    {
        SetInitialContainerState(P0T2C01, P1T1C00, P2T1C00, null);
        Assert.Equal(ContainerPriority.Zone2, m_Camera.__TestPriority);
        Check.Position(0.0f, 0.0f);
        Check.OrientationCloseTo(Vector3.forward);
    }

    // ----------------------------------------------------------------------------------------------------
    // Test adding a cut when lower priority transitions exist, they must be cleared and applied to the
    // camera
    // ----------------------------------------------------------------------------------------------------
    // INITIAL STATE : {P0T2C01} {} {} {} -> [P1T2C01] [] [] []
    // COMMAND       : Cut[P2T2C01]
    // FINAL STATE   : {P0T2C01} {P1T2C01} {P2T2C01} {} -> [] [] [] []
    // ----------------------------------------------------------------------------------------------------
    [UnitTest]
    public void TestInstructorCamera_CutToHigherPriorityWhileTransition()
    {
        SetInitialTransitionState(P1T2C01, false, null, false, null, false, null, false);
        m_Camera.AddCut(P2T2C01);
        Check.Containers(P0T2C01, P1T2C01, P2T2C01, null); Check.Transitions(null, null, null, null);
    }

    // TODO : Test release priority with a transition that is not the last container for this priority
}
