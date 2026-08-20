================================================================================
  Rust Custom Event Scheduler
  Version: 1.0.11  |  Author: Ftuoil Xelrash  |  License: GPL v3
  Platform: uMod / Oxide for Rust
================================================================================

Automated, randomized, and fully customizable premium event scheduling for
your Rust server — with Discord integration and live player visibility.

--------------------------------------------------------------------------------
FEATURES
--------------------------------------------------------------------------------

  - Randomized Event Queue
    Events fire in a different order every cycle to keep gameplay fresh.

  - Smart Buffer Timing
    Configurable random delay (min/max minutes) between every event.

  - Auto-Cycling
    When all events have run, the list re-randomizes and starts a new cycle.

  - Max Active Events
    Control how many events can run simultaneously.

  - Discord Notifications
    Rich embed messages for every scheduler action (load, queue, start,
    end, delay, cycle reset).

  - Sticky Live-Status Message
    A single self-updating Discord message (its own webhook) showing
    active events, the next event, and the full event roster. Enabled
    by default.

  - Console Logging
    Full console output for every scheduler action with local server time.

  - Player Command
    Players type !events or /events — both are silent (not shown in chat),
    and the result broadcasts to all players so everyone benefits from the
    global cooldown.

  - Dynamic Config
    Add your own events by editing the config file — no code required.

  - Local Server Time
    All times display in your server's local timezone (e.g. CST, PST, EDT).

  - Self-Managed Events
    Events without a stop command (convoys, trains) tracked by run time only.

  - Managed Events
    Boss monsters and custom spawners get an automatic stop command after
    their run time expires.

--------------------------------------------------------------------------------
INSTALLATION
--------------------------------------------------------------------------------

  1. Download rCEventScheduler.cs
  2. Place it in your oxide/plugins/ directory
  3. Restart the server or run: oxide.reload rCEventScheduler
  4. Edit the config at oxide/config/rCEventScheduler.json
  5. Set your Discord webhook URL for admin notifications (optional)
  6. Set a second Discord webhook URL for the sticky live-status message
     (optional — see STICKY LIVE-STATUS MESSAGE section below)

