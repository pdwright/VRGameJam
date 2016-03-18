using UnityEngine;
using System.Collections;
using SharpUnit;

// ----------------------------------------------------------------------------------------------------
// Utility functions used by camera tests
//
// Legend for camera test representation :
// Containers : {} {} {} {}
// Active container : {P0T1C00}
// Transitions : [] [] [] []
// PriorityRelease : (1)(2)(3)
// Camera Cut : Cut[]
// Camera Transition : Transition[]
// Release priority cut : ReleaseC[]
// Release priority : ReleaseT[]
//
// Example :
// STATE : {P0T1C00} {} {P2T1C00} {} -> [P2T1C01] [P0T1C00(2)] [P3T1C00] [] : Camera has a
// container in priority 0 and 2, transitions are being added at index 0, 1 and 2 of priorities 2,
// release 2 and 3
// State after both transitions are finished : 
// {P0T1C00} {} {} {P3T1C00} -> [] [] [] [] : First transition of priority 2 has finished, then has
// been released, then priority 3 finished and ended up in the container
// ----------------------------------------------------------------------------------------------------
// ----------------------------------------------------------------------------------------------------
// UNITTEST TEMPLATE DESCRIPTION
// ----------------------------------------------------------------------------------------------------
// INITIAL STATE : {P0T2C01} {} {} {} -> [] [] [] []
// COMMAND       : COMMAND[]
// INTER STATE   : {} {} {} {} -> [] [] [] []
// WAIT          : T
// FINAL STATE   : {} {} {} {} -> [] [] [] []
// ----------------------------------------------------------------------------------------------------
    //[UnitTest]
    //public void TestInstructorCamera_()
    //{
    //    SetInitialContainerState(CONTAINER, null, null, null);
    //    SetInitialTransitionState(TRANSITION, false, null, false, null, false, null, false);
    //    m_Camera.COMMAND();
    //    Check.Containers(P0T2C01, null, null, null); Check.Transitions(null, null, null, null);
    //    Wait(T);
    //    Check.Containers(P0T2C01, null, null, null); Check.Transitions(null, null, null, null);
    //}
// ----------------------------------------------------------------------------------------------------
//OutputCameraContainers();
//OutputCameraTransitions();
//Debug.Log("Position : [" + m_Camera.__TestPosition + "], Orientation : [" + m_Camera.__TestOrientation + "]");
// ----------------------------------------------------------------------------------------------------
public class TestUtilitiesInstructorCamera : TestCase
{
    private const float m_PositionEpsilon = 0.01f;
    private const float m_OrientationAngleEpsilon = 1.0f; // 1 degree
    private const float m_Dt = 0.02f; // 1 degree

    protected static IInstructorCamera m_Camera = null;

    // ----------------------------------------------------------------------------------------------------
    // Static class for camera checks
    // ----------------------------------------------------------------------------------------------------
    protected static class Check
    {
        public static void Containers(MockTransition _none, MockTransition _zone1, MockTransition _zone2, MockTransition _zone3)
        {
            Assert.True(m_Camera.__TestContainers[ContainerPriority.None] == _none);
            Assert.True(m_Camera.__TestContainers[ContainerPriority.Zone1] == _zone1);
            Assert.True(m_Camera.__TestContainers[ContainerPriority.Zone2] == _zone2);
            Assert.True(m_Camera.__TestContainers[ContainerPriority.Zone3] == _zone3);
        }

        public static void Transitions(MockTransition _transition1, MockTransition _transition2, MockTransition _transition3, MockTransition _transition4)
        {
            MockTransition[] transitions = { _transition1, _transition2, _transition3, _transition4 };

            for (int i = 0; i < InstructorProcessor.m_TransitionMaxCount; i++)
            {
                if (m_Camera.__TestTransitions[i] == null)
                {
                    Assert.Null(transitions[i]);
                }
                else
                {
                    Assert.True(m_Camera.__TestTransitions[i].Container == (InstructorContainer)transitions[i]);
                }
            }
        }

        public static void Position(float _x, float _y)
        {
            Vector3 currentPosition = m_Camera.__TestPosition;
            Vector3 comparison = new Vector3(_x, _y);
            Assert.True((currentPosition - comparison).magnitude < m_PositionEpsilon);
        }

        public static void OrientationCloseTo(Vector3 _comparison)
        {
            Vector3 currentOrientation = m_Camera.__TestOrientation;
            Assert.True(Vector3.Angle(currentOrientation, _comparison) < m_OrientationAngleEpsilon);
        }
    }

    private GameObject m_TransitionP0T1C00 = null;
    private GameObject m_TransitionP0T2C01 = null;
    private GameObject m_TransitionP0T1C10 = null;
    private GameObject m_TransitionP0T2C11 = null;
    private GameObject m_TransitionP1T1C00 = null;
    private GameObject m_TransitionP1T2C01 = null;
    private GameObject m_TransitionP2T1C00 = null;
    private GameObject m_TransitionP2T2C01 = null;

