/*Property of Dorothea "Dori" B-Maroti
----All rights reserved----*/

using UnityEngine.Events;

public class Enemy : Character
{
    public UnityEvent died;
    public UnityEvent<uint> increaseScore;
    public uint scoreValue;
    protected int xDirection = 1;

    public override void Die()
    {
        gameObject.SetActive(false);
        increaseScore.Invoke(scoreValue);
    }
}