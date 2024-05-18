using UnityEngine;
using static UnityEngine.Physics;
using static UnityEngine.Mathf;
using static Source.Utils.Utils;

public class RopeLegs : MonoBehaviour{
    [SerializeField] private float stepDistance = 5;
    [SerializeField] private float legLength = 20;
    [SerializeField] private float moveTime = 0.5f;
    [SerializeField] private Rope[] ropes;
    
    public Leg[] legs;
    
    private void Awake(){
        legs = new Leg[ropes.Length];
        for (int i = 0; i < legs.Length; i++){
            legs[i] = new Leg();
            legs[i].rope = ropes[i];
        }
    }
    
    private void Update(){
        for (int i = 0; i < ropes.Length; i++){
            if (legs[i].moving){
                legs[i].moveT += Time.deltaTime / moveTime;
                legs[i].rope.SetEndPos(Vector3.Lerp(legs[i].startMovePoint, legs[i].targetMovePoint, EaseInOutQuad(legs[i].moveT)));
                if (legs[i].moveT >= 1){
                    legs[i].moveT = 0;
                    legs[i].moving = false;
                    legs[i].lastMoveTimer = 0;
                    legs[i].standPoint = legs[i].targetMovePoint;
                    legs[i].connected = true;
                } else{
                    continue;
                }
            } else{
                legs[i].lastMoveTimer += Time.deltaTime;
            }
        
            if (Hit(legs[i].rope.transform, out var hit)){
                if (legs[i].lastMoveTimer > 0.1f
                                                && Vector3.Distance(hit.point, legs[i].targetMovePoint) > stepDistance
                                                && !legs[(i + 1) % ropes.Length].moving
                                                && !legs[(i - 1 + ropes.Length) % ropes.Length].moving){
                    legs[i].startMovePoint = legs[i].rope.EndPos();
                    legs[i].targetMovePoint = hit.point;
                    legs[i].normal = hit.normal;
                    legs[i].moving = true;
                    break;
                }
            } else if (Vector3.Distance(legs[i].standPoint, legs[i].rope.transform.position + legs[i].rope.transform.forward * legLength) > legLength){
                legs[i].connected = false;
                legs[i].rope.SetEndPos(Vector3.zero);
            }
        }
    }
    
    private bool Hit(Transform startTransform, out RaycastHit hit){
        if (Raycast(startTransform.position, startTransform.forward, out hit, legLength, Layers.Environment)){
            return true;
        } 
        // if (Raycast(startTransform.position, startTransform.right, out hit, legLength, Layers.Environment)){
        //     return true;
        // } 
        // if (Raycast(startTransform.position, -startTransform.right, out hit, legLength, Layers.Environment)){
        //     return true;
        // } 
        if (Raycast(startTransform.position, startTransform.forward - startTransform.right, out hit, legLength, Layers.Environment)){
            return true;
        } 
        if (Raycast(startTransform.position, startTransform.forward + startTransform.right, out hit, legLength, Layers.Environment)){
            return true;
        } 
        return false;
    }
}

public class Leg{
    public Rope rope;
    public Vector3 startMovePoint;
    public Vector3 targetMovePoint;
    public Vector3 standPoint;
    public Vector3 normal;
    public bool moving;
    public bool connected;
    public float moveT;
    public float lastMoveTimer;
}