    protected MockTransition P0T1C00 { get { return m_TransitionP0T1C00.GetComponent<MockTransition>(); } }
    protected MockTransition P0T2C01 { get { return m_TransitionP0T2C01.GetComponent<MockTransition>(); } }
    protected MockTransition P0T1C10 { get { return m_TransitionP0T1C10.GetComponent<MockTransition>(); } }
    protected MockTransition P0T2C11 { get { return m_TransitionP0T2C11.GetComponent<MockTransition>(); } }
    protected MockTransition P1T1C00 { get { return m_TransitionP1T1C00.GetComponent<MockTransition>(); } }
    protected MockTransition P1T2C01 { get { return m_TransitionP1T2C01.GetComponent<MockTransition>(); } }
    protected MockTransition P2T1C00 { get { return m_TransitionP2T1C00.GetComponent<MockTransition>(); } }
    protected MockTransition P2T2C01 { get { return m_TransitionP2T2C01.GetComponent<MockTransition>(); } }

    protected float m_FastTime = 1.0f;
    protected float m_SlowTime = 2.0f;

    // ----------------------------------------------------------------------------------------------------
    // Waits a specified time using multiple calls of m_Dt each
    // ----------------------------------------------------------------------------------------------------
    protected static void Wait(float _time)
    {
        float timer = 0.0f;
        while (timer < _time)
        {
            timer += m_Dt;
            m_Camera.Tick(m_Dt);
        }
    }

    // ----------------------------------------------------------------------------------------------------
    // Waits a fixed amount of time using a single call
    // ----------------------------------------------------------------------------------------------------
    protected static void FixedWait(float _time)
    {
        m_Camera.Tick(_time);
    }

    // ----------------------------------------------------------------------------------------------------
    // Outputs current containers in the camera (debug)
    // ----------------------------------------------------------------------------------------------------
    protected static void OutputCameraContainers()
    {
        Debug.Log("---------- ---------- Camera containers are : ---------- ----------");
        for (int i = 0; i < ContainerPriority.Count; i++)
        {
            if (m_Camera.__TestContainers[i] != null)
            {
                Debug.Log(m_Camera.__TestContainers[i].name + " at index " + i);
            }
        }
    }

    // ----------------------------------------------------------------------------------------------------
    // Outputs current transitions in the camera (debug)
    // ----------------------------------------------------------------------------------------------------
    protected static void OutputCameraTransitions()
    {
        Debug.Log("---------- ---------- Camera transitions are : ---------- ----------");
        for (int i = 0; i < InstructorProcessor.m_TransitionMaxCount; i++)
        {
            if (m_Camera.__TestTransitions[i] != null)
            {
                Debug.Log(m_Camera.__TestTransitions[i].Container.name + " at index " + i);
            }
        }
    }

    // ----------------------------------------------------------------------------------------------------
    // Acquires the test objects in the scene
    // ----------------------------------------------------------------------------------------------------
    protected void AcquireTestObjects()
    {
        m_Camera = GameObject.Find("InstructorCamera").GetComponent<InstructorCamera>();

        m_TransitionP0T1C00 = new GameObject("P0T1C00");
        m_TransitionP0T1C00.AddComponent<MockTransition>();
        m_TransitionP0T1C00.AddComponent<MockPositioner>();
        m_TransitionP0T1C00.AddComponent<MockOrienter>();
        P0T1C00.__TestSetPriority(ContainerPriority.None);
        P0T1C00.__TestSetTime(m_FastTime);

        m_TransitionP0T2C01 = new GameObject("P0T2C01");
        m_TransitionP0T2C01.AddComponent<MockTransition>();
        m_TransitionP0T2C01.AddComponent<MockPositioner>();
        m_TransitionP0T2C01.AddComponent<MockOrienter>();
        P0T2C01.__TestSetPriority(ContainerPriority.None);
        P0T2C01.__TestSetTime(m_SlowTime);

        m_TransitionP0T1C10 = new GameObject("P0T1C10");
        m_TransitionP0T1C10.AddComponent<MockTransition>();
        m_TransitionP0T1C10.AddComponent<MockPositioner>();
        m_TransitionP0T1C10.AddComponent<MockOrienter>();
        P0T1C10.__TestSetPriority(ContainerPriority.None);
        P0T1C10.__TestSetTime(m_FastTime);

        m_TransitionP0T2C11 = new GameObject("P0T2C11");
        m_TransitionP0T2C11.AddComponent<MockTransition>();
        m_TransitionP0T2C11.AddComponent<MockPositioner>();
        m_TransitionP0T2C11.AddComponent<MockOrienter>();
        P0T2C11.__TestSetPriority(ContainerPriority.None);
        P0T2C11.__TestSetTime(m_SlowTime);

        m_TransitionP1T1C00 = new GameObject("P1T1C00");
        m_TransitionP1T1C00.AddComponent<MockTransition>();
        m_TransitionP1T1C00.AddComponent<MockPositioner>();
        m_TransitionP1T1C00.AddComponent<MockOrienter>();
        P1T1C00.__TestSetPriority(ContainerPriority.Zone1);
        P1T1C00.__TestSetTime(m_FastTime);

        m_TransitionP1T2C01 = new GameObject("P1T2C01");
        m_TransitionP1T2C01.AddComponent<MockTransition>();
        m_TransitionP1T2C01.AddComponent<MockPositioner>();
        m_TransitionP1T2C01.AddComponent<MockOrienter>();
        P1T2C01.__TestSetPriority(ContainerPriority.Zone1);
        P1T2C01.__TestSetTime(m_SlowTime);

        m_TransitionP2T1C00 = new GameObject("P2T1C00");
        m_TransitionP2T1C00.AddComponent<MockTransition>();
        m_TransitionP2T1C00.AddComponent<MockPositioner>();
        m_TransitionP2T1C00.AddComponent<MockOrienter>();
        P2T1C00.__TestSetPriority(ContainerPriority.Zone2);
        P2T1C00.__TestSetTime(m_FastTime);

        m_TransitionP2T2C01 = new GameObject("P2T2C01");
        m_TransitionP2T2C01.AddComponent<MockTransition>();
        m_TransitionP2T2C01.AddComponent<MockPositioner>();
        m_TransitionP2T2C01.AddComponent<MockOrienter>();
        P2T2C01.__TestSetPriority(ContainerPriority.Zone2);
        P2T2C01.__TestSetTime(m_SlowTime);
    }

