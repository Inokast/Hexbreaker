using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownCamera : MonoBehaviour
{
    private Vector3 startPos;
    [SerializeField] private Transform _target;
    [SerializeField] private float _height = 7f;
    [SerializeField] private float _distance = 20f;
    [SerializeField] private float _angle = 45f;
    [SerializeField] private float _smoothSpeed = 0.5f;

    [SerializeField] private float _targetVerticalOffset = 0;

    private Vector3 refVelocity;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        HandleCamera();
        //_target = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        HandleCamera();
    }

    protected virtual void HandleCamera()
    {
        if (!_target)
        {
            return;
        }

        // Build world position vector
        Vector3 worldPosition = Vector3.forward * -_distance + Vector3.up * _height;
        Debug.DrawLine(_target.position, worldPosition, Color.red);

        // Build our Rotated vector
        Vector3 rotatedVector = Quaternion.AngleAxis(_angle, Vector3.up) * worldPosition;
        Debug.DrawLine(_target.position, rotatedVector, Color.green);

        // Move our position
        Vector3 flatTargetPos = _target.position;
        flatTargetPos.y = 0f;
        Vector3 finalPos = flatTargetPos + rotatedVector;
        Vector3 finalTargetPos = new Vector3(_target.position.x, _target.position.y + _targetVerticalOffset, _target.position.z);
        Debug.DrawLine(_target.position, finalPos, Color.blue);

        transform.position = Vector3.SmoothDamp(transform.position, finalPos, ref refVelocity, _smoothSpeed);
        transform.LookAt(finalTargetPos);

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.25f);
        if (_target) 
        {
            Gizmos.DrawLine(transform.position, _target.position);
            Gizmos.DrawSphere(_target.position, .5f);
        }

        Gizmos.DrawSphere(transform.position, .5f);
    }
}
