/*Property of Dorothea "Dori" B-Maroti
----All rights reserved----*/

using UnityEngine;

public class Block : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        gameObject.SetActive(false);
    }
}