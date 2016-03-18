using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Processes sets of instructors and transitions to return appropriate position/orientation
// ----------------------------------------------------------------------------------------------------
public class InstructorProcessor : MonoBehaviour, IInstructorProcessor
{
    // ----------------------------------------------------------------------------------------------------
    // For UnitTesting purposes, should never be used elsewhere
    // ----------------------------------------------------------------------------------------------------
    public TransitionInformation[] __TestTransitions { get { return m_Transitions; } }
    public int __TestPriority { get { return m_Priority; } }
    public Vector3 __TestPosition { get { return m_Position; } }
    public Vector3 __TestOrientation { get { return m_Orientation; } }
    public Vector3 __TestBanking { get { return m_Banking; } }
    public int __TestTransitionCount { get { return m_TransitionCount; } }
    public InstructorContainer[] __TestContainers { get { return m_Containers; } }
    // ----------------------------------------------------------------------------------------------------

    public const int m_TransitionMaxCount = 4;
    public InstructorContainer[] m_Containers = new InstructorContainer[ContainerPriority.Count];
    public Transform m_TargetTransform;

    protected Vector3 m_Position;
    protected Vector3 m_Orientation;
    protected Vector3 m_Banking;
    protected int m_Priority = ContainerPriority.None;

    // ----------------------------------------------------------------------------------------------------
    // All the necessary information for a transition to a new set of instructors
    // ----------------------------------------------------------------------------------------------------
    public class TransitionInformation
    {
        public TransitionInformation(InstructorContainer _container, int _priority, float _time, int _releasePriority)
        {
            m_Container = _container;
            m_Priority = _priority;
            m_Time = _time;
            m_ReleasePriority = _releasePriority;
        }

        public InstructorContainer Container
        {
            get { return m_Container; }
        }

        public int Priority
        {
            get { return m_Priority; }
        }

        public float Time
        {
            get { return m_Time; }
        }

        public float Completion
        {
            get { return m_Completion; }
            set { m_Completion = value; }
        }

        public float SmoothCompletion
        {
            get { return m_SmoothCompletion; }
            set { m_SmoothCompletion = value; }
        }

        public int ReleasePriority
        {
            get { return m_ReleasePriority; }
        }

        private InstructorContainer m_Container;
        private int m_Priority;
        private float m_Time;
        private float m_Completion = 0.0f;
        private float m_SmoothCompletion = 0.0f;
        private int m_ReleasePriority = ContainerPriority.None;
    }

    private TransitionInformation[] m_Transitions = new TransitionInformation[m_TransitionMaxCount];
    private int m_TransitionCount = 0;

    // ----------------------------------------------------------------------------------------------------
    // Gets the position from the specified set of instructors
    // ----------------------------------------------------------------------------------------------------
    protected Vector3 GetPosition(InstructorContainer _container)
    {
        return _container.m_Positioner.GetPosition();
    }

    // ----------------------------------------------------------------------------------------------------
    // Gets the collided position from the specified set of instructors (will not actually be implemented
    // in this class)
    // ----------------------------------------------------------------------------------------------------
    protected Vector3 GetCollidedPosition(InstructorContainer _container, Vector3 position)
    {
        return position;
    }

    // ----------------------------------------------------------------------------------------------------
    // Gets the orientation from the specified set of instructors
    // ----------------------------------------------------------------------------------------------------
    protected Vector3 GetOrientation(InstructorContainer _container, Vector3 _position)
    {
        return _container.m_Orienter.GetOrientation(_position);
    }

    // ----------------------------------------------------------------------------------------------------
    // Gets the banking from the specified set of instructors (will not actually be implemented
    // in this class)
    // ----------------------------------------------------------------------------------------------------
    protected Vector3 GetBanking(InstructorContainer _container)
    {
        return Vector3.up;
    }

    // ----------------------------------------------------------------------------------------------------
    // Checks if specified priority is being released
    // ----------------------------------------------------------------------------------------------------
    private bool IsPriorityBeingReleased(int _priority)
    {
        bool beingReleased = false;
        for (int i = 0; i < m_TransitionCount; i++)
        {
            if (m_Transitions[i].ReleasePriority == _priority)
            {
                beingReleased = true;
            }
            else if (m_Transitions[i].Priority == _priority)
            {
                beingReleased = false;
            }
        }

        return beingReleased;
    }

    // ----------------------------------------------------------------------------------------------------
    // Checks if a higher priority transition exists in the list and is not being canceled
    // ----------------------------------------------------------------------------------------------------
    private bool HigherPriorityInList(int _priority)
    {
        for (int i = 0; i < m_TransitionCount; i++)
        {
            if (m_Transitions[i].Priority > _priority && !IsPriorityBeingReleased(m_Transitions[i].Priority))
            {
                return true;
            }
        }

        return false;
    }

