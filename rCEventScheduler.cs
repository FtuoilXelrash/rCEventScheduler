using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Oxide.Core;
using Oxide.Core.Libraries;

namespace Oxide.Plugins
{
    [Info("Rust Custom Event Scheduler", "Ftuoil Xelrash", "1.0.11")]
    [Description("Schedules and manages custom Rust server events with randomized queues and Discord notifications.")]
    public class rCEventScheduler : RustPlugin
    {
        #region Fields

        private PluginConfig _config;
        private PluginData _data = new PluginData();
        private List<EventEntry> _eventQueue = new List<EventEntry>();
        private readonly List<string> _activeEvents = new List<string>();
        private readonly Dictionary<string, DateTime> _activeEventEndTimes = new Dictionary<string, DateTime>();
        private Timer _schedulerTimer;
        private DateTime _nextEventTime = DateTime.MinValue;
        private EventEntry _nextEvent;
        private readonly System.Random _rng = new System.Random();
        private DateTime _lastEventsCommand = DateTime.MinValue;
        private int _cycleTotal;

        private readonly Dictionary<string, string> _headers = new Dictionary<string, string>
        {
            { "Content-Type", "application/json" }
        };

        #endregion

        #region Configuration

        private class PluginConfig
        {
            [JsonProperty("Log Events to Console")]
            public bool LogToConsole = true;

            [JsonProperty("Log Events to Discord")]
            public bool LogToDiscord = true;

            [JsonProperty("Admin Discord Webhook URL")]
            public string WebhookUrl = "";

            [JsonProperty("Max Active Events")]
            public int MaxActiveEvents = 1;

            [JsonProperty("Event Buffer Time Enabled")]
            public bool BufferTimeEnabled = true;

            [JsonProperty("Event Min Buffer Time (minutes)")]
            public int MinBufferTime = 5;

            [JsonProperty("Event Max Buffer Time (minutes)")]
            public int MaxBufferTime = 15;

            [JsonProperty("Enable Player Events Command")]
            public bool EnablePlayerCommand = true;

            [JsonProperty("Show Next Event Scheduled on Event End")]
            public bool ShowNextEventOnEnd = true;

            [JsonProperty("Enable Status Sticky Message")]
            public bool EnableStickyStatus = true;

            [JsonProperty("Status Sticky Discord Webhook URL")]
            public string StickyWebhookUrl = "";

            [JsonProperty("Status Sticky Discord Bot Name")]
            public string StickyBotName = "Event Scheduler";

            [JsonProperty("Events")]
            public List<EventEntry> Events = new List<EventEntry>();
        }

        private class PluginData
        {
            [JsonProperty("Status Sticky Message ID")]
            public string StatusMessageId = null;
        }

        private class EventEntry
        {
            [JsonProperty("Event Name")]
            public string Name = "";

            [JsonProperty("Event Enabled")]
            public bool Enabled = true;

            [JsonProperty("Required Plugin")]
            public string RequiredPlugin = "";

            [JsonProperty("Event Run Time (minutes)")]
            public int RunTime = 60;

            [JsonProperty("Event Start Command")]
            public string StartCommand = "";

            [JsonProperty("Event Stop Command")]
            public string StopCommand = "";
        }

