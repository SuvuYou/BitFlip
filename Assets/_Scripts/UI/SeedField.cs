using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SeedField : MonoBehaviour
{
    [SerializeField] private TMP_InputField _textField;
    [SerializeField] private Image _backImage;

    private bool _isRandomMode;

    private void Awake()
    {
        SetSeed(_textField.text);
        
        _textField.onValueChanged.AddListener(SetSeed);
    }

    private int _seed;
    public int Seed => _isRandomMode ? -1 : _seed;

    public void SetSeed(string seed) 
    {
        if (int.TryParse(seed, out int result)) _seed = result;
    } 

    public void SetSeedText(int seed) => _textField.text = seed.ToString();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) 
        {
            _isRandomMode = !_isRandomMode;

            _backImage.color = _isRandomMode ? Color.red : Color.white;
        }
    }
}