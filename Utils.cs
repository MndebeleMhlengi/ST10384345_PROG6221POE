using System;
using System.IO;
using System.Media;

namespace CybersecurityChatbot
{
    internal class Utils
    {
        public void PlayGreeting()
        {
            try
            {
                string filepath = @"C:\Users\lab_services_student\Desktop\CyberSecurityBot\Audio\ElevenLabs_Text_to_Speech_audio.wav";

                // Removed Console.WriteLine relevant to console app
                if (!File.Exists(filepath))
                {
                    // Consider logging this to a UI status or message box in a GUI app
                    // For now, keeping as a console output for debugging in a hybrid scenario
                    Console.WriteLine($"❌ Audio file not found at: {filepath}");
                    return;
                }

                using (SoundPlayer player = new SoundPlayer(filepath))
                {
                    player.PlaySync();
                }
                // Removed Console.WriteLine relevant to console app
            }
            catch (Exception ex)
            {
                // Consider logging this to a UI status or message box in a GUI app
                Console.WriteLine($"❌ Error playing audio: {ex.Message}");
            }
        }
    }
}