        protected override void LoadDefaultConfig()
        {
            _config = new PluginConfig
            {
                Events = new List<EventEntry>
                {
                    new EventEntry { Name = "Air Event",                      Enabled = true, RequiredPlugin = "AirEvent",         RunTime = 60,  StartCommand = "airstart",                    StopCommand = "" },
                    new EventEntry { Name = "Airfield Event",                 Enabled = true, RequiredPlugin = "AirfieldEvent",    RunTime = 60,  StartCommand = "afestart",                    StopCommand = "" },
                    new EventEntry { Name = "Arctic Base Event",              Enabled = true, RequiredPlugin = "ArcticBaseEvent",  RunTime = 45,  StartCommand = "abstart",                     StopCommand = "" },
                    new EventEntry { Name = "Armored Train",                  Enabled = true, RequiredPlugin = "ArmoredTrain",     RunTime = 60,  StartCommand = "atrainstart",                 StopCommand = "" },
                    new EventEntry { Name = "Boss Monster Clown",             Enabled = true, RequiredPlugin = "BossMonster",      RunTime = 60,  StartCommand = "SpawnBoss Clown",             StopCommand = "KillBoss Clown" },
                    new EventEntry { Name = "Boss Monster Evil",              Enabled = true, RequiredPlugin = "BossMonster",      RunTime = 60,  StartCommand = "SpawnBoss Evil",              StopCommand = "KillBoss Evil" },
                    new EventEntry { Name = "Boss Monster Franken",           Enabled = true, RequiredPlugin = "BossMonster",      RunTime = 60,  StartCommand = "SpawnBoss Franken",           StopCommand = "KillBoss Franken" },
                    new EventEntry { Name = "Boss Monster Frankenstein",      Enabled = true, RequiredPlugin = "BossMonster",      RunTime = 60,  StartCommand = "SpawnBoss Frankenstein",      StopCommand = "KillBoss Frankenstein" },
                    new EventEntry { Name = "Boss Monster Heavy",             Enabled = true, RequiredPlugin = "BossMonster",      RunTime = 60,  StartCommand = "SpawnBoss Heavy",             StopCommand = "KillBoss Heavy" },
                    new EventEntry { Name = "Boss Monster Horror",            Enabled = true, RequiredPlugin = "BossMonster",      RunTime = 60,  StartCommand = "SpawnBoss Horror",            StopCommand = "KillBoss Horror" },
                    new EventEntry { Name = "Boss Monster Jason",             Enabled = true, RequiredPlugin = "BossMonster",      RunTime = 60,  StartCommand = "SpawnBoss Jason",             StopCommand = "KillBoss Jason" },
                    new EventEntry { Name = "Boss Monster King of the Night", Enabled = true, RequiredPlugin = "BossMonster",      RunTime = 60,  StartCommand = "SpawnBoss King of the Night", StopCommand = "KillBoss King of the Night" },
                    new EventEntry { Name = "Boss Monster Michael Myers",     Enabled = true, RequiredPlugin = "BossMonster",      RunTime = 60,  StartCommand = "SpawnBoss Michael Myers",     StopCommand = "KillBoss Michael Myers" },
                    new EventEntry { Name = "Boss Monster Oni",               Enabled = true, RequiredPlugin = "BossMonster",      RunTime = 60,  StartCommand = "SpawnBoss Oni",               StopCommand = "KillBoss Oni" },
                    new EventEntry { Name = "Boss Monster Raptor",            Enabled = true, RequiredPlugin = "BossMonster",      RunTime = 60,  StartCommand = "SpawnBoss Raptor",            StopCommand = "KillBoss Raptor" },
                    new EventEntry { Name = "Boss Monster Scary",             Enabled = true, RequiredPlugin = "BossMonster",      RunTime = 60,  StartCommand = "SpawnBoss Scary",             StopCommand = "KillBoss Scary" },
                    new EventEntry { Name = "Celestial Barrage",              Enabled = true, RequiredPlugin = "CelestialBarrage", RunTime = 5,   StartCommand = "cb.random",                   StopCommand = "" },
                    new EventEntry { Name = "Convoy",                         Enabled = true, RequiredPlugin = "Convoy",           RunTime = 60,  StartCommand = "convoystart",                 StopCommand = "" },
                    new EventEntry { Name = "Gas Station Event",              Enabled = true, RequiredPlugin = "GasStationEvent",  RunTime = 45,  StartCommand = "gsstart",                     StopCommand = "" },
                    new EventEntry { Name = "Gun Game",                       Enabled = true, RequiredPlugin = "GunGame",          RunTime = 45,  StartCommand = "ggstart",                     StopCommand = "" },
                    new EventEntry { Name = "Harbor Event",                   Enabled = true, RequiredPlugin = "HarborEvent",      RunTime = 60,  StartCommand = "harborstart",                 StopCommand = "" },
                    new EventEntry { Name = "Sputnik",                        Enabled = false, RequiredPlugin = "Sputnik",          RunTime = 60,  StartCommand = "sputnikstart",                StopCommand = "" },
                    new EventEntry { Name = "Supermarket Event",              Enabled = true, RequiredPlugin = "SupermarketEvent", RunTime = 45,  StartCommand = "supermarketstart",            StopCommand = "" },
                    new EventEntry { Name = "Twister",                        Enabled = true, RequiredPlugin = "Tornado",           RunTime = 5,   StartCommand = "tornado start random",        StopCommand = "" },
                    new EventEntry { Name = "Water Event",                    Enabled = true, RequiredPlugin = "WaterEvent",       RunTime = 120, StartCommand = "waterstart",                  StopCommand = "" },
                }
            };
            SaveConfig();
        }

        protected override void LoadConfig()
        {
            base.LoadConfig();
            try
            {
                _config = Config.ReadObject<PluginConfig>();
                if (_config?.Events == null)
                    LoadDefaultConfig();
                else
                    SaveConfig();
            }
            catch (Exception ex)
            {
                PrintError($"[rCEventScheduler] Config load error: {ex.Message} - Reverting to defaults.");
                LoadDefaultConfig();
            }
        }

        protected override void SaveConfig() => Config.WriteObject(_config);

        #endregion

        #region Data

        private void LoadData()
        {
            try
            {
                _data = Interface.Oxide.DataFileSystem.ReadObject<PluginData>("rCEventScheduler/rCEventScheduler_data");
                if (_data == null)
                    _data = new PluginData();
            }
            catch (Exception ex)
            {
                PrintError($"[rCEventScheduler] Error loading data file: {ex.Message}");
                _data = new PluginData();
            }
        }

        private void SaveData()
        {
            try
            {
                Interface.Oxide.DataFileSystem.WriteObject("rCEventScheduler/rCEventScheduler_data", _data);
            }
            catch (Exception ex)
            {
                PrintError($"[rCEventScheduler] Error saving data file: {ex.Message}");
            }
        }

