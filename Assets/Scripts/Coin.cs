using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour
{
    [SerializeField]
    private int _amount = 10;
    [SerializeField]
    private GameObject _cointTakenEffectPrefab;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Ball>())
        {
            CoinsManager.Instance.AddCoints(_amount);
            StartCoroutine(OnCollidedWithBall());
        }
    }

    private IEnumerator OnCollidedWithBall()
    {
        float animationTime = 0.2f;

        for (float t = 0; t < animationTime; t += Time.deltaTime)
        {
            float process = t / animationTime;
            Vector3 newScale = Vector3.Lerp(Vector3.one, Vector3.zero, process);
            transform.localScale = newScale;
            yield return null;
        }

        GameObject effect = Instantiate(_cointTakenEffectPrefab);
        effect.transform.position = transform.position;
        Destroy(gameObject);
    }
}
