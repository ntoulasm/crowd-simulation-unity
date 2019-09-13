using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleStatic : Obstacle {

    void Start() {
        Init();
    }

    void Update() {
        DrawBounds();
    }

    protected override void Init() {
        base.Init();
        bounds = ComputeBounds();
    }

}