        #endregion

        #region Oxide Hooks

        private void Init()
        {
            LoadData();
        }

        private void OnServerInitialized()
        {
            var enabled = _config.Events.Where(e => e.Enabled).ToList();

            if (enabled.Count == 0)
            {
                PrintWarning("[rCEventScheduler] No enabled events found in config. Scheduler will not start.");
                return;
            }

            // Validate required plugins
            var valid   = new List<EventEntry>();
            var skipped = new List<EventEntry>();

            foreach (var evt in enabled)
            {
                if (!string.IsNullOrEmpty(evt.RequiredPlugin) && plugins.Find(evt.RequiredPlugin) == null)
                    skipped.Add(evt);
                else
                    valid.Add(evt);
            }

            if (valid.Count == 0)
            {
                PrintWarning("[rCEventScheduler] No valid events after plugin validation. Scheduler will not start.");
                return;
            }

            // Plugin Loaded message — valid events only
            string names     = string.Join(", ", valid.Select(e => e.Name));
            string eventList = string.Join("\n", valid.Select(e => $"• {e.Name}"));

            LogEvent(
                consoleMsg: $"[rCEventScheduler] {valid.Count} event(s) loaded: {names}",
                title:      $"{ConVar.Server.hostname} Event Scheduler",
                desc:       $"Plugin loaded - **{valid.Count} event(s)** are ready to schedule.",
                fields:     new List<EmbedField> { new EmbedField("Loaded Events", eventList, false) },
                color:      EmbedColors.Blue
            );

            timer.Once(2f, () =>
            {
                // Skipped events message (one combined message after load)
                if (skipped.Count > 0)
                {
                    string skippedList = string.Join("\n", skipped.Select(e => $"• {e.Name} - plugin: {e.RequiredPlugin}"));
                    string skippedNames = string.Join(", ", skipped.Select(e => e.Name));

                    LogEvent(
                        consoleMsg: $"[rCEventScheduler] {skipped.Count} event(s) skipped - required plugin not loaded: {skippedNames}",
                        title:      $"{ConVar.Server.hostname} Event Scheduler",
                        desc:       $"**Events Skipped - Plugin Not Loaded**\n{skipped.Count} event(s) were omitted from the scheduler.",
                        fields:     new List<EmbedField> { new EmbedField("Skipped Events", skippedList, false) },
                        color:      EmbedColors.Orange
                    );

                    timer.Once(2f, () =>
                    {
                        BuildQueue(valid);
                        timer.Once(2f, ScheduleNext);
                    });
                }
                else
                {
                    BuildQueue(valid);
                    timer.Once(2f, ScheduleNext);
                }
            });
        }

        private void Unload()
        {
            _schedulerTimer?.Destroy();
            SaveData();
        }

        private object OnPlayerChat(BasePlayer player, string message, ConVar.Chat.ChatChannel channel)
        {
            if (!_config.EnablePlayerCommand) return null;
            if (string.IsNullOrEmpty(message)) return null;
            if (message.Trim().ToLower() != "!events") return null;

            if (IsEventsCooldownActive(player)) return true;

            _lastEventsCommand = DateTime.Now;
            ShowEventStatus();
            return true;
        }

        [ChatCommand("events")]
        private void CmdEvents(BasePlayer player, string command, string[] args)
        {
            if (!_config.EnablePlayerCommand) return;

            if (IsEventsCooldownActive(player)) return;

            _lastEventsCommand = DateTime.Now;
            ShowEventStatus();
        }

        private bool IsEventsCooldownActive(BasePlayer player)
        {
            double elapsed = (DateTime.Now - _lastEventsCommand).TotalSeconds;
            if (elapsed >= 300) return false;

            int secsLeft = (int)(300 - elapsed) + 1;
            player.ChatMessage($"<color=#E67E22>[ {ConVar.Server.hostname} Event Scheduler ]</color>\n<color=#95A5A6>Command on cooldown. Try again in <color=#F1C40F>{secsLeft}s</color>.</color>");
            return true;
        }

        #endregion

        #region Scheduler

        private void BuildQueue(List<EventEntry> events)
        {
            _eventQueue  = events.OrderBy(_ => _rng.Next()).ToList();
            _cycleTotal  = _eventQueue.Count;

            string consoleOrder = string.Join(" > ", _eventQueue.Select(e => e.Name));

            int idx = 1;
            string numberedList = string.Join("\n", _eventQueue.Select(e => $"{idx++}. {e.Name}"));

            LogEvent(
                consoleMsg: $"[rCEventScheduler] Randomized event queue: {consoleOrder}",
                title:      $"{ConVar.Server.hostname} Event Scheduler",
                desc:       $"A new randomized event queue has been built.\n**{_eventQueue.Count} event(s)** in this cycle.",
                fields:     new List<EmbedField> { new EmbedField("Queue Order", numberedList, false) },
                color:      EmbedColors.Purple
            );
        }