    // ----------------------------------------------------------------------------------------------------
    // Checks if priorities higher than the one specified are being released or do not exist
    // ----------------------------------------------------------------------------------------------------
    private bool HigherPrioritiesBeingReleased(int _priority)
    {
        if (m_TransitionCount == 0)
        {
            for (int i = _priority + 1; i < ContainerPriority.Count; i++)
            {
                if (m_Containers[i] != null)
                {
                    return false;
                }
            }
        }
        else if (HigherPriorityInList(_priority))
        {
            return false;
        }

        for (int i = _priority + 1; i < ContainerPriority.Count; i++)
        {
            if (m_Containers[i] != null && !IsPriorityBeingReleased(i))
            {
                return false;
            }
        }

        return true;
    }

    // ----------------------------------------------------------------------------------------------------
    // Checks if specified transition is the last added that has specified priority
    // ----------------------------------------------------------------------------------------------------
    private bool IsLastAddedTransition(Transition _transition)
    {
        for (int i = m_TransitionCount - 1; i >= 0; i--)
        {
            if (m_Transitions[i].Priority == _transition.m_Priority)
            {
                return m_Transitions[i].Container == _transition;
            }
        }

        return m_Containers[_transition.m_Priority] == _transition;
    }

    // ----------------------------------------------------------------------------------------------------
    // Gets the last used container for this priority, first goes through the transition list and if none
    // are found, returns the container
    // ----------------------------------------------------------------------------------------------------
    private InstructorContainer GetLastContainer(int _priority)
    {
        for (int i = m_TransitionCount - 1; i >= 0; i--)
        {
            if (m_Transitions[i].Priority == _priority)
            {
                return m_Transitions[i].Container;
            }
        }

        return m_Containers[_priority];
    }

    // ----------------------------------------------------------------------------------------------------
    // Inits instructors in the specified transition
    // ----------------------------------------------------------------------------------------------------
    protected void InitContainer(InstructorContainer _container)
    {
        _container.m_Positioner.Init(m_TargetTransform);
        _container.m_Orienter.Init(m_TargetTransform);
    }
    
    // ----------------------------------------------------------------------------------------------------
    // Swaps instructors to specified transition
    // ----------------------------------------------------------------------------------------------------
    public void AddCut(Transition _transition)
    {
        InitContainer(_transition);
        m_Containers[_transition.m_Priority] = _transition;

        for (int i = 0; i < m_TransitionCount; i++)
        {
            if (m_Transitions[i].Priority <= _transition.m_Priority)
            {
                if (m_Transitions[i].Priority < _transition.m_Priority)
                {
                    m_Containers[m_Transitions[i].Priority] = m_Transitions[i].Container;
                }
                
                if (m_Transitions[i].ReleasePriority != ContainerPriority.None)
                {
                    m_Containers[m_Transitions[i].ReleasePriority] = null;
                }

                // Remove the transition
                for (int j = i + 1; j < m_TransitionCount; j++)
                {
                    m_Transitions[j - 1] = m_Transitions[j];
                }

                m_Transitions[m_TransitionCount - 1] = null;
                m_TransitionCount--;
                i--;
            }
        }

        if (_transition.m_Priority > m_Priority)
        {
            m_Priority = _transition.m_Priority;
        }
    }

    // ----------------------------------------------------------------------------------------------------
    // Inserts transition in list after its corresponding priority
    // ----------------------------------------------------------------------------------------------------
    private void InsertTransition(InstructorContainer _container, int _priority, float _time, int _priorityRelease)
    {
        int insertionPriority = _priorityRelease != ContainerPriority.None ? _priorityRelease : _priority;

        for (int i = 0; i < m_TransitionCount; i++)
        {
            if (m_Transitions[i].Priority > insertionPriority)
            {
                Assertion.Assert(i < InstructorProcessor.m_TransitionMaxCount - 1, "Transition list is full, cannot insert.");

                for (int j = InstructorProcessor.m_TransitionMaxCount - 1; j > i; j--)
                {
                    m_Transitions[j] = m_Transitions[j - 1];
                }

                InitContainer(_container);
                m_Transitions[i] = new TransitionInformation(_container, _priority, _time, _priorityRelease);
                m_TransitionCount++;

                break;
            }
        }
    }

    // ----------------------------------------------------------------------------------------------------
    // Smoothly transfers instructors to specified transition
    // ----------------------------------------------------------------------------------------------------
    public void AddTransition(Transition _transition)
    {
        // First checks if we are not adding the same transition as we last added
        if ((m_TransitionCount == 0 && m_Containers[_transition.Priority] == _transition) || (m_TransitionCount > 0 && m_Transitions[m_TransitionCount - 1].Container == _transition))
        {
            // Well, nothing to do here
        }
        else if (HigherPrioritiesBeingReleased(_transition.m_Priority))
        {
            InitContainer(_transition);
            m_Transitions[m_TransitionCount] = 
                new TransitionInformation(
                    _transition, 
                    _transition.m_Priority, 
                    _transition.Time, 
                    ContainerPriority.None);
            m_TransitionCount++;
        }
        else if (_transition.m_Priority < m_Priority)
        {
            InitContainer(_transition);
            m_Containers[_transition.m_Priority] = _transition;
        }
        else
        {
            InsertTransition(_transition, _transition.Priority, _transition.Time, ContainerPriority.None);
        }
    }

