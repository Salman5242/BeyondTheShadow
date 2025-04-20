using Items.Logic;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HingeJoint))]
public class DoorDraggable : MonoBehaviour, IDraggable, IPrintsUV
{
    public bool IsDoorFullyOpened = false;
    public bool IsDoorClosed = false;

    public Transform InteractionTransform;
    [SerializeField]
    private GameObject _handprints;
    [SerializeField]
    private AudioSource _closeDoorAudioSource; 
    [SerializeField]
    private AudioSource _doorDragAudioSource;

    [SerializeField]
    private float _forceAmmount = 15f;
    [SerializeField]
    private float _distance = 1.5f;
    [SerializeField]
    private float _maxDrugVolume;

    private Collider _collider;
    private Rigidbody _rb;

    private Camera _cam;
    private bool _isInterracting = false;
    private bool _isDoorLocked = false;
    private HingeJoint _hingeJoint;
    private float _openedRotation; // Rotation of the door, when it is fully opened

    private float _doorDragCurrSpeed = 0f;
    private Vector3 _prevRotation;
    private float doorCurrRotation;

    private float _currDragAudioPercent = 0f;
    private const float MinDoorDrugSpeed = 0f;
    private const float MaxDoorDrugSpeed = 20f;
    private const float AverageDrugSpeed = 15f;

    private const float MinGhostsForcePower = 7f;
    private const float MaxGhostsForcePower = 17f;
    private const float CloseDoorForcePower = 250f;
    private const float MinGhostForceTime = 1.2f;
    private const float MaxGhostForceTime = 3f;

    private const float Epsilon = 1f;
    private const float CheckDoorStateCD = 0.3f;
    private const float CheckDoorRotationCD = 0.1f;
    private const float MotorForce = 100f;

    private WaitForSeconds WaitForDoorStateCheck;
    private WaitForSeconds WaitForDoorRotationCheck;
    void Awake()
    {
        _cam = Camera.main;
    }
    void Start()
    {
        _hingeJoint = GetComponent<HingeJoint>();
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();

        WaitForDoorStateCheck = new WaitForSeconds(CheckDoorStateCD);
        WaitForDoorRotationCheck = new WaitForSeconds(CheckDoorRotationCD);

        if (_doorDragAudioSource != null)
        {
            _doorDragAudioSource.volume = 0f;
            _doorDragAudioSource.Play();
        }
        _openedRotation = _hingeJoint.limits.max * _hingeJoint.axis.y;
        if (_openedRotation < -1f) _openedRotation += 360f;

        StartCoroutine(CheckDoorState());
        StartCoroutine(AdjustDragSound());
    }

    void FixedUpdate()
    {
        if (_isInterracting && !_isDoorLocked)
        {
            ToggleAudioSource(true);
            DragDoor();
            // if (IsDoorClosed) { Debug.Log("Door is closed"); }
            // else if (IsDoorFullyOpened) Debug.Log("Door is fully opened");
        }
        else ToggleAudioSource(false);
        
    }

    public void GhostDrugDoor()
    {
        GhostInterrectWithDoor(GenerateForce(), (float)GenerateDirection(), UnityEngine.Random.Range(MinGhostForceTime, MaxGhostForceTime));
    }

    public void LockTheDoor()
    {
        StartCoroutine(nameof(CloseDoorCoroutine));
    }

    public void UnlockTheDoor()
    {
        _rb.isKinematic = false;
        _isDoorLocked = false;
    }

    private IEnumerator CloseDoorCoroutine()
    {
        _collider.enabled = false;
        _isDoorLocked = true;
        GhostInterrectWithDoor(CloseDoorForcePower, -1f, 1f);

        while (true)
        {
            if (IsDoorFullyClosed()) { EnableCollider(); yield break; }
            yield return null;
        }
    }

    private void EnableCollider() { _collider.enabled = true; _rb.isKinematic = true; }
    private void GhostInterrectWithDoor(float force, float direction, float time)
    {
        _hingeJoint.useMotor = true;
        var motor = _hingeJoint.motor;
        motor.targetVelocity = force * direction;
        motor.force = MotorForce;
        _hingeJoint.motor = motor;
        Invoke(nameof(StopDruggingDoor), time);
    }
    private void StopDruggingDoor()
    {
        var motor = _hingeJoint.motor;
        motor.targetVelocity = 0f;
        _hingeJoint.motor = motor;
    }
    private float GenerateForce() => UnityEngine.Random.Range(MinGhostsForcePower, MaxGhostsForcePower);

    private int GenerateDirection()
    {
        int randomNum = UnityEngine.Random.Range(0, 1);
        if (randomNum == 0) randomNum = -1;
        if (IsDoorClosed) randomNum = 1;
        else if (IsDoorFullyOpened) randomNum = -1;
        return randomNum;
    }

    private IEnumerator CheckDoorState()
    {
        while (true)
        {
            CheckDoorRotation();
            yield return WaitForDoorStateCheck;
        }
    }
    private void CheckDoorRotation()
    {
        doorCurrRotation = transform.localEulerAngles.y;
        if (doorCurrRotation < -5f) doorCurrRotation += 360;
       
        if( Mathf.Abs( transform.localEulerAngles.y) <= Epsilon)
        {
            IsDoorClosed = true;
            IsDoorFullyOpened = false;
        }
        else if( Mathf.Abs(transform.localEulerAngles.y - _openedRotation) <= Epsilon)
        {
            IsDoorClosed = false;
            IsDoorFullyOpened = true;
        }
        else
        {
            IsDoorClosed = false;
            IsDoorFullyOpened = false;
        }
    }

    private bool IsDoorFullyClosed() => (Mathf.Abs(transform.localEulerAngles.y) <= Epsilon);

    private void DragDoor()
    {
        Ray playerAim = _cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        Vector3 nextPos = _cam.transform.position + playerAim.direction * _distance;
        Vector3 currPos = transform.position;

        _rb.velocity = (nextPos - currPos) * _forceAmmount;

    }

    private IEnumerator AdjustDragSound()
    {
        while (true)
        {
            if (_isInterracting)
            {
                _doorDragCurrSpeed = Vector3.Distance(transform.localEulerAngles, _prevRotation);
                _prevRotation = transform.localEulerAngles;

                _doorDragCurrSpeed = Mathf.Clamp(_doorDragCurrSpeed, MinDoorDrugSpeed, MaxDoorDrugSpeed);
                _currDragAudioPercent = _doorDragCurrSpeed / AverageDrugSpeed;

                _doorDragAudioSource.volume = Mathf.Clamp(_currDragAudioPercent , 0, _maxDrugVolume);
                AudioHelper.ChangeSoundSpeed(_doorDragAudioSource, _currDragAudioPercent, "DoorPitch");
            }

            yield return WaitForDoorRotationCheck;
        }
    }

    private void ToggleAudioSource(bool isAudioEnable)
    {
        if (!isAudioEnable && _doorDragAudioSource.isPlaying)
        {
            _doorDragAudioSource.Pause();
        }
        else if (isAudioEnable && !_doorDragAudioSource.isPlaying)
        {
            _doorDragAudioSource.Play();
        }
    }
        public void OnDragBegin()
    {
        _isInterracting = true;
        //Debug.Log("DrugBegin");
    }

    public void OnDragEnd()
    {
        _isInterracting = false;
        if (IsDoorFullyClosed())
        {
            if(!_closeDoorAudioSource.isPlaying)
            _closeDoorAudioSource.Play();
        }
    }

    public void LeavePrintsUV()
    {
        Instantiate(_handprints, InteractionTransform.position, InteractionTransform.rotation, InteractionTransform);
    }
}
