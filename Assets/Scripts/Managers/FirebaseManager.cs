using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using UnityEngine.SceneManagement;
using Firebase.Database;
using System.Linq;

public class FirebaseManager : MonoBehaviour
{
	// Constants
	private const int GAME_SCENE = 1;

	// Singleton
	public static FirebaseManager Instance { get; private set; }

	// Firebase variables
	[Header("Firebase")]
	[SerializeField] private DependencyStatus _dependencyStatus;
	private FirebaseAuth _auth;
	private FirebaseUser _user;
	private DatabaseReference _DBReference;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	private void Start()
	{
		_auth?.SignOut();

		// Check and fix Firebase dependencies asynchronously
		FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
		{
			_dependencyStatus = task.Result;

			if (_dependencyStatus == DependencyStatus.Available)
			{
				InitializeFirebase();
			}
			else
			{
				Debug.LogError("Could not resolve all firebase dependencies: " + _dependencyStatus);
			}
		});
	}

	// Initialize Firebase authentication
	private void InitializeFirebase()
	{
		_auth = FirebaseAuth.DefaultInstance;

		// Subscribe to the authentication state changed event
		_auth.StateChanged += AuthStateChanged;
		AuthStateChanged(this, null);

		_DBReference = FirebaseDatabase.DefaultInstance.RootReference;
	}

	#region AUTH
	// Handle authentication state change
	private void AuthStateChanged(object sender, System.EventArgs eventArgs)
	{
		if (_auth.CurrentUser != _user)
		{
			bool signedIn = _user != _auth.CurrentUser && _auth.CurrentUser != null;

			if (!signedIn && _user != null)
			{
				Debug.Log("Signed out " + _user.UserId);
			}

			_user = _auth.CurrentUser;

			if (signedIn)
			{
				Debug.Log("Signed int " + _user.UserId);
			}
		}
	}
	// Method for handling user login
	public void Login()
	{
		StartCoroutine(LoginAsync(AuthUIHandler.Instance.EmailLoginField.text, AuthUIHandler.Instance.PasswordLoginField.text));
	}
	// Coroutine for asynchronously logging in a user
	private IEnumerator LoginAsync(string email, string password)
	{
		var loginTask = _auth.SignInWithEmailAndPasswordAsync(email, password);

		yield return new WaitUntil(() => loginTask.IsCompleted);

		if (loginTask.Exception != null)
		{
			Debug.LogError(loginTask.Exception);

			FirebaseException firebaseException = loginTask.Exception.GetBaseException() as FirebaseException;
			AuthError authError = (AuthError)firebaseException.ErrorCode;

			string failedMessage = "Login Failed! Because ";

			switch (authError)
			{
				case AuthError.InvalidEmail:
					failedMessage += "Email is invalid";
					break;
				case AuthError.WrongPassword:
					failedMessage += "Wrong Password";
					break;
				case AuthError.MissingEmail:
					failedMessage += "Email is missing";
					break;
				case AuthError.MissingPassword:
					failedMessage += "Password is missing";
					break;
				default:
					failedMessage = "Login Failed";
					break;
			}

			AuthUIHandler.Instance.ErrorText.text = failedMessage;
		}
		else
		{
			_user = loginTask.Result.User;

			Debug.LogFormat("{0} You Are Successfully Logged In", _user.DisplayName);

			GameManager.CurrentUser = _user.DisplayName;
			SceneManager.LoadScene(GAME_SCENE);
		}
	}
	// Method for handling user registration
	public void Register()
	{
		StartCoroutine(RegisterAsync
			(AuthUIHandler.Instance.UsernameRegistrationField.text, AuthUIHandler.Instance.EmailRegistrationField.text,
			AuthUIHandler.Instance.PasswordRegistrationField.text, AuthUIHandler.Instance.ConfirmPasswordRegistrationField.text)
			);
	}
	// Coroutine for asynchronously registering a user
	private IEnumerator RegisterAsync(string username, string email, string password, string confirmPassword)
	{
		if (username == string.Empty)
		{
			var errorMsg = "Username field is empty";
			Debug.LogError(errorMsg);
			AuthUIHandler.Instance.ErrorText.text = errorMsg;
		}
		else if (email == string.Empty)
		{
			var errorMsg = "Email field is empty";
			Debug.LogError(errorMsg);
			AuthUIHandler.Instance.ErrorText.text = errorMsg;
		}
		else if (password != confirmPassword)
		{
			var errorMsg = "Password does not match";
			Debug.LogError(errorMsg);
			AuthUIHandler.Instance.ErrorText.text = errorMsg;
		}
		else
		{
			var registerTask = _auth.CreateUserWithEmailAndPasswordAsync(email, password);

			yield return new WaitUntil(() => registerTask.IsCompleted);

			if (registerTask.Exception != null)
			{
				Debug.LogError(registerTask.Exception);

				FirebaseException firebaseException = registerTask.Exception.GetBaseException() as FirebaseException;
				AuthError authError = (AuthError)firebaseException.ErrorCode;

				string failedMessage = "Registration Failed! Because ";
				switch (authError)
				{
					case AuthError.InvalidEmail:
						failedMessage += "Email is invalid";
						break;
					case AuthError.WrongPassword:
						failedMessage += "Wrong Password";
						break;
					case AuthError.MissingEmail:
						failedMessage += "Email is missing";
						break;
					case AuthError.MissingPassword:
						failedMessage += "Password is missing";
						break;
					default:
						failedMessage = "Registration Failed!";
						break;
				}

				AuthUIHandler.Instance.ErrorText.text = failedMessage;
			}
			else
			{
				_user = registerTask.Result.User;

				UserProfile userProfile = new UserProfile { DisplayName = username };

				var updateProfileTask = _user.UpdateUserProfileAsync(userProfile);

				yield return new WaitUntil(() => updateProfileTask.IsCompleted);

				if (updateProfileTask.Exception != null)
				{
					_user.DeleteAsync();

					Debug.LogError(updateProfileTask.Exception);

					FirebaseException firebaseException = updateProfileTask.Exception.GetBaseException() as FirebaseException;
					AuthError authError = (AuthError)firebaseException.ErrorCode;

					string failedMessage = "Profile update Failed! Because ";
					switch (authError)
					{
						case AuthError.InvalidEmail:
							failedMessage += "Email is invalid";
							break;
						case AuthError.WrongPassword:
							failedMessage += "Wrong Password";
							break;
						case AuthError.MissingEmail:
							failedMessage += "Email is missing";
							break;
						case AuthError.MissingPassword:
							failedMessage += "Password is missing";
							break;
						default:
							failedMessage = "Profile update Failed!";
							break;
					}

					AuthUIHandler.Instance.ErrorText.text = failedMessage;
				}
				else
				{
					Debug.Log("Registration Successful! Welcome " + _user.DisplayName);
					AuthUIHandler.Instance.OpenLoginPanel();

					AuthUIHandler.Instance.EmailLoginField.text = AuthUIHandler.Instance.EmailRegistrationField.text;
					AuthUIHandler.Instance.PasswordLoginField.text = string.Empty;
				}
			}
		}
	}
	#endregion

	#region DATABASE
	private IEnumerator UpdateUsernameAuthAsync(string username)
	{
		UserProfile profile = new UserProfile { DisplayName = username };

		var ProfileTask = _user.UpdateUserProfileAsync(profile);

		yield return new WaitUntil(() => ProfileTask.IsCompleted);

		if (ProfileTask.Exception != null)
		{
			Debug.LogWarning($"Failed to register task with {ProfileTask.Exception}");
		}
		else
		{
			// Auth username is now updated
		}
	}

	private IEnumerator UpdateUsernameDatabaseAsync(string username)
	{
		var DBTask = _DBReference.Child("users").Child(_user.UserId).Child("username").SetValueAsync(username);

		yield return new WaitUntil(() => DBTask.IsCompleted);

		if (DBTask.Exception != null)
		{
			Debug.LogWarning($"Failed to register task with {DBTask.Exception}");
		}
		else
		{
			// Database username is now updated
		}
	}

	public void SaveData()
	{
		if (GameManager.CurrentUser != null)
		{
			StartCoroutine(UpdateUsernameAuthAsync(GameManager.CurrentUser));
			StartCoroutine(UpdateUsernameDatabaseAsync(GameManager.CurrentUser));

			StartCoroutine(UpdateScoreAsync(ScoreManager.Instance.GetScore()));
		}
	}

	private IEnumerator UpdateScoreAsync(int score)
	{
		var DBTask = _DBReference.Child("users").Child(_user.UserId).Child("score").SetValueAsync(score);

		yield return new WaitUntil(() => DBTask.IsCompleted);

		if (DBTask.Exception != null)
		{
			Debug.LogWarning($"Failed to register task with {DBTask.Exception}");
		}
		else
		{
			// Scores are now updated
		}
	}

	private IEnumerator LoadLeaderboardDataAsync()
	{
		var DBTask = _DBReference.Child("users").OrderByChild("score").GetValueAsync();

		yield return new WaitUntil(() => DBTask.IsCompleted);

		if (DBTask.Exception != null)
		{
			Debug.LogWarning($"Failed to register task with {DBTask.Exception}");
		}
		else
		{
			// Data has been retrieved
			DataSnapshot snapshot = DBTask.Result;

			// Destroy any existing leaderboard elements
			foreach (Transform child in GameUIHandler.Instance.LeaderboardContext)
			{
				Destroy(child.gameObject);
			}

			// Loop through every userd UID
			int rank = 1;
			foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
			{
				string username = childSnapshot.Child("username").Value.ToString();
				int score = int.Parse(childSnapshot.Child("score").Value.ToString());

				// Instantiate new leaderboard elements
				var leaderboardElement =
					Instantiate(GameUIHandler.Instance.ScoreElement, GameUIHandler.Instance.LeaderboardContext);
				leaderboardElement.GetComponent<ScoreElement>().NewScoreElement(rank++, username, score);
			}
		}
	}

	public void LoadLeaderboard()
	{
		StartCoroutine(LoadLeaderboardDataAsync());
	}
	#endregion
}