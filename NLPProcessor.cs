using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CybersecurityChatbot
{

    public class NLPProcessor
    {
            public NLPIntent ProcessInput(string input)
            {
                var intent = new NLPIntent { OriginalInput = input };
                string lowerInput = input.ToLower().Trim();

                // Task management patterns
                if (Regex.IsMatch(lowerInput, @"^(add|new|create)\s+task\s*(.*)$"))
                {
                    intent.Type = IntentType.AddTask;
                    ExtractTaskInfo(input, intent);
                }
                else if (Regex.IsMatch(lowerInput, @"^(view|show|list)\s+tasks|my tasks$"))
                {
                    intent.Type = IntentType.ViewTasks;
                }
                else if (Regex.IsMatch(lowerInput, @"^(complete|finish|done)\s+task\s*(\d*)$"))
                {
                    intent.Type = IntentType.CompleteTask;
                    ExtractTaskIndex(lowerInput, intent);
                }
                // Quiz patterns
                else if (Regex.IsMatch(lowerInput, @"^(start|begin|take)\s+quiz|quiz\s+me$"))
                {
                    intent.Type = IntentType.StartQuiz;
                }
                else if (Regex.IsMatch(lowerInput, @"^[abcd]$|^[1-4]$|^my\s+answer\s+is\s+[abcd1-4]$|^answer\s+is\s+[abcd1-4]$", RegexOptions.IgnoreCase))
                {
                    intent.Type = IntentType.AnswerQuiz;
                    ExtractQuizAnswer(lowerInput, intent);
                }
                // Activity log
                else if (Regex.IsMatch(lowerInput, @"^(activity|show|view)\s+log|my\s+activity$"))
                {
                    intent.Type = IntentType.ShowActivityLog;
                }
                // Cybersecurity questions
                else if (ContainsCybersecurityKeywords(lowerInput))
                {
                    intent.Type = IntentType.CybersecurityQuestion;
                }
                // General questions/fallback
                else
                {
                    intent.Type = IntentType.GeneralQuestion;
                }

                return intent;
            }

            private void ExtractTaskInfo(string input, NLPIntent intent)
            {
                var match = Regex.Match(input, @"^(add|new|create)\s+task\s+""([^""]+)""(?:\s+-\s*""([^""]+)"")?(?:\s+(?:on|by|for)\s+(.*))?$", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    intent.TaskTitle = match.Groups[2].Value.Trim();
                    if (match.Groups[3].Success) // Description group
                    {
                        intent.TaskDescription = match.Groups[3].Value.Trim();
                    }
                    if (match.Groups[4].Success) // Date/Time group
                    {
                        intent.ReminderDate = ExtractDateFromInput(match.Groups[4].Value);
                    }
                }
                else
                {
                    // Fallback for simpler "add task my task" without quotes
                    match = Regex.Match(input, @"^(?:add|new|create)\s+task\s+([^\-]+)(?:\-\s*(.*))?$", RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        intent.TaskTitle = match.Groups[1].Value.Trim();
                        if (match.Groups.Count > 2 && !string.IsNullOrWhiteSpace(match.Groups[2].Value))
                        {
                            // Try to split description and potential date
                            string remaining = match.Groups[2].Value.Trim();
                            var dateMatch = Regex.Match(remaining, @"^(.*)(?:on|by|for)\s+(.*)$", RegexOptions.IgnoreCase);
                            if (dateMatch.Success)
                            {
                                intent.TaskDescription = dateMatch.Groups[1].Value.Trim();
                                intent.ReminderDate = ExtractDateFromInput(dateMatch.Groups[2].Value);
                            }
                            else
                            {
                                intent.TaskDescription = remaining;
                            }
                        }
                    }
                }
            }

            private void ExtractTaskIndex(string input, NLPIntent intent)
            {
                var match = Regex.Match(input, @"task\s+(\d+)", RegexOptions.IgnoreCase);
                if (match.Success && int.TryParse(match.Groups[1].Value, out int index))
                {
                    intent.TaskIndex = index;
                }
            }

            private void ExtractQuizAnswer(string input, NLPIntent intent)
            {
                var match = Regex.Match(input, @"([abcd1-4])", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    intent.QuizAnswer = match.Groups[1].Value.ToUpper();
                }
            }

            // NEW: Method to extract date from common natural language phrases
            public DateTime? ExtractDateFromInput(string input)
            {
                string lowerInput = input.ToLower();
                DateTime now = DateTime.Now;

                if (lowerInput.Contains("today"))
                {
                    return now.Date.AddHours(17); // Default to 5 PM today
                }
                if (lowerInput.Contains("tomorrow"))
                {
                    return now.Date.AddDays(1).AddHours(17); // Default to 5 PM tomorrow
                }
                if (lowerInput.Contains("next week"))
                {
                    // Find next Monday
                    int daysUntilMonday = ((int)DayOfWeek.Monday - (int)now.DayOfWeek + 7) % 7;
                    if (daysUntilMonday == 0) daysUntilMonday = 7; // If today is Monday, get next Monday
                    return now.Date.AddDays(daysUntilMonday).AddHours(17);
                }
                if (lowerInput.Contains("in ") && lowerInput.Contains(" day"))
                {
                    var match = Regex.Match(lowerInput, @"in (\d+)\s+days?");
                    if (match.Success && int.TryParse(match.Groups[1].Value, out int days))
                    {
                        return now.Date.AddDays(days).AddHours(17);
                    }
                }
                if (lowerInput.Contains("in ") && lowerInput.Contains(" week"))
                {
                    var match = Regex.Match(lowerInput, @"in (\d+)\s+weeks?");
                    if (match.Success && int.TryParse(match.Groups[1].Value, out int weeks))
                    {
                        return now.Date.AddDays(weeks * 7).AddHours(17);
                    }
                }

                // Specific date format: YYYY-MM-DD or MM/DD/YYYY
                var dateMatch = Regex.Match(lowerInput, @"(\d{4}-\d{2}-\d{2})|(\d{1,2}/\d{1,2}/\d{4})");
                if (dateMatch.Success && DateTime.TryParse(dateMatch.Value, out DateTime specificDate))
                {
                    // Try to extract time as well if present
                    var timeMatch = Regex.Match(lowerInput, @"(\d{1,2}:\d{2})\s*(am|pm)?");
                    if (timeMatch.Success && DateTime.TryParse(timeMatch.Value, out DateTime specificTime))
                    {
                        return specificDate.Date.AddHours(specificTime.Hour).AddMinutes(specificTime.Minute);
                    }
                    return specificDate.Date.AddHours(17); // Default time if only date is provided
                }

                // Day of week (e.g., "next monday", "on friday")
                var dayOfWeekMatch = Regex.Match(lowerInput, @"(next\s+)?(monday|tuesday|wednesday|thursday|friday|saturday|sunday)");
                if (dayOfWeekMatch.Success)
                {
                    DayOfWeek targetDay;
                    if (Enum.TryParse(dayOfWeekMatch.Groups[2].Value, true, out targetDay))
                    {
                        DateTime targetDate = now.Date;
                        while (targetDate.DayOfWeek != targetDay)
                        {
                            targetDate = targetDate.AddDays(1);
                        }
                        if (dayOfWeekMatch.Groups[1].Success || targetDate <= now.Date) // If "next" or if today is the target day or has passed
                        {
                            targetDate = targetDate.AddDays(7); // Move to the next instance of that day
                        }
                        // Try to extract time as well if present
                        var timeMatch = Regex.Match(lowerInput, @"(\d{1,2}:\d{2})\s*(am|pm)?");
                        if (timeMatch.Success && DateTime.TryParse(timeMatch.Value, out DateTime specificTime))
                        {
                            return targetDate.Date.AddHours(specificTime.Hour).AddMinutes(specificTime.Minute);
                        }
                        return targetDate.Date.AddHours(17); // Default time
                    }
                }


                return null; // No recognizable date/time found
            }


            // Moved from ChatEngine to NLPProcessor, as it's an NLP concern
            public bool ContainsCybersecurityKeywords(string input)
            {
                var keywords = new[] { "password", "phishing", "malware", "virus", "hack", "security",
                                   "privacy", "data", "breach", "scam", "firewall", "antivirus",
                                   "encryption", "vpn", "wifi", "email", "social engineering",
                                   "ransomware", "spyware", "trojan", "authentication", "backup",
                                   "cybersecurity", "cyber", "online safety", "digital protection" }; // Added more keywords

                // Use Contains for general matching, but ensure it's not part of another word
                // More robust check: use word boundaries for most, but allow partial for some (e.g., "cyber")
                return keywords.Any(keyword => Regex.IsMatch(input, $@"\b{keyword}\b", RegexOptions.IgnoreCase) ||
                                               (keyword == "cybersecurity" && input.Contains(keyword)) ||
                                               (keyword == "cyber" && input.Contains(keyword)));
            }
        }
    }