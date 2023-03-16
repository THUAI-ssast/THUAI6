using System;
using System.Collections.Generic;

// mainly written by ChatGPT

public class FutureAction {
    public Action action;
    public float remainingTime;

    public FutureAction(Action action, float remainingTime) {
        this.action = action;
        this.remainingTime = remainingTime;
    }
}

public class FutureActionManager {
    private Queue<FutureAction> futureActions = new Queue<FutureAction>();

    public void AddFutureAction(Action action, float delay) {
        futureActions.Enqueue(new FutureAction(action, delay));
    }

    public void Update(float deltaTime) {
        int count = futureActions.Count;
        for (int i = 0; i < count; i++) {
            FutureAction futureAction = futureActions.Dequeue();
            futureAction.remainingTime -= deltaTime;
            if (futureAction.remainingTime <= 0) {
                futureAction.action();
            } else {
                futureActions.Enqueue(futureAction);
            }
        }
    }
}