--------------------------------------------------------------------------------
CONFIGURATION  (oxide/config/rCEventScheduler.json)
--------------------------------------------------------------------------------

  {
    "Log Events to Console": true,
    "Log Events to Discord": true,
    "Admin Discord Webhook URL": "",
    "Max Active Events": 1,
    "Event Buffer Time Enabled": true,
    "Event Min Buffer Time (minutes)": 5,
    "Event Max Buffer Time (minutes)": 15,
    "Enable Player Events Command": true,
    "Show Next Event Scheduled on Event End": true,
    "Enable Status Sticky Message": true,
    "Status Sticky Discord Webhook URL": "",
    "Status Sticky Discord Bot Name": "Event Scheduler",
    "Events": [
      {
        "Event Name": "Air Event",
        "Event Enabled": true,
        "Required Plugin": "AirEvent",
        "Event Run Time (minutes)": 60,
        "Event Start Command": "airstart",
        "Event Stop Command": ""
      },
      {
        "Event Name": "Airfield Event",
        "Event Enabled": true,
        "Required Plugin": "AirfieldEvent",
        "Event Run Time (minutes)": 60,
        "Event Start Command": "afestart",
        "Event Stop Command": ""
      },
      {
        "Event Name": "Arctic Base Event",
        "Event Enabled": true,
        "Required Plugin": "ArcticBaseEvent",
        "Event Run Time (minutes)": 45,
        "Event Start Command": "abstart",
        "Event Stop Command": ""
      },
      {
        "Event Name": "Armored Train",
        "Event Enabled": true,
        "Required Plugin": "ArmoredTrain",
        "Event Run Time (minutes)": 60,
        "Event Start Command": "atrainstart",
        "Event Stop Command": ""
      },
      {
        "Event Name": "Boss Monster Clown",
        "Event Enabled": true,
        "Required Plugin": "BossMonster",
        "Event Run Time (minutes)": 60,
        "Event Start Command": "SpawnBoss Clown",
        "Event Stop Command": "KillBoss Clown"
      },
      {
        "Event Name": "Boss Monster Evil",
        "Event Enabled": true,
        "Required Plugin": "BossMonster",
        "Event Run Time (minutes)": 60,
        "Event Start Command": "SpawnBoss Evil",
        "Event Stop Command": "KillBoss Evil"
      },
      {
        "Event Name": "Boss Monster Franken",
        "Event Enabled": true,
        "Required Plugin": "BossMonster",
        "Event Run Time (minutes)": 60,
        "Event Start Command": "SpawnBoss Franken",
        "Event Stop Command": "KillBoss Franken"
      },
      {
        "Event Name": "Boss Monster Frankenstein",
        "Event Enabled": true,
        "Required Plugin": "BossMonster",
        "Event Run Time (minutes)": 60,
        "Event Start Command": "SpawnBoss Frankenstein",
        "Event Stop Command": "KillBoss Frankenstein"
      },
      {
        "Event Name": "Boss Monster Heavy",
        "Event Enabled": true,
        "Required Plugin": "BossMonster",
        "Event Run Time (minutes)": 60,
        "Event Start Command": "SpawnBoss Heavy",
        "Event Stop Command": "KillBoss Heavy"
      },
      {
        "Event Name": "Boss Monster Horror",
        "Event Enabled": true,
        "Required Plugin": "BossMonster",
        "Event Run Time (minutes)": 60,
        "Event Start Command": "SpawnBoss Horror",
        "Event Stop Command": "KillBoss Horror"
      },
      {
        "Event Name": "Boss Monster Jason",
        "Event Enabled": true,
        "Required Plugin": "BossMonster",
        "Event Run Time (minutes)": 60,
        "Event Start Command": "SpawnBoss Jason",
        "Event Stop Command": "KillBoss Jason"
      },
      {
        "Event Name": "Boss Monster King of the Night",
        "Event Enabled": true,
        "Required Plugin": "BossMonster",
        "Event Run Time (minutes)": 60,
        "Event Start Command": "SpawnBoss King of the Night",
        "Event Stop Command": "KillBoss King of the Night"
      },
      {
        "Event Name": "Boss Monster Michael Myers",
        "Event Enabled": true,
        "Required Plugin": "BossMonster",
        "Event Run Time (minutes)": 60,
        "Event Start Command": "SpawnBoss Michael Myers",
        "Event Stop Command": "KillBoss Michael Myers"
      },
      {
        "Event Name": "Boss Monster Oni",
        "Event Enabled": true,
        "Required Plugin": "BossMonster",
        "Event Run Time (minutes)": 60,
        "Event Start Command": "SpawnBoss Oni",
        "Event Stop Command": "KillBoss Oni"
      },
      {
        "Event Name": "Boss Monster Raptor",
        "Event Enabled": true,
        "Required Plugin": "BossMonster",
        "Event Run Time (minutes)": 60,
        "Event Start Command": "SpawnBoss Raptor",
        "Event Stop Command": "KillBoss Raptor"
      },
      {
        "Event Name": "Boss Monster Scary",
        "Event Enabled": true,
        "Required Plugin": "BossMonster",
        "Event Run Time (minutes)": 60,
        "Event Start Command": "SpawnBoss Scary",
        "Event Stop Command": "KillBoss Scary"
      },
      {
        "Event Name": "Celestial Barrage",
        "Event Enabled": true,
        "Required Plugin": "CelestialBarrage",
        "Event Run Time (minutes)": 5,
        "Event Start Command": "cb.random",
        "Event Stop Command": ""
      },
      {
        "Event Name": "Convoy",
        "Event Enabled": true,
        "Required Plugin": "Convoy",
        "Event Run Time (minutes)": 60,
        "Event Start Command": "convoystart",
        "Event Stop Command": ""
      },
      {
        "Event Name": "Gas Station Event",
        "Event Enabled": true,
        "Required Plugin": "GasStationEvent",
        "Event Run Time (minutes)": 45,
        "Event Start Command": "gsstart",
        "Event Stop Command": ""
      },
      {
        "Event Name": "Gun Game",
        "Event Enabled": true,
        "Required Plugin": "GunGame",
        "Event Run Time (minutes)": 45,
        "Event Start Command": "ggstart",
        "Event Stop Command": ""
      },
      {
        "Event Name": "Harbor Event",
        "Event Enabled": true,
        "Required Plugin": "HarborEvent",
        "Event Run Time (minutes)": 60,
        "Event Start Command": "harborstart",
        "Event Stop Command": ""
      },
      {
        "Event Name": "Sputnik",
        "Event Enabled": false,
        "Required Plugin": "Sputnik",
        "Event Run Time (minutes)": 60,
        "Event Start Command": "sputnikstart",
        "Event Stop Command": ""
      },
      {
        "Event Name": "Supermarket Event",
        "Event Enabled": true,
        "Required Plugin": "SupermarketEvent",
        "Event Run Time (minutes)": 45,
        "Event Start Command": "supermarketstart",
        "Event Stop Command": ""
      },
      {
        "Event Name": "Twister",
        "Event Enabled": true,
        "Required Plugin": "Tornado",
        "Event Run Time (minutes)": 5,
        "Event Start Command": "tornado start random",
        "Event Stop Command": ""
      },
      {
        "Event Name": "Water Event",
        "Event Enabled": true,
        "Required Plugin": "WaterEvent",
        "Event Run Time (minutes)": 120,
        "Event Start Command": "waterstart",
        "Event Stop Command": ""
      }
    ]
  }

  GLOBAL SETTINGS
  ---------------
  Log Events to Console          Print all scheduler events to server console
  Log Events to Discord          Send Discord embed messages for all events
  Admin Discord Webhook URL      Private Discord webhook URL (leave "" to skip)
  Max Active Events              Max simultaneous events (default: 1)
  Event Buffer Time Enabled      Enable random delay between events
  Event Min Buffer Time (min)    Minimum random delay before next event
  Event Max Buffer Time (min)    Maximum random delay before next event
  Enable Player Events Command   Allow players to use !events in chat
  Show Next Event Scheduled      After an event ends, re-send the Next Event
  on Event End                   Scheduled Discord embed as a reminder (true)
  Enable Status Sticky Message   Enable the self-updating sticky live-status
                                 Discord message (default: true)
  Status Sticky Discord          Webhook URL for the sticky live-status
  Webhook URL                    message — separate from the admin webhook
  Status Sticky Discord          Discord display name sent with the sticky
  Bot Name                       message (default: "Event Scheduler") —
                                 overrides the raw webhook's configured name

  PER-EVENT OPTIONS
  -----------------
  Event Name                     Display name of the event
  Event Enabled                  false = skip this event without removing it
  Required Plugin                Oxide plugin name that must be loaded for this
                                 event to run. Leave "" to always include it.
                                 If the plugin is missing at startup, the event
                                 is skipped and listed in a single warning.
  Event Run Time (minutes)       How long the event runs
  Event Start Command            Server console command to start the event
  Event Stop Command             Server console command to stop the event
                                 Leave "" for self-managed events

