using System;
using UnityEngine;
namespace SMI.Example
{
    public class TimedObjectDestructor : MonoBehaviour
    {
        [SerializeField]
        private float destroyThisAfter = 2.0f;


        private void Awake()
        {
            Invoke("DestroyThisObject", destroyThisAfter);
        }


        private void DestroyThisObject()
        {
            DestroyObject(gameObject);
        }
    }

}