        private void ScheduleNext()
        {
            if (_eventQueue.Count == 0)
            {
                var enabled = _config.Events.Where(e => e.Enabled).ToList();

                if (enabled.Count == 0)
                {
                    PrintWarning("[rCEventScheduler] No enabled events to schedule. Scheduler stopped.");
                    return;
                }

                // Validate required plugins — same check as initial load
                var valid   = enabled.Where(e => string.IsNullOrEmpty(e.RequiredPlugin) || plugins.Find(e.RequiredPlugin) != null).ToList();
                var skipped = enabled.Where(e => !string.IsNullOrEmpty(e.RequiredPlugin) && plugins.Find(e.RequiredPlugin) == null).ToList();

                if (valid.Count == 0)
                {
                    PrintWarning("[rCEventScheduler] No valid events after plugin validation. Scheduler stopped.");
                    return;
                }

                if (skipped.Count > 0)
                {
                    string skippedNames = string.Join(", ", skipped.Select(e => $"{e.Name} ({e.RequiredPlugin})"));
                    Puts($"[rCEventScheduler] New cycle - {skipped.Count} event(s) skipped (plugin not loaded): {skippedNames}");
                }

                // Staggered: T+0 Cycle Complete → T+2s Queue Randomized → T+4s Next Event Scheduled
                LogEvent(
                    consoleMsg: "[rCEventScheduler] All events have run. Starting a new cycle.",
                    title:      $"{ConVar.Server.hostname} Event Scheduler",
                    desc:       "**Cycle Complete**\nAll events in the cycle have run. A fresh randomized cycle is starting.",
                    fields:     null,
                    color:      EmbedColors.Purple
                );

                timer.Once(2f, () =>
                {
                    BuildQueue(valid);
                    timer.Once(2f, ScheduleNextEvent);
                });
                return;
            }

            ScheduleNextEvent();
        }

        private void ScheduleNextEvent()
        {
            _nextEvent = _eventQueue[0];

            int bufferSecs  = (_config.BufferTimeEnabled
                ? _rng.Next(_config.MinBufferTime, _config.MaxBufferTime + 1)
                : 0) * 60;

            int slotSecs    = _activeEvents.Count >= _config.MaxActiveEvents ? SecsUntilSlot() : 0;
            int totalSecs   = slotSecs + bufferSecs;
            int displayMins = totalSecs / 60;

            int queuePos  = _cycleTotal - _eventQueue.Count + 1;
            int afterThis = _eventQueue.Count - 1;

            _nextEventTime = DateTime.Now.AddSeconds(totalSecs);

            string tz      = GetTzAbbr();
            string timeStr = _nextEventTime.ToString("h:mm tt") + " " + tz;

            LogEvent(
                consoleMsg: $"[rCEventScheduler] Next event: {_nextEvent.Name} - scheduled at {timeStr} (in ~{displayMins} min) [{queuePos}/{_cycleTotal}]",
                title:      $"{ConVar.Server.hostname} Event Scheduler",
                desc:       "**Next Event Scheduled**\nThe next event has been queued.",
                fields:     new List<EmbedField>
                {
                    new EmbedField("Event",           _nextEvent.Name,                                                                             false),
                    new EmbedField("Scheduled Time",  timeStr,                                                                                     false),
                    new EmbedField("In",              $"~{displayMins} minutes",                                                                   false),
                    new EmbedField("Queue Position",  $"{queuePos} of {_cycleTotal}",                                                              false),
                    new EmbedField("Until Reshuffle", afterThis == 0 ? "This is the last event - reshuffle next" : $"{afterThis} event(s) after this one", false)
                },
                color: EmbedColors.Teal
            );

            UpdateStickyStatus();

            _schedulerTimer?.Destroy();
            _schedulerTimer = timer.Once(totalSecs, TryFire);
        }

        private void TryFire()
        {
            if (_activeEvents.Count >= _config.MaxActiveEvents)
            {
                int bufferSecs = _rng.Next(_config.MinBufferTime, _config.MaxBufferTime + 1) * 60;
                int slotSecs   = SecsUntilSlot();
                int waitSecs   = slotSecs + bufferSecs;
                int waitMins   = waitSecs / 60;

                _nextEventTime = DateTime.Now.AddSeconds(waitSecs);

                string tz      = GetTzAbbr();
                string timeStr = _nextEventTime.ToString("h:mm tt") + " " + tz;

                LogEvent(
                    consoleMsg: $"[rCEventScheduler] Max active events ({_config.MaxActiveEvents}) reached. {_nextEvent.Name} delayed ~{waitMins} min - retrying at {timeStr}",
                    title:      $"{ConVar.Server.hostname} Event Scheduler",
                    desc:       $"**Event Delayed**\nMax active events reached. **{_nextEvent.Name}** has been delayed.",
                    fields:     new List<EmbedField>
                    {
                        new EmbedField("Event",         _nextEvent.Name,                                          false),
                        new EmbedField("Delayed Until", timeStr,                                                  false),
                        new EmbedField("In",            $"~{waitMins} minutes",                                   false),
                        new EmbedField("Reason",        $"Max active events ({_config.MaxActiveEvents}) reached", false)
                    },
                    color: EmbedColors.Orange
                );

                UpdateStickyStatus();

                _schedulerTimer?.Destroy();
                _schedulerTimer = timer.Once(waitSecs, TryFire);
                return;
            }

            var evt = _nextEvent;
            _eventQueue.RemoveAt(0);
            FireEvent(evt);
            timer.Once(2f, ScheduleNext);
        }

