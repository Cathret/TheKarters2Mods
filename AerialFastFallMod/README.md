# Aerial Fast Fall Mod v1.0.0

## Description
Mod that permits to activate a **Fast Fall behaviour** when in the air.

Permits to use inputs to be able to **add velocity to the Kart** for being able to fast Fall.

This permits to be able to have **more control on the height of the jumps**, and **avoid being blocked in high locations**.

## Dependency
- **Disable Leaderboards** v1.0.0
- **Auto Reload Config Mod SDK** v1.0.0

## Two modes

### One Button Pressed
By clicking on **Triangle (Y)**, the kart will have some **velocity downwards added until it hits the ground**.

### Keep Input
By either **keep pressing on Triangle** or **pushing the analog down**, the kart will have some **velocity downwards based on the power of the input**.

## Parameters

### Global
#### Use Single Press
Should use the **Single Press input system**.

#### Fast Fall Rate
**Fast Fall Rate**. The higher, the faster the kart will be directed to the ground.

#### Minimum Jump Time Before Fast Fall
In **Seconds**. Minimum time after being airborne before Fast Fall is allowed.

### Directional Input
#### Minimum Joystick Input Before Fast Fall
In **Seconds**. **Fast Fall Dead-zone**. While the joystick input is lower than this value, the Fast Fall will not be triggered.

### Dodge
#### Should Dodge On Press
If this is set to true, pressing the Fast Fall button will actually make it possible to **dodge damages**.

#### Dodge Duration
**In Seconds**. If it should dodge when Fast Falling, duration of invincibility in seconds after pressing the fast fall button.
