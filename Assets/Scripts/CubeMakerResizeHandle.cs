using UnityEngine;
using System.Collections;

public class CubeMakerResizeHandle : MonoBehaviour
{
    public void OnDrawGizmos()
    {
        Gizmos.DrawSphere(this.transform.position, 0.05f);
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, 0.05f);
    }
}
