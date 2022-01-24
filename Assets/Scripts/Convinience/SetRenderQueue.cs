using UnityEngine;

[AddComponentMenu("Rendering/SetRenderQueue")]

public class SetRenderQueue : MonoBehaviour
{
    public bool hasRenderPriority;

    [SerializeField]
    protected int[] m_queues = new int[] { 3000 };

    protected void Awake()
    {
        if (hasRenderPriority)
        {
            Material[] materials = GetComponent<Renderer>().materials;
            for (int i = 0; i < materials.Length && i < m_queues.Length; ++i)
            {
                materials[i].renderQueue = m_queues[i];
            }
        }
    }
}
