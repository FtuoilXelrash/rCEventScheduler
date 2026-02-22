# Event Commands Reference

This file documents all events configured for **Rust Custom Event Scheduler**.

To add new events, edit `oxide/config/rCEventScheduler.json` and reload the plugin with `oxide.reload rCEventScheduler`.

---

## All Events (Alphabetical)

| # | Event Name | Required Plugin | Run Time | Start Command | Stop Command | Notes |
|---|---|---|---|---|---|---|
| 1 | Air Event | `AirEvent` | 60 min | `airstart` | *(none)* | Self-managed |
| 2 | Airfield Event | `AirfieldEvent` | 60 min | `afestart` | *(none)* | Self-managed |
| 3 | Arctic Base Event | `ArcticBaseEvent` | 60 min | `abstart` | *(none)* | Self-managed |
| 4 | Armored Train | `ArmoredTrain` | 60 min | `atrainstart` | *(none)* | Self-managed |
| 5 | Boss Monster Clown | `BossMonster` | 60 min | `SpawnBoss Clown` | `KillBoss Clown` | Requires stop command |
| 6 | Boss Monster Evil | `BossMonster` | 60 min | `SpawnBoss Evil` | `KillBoss Evil` | Requires stop command |
| 7 | Boss Monster Franken | `BossMonster` | 60 min | `SpawnBoss Franken` | `KillBoss Franken` | Requires stop command |
| 8 | Boss Monster Frankenstein | `BossMonster` | 60 min | `SpawnBoss Frankenstein` | `KillBoss Frankenstein` | Requires stop command |
| 9 | Boss Monster Heavy | `BossMonster` | 60 min | `SpawnBoss Heavy` | `KillBoss Heavy` | Requires stop command |
| 10 | Boss Monster Horror | `BossMonster` | 60 min | `SpawnBoss Horror` | `KillBoss Horror` | Requires stop command |
| 11 | Boss Monster Jason | `BossMonster` | 60 min | `SpawnBoss Jason` | `KillBoss Jason` | Requires stop command |
| 12 | Boss Monster King of the Night | `BossMonster` | 60 min | `SpawnBoss King of the Night` | `KillBoss King of the Night` | Requires stop command |
| 13 | Boss Monster Michael Myers | `BossMonster` | 60 min | `SpawnBoss Michael Myers` | `KillBoss Michael Myers` | Requires stop command |
| 14 | Boss Monster Oni | `BossMonster` | 60 min | `SpawnBoss Oni` | `KillBoss Oni` | Requires stop command |
| 15 | Boss Monster Raptor | `BossMonster` | 60 min | `SpawnBoss Raptor` | `KillBoss Raptor` | Requires stop command |
| 16 | Boss Monster Scary | `BossMonster` | 60 min | `SpawnBoss Scary` | `KillBoss Scary` | Requires stop command |
| 17 | Celestial Barrage | `CelestialBarrage` | 5 min | `cb.random` | *(none)* | Self-managed |
| 18 | Convoy | `Convoy` | 60 min | `convoystart` | *(none)* | Self-managed |
| 19 | Gas Station Event | `GasStationEvent` | 60 min | `gsstart` | *(none)* | Self-managed |
| 20 | Gun Game | `GunGame` | 45 min | `ggstart` | *(none)* | Self-managed |
| 21 | Harbor Event | `HarborEvent` | 60 min | `harborstart` | *(none)* | Self-managed |
| 22 | Supermarket Event | `SupermarketEvent` | 60 min | `supermarketstart` | *(none)* | Self-managed |
| 23 | Twister | `Tornado` | 5 min | `tornado start random` | *(none)* | Self-managed |
| 24 | Water Event | `WaterEvent` | 120 min | `waterstart` | *(none)* | Self-managed — longer run time |

---

## Notes

- **Required Plugin** is the Oxide plugin name (no `.cs` extension). If the plugin is not loaded when the scheduler starts, the event is omitted from the queue entirely and listed in a single warning message.
- Leave `"Required Plugin": ""` if no plugin validation is needed for that event.
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
  "Required Plugin": "YourPluginName",
  "Event Run Time (minutes)": 60,
  "Event Start Command": "yourcommandhere",
  "Event Stop Command": ""
}
```

Then reload: `oxide.reload rCEventScheduler`

Leave `"Required Plugin": ""` to skip validation and always include the event.

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
