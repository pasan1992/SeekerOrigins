using UnityEngine;
using UnityEngine.SceneManagement;

public class TheWorldScript : MonoBehaviour
{
    [SerializeField] float _alphaValue;
    [SerializeField] float _alphaValueSpeed;
    [SerializeField] int _speed;
    [SerializeField] int _camSpeed;
    [SerializeField] GameObject _cam;
    [SerializeField] CanvasGroup _canvasGroup;

    bool _isMenuActive = false;

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            _isMenuActive = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_isMenuActive)
        {
            CamAnim();
        }

        transform.Rotate(Vector3.right * Time.deltaTime * _speed);
    }

    void CamAnim()
    {
        if (_cam.transform.localRotation.x < 0)
        {
            _cam.transform.Rotate(Vector3.right * Time.deltaTime * _camSpeed);
        }
        else
        {
            _alphaValue += _alphaValue * _alphaValueSpeed;

            if (_canvasGroup.alpha != 1)
            {
                _canvasGroup.alpha = _alphaValue;

            }
            else
            {
                _isMenuActive = false;
            }
        }
    }
}
