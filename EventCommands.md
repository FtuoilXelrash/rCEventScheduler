# Event Commands Reference

This file documents all events configured for **Rust Custom Event Scheduler**.

To add new events, edit `oxide/config/rCEventScheduler.json` and reload the plugin with `oxide.reload rCEventScheduler`.

---

## All Events (Alphabetical)

| # | Event Name | Run Time | Start Command | Stop Command | Notes |
|---|---|---|---|---|---|
| 1 | AIR EVENT | 60 min | `airstart` | *(none)* | Self-managed |
| 2 | Airfield Event | 60 min | `afestart` | *(none)* | Self-managed |
| 3 | Arctic Base Event | 60 min | `abstart` | *(none)* | Self-managed |
| 4 | Armored Train | 60 min | `atrainstart` | *(none)* | Self-managed |
| 5 | Boss Monster Clown | 60 min | `SpawnBoss Clown` | `KillBoss Clown` | Requires stop command |
| 6 | Boss Monster Evil | 60 min | `SpawnBoss Evil` | `KillBoss Evil` | Requires stop command |
| 7 | Boss Monster Franken | 60 min | `SpawnBoss Franken` | `KillBoss Franken` | Requires stop command |
| 8 | Boss Monster Frankenstein | 60 min | `SpawnBoss Frankenstein` | `KillBoss Frankenstein` | Requires stop command |
| 9 | Boss Monster Heavy | 60 min | `SpawnBoss Heavy` | `KillBoss Heavy` | Requires stop command |
| 10 | Boss Monster Horror | 60 min | `SpawnBoss Horror` | `KillBoss Horror` | Requires stop command |
| 11 | Boss Monster Jason | 60 min | `SpawnBoss Jason` | `KillBoss Jason` | Requires stop command |
| 12 | Boss Monster King of the Night | 60 min | `SpawnBoss King of the Night` | `KillBoss King of the Night` | Requires stop command |
| 13 | Boss Monster Michael Myers | 60 min | `SpawnBoss Michael Myers` | `KillBoss Michael Myers` | Requires stop command |
| 14 | Boss Monster Oni | 60 min | `SpawnBoss Oni` | `KillBoss Oni` | Requires stop command |
| 15 | Boss Monster Raptor | 60 min | `SpawnBoss Raptor` | `KillBoss Raptor` | Requires stop command |
| 16 | Boss Monster Scary | 60 min | `SpawnBoss Scary` | `KillBoss Scary` | Requires stop command |
| 17 | Convoy | 60 min | `convoystart` | *(none)* | Self-managed |
| 18 | Gas Station Event | 60 min | `gsstart` | *(none)* | Self-managed |
| 19 | Gun Game | 45 min | `ggstart` | *(none)* | Self-managed |
| 20 | Harbor Event | 60 min | `harborstart` | *(none)* | Self-managed |
| 21 | Supermarket Event | 60 min | `supermarketstart` | *(none)* | Self-managed |
| 22 | Water Event | 120 min | `waterstart` | *(none)* | Self-managed — longer run time |

---

## Notes

- **Start/Stop Commands** are standard Rust server console commands run directly by the scheduler.
- If **Stop Command** is *(none)* / `""`, the event is **self-managed** — it ends on its own after its run time.
- Events with a **Stop Command** will have it fired automatically when the Event Run Time expires.
- Only events with `"Event Enabled": true` are loaded into the randomized queue at startup.
- The scheduler randomizes the order every cycle — no two cycles will run in the same order.

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
Convoys, trains, airdrops, base events, etc. — clean themselves up naturally.

```json
"Event Stop Command": ""
```

### Managed Events (with Stop Command)
Boss monsters and custom spawners that require an explicit kill/stop.

```json
"Event Start Command": "SpawnBoss Oni",
"Event Stop Command": "KillBoss Oni"
```
