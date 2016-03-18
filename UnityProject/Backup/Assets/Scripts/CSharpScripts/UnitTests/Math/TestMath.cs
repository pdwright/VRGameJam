using UnityEngine;
using System.Collections;
using SharpUnit;

// ----------------------------------------------------------------------------------------------------
// Tests the Math module
// ----------------------------------------------------------------------------------------------------
public class TestMath : TestCase
{
    private Vector3 m_P000 = new Vector3(0, 0, 0);
    private Vector3 m_P002 = new Vector3(0, 0, 2);

    // ----------------------------------------------------------------------------------------------------
    // Setup test resources, called before each test
    // ----------------------------------------------------------------------------------------------------
    public override void SetUp()
    {
        m_P000 = new Vector3(0, 0, 0);
        m_P002 = new Vector3(0, 0, 2);
    }

    // ----------------------------------------------------------------------------------------------------
    // Dispose of test resources, called after each test
    // ----------------------------------------------------------------------------------------------------
    public override void TearDown()
    {
        m_P000 = Vector3.zero;
        m_P002 = Vector3.zero;
    }

    // ----------------------------------------------------------------------------------------------------
    // Verifies lower return value limit of GetSinEquivalent
    // ----------------------------------------------------------------------------------------------------
    [UnitTest]
    public void TestMath_GetSinEquivalentLower()
    {
        Assert.Equal(Math.GetSinEquivalent(0.0f), 0.0f);
    }

    // ----------------------------------------------------------------------------------------------------
    // Verifies middle return value of GetSinEquivalent
    // ----------------------------------------------------------------------------------------------------
    [UnitTest]
    public void TestMath_GetSinEquivalentMiddle()
    {
        Assert.Equal(Math.GetSinEquivalent(0.5f), 0.5f);
    }

    // ----------------------------------------------------------------------------------------------------
    // Verifies higher return value limit of GetSinEquivalent
    // ----------------------------------------------------------------------------------------------------
    [UnitTest]
    public void TestMath_GetSinEquivalentHigher()
    {
        Assert.Equal(Math.GetSinEquivalent(1.0f), 1.0f);
    }

    // ----------------------------------------------------------------------------------------------------
    // Verifies a simple projection on a segment
    // ----------------------------------------------------------------------------------------------------
    [UnitTest]
    public void TestMath_ProjectOnSegmentMiddle()
    {
        Vector3 p011 = new Vector3(0, 1, 1);
        Vector3 p001 = new Vector3(0, 0, 1);

        Vector3 projection = Math.ProjectPointOnSegment(m_P000, m_P002, p011);
        Assert.Equal(p001.x, projection.x);
        Assert.Equal(p001.y, projection.y);
        Assert.Equal(p001.z, projection.z);
    }

    // ----------------------------------------------------------------------------------------------------
    // Verifies a projection resulting to segment start
    // ----------------------------------------------------------------------------------------------------
    [UnitTest]
    public void TestMath_ProjectOnSegmentStart()
    {
        Vector3 p050 = new Vector3(0, 5, 0);

        Vector3 projection = Math.ProjectPointOnSegment(m_P000, m_P002, p050);
        Assert.Equal(m_P000.x, projection.x);
        Assert.Equal(m_P000.y, projection.y);
        Assert.Equal(m_P000.z, projection.z);
    }

    // ----------------------------------------------------------------------------------------------------
    // Verifies a projection resulting to segment end
    // ----------------------------------------------------------------------------------------------------
    [UnitTest]
    public void TestMath_ProjectOnSegmentEnd()
    {
        Vector3 p055 = new Vector3(0, 5, 5);

        Vector3 projection = Math.ProjectPointOnSegment(m_P000, m_P002, p055);
        Assert.Equal(m_P002.x, projection.x);
        Assert.Equal(m_P002.y, projection.y);
        Assert.Equal(m_P002.z, projection.z);
    }
}