    // ----------------------------------------------------------------------------------------------------
    // Setup test resources, called before each test
    // ----------------------------------------------------------------------------------------------------
    public override void SetUp()
    {
        AcquireTestObjects();

        P0T1C00.__TestInit();
        P0T2C01.__TestInit();
        P0T1C10.__TestInit();
        P0T2C11.__TestInit();
        P1T1C00.__TestInit();
        P1T2C01.__TestInit();
        P2T1C00.__TestInit();
        P2T2C01.__TestInit();

        // Camera does not support having no instructors
        m_Camera.AddCut(P0T2C01);
        m_Camera.__TestInit();
    }

    // ----------------------------------------------------------------------------------------------------
    // Dispose of test resources, called after each test
    // ----------------------------------------------------------------------------------------------------
    public override void TearDown()
    {
        m_Camera.Reset();

        GameObject.Destroy(m_TransitionP0T1C00);
        GameObject.Destroy(m_TransitionP0T2C01);
        GameObject.Destroy(m_TransitionP0T1C10);
        GameObject.Destroy(m_TransitionP0T2C11);
        GameObject.Destroy(m_TransitionP1T1C00);
        GameObject.Destroy(m_TransitionP1T2C01);
        GameObject.Destroy(m_TransitionP2T1C00);
        GameObject.Destroy(m_TransitionP2T2C01);
    }

    // ----------------------------------------------------------------------------------------------------
    // Sets the initial containers in the camera
    // ----------------------------------------------------------------------------------------------------
    protected void SetInitialContainerState(MockTransition _none, MockTransition _zone1, MockTransition _zone2, MockTransition _zone3)
    {
        if (_none != null) m_Camera.AddCut(_none);
        if (_zone1 != null) m_Camera.AddCut(_zone1);
        if (_zone2 != null) m_Camera.AddCut(_zone2);
        if (_zone3 != null) m_Camera.AddCut(_zone3);

        // Tick so the position & orientation is updated
        FixedWait(0.0f);
    }

    // ----------------------------------------------------------------------------------------------------
    // Sets the initial transitions in the camera
    // ----------------------------------------------------------------------------------------------------
    protected void SetInitialTransitionState(MockTransition _transition1, bool _release1, MockTransition _transition2, bool _release2, MockTransition _transition3, bool _release3, MockTransition _transition4, bool _release4)
    {
        if (_transition1)
        {
            if (_release1) m_Camera.ReleasePriority(_transition1);
            else m_Camera.AddTransition(_transition1);
        }

        if (_transition2)
        {
            if (_release2) m_Camera.ReleasePriority(_transition2);
            else m_Camera.AddTransition(_transition2);
        }

        if (_transition3)
        {
            if (_release3) m_Camera.ReleasePriority(_transition3);
            else m_Camera.AddTransition(_transition3);
        }

        if (_transition4)
        {
            if (_release4) m_Camera.ReleasePriority(_transition4);
            else m_Camera.AddTransition(_transition4);
        }
    }
}
