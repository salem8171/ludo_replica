using UnityEngine;

public class Dice {

    public int value = 6;

    public void Roll()
    {
        value = Random.Range(1, 7);
    }
}
