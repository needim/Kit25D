# What's the Kit25D?

Basically, It allows you to create a fake 3D world with 2D colliders and sprites. I have to say, it has a lot of tricks to do that. If you wanna use this kit, sometimes you have to twist your brain a little bit. The kit heavily depends on sprite masks (for shadow) & sprite pivots (for sorting). And while you are preparing your prefabs, you can have some hard time.

# Features

- Player Movement
  - X, Y and FakeZ (for jump)
  - Raycast based collision detection
  - Slopes Handling
- Environment
  - Static obstacles (like trees, walls with no roof)
  - Dynamic obstacles (like boxes, short walls, player can jump on)
  - Floating Platforms (player can jump on, player can't jump from under)

## Recommended Settings

- Project Settings -> Time -> **Fixed Timestamp** = **0.01**
- Add Layers ( **Obstacle** )
- Add Tags ( **LevelBounds** , **JumpBlocker** )