    // ----------------------------------------------------------------------------------------------------
    // Releases specified priority if no one has overriden it yet
    // ----------------------------------------------------------------------------------------------------
    public void ReleasePriority(Transition _transition)
    {
        Assertion.Assert(_transition.m_Priority != ContainerPriority.None, "Cannot release priority 0, logic error.");

        // Only release priority if there are no other transitions following it with higher or equal priority
        // or if it has already been overriden in the containers
        if (IsLastAddedTransition(_transition))
        {
            if (m_Priority > _transition.m_Priority)
            {
                ReleasePriorityCut(_transition);
            }
            else
            {
                InstructorContainer lastContainer;
                for (int i = _transition.m_Priority - 1; i >= 0; i--)
                {
                    lastContainer = GetLastContainer(i);
                    if (lastContainer != null)
                    {
                        if (HigherPriorityInList(_transition.m_Priority))
                        {
                            InsertTransition(lastContainer, i, _transition.Time, _transition.Priority);
                        }
                        else
                        {
                            m_Transitions[m_TransitionCount] = new TransitionInformation(lastContainer, i, _transition.Time, _transition.Priority);
                            m_TransitionCount++;
                        }

                        break;
                    }
                }
            }
        }
    }

    // ----------------------------------------------------------------------------------------------------
    // Removes the first transition from the list and moves the whole list to the left. It is always the
    // first transition which gets removed since the transition list is ordered.
    // ----------------------------------------------------------------------------------------------------
    private void TerminateFirstTransition()
    {
        m_Priority = m_Transitions[0].Priority;
        m_Containers[m_Transitions[0].Priority] = m_Transitions[0].Container;

        if (m_Transitions[0].ReleasePriority != ContainerPriority.None)
        {
            m_Containers[m_Transitions[0].ReleasePriority] = null;
        }

        for (int j = 1; j < m_TransitionCount; j++)
        {
            m_Transitions[j - 1] = m_Transitions[j];
        }

        m_Transitions[m_TransitionCount - 1] = null;
        m_TransitionCount--;
    }

    // ----------------------------------------------------------------------------------------------------
    // Removes finished transitions from the list
    // ----------------------------------------------------------------------------------------------------
    private void RemoveFinishedTransitions()
    {
        for (int i = m_TransitionCount - 1; i >= 0; i--)
        {
            if (m_Transitions[i].Completion >= 1.0f)
            {
                for (int j = 0; j <= i; j++)
                {
                    TerminateFirstTransition();
                }

                break;
            }
        }
    }

    // ----------------------------------------------------------------------------------------------------
    // Instantly removes specified priority
    // ----------------------------------------------------------------------------------------------------
    public void ReleasePriorityCut(Transition _transition)
    {
        Assertion.Assert(_transition.m_Priority != ContainerPriority.None, "Cannot release priority 0, logic error.");

        if (m_Containers[_transition.m_Priority] == _transition)
        {
            m_Containers[_transition.m_Priority] = null;
            m_Priority = 0;

            for (int i = _transition.m_Priority - 1; i >= 1; i--)
            {
                if (m_Containers[i] != null)
                {
                    m_Priority = i;
                    break;
                }
            }
        }
    }

    // ----------------------------------------------------------------------------------------------------
    // Resets the state of the instructor processor
    // ----------------------------------------------------------------------------------------------------
    public void Reset()
    {
        for (int i = 0; i < ContainerPriority.Count; i++)
        {
            m_Containers[i] = null;
        }

        m_Position = Vector3.zero;
        m_Orientation = Vector3.forward;
        m_Banking = Vector3.up;

        m_Priority = ContainerPriority.None;

        for (int i = 0; i < m_TransitionMaxCount; i++)
        {
            m_Transitions[i] = null;
        }

        m_TransitionCount = 0;
    }

    // ----------------------------------------------------------------------------------------------------
    // Increases the percent of completion of each transition depending on transition's time
    // ----------------------------------------------------------------------------------------------------
    protected void IncrementTransitionCompletions(float _dt)
    {
        for (int i = 0; i < m_TransitionCount; i++)
        {
            m_Transitions[i].Completion += _dt / m_Transitions[i].Time;
        }
    }

    // ----------------------------------------------------------------------------------------------------
    // Updates the position after processing the transition list
    // ----------------------------------------------------------------------------------------------------
    protected void ProcessTransitionsPosition()
    {
        for (int i = 0; i < m_TransitionCount; i++)
        {
            m_Transitions[i].SmoothCompletion = 
                Math.GetSinEquivalent(m_Transitions[i].Completion);

            m_Position = 
                Vector3.Lerp(m_Position,
                             GetPosition(m_Transitions[i].Container),
                             m_Transitions[i].SmoothCompletion);
        }
    }

    // ----------------------------------------------------------------------------------------------------
    // Updates the orientation after processing the transition list, removes ended transitions afterwards
    // ----------------------------------------------------------------------------------------------------
    protected void ProcessTransitionsOrientation()
    {
        for (int i = 0; i < m_TransitionCount; i++)
        {
            m_Orientation = Vector3.Slerp(m_Orientation, GetOrientation(m_Transitions[i].Container, m_Position), m_Transitions[i].SmoothCompletion);
        }

        RemoveFinishedTransitions();
    }
}
