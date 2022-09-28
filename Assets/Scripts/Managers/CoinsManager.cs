using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CoinsManager : MonoBehaviour
{
    public static CoinsManager Instance
    {
        get; private set;
    }
    [SerializeField]
    private Text _coinsAmountText;
    // updating UI coins time 
    [SerializeField]
    private float _updatingRate = 0.15f;
    public int coins { get; private set; }
    // amount of coins showing in UI 
    private int _coinsInUI;
    private int _startTextFontSize;
    private Coroutine _coinsAmountUptadingCoroutine;
    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    private void Start()
    {
        _startTextFontSize = _coinsAmountText.fontSize;
    }
    public void AddCoints(int amountOfCoins)
    {
        try
        {
            StopCoroutine(_coinsAmountUptadingCoroutine);
        }
        catch (System.NullReferenceException)
        { }

        coins += amountOfCoins;
        _coinsAmountUptadingCoroutine = StartCoroutine(UICoinsUpdating());
    }

    private IEnumerator UICoinsUpdating()
    {
        while (_coinsInUI != coins)
        {
            if (_coinsInUI < coins)
                _coinsInUI++;
            else
                _coinsInUI--;

            _coinsAmountText.text = _coinsInUI.ToString();

            yield return StartCoroutine(OnUICointsTextAnimation());
        }
    }

    private IEnumerator OnUICointsTextAnimation()
    {
        float animationTime = _updatingRate;
        int endTextSize = _coinsAmountText.fontSize + _coinsAmountText.fontSize / 3;
        for (float t = 0; t < animationTime + Time.deltaTime; t += Time.deltaTime)
        {
            float process = t / animationTime;
            float value = Mathf.Sin(process * Mathf.PI);
            _coinsAmountText.fontSize = Mathf.RoundToInt(Mathf.Lerp(_startTextFontSize, endTextSize, value));
            yield return null;
        }
    }
}
