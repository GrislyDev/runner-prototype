using TMPro;
using UnityEngine;

public class AuthUIHandler : MonoBehaviour
{
	public static AuthUIHandler Instance { get; private set; }

	[SerializeField] private GameObject _registrationPanel;
	[SerializeField] private GameObject _loginPanel;
	[SerializeField] private TextMeshProUGUI _errorMsgText;

	// Login UI variables
	[Header("Login")]
	public TMP_InputField EmailLoginField;
	public TMP_InputField PasswordLoginField;

	// Registration UI variables
	[Header("Registration")]
	public TMP_InputField EmailRegistrationField;
	public TMP_InputField UsernameRegistrationField;
	public TMP_InputField PasswordRegistrationField;
	public TMP_InputField ConfirmPasswordRegistrationField;

	// Error text UI
	[Header("Log")]
	public TextMeshProUGUI ErrorText;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}

	private void Start()
	{
		_errorMsgText.text = string.Empty;
	}
	public void OpenRegistrationPanel()
	{
		_registrationPanel.SetActive(true);
		_loginPanel.SetActive(false);
		ClearRegistrationFields();
		_errorMsgText.text = string.Empty;
	}
	public void OpenLoginPanel()
	{
		_loginPanel.SetActive(true);
		_registrationPanel.SetActive(false);
		_errorMsgText.text = string.Empty;
	}

	public void ClearRegistrationFields()
	{
		EmailRegistrationField.text = string.Empty;
		UsernameRegistrationField.text = string.Empty;
		PasswordRegistrationField.text = string.Empty;
		ConfirmPasswordRegistrationField.text = string.Empty;
	}

	public void Login()
	{
		FirebaseManager.Instance.Login();
	}

	public void Register()
	{
		FirebaseManager.Instance.Register();
	}
}