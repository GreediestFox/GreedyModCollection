# 26 VIP slots (inactive players make room for approved IPs)

`server/mods/LiFx/VipSlotsPack`: when the server is full and a connection comes from an IP listed in `config.cs` (`$LiFxVip::ips`), the player who has been inactive the longest (no movement and no chat for `$LiFxVip::idleMinutes`, default 20) is removed so the VIP can join. GM accounts (flagged IsGM or in `$LiFxGM::accounts`) are never removed.

- The engine decides "server full" in `GameConnection::onConnectRequest` (script, `CR_SERVERFULL`), before the account of the connecting client is known. A VIP can therefore only be recognised by the IP of the connection (`netAddress` is `IP:a.b.c.d:port`). The framework hook `onConnectRequestCallbacks` ignores return values, so the mod works by freeing a slot first; the engine check then passes.
- Activity is sampled every 20 s (position changes) plus every local chat message (package override of `serverCmdLocalChatMessage`).
- Log lines start with `[VIP]`. Written 2026-10-24 and not yet tested with a full server: test with `maxPlayers` 1-2 and `idleMinutes` 1.
- Keep `config.cs` per server (the GMC pack installs it only when missing). Home IPs change: update the line.