# Event Commands Reference

This file documents the events configured for **Rust Custom Event Scheduler**.

To add new events, edit `oxide/config/rCEventScheduler.json` and reload the plugin with `oxide.reload rCEventScheduler`.

---

## Default Events

| # | Event Name | Run Time | Start Command | Stop Command | Notes |
|---|---|---|---|---|---|
| 1 | Convoy | 60 min | `convoystart` | *(none)* | Self-managed — ends on its own |
| 2 | Armored Train | 60 min | `atrainstart` | *(none)* | Self-managed — ends on its own |
| 3 | Boss Monster Oni | 60 min | `SpawnBoss Oni` | `KillBoss Oni` | Requires an explicit stop command |

---

## Notes

- **Start/Stop Commands** are standard Rust server console commands run directly by the scheduler.
- If **Stop Command** is left empty (`""`), the event is considered **self-managed** — the scheduler tracks its run time but sends no stop command. The event ends on its own (e.g., a convoy that despawns naturally).
- Events with a defined **Stop Command** will have it executed automatically after the **Event Run Time** expires (e.g., boss monsters that must be explicitly killed).
- Only events with `"Event Enabled": true` are loaded into the scheduler at startup.
- The scheduler randomizes the order of enabled events each cycle.

---

## Adding New Events

Add a new block inside the `"Events": [ ]` array in `oxide/config/rCEventScheduler.json`:

```json
{
  "Event Name": "Your Event Name",
  "Event Enabled": true,
  "Event Run Time (minutes)": 60,
  "Event Start Command": "yourcommandhere",
  "Event Stop Command": ""
}
```

Then reload: `oxide.reload rCEventScheduler`

---

## Special Event Types

### Self-Managed Events (no Stop Command)
Events that clean themselves up — convoys, trains, airdrops, cargo ships, etc.

```json
"Event Stop Command": ""
```

### Managed Events (with Stop Command)
Events that require an explicit stop — boss monsters, custom spawners, etc.

```json
"Event Start Command": "SpawnBoss Oni",
"Event Stop Command": "KillBoss Oni"
```
