using UnityEngine;
using System.Collections;

public class BoundaryNode : MonoBehaviour
{
	public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position, 1.0f);
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, 1.0f);
    }
}
