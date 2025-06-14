using UnityEngine;

public class Player : MonoBehaviour
{
    private int _hp = 2;
    public GameManager manager;

    public void ChangeHp(int amount = -1)
    {
        _hp += amount;
        Debug.Log(_hp);
        if (_hp <= 0)
        {
            manager.EndGame();
        }
    }
}