--------------------------------------------------------------------------------
PLAYER COMMAND
--------------------------------------------------------------------------------

  Commands: !events  (type in chat — message is suppressed, not visible)
            /events  (slash command — suppressed by Oxide, not visible)
  Access  : All players
  Result  : Broadcast to ALL players on the server (not just the caller)
  Cooldown: 5 minutes (global, server-wide)
            Only the player who hits the cooldown sees the cooldown message
            (private). Other players are not notified.

  Example output (visible to all players):
    [ My Rust Server Event Scheduler ]
    Active Events:
      - Boss Monster Oni
    Next Event: Convoy
    Starts at: 4:23 PM CST  (~38 min)

  Cooldown message (visible only to the player who triggered it):
    [ My Rust Server Event Scheduler ]
    Command on cooldown. Try again in 47s.

  Note: The header uses your server name automatically (ConVar.Server.hostname)

  Disable with: "Enable Player Events Command": false

--------------------------------------------------------------------------------
DISCORD NOTIFICATIONS
--------------------------------------------------------------------------------

  When a webhook URL is configured, the plugin sends embed messages for:
  All embed titles use your server's name automatically (e.g. "My Server Event Scheduler")


  Plugin Loaded       Lists all loaded events
  Queue Randomized    Shows the new randomized event order
  Next Event          Event name, scheduled time, countdown, queue position,
                      and events remaining until reshuffle
  Event Delayed       Max active events reached — shows retry time
  Event Started       Event name, run time, expected end time
  Event Ended         Event name and how it ended
  Cycle Complete      All events ran — new cycle starting

