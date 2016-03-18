using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------------------------------------
// Functions necessary for making an instructor processor
// ----------------------------------------------------------------------------------------------------
public interface IInstructorProcessor
{
    // ----------------------------------------------------------------------------------------------------
    // For UnitTesting purposes, should never be used elsewhere
    // ----------------------------------------------------------------------------------------------------
    InstructorProcessor.TransitionInformation[] __TestTransitions { get; }
    int __TestPriority { get; }
    Vector3 __TestPosition { get; }
    Vector3 __TestOrientation { get; }
    Vector3 __TestBanking { get; }
    int __TestTransitionCount { get; }
    InstructorContainer[] __TestContainers { get; }
    // ----------------------------------------------------------------------------------------------------

    // ----------------------------------------------------------------------------------------------------
    // This instantly changes the instructors being used to compute position/orientation
    // ----------------------------------------------------------------------------------------------------
    void AddCut(Transition _transition);

    // ----------------------------------------------------------------------------------------------------
    // Adds a transition from a set of instructors to another
    // ----------------------------------------------------------------------------------------------------
    void AddTransition(Transition _transition);

    // ----------------------------------------------------------------------------------------------------
    // Releases the specified container if it is the last one for this priority by adding a transition to
    // the previous priority's container and setting the PriorityRelease of this transition
    // ----------------------------------------------------------------------------------------------------
    void ReleasePriority(Transition _transition);

    // ----------------------------------------------------------------------------------------------------
    // Instantly releases specified priority
    // ----------------------------------------------------------------------------------------------------
    void ReleasePriorityCut(Transition _transition);

    // ----------------------------------------------------------------------------------------------------
    // Resets the state of the instructor processor
    // ----------------------------------------------------------------------------------------------------
    void Reset();
}