        private void FireEvent(EventEntry evt)
        {
            _activeEvents.Add(evt.Name);
            _activeEventEndTimes[evt.Name] = DateTime.Now.AddMinutes(evt.RunTime);
            RunCmd(evt.StartCommand);

            string tz      = GetTzAbbr();
            string endTime = DateTime.Now.AddMinutes(evt.RunTime).ToString("h:mm tt") + " " + tz;
            string stopMethod = string.IsNullOrEmpty(evt.StopCommand) ? "None (self-managed)" : evt.StopCommand;

            LogEvent(
                consoleMsg: $"[rCEventScheduler] >> Event STARTED: {evt.Name}  |  Runs until ~{endTime}",
                title:      $"{ConVar.Server.hostname} Event Scheduler",
                desc:       $"**Event Started**\n**{evt.Name}** is now active!",
                fields:     new List<EmbedField>
                {
                    new EmbedField("Event",        evt.Name,                false),
                    new EmbedField("Run Time",     $"{evt.RunTime} minutes", false),
                    new EmbedField("Expected End", endTime,                 false),
                    new EmbedField("Stop Method",  stopMethod,              false)
                },
                color: EmbedColors.Green
            );

            UpdateStickyStatus();

            timer.Once(evt.RunTime * 60f, () => EndEvent(evt));
        }

        private void EndEvent(EventEntry evt)
        {
            if (!string.IsNullOrEmpty(evt.StopCommand))
                RunCmd(evt.StopCommand);

            _activeEvents.Remove(evt.Name);
            _activeEventEndTimes.Remove(evt.Name);

            string status = string.IsNullOrEmpty(evt.StopCommand)
                ? "Ended (self-managed)"
                : "Stopped via command";

            LogEvent(
                consoleMsg: $"[rCEventScheduler] -- Event ENDED: {evt.Name}  ({status})",
                title:      $"{ConVar.Server.hostname} Event Scheduler",
                desc:       $"**Event Ended**\n**{evt.Name}** has ended.",
                fields:     new List<EmbedField>
                {
                    new EmbedField("Event",  evt.Name, false),
                    new EmbedField("Status", status,   false)
                },
                color: EmbedColors.Orange
            );

            UpdateStickyStatus();

            if (_config.ShowNextEventOnEnd && _config.LogToDiscord && !string.IsNullOrEmpty(_config.WebhookUrl)
                && _nextEvent != null && _nextEventTime > DateTime.Now)
            {
                timer.Once(2f, () =>
                {
                    int queuePos  = _cycleTotal - _eventQueue.Count + 1;
                    int afterThis = _eventQueue.Count - 1;

                    string tz         = GetTzAbbr();
                    string timeStr    = _nextEventTime.ToString("h:mm tt") + " " + tz;
                    int    displayMins = (int)(_nextEventTime - DateTime.Now).TotalMinutes;

                    SendEmbed(
                        $"{ConVar.Server.hostname} Event Scheduler",
                        "**Next Event Scheduled**\nReminder after event end.",
                        new List<EmbedField>
                        {
                            new EmbedField("Event",           _nextEvent.Name,                                                                                     false),
                            new EmbedField("Scheduled Time",  timeStr,                                                                                             false),
                            new EmbedField("In",              $"~{displayMins} minutes",                                                                           false),
                            new EmbedField("Queue Position",  $"{queuePos} of {_cycleTotal}",                                                                      false),
                            new EmbedField("Until Reshuffle", afterThis == 0 ? "This is the last event - reshuffle next" : $"{afterThis} event(s) after this one", false)
                        },
                        EmbedColors.Teal
                    );
                });
            }
        }

        #endregion

        #region Player Command

        private void ShowEventStatus()
        {
            string tz = GetTzAbbr();
            var sb = new StringBuilder();

            sb.Append($"<color=#00BFFF><b>[ {ConVar.Server.hostname} Event Scheduler ]</b></color>\n");

            if (_activeEvents.Count > 0)
            {
                sb.Append("<color=#2ECC71><b>Active Events:</b></color>\n");
                foreach (string name in _activeEvents)
                    sb.Append($"  <color=#F1C40F>* {name}</color>\n");
            }
            else
            {
                sb.Append("<color=#95A5A6>  No events currently active.</color>\n");
            }

            if (_nextEvent != null && _nextEventTime > DateTime.Now)
            {
                TimeSpan remaining = _nextEventTime - DateTime.Now;
                string eta     = remaining.TotalMinutes >= 1
                    ? $"~{(int)remaining.TotalMinutes} min"
                    : "< 1 min";
                string timeStr = _nextEventTime.ToString("h:mm tt") + " " + tz;

                sb.Append($"<color=#3498DB><b>Next Event:</b></color> <color=#F1C40F>{_nextEvent.Name}</color>\n");
                sb.Append($"<color=#3498DB><b>Starts at:</b></color> {timeStr}  ({eta})");
            }
            else
            {
                sb.Append("<color=#95A5A6>  Next event: Not yet scheduled.</color>");
            }

            Server.Broadcast(sb.ToString());
        }

