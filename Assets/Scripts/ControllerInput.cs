using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("Event/Controller Input Module")]
public class ControllerInput: BaseInputModule
{
	private float m_NextAction;

	protected ControllerInput()
	{ }

	[SerializeField]
	private string m_HorizontalAxis = "Horizontal";

	/// <summary>
	/// Name of the vertical axis for movement (if axis events are used).
	/// </summary>
	[SerializeField]
	private string m_VerticalAxis = "Vertical";

	/// <summary>
	/// Name of the submit button.
	/// </summary>
	[SerializeField]
	private string m_SubmitButton = "Submit";

	/// <summary>
	/// Name of the submit button.
	/// </summary>
	[SerializeField]
	private string m_CancelButton = "Cancel";

	[SerializeField]
	private float m_InputActionsPerSecond = 10;

	/// <summary>
	/// Name of the horizontal axis for movement (if axis events are used).
	/// </summary>
	public string HorizontalAxis
	{
		get { return m_HorizontalAxis; }
		set { m_HorizontalAxis = value; }
	}

	/// <summary>
	/// Name of the vertical axis for movement (if axis events are used).
	/// </summary>
	public string VerticalAxis
	{
		get { return m_VerticalAxis; }
		set { m_VerticalAxis = value; }
	}

	public string SubmitButton
	{
		get { return m_SubmitButton; }
		set { m_SubmitButton = value; }
	}

	public string CancelButton
	{
		get { return m_CancelButton; }
		set { m_CancelButton = value; }
	}

	public override bool ShouldActivateModule()
	{
		if (!base.ShouldActivateModule())
			return false;

		bool shouldActivate = false;
		shouldActivate |= Input.GetButtonDown(m_SubmitButton);
		shouldActivate |= Input.GetButtonDown(m_CancelButton);
		shouldActivate |= !Mathf.Approximately(Input.GetAxis(m_HorizontalAxis), 0.0f);
		shouldActivate |= !Mathf.Approximately(Input.GetAxis(m_VerticalAxis), 0.0f);
		return shouldActivate;
	}

	public override void ActivateModule()
	{
		base.ActivateModule();

		var baseEventData = GetBaseEventData();
		eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject, baseEventData);
	}

	public override void DeactivateModule()
	{
		base.DeactivateModule();

		var baseEventData = GetBaseEventData();
		eventSystem.SetSelectedGameObject(null, baseEventData);
	}

	public override void Process()
	{
		bool usedEvent = SendMoveEventToSelectedObject();

		if (!usedEvent)
			SendSubmitEventToSelectedObject();
	}

	/// <summary>
	/// Process submit keys.
	/// </summary>
	private bool SendSubmitEventToSelectedObject()
	{
		if (eventSystem.currentSelectedGameObject == null)
			return false;

		var data = GetBaseEventData();
		if (Input.GetButtonDown(m_SubmitButton))
			ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.submitHandler);

		if (Input.GetButtonDown(m_CancelButton))
			ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.cancelHandler);
		return data.used;
	}

	private bool AllowMoveEventProcessing(float time)
	{
		bool allow = Input.GetButtonDown(m_HorizontalAxis);
		allow |= Input.GetButtonDown(m_VerticalAxis);
		allow |= (time > m_NextAction);
		return allow;
	}

	private Vector2 GetRawMoveVector()
	{
		Vector2 move = Vector2.zero;
		move.x = Input.GetAxis(m_HorizontalAxis);
		move.y = Input.GetAxis(m_VerticalAxis);

		if (Input.GetButtonDown(m_HorizontalAxis))
		{
			if (move.x < 0)
				move.x = -1f;
			if (move.x > 0)
				move.x = 1f;
		}
		if (Input.GetButtonDown(m_VerticalAxis))
		{
			if (move.y < 0)
				move.y = -1f;
			if (move.y > 0)
				move.y = 1f;
		}
		return move;
	}

	/// <summary>
	/// Process keyboard events.
	/// </summary>
	private bool SendMoveEventToSelectedObject()
	{
		float time = Time.unscaledTime;

		if (!AllowMoveEventProcessing(time))
			return false;

		Vector2 movement = GetRawMoveVector();
		var axisEventData = GetAxisEventData(movement.x, movement.y, 0.6f);
		if (!Mathf.Approximately(axisEventData.moveVector.x, 0f)
			|| !Mathf.Approximately(axisEventData.moveVector.y, 0f))
		{
			ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler);
		}
		m_NextAction = time + 1f / m_InputActionsPerSecond;
		return axisEventData.used;
	}
}
