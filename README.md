![Version](https://img.shields.io/badge/Version-0.0.1-brightgreen) ![Game](https://img.shields.io/badge/Game-Rust-orange) ![Framework](https://img.shields.io/badge/uMod%2FOxide-Oxide-blue) ![License](https://img.shields.io/badge/License-GPL%20v3-lightgrey)

# Rust Custom Event Scheduler

**Automated, randomized, and fully customizable premium event scheduling for your Rust server — with Discord integration and live player visibility.**

---

## ✨ Features

- **🎲 Randomized Event Queue** — Events fire in a different order every cycle to keep gameplay fresh and unpredictable
- **⏱️ Smart Buffer Timing** — Configurable random delay (min/max minutes) between every event keeps spawn timing varied
- **🔁 Auto-Cycling** — When all events have run, the list re-randomizes automatically and starts a new cycle
- **⚡ Max Active Events** — Control how many events can run simultaneously; prevents event stacking
- **📢 Discord Notifications** — Rich embed messages for every scheduler action (load, queue, start, end, delay, cycle reset)
- **🖥️ Console Logging** — Full console output for every scheduler action with local server time
- **💬 Player Command** — Players type `!events` in chat to see active events and the next scheduled event with countdown
- **🔧 Dynamic Config** — Add your own events by editing the config file — no code, no reload required on first load
- **🕐 Local Server Time** — All times display in your server's local timezone, auto-detected and abbreviated (e.g. CST, PST, EDT)
- **🛑 Self-Managed Events** — Events without a stop command (convoys, trains, etc.) are tracked by run time with no stop needed
- **👹 Managed Events** — Events like boss monsters get an automatic stop command fired after their run time expires

---

## 📦 Installation

1. Download `rCEventScheduler.cs`
2. Place it in your `oxide/plugins/` directory
3. Restart the server or run: `oxide.reload rCEventScheduler`
4. Edit the config at `oxide/config/rCEventScheduler.json`
5. Set your Discord webhook URL for admin notifications (optional)

---

## ⚙️ Configuration

**File:** `oxide/config/rCEventScheduler.json`

```json
{
  "Log Events to Console": true,
  "Log Events to Discord": true,
  "Admin Discord Webhook URL": "",
  "Max Active Events": 1,
  "Event Buffer Time Enabled": true,
  "Event Min Buffer Time (minutes)": 30,
  "Event Max Buffer Time (minutes)": 60,
  "Enable Player Events Command": true,
  "Events": [
    {
      "Event Name": "Convoy",
      "Event Enabled": true,
      "Event Run Time (minutes)": 60,
      "Event Start Command": "convoystart",
      "Event Stop Command": ""
    },
    {
      "Event Name": "Armored Train",
      "Event Enabled": true,
      "Event Run Time (minutes)": 60,
      "Event Start Command": "atrainstart",
      "Event Stop Command": ""
    },
    {
      "Event Name": "Boss Monster Oni",
      "Event Enabled": true,
      "Event Run Time (minutes)": 60,
      "Event Start Command": "SpawnBoss Oni",
      "Event Stop Command": "KillBoss Oni"
    }
  ]
}
```

### Global Settings

| Option | Default | Description |
|---|---|---|
| `Log Events to Console` | `true` | Print all scheduler events to the server console |
| `Log Events to Discord` | `true` | Send Discord embed messages for all scheduler events |
| `Admin Discord Webhook URL` | `""` | Private Discord webhook URL for admin-only notifications |
| `Max Active Events` | `1` | Maximum number of events that can run at the same time |
| `Event Buffer Time Enabled` | `true` | Enable random delay between events |
| `Event Min Buffer Time (minutes)` | `30` | Minimum random delay (minutes) before next event fires |
| `Event Max Buffer Time (minutes)` | `60` | Maximum random delay (minutes) before next event fires |
| `Enable Player Events Command` | `true` | Allow all players to use `!events` in chat |

### Per-Event Options

| Option | Type | Description |
|---|---|---|
| `Event Name` | string | Display name of the event |
| `Event Enabled` | bool | Set to `false` to skip this event without removing it from the config |
| `Event Run Time (minutes)` | int | How long the event runs before being stopped or expired |
| `Event Start Command` | string | Server console command to start the event |
| `Event Stop Command` | string | Server console command to stop the event — leave `""` if self-managed |

> **Note:** Events with an empty `Event Stop Command` are self-managed. The scheduler tracks their run time but does not issue a stop command — the event ends on its own (e.g. a convoy that despawns naturally).

---

## 💬 Player Command

| Command | Access | Description |
|---|---|---|
| `!events` | All Players | Shows active events and the next scheduled event with local time and countdown |

**Example output in chat:**
```
[ Rust Custom Event Scheduler ]
Active Events:
  ● Boss Monster Oni
Next Event: Convoy
Starts at: 4:23 PM CST  (~38 min)
```

> Disable this command in config with `"Enable Player Events Command": false`

---

## 📣 Discord Notifications

When a webhook URL is configured, the plugin sends Discord embed messages for every scheduler action:

| Event | Color | Description |
|---|---|---|
| Plugin Loaded | 🔵 Blue | Lists all loaded events with run time and stop info |
| Queue Randomized | 🟣 Purple | Shows the new randomized event order |
| Next Event Scheduled | 🩵 Teal | Event name, scheduled time, and countdown |
| Event Delayed | 🟠 Orange | Max active events reached — shows new retry time |
| Event Started | 🟢 Green | Event name, run time, expected end time |
| Event Ended | 🟠 Orange | Event name and how it ended (self-managed or stopped) |
| Cycle Complete | 🟣 Purple | All events ran — new randomized cycle starting |

---

## ⚙️ How It Works

```
PLUGIN LOADS
  └── Counts and logs all enabled events (console + Discord)
  └── Randomizes the event queue and logs the order
  └── Calculates first buffer time (random between min/max minutes)
  └── Waits buffer time, then fires first event

BUFFER FIRES
  ├── Active events < Max Active Events?
  │     YES → Fire next event → run start command
  │           → Schedule stop command after run time (if defined)
  │           → Schedule next buffer → repeat
  │     NO  → Wait: Event Run Time + new buffer calculation → retry

ALL EVENTS IN QUEUE HAVE RUN
  └── Log "Cycle Complete" to console + Discord
  └── Re-randomize the full event list
  └── Log new randomized order to console + Discord
  └── Resume scheduling from the top
```

---

## ➕ Adding Custom Events

Edit `oxide/config/rCEventScheduler.json` and add a new block to the `"Events"` array:

```json
{
  "Event Name": "My Custom Event",
  "Event Enabled": true,
  "Event Run Time (minutes)": 45,
  "Event Start Command": "myeventstart",
  "Event Stop Command": ""
}
```

Then reload: `oxide.reload rCEventScheduler`

The event will be picked up automatically, mixed into the randomized queue, and scheduled like any other event.

---

## 📄 License

This plugin is licensed under the [GNU General Public License v3.0](LICENSE).

---

## 👤 Author

**Ftuoil Xelrash**

---

⭐ **Star this repository if you find it useful!** ⭐