        #endregion

        #region Discord

        private static class EmbedColors
        {
            public const int Blue   = 3447003;   // #3498DB — info / plugin load
            public const int Green  = 3066993;   // #2ECC71 — event started
            public const int Orange = 15105570;  // #E67E22 — event ended / delayed
            public const int Purple = 10181046;  // #9B59B6 — queue randomized / cycle complete
            public const int Teal   = 1752220;   // #1ABC9C — next event scheduled
            public const int Gold   = 16776960;  // #FFFF00 — live sticky status
        }

        private class EmbedField
        {
            public string Name;
            public string Value;
            public bool   Inline;

            public EmbedField(string n, string v, bool i) { Name = n; Value = v; Inline = i; }
        }

        private void LogEvent(string consoleMsg, string title, string desc, List<EmbedField> fields, int color)
        {
            if (_config.LogToConsole)
                Puts(consoleMsg);

            if (_config.LogToDiscord && !string.IsNullOrEmpty(_config.WebhookUrl))
                SendEmbed(title, desc, fields, color);
        }

        private void SendEmbed(string title, string description, List<EmbedField> fields, int color)
        {
            var embed = new DiscordEmbed()
                .SetTitle(title)
                .SetDescription(description)
                .SetColor(color)
                .SetTimestamp(DateTimeOffset.UtcNow)
                .SetFooter($"rCEventScheduler v{Version}");

            if (fields != null)
                foreach (var f in fields)
                    embed.AddField(f.Name, f.Value, f.Inline);

            var msg = new DiscordMessage().AddEmbed(embed);
            webrequest.Enqueue(_config.WebhookUrl, msg.ToJson(), DiscordCallback, this, RequestMethod.POST, _headers);
        }

        private void DiscordCallback(int code, string response)
        {
            if (code != 204 && code != 200)
                PrintWarning($"[rCEventScheduler] Discord webhook error ({code}): {response}");
        }

        public class DiscordMessage
        {
            [JsonProperty("username", NullValueHandling = NullValueHandling.Ignore)]
            public string Username { get; set; }

            [JsonProperty("embeds")]
            private List<DiscordEmbed> Embeds { get; set; } = new List<DiscordEmbed>();

            public DiscordMessage AddEmbed(DiscordEmbed embed) { Embeds.Add(embed); return this; }
            public DiscordMessage SetUsername(string username) { Username = username; return this; }

            public string ToJson() => JsonConvert.SerializeObject(this, Formatting.None,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }

        public class DiscordEmbed
        {
            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }

            [JsonProperty("color")]
            public int Color { get; set; }

            [JsonProperty("fields", NullValueHandling = NullValueHandling.Ignore)]
            public List<DiscordEmbedField> Fields { get; set; }

            [JsonProperty("footer")]
            public DiscordEmbedFooter Footer { get; set; }

            [JsonProperty("timestamp")]
            public DateTimeOffset? Timestamp { get; set; }

            public DiscordEmbed SetTitle(string title)           { Title       = title;                              return this; }
            public DiscordEmbed SetDescription(string desc)      { Description = desc;                               return this; }
            public DiscordEmbed SetColor(int color)              { Color       = color;                              return this; }
            public DiscordEmbed SetTimestamp(DateTimeOffset ts)  { Timestamp   = ts;                                 return this; }
            public DiscordEmbed SetFooter(string text)           { Footer      = new DiscordEmbedFooter { Text = text }; return this; }

            public DiscordEmbed AddField(string name, string value, bool inline = false)
            {
                if (Fields == null) Fields = new List<DiscordEmbedField>();
                Fields.Add(new DiscordEmbedField { Name = name, Value = value, Inline = inline });
                return this;
            }
        }

        public class DiscordEmbedField
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("value")]
            public string Value { get; set; }

