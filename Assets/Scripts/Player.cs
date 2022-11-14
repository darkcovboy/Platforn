using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private UnityEvent _hit;

    public int CoinCoint { get; private set; }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent(out Enemy enemy))
        {
            _hit?.Invoke();
        }

        if (collision.collider.TryGetComponent(out Coin coin))
        {
            Destroy(coin.gameObject);
            CoinCoint++;
        }
    }
}
