using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Oxide.Core.Libraries;

namespace Oxide.Plugins
{
    [Info("Rust Custom Event Scheduler", "Ftuoil Xelrash", "0.0.20")]
    [Description("Schedules and manages custom Rust server events with randomized queues and Discord notifications.")]
    public class rCEventScheduler : RustPlugin
    {
        #region Fields

        private PluginConfig _config;
        private List<EventEntry> _eventQueue = new List<EventEntry>();
        private readonly List<string> _activeEvents = new List<string>();
        private Timer _schedulerTimer;
        private DateTime _nextEventTime = DateTime.MinValue;
        private EventEntry _nextEvent;
        private readonly System.Random _rng = new System.Random();
        private DateTime _lastEventsCommand = DateTime.MinValue;

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
            public int MinBufferTime = 15;

            [JsonProperty("Event Max Buffer Time (minutes)")]
            public int MaxBufferTime = 30;

            [JsonProperty("Enable Player Events Command")]
            public bool EnablePlayerCommand = true;

            [JsonProperty("Events")]
            public List<EventEntry> Events = new List<EventEntry>();
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
                    new EventEntry { Name = "Arctic Base Event",              Enabled = true, RequiredPlugin = "ArcticBaseEvent",  RunTime = 60,  StartCommand = "abstart",                     StopCommand = "" },
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
                    new EventEntry { Name = "Gas Station Event",              Enabled = true, RequiredPlugin = "GasStationEvent",  RunTime = 60,  StartCommand = "gsstart",                     StopCommand = "" },
                    new EventEntry { Name = "Gun Game",                       Enabled = true, RequiredPlugin = "GunGame",          RunTime = 45,  StartCommand = "ggstart",                     StopCommand = "" },
                    new EventEntry { Name = "Harbor Event",                   Enabled = true, RequiredPlugin = "HarborEvent",      RunTime = 60,  StartCommand = "harborstart",                 StopCommand = "" },
                    new EventEntry { Name = "Sputnik",                        Enabled = true, RequiredPlugin = "Sputnik",          RunTime = 5,   StartCommand = "sputnikstart",                StopCommand = "" },
                    new EventEntry { Name = "Supermarket Event",              Enabled = true, RequiredPlugin = "SupermarketEvent", RunTime = 60,  StartCommand = "supermarketstart",            StopCommand = "" },
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
                PrintError($"[rCEventScheduler] Config load error: {ex.Message} — Reverting to defaults.");
                LoadDefaultConfig();
            }
        }

        protected override void SaveConfig() => Config.WriteObject(_config);

        #endregion

        #region Oxide Hooks

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
                title:      "Rust Custom Event Scheduler",
                desc:       $"Plugin loaded — **{valid.Count} event(s)** are ready to schedule.",
                fields:     new List<EmbedField> { new EmbedField("Loaded Events", eventList, false) },
                color:      EmbedColors.Blue
            );

            timer.Once(2f, () =>
            {
                // Skipped events message (one combined message after load)
                if (skipped.Count > 0)
                {
                    string skippedList = string.Join("\n", skipped.Select(e => $"• {e.Name}  —  plugin: {e.RequiredPlugin}"));
                    string skippedNames = string.Join(", ", skipped.Select(e => e.Name));

                    LogEvent(
                        consoleMsg: $"[rCEventScheduler] {skipped.Count} event(s) skipped — required plugin not loaded: {skippedNames}",
                        title:      "Rust Custom Event Scheduler",
                        desc:       $"**Events Skipped — Plugin Not Loaded**\n{skipped.Count} event(s) were omitted from the scheduler.",
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
        }

        private void OnPlayerChat(BasePlayer player, string message, ConVar.Chat.ChatChannel channel)
        {
            if (!_config.EnablePlayerCommand) return;
            if (string.IsNullOrEmpty(message)) return;
            if (message.Trim().ToLower() != "!events") return;

            if ((DateTime.Now - _lastEventsCommand).TotalMinutes < 5) return;

            _lastEventsCommand = DateTime.Now;
            ShowEventStatus(player);
        }

        #endregion

        #region Scheduler

        private void BuildQueue(List<EventEntry> events)
        {
            _eventQueue = events.OrderBy(_ => _rng.Next()).ToList();

            string consoleOrder = string.Join(" > ", _eventQueue.Select(e => e.Name));

            int idx = 1;
            string numberedList = string.Join("\n", _eventQueue.Select(e => $"{idx++}. {e.Name}"));

            LogEvent(
                consoleMsg: $"[rCEventScheduler] Randomized event queue: {consoleOrder}",
                title:      "Rust Custom Event Scheduler",
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

                LogEvent(
                    consoleMsg: "[rCEventScheduler] All events have run. Starting a new cycle.",
                    title:      "Rust Custom Event Scheduler",
                    desc:       "**Cycle Complete**\nAll events in the cycle have run. A fresh randomized cycle is starting.",
                    fields:     null,
                    color:      EmbedColors.Purple
                );

                BuildQueue(enabled);
            }

            _nextEvent = _eventQueue[0];

            int bufferMins = _config.BufferTimeEnabled
                ? _rng.Next(_config.MinBufferTime, _config.MaxBufferTime + 1)
                : 0;

            _nextEventTime = DateTime.Now.AddMinutes(bufferMins);

            string tz      = GetTzAbbr();
            string timeStr = _nextEventTime.ToString("h:mm tt") + " " + tz;

            LogEvent(
                consoleMsg: $"[rCEventScheduler] Next event: {_nextEvent.Name} — scheduled at {timeStr} (in {bufferMins} min)",
                title:      "Rust Custom Event Scheduler",
                desc:       "**Next Event Scheduled**\nThe next event has been queued.",
                fields:     new List<EmbedField>
                {
                    new EmbedField("Event",          _nextEvent.Name,         false),
                    new EmbedField("Scheduled Time", timeStr,                 false),
                    new EmbedField("In",             $"{bufferMins} minutes", false)
                },
                color: EmbedColors.Teal
            );

            _schedulerTimer?.Destroy();
            _schedulerTimer = timer.Once(bufferMins * 60f, TryFire);
        }

        private void TryFire()
        {
            if (_activeEvents.Count >= _config.MaxActiveEvents)
            {
                int waitBuf  = _rng.Next(_config.MinBufferTime, _config.MaxBufferTime + 1);
                int waitMins = _nextEvent.RunTime + waitBuf;

                _nextEventTime = DateTime.Now.AddMinutes(waitMins);

                string tz      = GetTzAbbr();
                string timeStr = _nextEventTime.ToString("h:mm tt") + " " + tz;

                LogEvent(
                    consoleMsg: $"[rCEventScheduler] Max active events ({_config.MaxActiveEvents}) reached. {_nextEvent.Name} delayed {waitMins} min — retrying at {timeStr}",
                    title:      "Rust Custom Event Scheduler",
                    desc:       $"**Event Delayed**\nMax active events reached. **{_nextEvent.Name}** has been delayed.",
                    fields:     new List<EmbedField>
                    {
                        new EmbedField("Event",         _nextEvent.Name,                                          false),
                        new EmbedField("Delayed Until", timeStr,                                                  false),
                        new EmbedField("In",            $"{waitMins} minutes",                                    false),
                        new EmbedField("Reason",        $"Max active events ({_config.MaxActiveEvents}) reached", false)
                    },
                    color: EmbedColors.Orange
                );

                _schedulerTimer?.Destroy();
                _schedulerTimer = timer.Once(waitMins * 60f, TryFire);
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
            RunCmd(evt.StartCommand);

            string tz      = GetTzAbbr();
            string endTime = DateTime.Now.AddMinutes(evt.RunTime).ToString("h:mm tt") + " " + tz;
            string stopMethod = string.IsNullOrEmpty(evt.StopCommand) ? "None (self-managed)" : evt.StopCommand;

            LogEvent(
                consoleMsg: $"[rCEventScheduler] >> Event STARTED: {evt.Name}  |  Runs until ~{endTime}",
                title:      "Rust Custom Event Scheduler",
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

            timer.Once(evt.RunTime * 60f, () => EndEvent(evt));
        }

        private void EndEvent(EventEntry evt)
        {
            if (!string.IsNullOrEmpty(evt.StopCommand))
                RunCmd(evt.StopCommand);

            _activeEvents.Remove(evt.Name);

            string status = string.IsNullOrEmpty(evt.StopCommand)
                ? "Ended (self-managed)"
                : "Stopped via command";

            LogEvent(
                consoleMsg: $"[rCEventScheduler] -- Event ENDED: {evt.Name}  ({status})",
                title:      "Rust Custom Event Scheduler",
                desc:       $"**Event Ended**\n**{evt.Name}** has ended.",
                fields:     new List<EmbedField>
                {
                    new EmbedField("Event",  evt.Name, false),
                    new EmbedField("Status", status,   false)
                },
                color: EmbedColors.Orange
            );
        }

        #endregion

        #region Player Command

        private void ShowEventStatus(BasePlayer player)
        {
            string tz = GetTzAbbr();
            var sb = new StringBuilder();

            sb.Append("<color=#00BFFF><b>[ Rust Custom Event Scheduler ]</b></color>\n");

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

            player.ChatMessage(sb.ToString());
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
            [JsonProperty("embeds")]
            private List<DiscordEmbed> Embeds { get; set; } = new List<DiscordEmbed>();

            public DiscordMessage AddEmbed(DiscordEmbed embed) { Embeds.Add(embed); return this; }

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

        #region Helpers

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