            [JsonProperty("inline")]
            public bool Inline { get; set; }
        }

        public class DiscordEmbedFooter
        {
            [JsonProperty("text")]
            public string Text { get; set; }
        }

        #endregion

        #region Sticky Status (Discord)

        private void UpdateStickyStatus()
        {
            if (!_config.EnableStickyStatus) return;
            if (string.IsNullOrEmpty(_config.StickyWebhookUrl)) return;

            string tz = GetTzAbbr();
            var fields = new List<EmbedField>();

            if (_activeEvents.Count > 0)
            {
                var activeLines = new List<string>();
                foreach (string name in _activeEvents)
                {
                    string line = $"• {name}";
                    if (_activeEventEndTimes.TryGetValue(name, out DateTime endTime))
                    {
                        string endStr   = endTime.ToString("h:mm tt") + " " + tz;
                        int    minsLeft = Math.Max(0, (int)(endTime - DateTime.Now).TotalMinutes);
                        line += $" - ends {endStr} (~{minsLeft} min left)";
                    }
                    activeLines.Add(line);
                }
                fields.Add(new EmbedField("Active Event(s)", string.Join("\n", activeLines), false));
            }
            else
            {
                fields.Add(new EmbedField("Active Event(s)", "No events currently active.", false));
            }

            if (_nextEvent != null && _nextEventTime > DateTime.Now)
            {
                int queuePos  = _cycleTotal - _eventQueue.Count + 1;
                int afterThis = _eventQueue.Count - 1;

                string timeStr     = _nextEventTime.ToString("h:mm tt") + " " + tz;
                int    displayMins = Math.Max(0, (int)(_nextEventTime - DateTime.Now).TotalMinutes);

                fields.Add(new EmbedField("Next Event",       _nextEvent.Name,                                                                             false));
                fields.Add(new EmbedField("Scheduled Time",   $"{timeStr} (~{displayMins} min)",                                                           false));
                fields.Add(new EmbedField("Queue Position",   $"{queuePos} of {_cycleTotal}",                                                              false));
                fields.Add(new EmbedField("Until Reshuffle",  afterThis == 0 ? "This is the last event - reshuffle next" : $"{afterThis} event(s) after this one", false));
            }
            else
            {
                fields.Add(new EmbedField("Next Event", "Not yet scheduled.", false));
            }

            var queueLines = new List<string>();
            int qn = 1;
            foreach (var proj in ProjectQueueTimes())
            {
                string timeStr = proj.Time.ToString("h:mm tt") + " " + tz;
                queueLines.Add($"{qn}. {proj.Event.Name} - {timeStr}{(proj.Exact ? "" : " (est.)")}");
                qn++;
            }

            if (queueLines.Count == 0)
            {
                fields.Add(new EmbedField("Upcoming Queue", "Not yet scheduled.", false));
            }
            else
            {
                var chunks = ChunkLines(queueLines, 1000);
                if (chunks.Count == 1)
                {
                    fields.Add(new EmbedField("Upcoming Queue", chunks[0], false));
                }
                else
                {
                    for (int i = 0; i < chunks.Count; i++)
                        fields.Add(new EmbedField($"Upcoming Queue ({i + 1}/{chunks.Count})", chunks[i], false));
                }
            }

            SendOrEditStickyMessage(
                $"{ConVar.Server.hostname} Event Scheduler",
                "**Live Event Status**\nThis message updates automatically as events start, end, and get scheduled.",
                fields,
                EmbedColors.Gold
            );
        }

        private struct QueuedProjection
        {
            public EventEntry Event;
            public DateTime Time;
            public bool Exact;
        }

        // Projects a local kickoff time for every event remaining in this cycle's queue.
        // The first entry is exact (it's _nextEventTime, already computed with slot/buffer logic).
        // Every entry after that is an estimate - it assumes sequential firing using the average
        // of Min/Max buffer time, since the real buffer for each event is only randomized once
        // it actually becomes next in ScheduleNextEvent().
        private List<QueuedProjection> ProjectQueueTimes()
        {
            var results = new List<QueuedProjection>();
            if (_nextEvent == null || _nextEventTime <= DateTime.Now || _eventQueue.Count == 0)
                return results;

            double avgBufferMin = _config.BufferTimeEnabled
                ? (_config.MinBufferTime + _config.MaxBufferTime) / 2.0
                : 0;

            DateTime cursor = _nextEventTime;
            for (int i = 0; i < _eventQueue.Count; i++)
            {
                var evt = _eventQueue[i];
                DateTime time = cursor;
                results.Add(new QueuedProjection { Event = evt, Time = time, Exact = i == 0 });
                cursor = time.AddMinutes(evt.RunTime + avgBufferMin);
            }

            return results;
        }

        private List<string> ChunkLines(List<string> lines, int maxChars)
        {
            var chunks = new List<string>();
            var sb = new StringBuilder();

            foreach (string line in lines)
            {
                if (sb.Length > 0 && sb.Length + line.Length + 1 > maxChars)
                {
                    chunks.Add(sb.ToString());
                    sb.Clear();
                }
                if (sb.Length > 0) sb.Append('\n');
                sb.Append(line);
            }

            if (sb.Length > 0) chunks.Add(sb.ToString());
            if (chunks.Count == 0) chunks.Add("None");

            return chunks;
        }

        private void SendOrEditStickyMessage(string title, string description, List<EmbedField> fields, int color)
        {
            var embed = new DiscordEmbed()
                .SetTitle(title)
                .SetDescription(description)
                .SetColor(color)
                .SetTimestamp(DateTimeOffset.UtcNow)
                .SetFooter($"rCEventScheduler v{Version}");

            foreach (var f in fields)
                embed.AddField(f.Name, f.Value, f.Inline);

            var stickyMsg = new DiscordMessage().AddEmbed(embed);
            if (!string.IsNullOrEmpty(_config.StickyBotName))
                stickyMsg.SetUsername(_config.StickyBotName);

            string payload = stickyMsg.ToJson();

            if (!string.IsNullOrEmpty(_data.StatusMessageId))
            {
                string editUrl = $"{_config.StickyWebhookUrl}/messages/{_data.StatusMessageId}";

                webrequest.Enqueue(editUrl, payload, (code, response) =>
                {
                    if (code == 200 || code == 204) return;

                    if (code == 404 || code == 400)
                    {
                        PrintWarning($"[rCEventScheduler] Sticky status message missing/invalid (HTTP {code}) - creating a new one.");
                        _data.StatusMessageId = null;
                        SaveData();
                        CreateStickyMessage(payload);
                    }
                    else
                    {
                        PrintWarning($"[rCEventScheduler] Failed to edit sticky status message (HTTP {code}): {response}");
                    }
                }, this, RequestMethod.PATCH, _headers);
            }
            else
            {
                CreateStickyMessage(payload);
            }
        }

        private void CreateStickyMessage(string payload)
        {
            webrequest.Enqueue(_config.StickyWebhookUrl + "?wait=true", payload, (code, response) =>
            {
                if (code != 200)
                {
                    PrintWarning($"[rCEventScheduler] Failed to create sticky status message (HTTP {code}): {response}");
                    return;
                }

                try
                {
                    var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);
                    if (data != null && data.ContainsKey("id"))
                    {
                        _data.StatusMessageId = data["id"].ToString();
                        SaveData();
                        Puts($"[rCEventScheduler] Created sticky status Discord message (ID: {_data.StatusMessageId})");
                    }
                }
                catch (Exception ex)
                {
                    PrintError($"[rCEventScheduler] Error parsing sticky status message response: {ex.Message}");
                }
            }, this, RequestMethod.POST, _headers);
        }

        #endregion

        #region Sticky Status Console Commands

        [ConsoleCommand("rces.resetstatus")]
        private void CmdResetStatus(ConsoleSystem.Arg arg)
        {
            _data.StatusMessageId = null;
            SaveData();
            Puts("[rCEventScheduler] Sticky status message ID cleared. A new message will be created on the next update.");
        }

        [ConsoleCommand("rces.forcestatus")]
        private void CmdForceStatus(ConsoleSystem.Arg arg)
        {
            if (!_config.EnableStickyStatus)
            {
                Puts("[rCEventScheduler] Sticky status message is disabled in config (\"Enable Status Sticky Message\": false).");
                return;
            }

            if (string.IsNullOrEmpty(_config.StickyWebhookUrl))
            {
                Puts("[rCEventScheduler] No \"Status Sticky Discord Webhook URL\" configured.");
                return;
            }

            UpdateStickyStatus();
            Puts("[rCEventScheduler] Sticky status message update forced.");
        }

        [ConsoleCommand("rces.status")]
        private void CmdStickyStatusInfo(ConsoleSystem.Arg arg)
        {
            Puts("=== rCEventScheduler Sticky Status ===");
            Puts($"Enabled: {_config.EnableStickyStatus}");
            Puts($"Webhook Configured: {!string.IsNullOrEmpty(_config.StickyWebhookUrl)}");
            Puts($"Stored Message ID: {(_data.StatusMessageId ?? "(none)")}");
            Puts($"Active Events: {(_activeEvents.Count > 0 ? string.Join(", ", _activeEvents) : "None")}");
            Puts($"Next Event: {(_nextEvent != null ? _nextEvent.Name : "Not yet scheduled")}");
        }

        #endregion

        #region Helpers

        // Returns seconds until the earliest active event is expected to end (0 if no tracked events)
        private int SecsUntilSlot()
        {
            if (_activeEventEndTimes.Count == 0) return 0;
            DateTime earliest = _activeEventEndTimes.Values.Min();
            int secs = (int)(earliest - DateTime.Now).TotalSeconds;
            return Math.Max(secs, 0);
        }

        private void RunCmd(string fullCmd)
        {
            if (string.IsNullOrEmpty(fullCmd)) return;

            int sp = fullCmd.IndexOf(' ');
            if (sp < 0)
                ConsoleSystem.Run(ConsoleSystem.Option.Server.Quiet(), fullCmd);
            else
                ConsoleSystem.Run(ConsoleSystem.Option.Server.Quiet(), fullCmd.Substring(0, sp), fullCmd.Substring(sp + 1));
        }

        private string GetTzAbbr()
        {
            var    tz    = TimeZoneInfo.Local;
            bool   isDst = tz.IsDaylightSavingTime(DateTime.Now);
            string name  = isDst ? tz.DaylightName : tz.StandardName;
            return string.Concat(
                name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(w => w[0])
            );
        }

        #endregion
    }
}
