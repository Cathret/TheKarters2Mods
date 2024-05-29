# Twitch Cathret Commands SDK v1.0.0

## Description
**Basic Twitch commands** using the Twitch Basic Commands SDK.

Makes possible to **kill player** in specific position, all humans, all AIs. It's also possible to **impact the Reserve and the Hp** of the streamer.

## Dependency
- **Disable Leaderboards** v1.0.0
- **Auto Reload Config Mod SDK** v1.0.0
- **Twitch Basic Commands SDK** v1.0.0
- **The Karters Modding Assistant** v0.2.0 by [@Arca](https://github.com/iArcadia), available [here](https://github.com/The-Karters-Community/The-Karters-Modding-Assistant-SDK)

## Available Commands

### Kill

- #### At position
It's possible to **force kill a Karter at a specific position**.\
Usage: `kill pos X`, with X the position of the Karter.

- #### Karter with Name
It's possible to **force kill a Karter with a specific name**.\
Usage: `kill karter X`, with X the name of the Karter.

- #### All Humans
It's possible to **force kill all humans** karters at once.\
Usage: `kill humans`.

- #### All Ais
It's possible to **force kill all AIs** karters at once.\
Usage: `kill ais`.

### Reserve
All values are **clamped to 999** by default.

- #### Set
It's possible to **set the reserve value** of the Streamer.\
Usage: `reserve set X`, with X the value of the Reserve in %.

- #### Gain
It's possible to **increase the reserve value** of the Streamer.\
Usage: `reserve set [X]`, with X the value of the Reserve in %.\
If not specified, X = 20.

- #### Lose
It's possible to **decrease the reserve value** of the Streamer.\
Usage: `reserve set [X]`, with X the value of the Reserve in %.\
If not specified, X = 20.

### Health
All values are clamped to 999 by default.

- #### Gain
It's possible to **increase the health** of the Streamer.\
Usage: `reserve set [X]`, with X the value of the health.\
If not specified, X = 20.

- #### Lose
It's possible to **decrease the health** of the Streamer.\
Usage: `reserve set [X]`, with X the value of the health.\
If not specified, X = 20.

## Parameters
Parameters have the name and description of all the Available Commands.
