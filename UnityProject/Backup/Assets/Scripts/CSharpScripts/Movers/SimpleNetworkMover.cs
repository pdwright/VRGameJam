using UnityEngine;
using System.Collections;

public class SimpleNetworkMover : MonoBehaviour
{
    private NetworkCursor m_Cursor = new NetworkCursor();

    public Network m_Network;
    public float m_Speed;
    public Network.Mode m_Mode = Network.Mode.Forward;
    public bool m_ReverseDirection = false;

	void Start()
    {
        m_Cursor.Init(m_Network, m_ReverseDirection);
	}

	void Update()
    {
        m_Cursor.MoveFor(m_Speed * Time.deltaTime, m_Mode, ref m_ReverseDirection);
        transform.position = m_Cursor.Position;
	}
}
