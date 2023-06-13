using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject : MonoBehaviour
{
    // Start is called before the first frame update
    public ProjectilePool.POOL_OBJECT_TYPE type;
    public List<GameObject> poolObjectList;

    void OnDestroy()
    {
        poolObjectList.Remove(this.gameObject);
    }
}
