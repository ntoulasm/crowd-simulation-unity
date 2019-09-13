using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {

    private static List<Obstacle> obstacles = new List<Obstacle>();

    protected Bounds bounds;

    protected virtual void Init() {
        obstacles.Add(this);
    }

    protected void DrawBounds() {

        Color color = Color.red;

        var p1 = new Vector3(bounds.min.x, bounds.min.y, bounds.min.z);
        var p2 = new Vector3(bounds.max.x, bounds.min.y, bounds.min.z);
        var p3 = new Vector3(bounds.max.x, bounds.min.y, bounds.max.z);
        var p4 = new Vector3(bounds.min.x, bounds.min.y, bounds.max.z);

        Debug.DrawLine(p1, p2, color);
        Debug.DrawLine(p2, p3, color);
        Debug.DrawLine(p3, p4, color);
        Debug.DrawLine(p4, p1, color);

        // top
        var p5 = new Vector3(bounds.min.x, bounds.max.y, bounds.min.z);
        var p6 = new Vector3(bounds.max.x, bounds.max.y, bounds.min.z);
        var p7 = new Vector3(bounds.max.x, bounds.max.y, bounds.max.z);
        var p8 = new Vector3(bounds.min.x, bounds.max.y, bounds.max.z);

        Debug.DrawLine(p5, p6, color);
        Debug.DrawLine(p6, p7, color);
        Debug.DrawLine(p7, p8, color);
        Debug.DrawLine(p8, p5, color);

        // sides
        Debug.DrawLine(p1, p5, color);
        Debug.DrawLine(p2, p6, color);
        Debug.DrawLine(p3, p7, color);
        Debug.DrawLine(p4, p8, color);

    }

    protected Bounds ComputeBounds() {
        Bounds bounds = new Bounds(transform.position, Vector3.zero);
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>()) {
            bounds.Encapsulate(renderer.bounds);
        }
        return bounds;
    }

    public static List<Obstacle> GetObstacles() {
        return obstacles;
    }

    public Bounds GetBounds() {
        return bounds;
    }

    public Vector3 ComputeClosestPoint(Vector3 point) {
        return bounds.ClosestPoint(point);
    }

}
