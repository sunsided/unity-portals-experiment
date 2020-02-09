# Playing with Portals

An attempt at re-implementing Sebastian Lague's
[Portals](https://www.youtube.com/watch?v=cWpFZbjtSQg) conding adventure.

![](.readme/editor.jpg)

## Implemented features

- Rendering of portals (only) when in view, and
- Teleportation between portals.

## Missing features

- Recursive portal rendering,
- Momentum, a function of mass and velocity, is conserved between portals;
  in layman's terms: Speedy thing goes in, speedy thing comes out.

## Current issues

- So far, rendering the portals works eratically, with the player
  being either not relocated at all or being moved too far.
  In essence, while "ownership" of the player is transferred to the
  linked portal, the location isn't updated. This might be an
  issue with the FPS controller used, and further investigation is needed.
- Portals are being rendered too late which results in a lagged update.
  This can be observed by moving the view quickly when close to a portal;
  the portal's "screen" then seems to wander around.

## Open ideas

- Limiting the portal's own cameras to only rendering the scene
  visible _through_ the portal. Right now, the whole view is rendered
  and then projected onto the portal; instead, fiddling with the
  camera's frustum or using clipping by fiddling with the Z buffer
  may improve performance.
