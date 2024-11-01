using UnityEngine;
using UnityEngine.SceneManagement;

public class Yandere : MonoBehaviour
{
	public bool CanMove = true;

	public bool Running;

	[Space]
	public float WalkSpeed = 1f;

	public float RunSpeed = 5f;

	[Space]
	public string IdleAnimation = "f02_idleShort_00";

	public string WalkAnimation = "f02_newWalk_00";

	public string SprintAnimation = "f02_newSprint_00";

	[Space]
	public Camera mainCamera;

	private CharacterController controller;

	private Animation animations;

	private Vector3 targetDirection;

	private Quaternion targetRotation;

	private void Start()
	{
		controller = GetComponent<CharacterController>();
		animations = GetComponent<Animation>();
	}

	public void Update()
	{
		if (Input.GetKeyDown("=") && Time.timeScale < 9f)
		{
			Time.timeScale += 1f;
		}
		if (Input.GetKeyDown("-") && Time.timeScale > 1f)
		{
			Time.timeScale -= 1f;
		}
		if (Input.GetKeyDown("`"))
		{
			Time.timeScale = 1f;
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			return;
		}
		if (base.transform.position.y < -5f)
		{
			base.transform.position = Vector3.zero;
		}
		Running = CanMove && Input.GetKey(KeyCode.LeftShift);
		if (CanMove)
		{
			controller.Move(Physics.gravity * 0.1f);
			float axisRaw = Input.GetAxisRaw("Vertical");
			float axisRaw2 = Input.GetAxisRaw("Horizontal");
			if (mainCamera.orthographic)
			{
				targetDirection = new Vector3(axisRaw2, 0f, axisRaw);
			}
			else
			{
				Vector3 vector = mainCamera.transform.TransformDirection(Vector3.forward);
				vector.y = 0f;
				vector = vector.normalized;
				Vector3 vector2 = new Vector3(vector.z, 0f, 0f - vector.x);
				targetDirection = axisRaw2 * vector2 + axisRaw * vector;
			}
			if (targetDirection != Vector3.zero)
			{
				targetRotation = Quaternion.LookRotation(targetDirection);
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, targetRotation, Time.deltaTime * 10f);
			}
			else
			{
				targetRotation = new Quaternion(0f, 0f, 0f, 0f);
			}
			if (axisRaw != 0f || axisRaw2 != 0f)
			{
				animations.CrossFade((!Running) ? WalkAnimation : SprintAnimation);
				controller.Move(base.transform.forward * Time.deltaTime * ((!Running) ? WalkSpeed : RunSpeed));
			}
			else
			{
				animations.CrossFade(IdleAnimation);
			}
		}
		else
		{
			animations.CrossFade(IdleAnimation);
		}
	}
}
