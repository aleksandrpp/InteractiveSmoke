# Competitive Controller
A good starting point for creating your own competitive character controller in Unity is needed primarily for online games, where you cannot rely on the rigidbody, and the PhysX character controller hides too much logic.

You can see example [heare](https://github.com/aleksandrpp/CompetitiveMan)

```mermaid
---
title: Move loop
---
flowchart LR
    A[Trace ground] --> B{Grounded}
    B -- Yes --> C[Set horizontal velocity]
    B -- No --> F[Reduce velocity]
    C --> D{Jump pressed}
    D -- Yes --> E[Add vertical velocity]
    D -- No --> G
    E --> G
    F --> G[Resolve collisions]
    G --> H[Add Gravity]
```

`v1.0.0`
<br>

https://github.com/aleksandrpp/CompetitiveController