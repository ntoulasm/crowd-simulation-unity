using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDynamic : Obstacle {

    void Start() {
        Init();
    }

    void Update() {
        bounds = ComputeBounds();
        DrawBounds();
    }

    protected override void Init() {
        base.Init();
    }

}