--------------------------------------------------------------------------------
STICKY LIVE-STATUS MESSAGE
--------------------------------------------------------------------------------

  A single Discord message that updates itself in place — like a live status
  board — instead of posting a new message every time. Uses its OWN webhook
  URL, separate from the admin notifications webhook above, so you can post
  it to a public channel while keeping admin logs private.

  Shown in the message:
    Active Event(s)   Name, expected end time, and minutes remaining
                       (or "No events currently active")
    Next Event         Name, scheduled time, countdown, queue position,
                       and events until reshuffle
    Upcoming Queue     Every event remaining in the current cycle's
                       randomized queue, in firing order, each with a
                       projected local kickoff time. First entry is exact;
                       later entries are marked (est.) since real per-event
                       delay is only randomized once that event actually
                       becomes next. Events already fired this cycle drop
                       off the list until the next reshuffle.

  All times use your server's local time, same as every other message this
  plugin sends — no special Discord timestamp formatting.

  The message's Discord display name is set by "Status Sticky Discord Bot
  Name" (default "Event Scheduler") — this overrides whatever your raw
  webhook happens to be named in Discord's own settings.

  The message is edited only when something actually changes (an event
  starts, ends, or the next event gets scheduled/delayed) — there is no
  background refresh timer. If the message is deleted from Discord, the
  plugin automatically posts a new one on the next update.

  Setup:
    1. Create a second Discord webhook in the channel you want the live
       status to appear
    2. Set "Status Sticky Discord Webhook URL" in the config to that
       webhook URL
    3. Leave "Enable Status Sticky Message" as true (default), or set it
       false to disable

  Console commands:
    rces.status         Print current sticky status state to console
                         (enabled, webhook configured, message ID,
                         active/next event)
    rces.forcestatus     Force an immediate sticky message update
    rces.resetstatus     Clear the stored Discord message ID — a new
                         message will be created on the next update

--------------------------------------------------------------------------------
HOW IT WORKS
--------------------------------------------------------------------------------

  1. Plugin loads
     - Counts and logs all enabled events
     - Randomizes the event queue and logs the order
     - Calculates first buffer time (random between min/max)
     - Waits buffer time, then fires first event

  2. Buffer fires
     - Active events < Max Active Events?
         YES: Fire next event, run start command,
              schedule stop after run time (if defined),
              schedule next buffer, repeat.
         NO:  Wait RunTime + new buffer calculation, then retry.

  3. All events have run
     - Log "Cycle Complete" to console + Discord
     - Re-randomize the full event list
     - Log new randomized order
     - Resume from step 1

--------------------------------------------------------------------------------
ADDING CUSTOM EVENTS
--------------------------------------------------------------------------------

  Add a new block to the "Events" array in the config file:

  {
    "Event Name": "My Custom Event",
    "Event Enabled": true,
    "Required Plugin": "MyPluginName",
    "Event Run Time (minutes)": 45,
    "Event Start Command": "myeventstart",
    "Event Stop Command": ""
  }

  Then reload: oxide.reload rCEventScheduler

--------------------------------------------------------------------------------
LICENSE
--------------------------------------------------------------------------------

  GNU General Public License v3.0  —  See LICENSE file for full terms.

--------------------------------------------------------------------------------
AUTHOR
--------------------------------------------------------------------------------

  Ftuoil Xelrash

================================================================================
