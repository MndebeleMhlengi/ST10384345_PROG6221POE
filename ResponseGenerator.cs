// ResponseGenerator.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CybersecurityChatbot
{
    // Generates responses to cybersecurity questions and provides general help messages.
    public class ResponseGenerator
    {
        private Random rand = new Random(); // Use a single Random instance for better distribution
        private Action<string> addSystemMessageCallback; // Delegate to send messages back to ChatEngine/ChatHistoryManager

        // Constructor requires a callback to report messages back to the chat history.
        public ResponseGenerator(Action<string> addSystemMessageCallback)
        {
            this.addSystemMessageCallback = addSystemMessageCallback ?? throw new ArgumentNullException(nameof(addSystemMessageCallback));
        }

        // Determines if the input string contains keywords related to cybersecurity topics.
        public bool IsCybersecurityQuestion(string input)
        {
            string lowerInput = input.ToLower();
            // Added "scam" to the keywords array
            string[] keywords = { "phishing", "malware", "password", "mfa", "antivirus", "brute-force", "encryption", "firewall", "vpn", "cybersecurity", "trojan", "worm", "ransomware", "spyware", "privacy", "social engineering", "backup", "scam" };
            return keywords.Any(k => lowerInput.Contains(k));
        }

        // Provides a varied response to a cybersecurity-related question based on keywords.
        public string GetCybersecurityResponse(string input)
        {
            input = input.ToLower();

            if (input.Contains("phishing"))
            {
                string[] responses = {
                    "Phishing is a deceptive cyberattack where criminals impersonate trustworthy entities (like banks or popular websites) to trick you into revealing sensitive information, such as login credentials or credit card numbers. Always double-check the sender and links!",
                    "Beware of phishing scams! These involve fake emails, messages, or websites designed to steal your personal data. They often create a sense of urgency or fear to make you act quickly.",
                    "To protect yourself from phishing, always verify the sender's identity through official channels, hover over links to see their true destination before clicking, and never give out personal info in response to unsolicited requests."
                };
                return responses[rand.Next(responses.Length)];
            }
            else if (input.Contains("malware"))
            {
                string[] responses = {
                    "Malware is a broad term for malicious software, including viruses, worms, Trojans, ransomware, and spyware, designed to harm or gain unauthorized access to computer systems. Regular antivirus scans and cautious downloading are key to prevention.",
                    "Different types of malware operate in unique ways: Viruses attach to legitimate programs, worms self-replicate across networks, and ransomware encrypts your files until a ransom is paid. Understanding these helps in defense.",
                    "Protecting against malware requires robust antivirus software, keeping your operating system and applications updated, and being wary of suspicious attachments or untrusted websites."
                };
                return responses[rand.Next(responses.Length)];
            }
            else if (input.Contains("password"))
            {
                string[] responses = {
                    "A strong password is your first line of defense. It should be long (at least 12 characters), unique for each account, and include a mix of uppercase letters, lowercase letters, numbers, and symbols. Consider using a password manager!",
                    "Never reuse passwords across different online services. If one service is breached, all your accounts using that same password become vulnerable. Multi-factor authentication adds an essential second layer of security.",
                    "Instead of a complex password, try a passphrase – a string of several random, unrelated words. They are often easier to remember but much harder to guess than traditional passwords."
                };
                return responses[rand.Next(responses.Length)];
            }
            else if (input.Contains("mfa") || input.Contains("multi-factor authentication"))
            {
                string[] responses = {
                    "MFA (Multi-Factor Authentication) significantly boosts your account security by requiring two or more different forms of verification before granting access. This typically includes something you know (like a password) and something you have (like a phone or security key).",
                    "Implementing MFA is one of the most effective ways to prevent unauthorized access to your accounts. Even if a criminal steals your password, they won't be able to log in without the second factor.",
                    "Common MFA methods include SMS codes, authenticator apps (like Google Authenticator), biometric scans (fingerprint, face ID), and physical security keys. Use it whenever available!"
                };
                return responses[rand.Next(responses.Length)];
            }
            else if (input.Contains("antivirus"))
            {
                string[] responses = {
                    "Antivirus software is essential for detecting, preventing, and removing malicious software from your computer. It constantly scans files and activity for known threats and can often identify new ones through behavioral analysis.",
                    "To maximize the effectiveness of your antivirus, ensure it's always running in the background and its virus definitions are updated regularly. New threats emerge daily!",
                    "While antivirus is critical, it's not a complete solution. Combine it with good Browse habits, firewalls, and regular software updates for comprehensive protection."
                };
                return responses[rand.Next(responses.Length)];
            }
            else if (input.Contains("encryption"))
            {
                string[] responses = {
                    "Encryption transforms data into a secret code to protect it from unauthorized access. Only those with the correct key can decrypt and read the information. It's fundamental for privacy and security online.",
                    "You encounter encryption daily when you browse secure websites (HTTPS), send encrypted messages, or use VPNs. It's crucial for protecting sensitive data both in transit and at rest on your devices.",
                    "Data encryption ensures confidentiality. If encrypted data falls into the wrong hands, it appears as gibberish, rendering it useless to attackers without the decryption key."
                };
                return responses[rand.Next(responses.Length)];
            }
            else if (input.Contains("firewall"))
            {
                string[] responses = {
                    "A firewall acts as a digital barrier between your computer/network and the internet. It monitors incoming and outgoing traffic and blocks anything suspicious based on predefined security rules. Think of it as a security guard for your network.",
                    "Both hardware firewalls (like those in your router) and software firewalls (like Windows Defender Firewall) are important. They work together to control what enters and leaves your network, protecting against unauthorized access.",
                    "Properly configured firewalls can prevent many types of cyberattacks by blocking malicious connections and preventing unauthorized programs from accessing the internet."
                };
                return responses[rand.Next(responses.Length)];
            }
            else if (input.Contains("vpn"))
            {
                string[] responses = {
                    "A VPN (Virtual Private Network) creates a secure, encrypted tunnel over a public network, like the internet. It masks your IP address and encrypts your data, enhancing your online privacy and security.",
                    "Using a VPN is highly recommended, especially when connecting to public Wi-Fi networks, as it protects your data from snoopers and potential eavesdropping. It also allows you to bypass geo-restrictions.",
                    "When you connect to a VPN, your internet traffic is routed through a remote server operated by the VPN provider. This makes it appear as though you're Browse from the VPN server's location, adding an extra layer of anonymity."
                };
                return responses[rand.Next(responses.Length)];
            }
            else if (input.Contains("trojan"))
            {
                return "A Trojan horse is a type of malware that disguises itself as legitimate software. Unlike viruses, it doesn't self-replicate but relies on users unknowingly installing it. Once inside, it can perform various malicious activities.";
            }
            else if (input.Contains("worm"))
            {
                return "A computer worm is a standalone malware computer program that replicates itself to spread to other computers. Unlike a virus, it does not need to attach itself to an existing program and can spread quickly over networks.";
            }
            else if (input.Contains("ransomware"))
            {
                return "Ransomware is a type of malicious software that blocks access to a computer system or encrypts data until a sum of money (ransom) is paid. Always back up your data regularly to protect against ransomware attacks!";
            }
            else if (input.Contains("spyware"))
            {
                return "Spyware is malicious software designed to secretly observe and collect information about users, such as their Browse habits, personal data, or keystrokes, without their knowledge or consent.";
            }
            else if (input.Contains("privacy"))
            {
                string[] responses = {
                    "Online privacy is about controlling your personal information on the internet. This includes what data companies collect about you, how it's used, and who can access it. Reviewing privacy settings on social media and apps is a good start.",
                    "To enhance your digital privacy, consider using privacy-focused browsers or search engines, disabling location tracking when not needed, and being mindful of the permissions you grant to mobile apps. VPNs can also help encrypt your traffic.",
                    "Understanding privacy policies can be complex, but it's important to know what you're agreeing to when using new services. Limiting the personal information you share publicly online also contributes greatly to your privacy."
                };
                return responses[rand.Next(responses.Length)];
            }
            else if (input.Contains("social engineering"))
            {
                string[] responses = {
                    "Social engineering is the psychological manipulation of people into performing actions or divulging confidential information. Attackers exploit human psychology, rather than technical vulnerabilities.",
                    "Common social engineering tactics include pretexting (creating a fake scenario), baiting (offering something tempting), and phishing. Always be skeptical of urgent or unusual requests, even if they seem to come from a trusted source.",
                    "The best defense against social engineering is awareness and skepticism. Verify requests through official channels, don't feel pressured to act immediately, and remember that it's okay to question anything that seems suspicious."
                };
                return responses[rand.Next(responses.Length)];
            }
            else if (input.Contains("backup"))
            {
                string[] responses = {
                    "Regular data backups are crucial! They are your safety net against data loss due to hardware failure, cyberattacks (like ransomware), or accidental deletion. Follow the 3-2-1 rule: 3 copies, 2 different media types, 1 offsite.",
                    "Cloud services (like Google Drive, OneDrive) or external hard drives are popular options for backups. Ensure your sensitive backups are encrypted, especially if stored offsite or in the cloud.",
                    "Test your backups periodically to ensure they can actually be restored when needed. An untested backup is not a backup! Offline or 'air-gapped' backups provide the best protection against ransomware."
                };
                return responses[rand.Next(responses.Length)];
            }
            else if (input.Contains("scam")) 
            {
                string[] responses = {
                    "Scams are deceptive schemes used to trick people into giving away money or personal information. They come in many forms, from fake lottery wins to urgent requests for help from 'friends' in trouble. Always be suspicious of unsolicited offers or demands for personal data.",
                    "Common signs of a scam include unexpected requests for money, pressure to act quickly, requests for personal information, unrealistic promises (like huge returns on investment), and poorly written messages. If it sounds too good to be true, it probably is!",
                    "To avoid scams, never share personal or financial information with unverified sources. If you receive a suspicious call or message, verify the sender through official contact methods, not those provided in the suspicious communication. Report scams to the authorities."
                };
                return responses[rand.Next(responses.Length)];
            }
            else if (input.Contains("cybersecurity"))
            {
                string[] responses = {
                    "Cybersecurity is the practice of protecting systems, networks, and programs from digital attacks. These cyberattacks are usually aimed at accessing, changing, or destroying sensitive information; extorting money from users; or interrupting normal business processes. It's a vast and crucial field in our digital age.",
                    "At its core, cybersecurity is about safeguarding your digital life. It involves understanding threats, implementing protective measures, and reacting effectively to incidents.",
                    "The importance of cybersecurity cannot be overstated. With our increasing reliance on digital systems, protecting data and infrastructure from cyber threats is vital for individuals, businesses, and governments alike."
                };
                return responses[rand.Next(responses.Length)];
            }
            else
            {
                return "That's an interesting cybersecurity topic! Could you please be more specific, or ask about common threats like phishing, malware, or the importance of strong passwords?";
            }
        }

        // Provides general help information about chatbot capabilities.
        public string GetHelpMessage()
        {
            return "I can help with the following:\n" +
                   " • Ask about cybersecurity topics (e.g., 'What is phishing?', 'Tell me about passwords', 'What is a scam?')\n" + // Updated help message
                   " • Manage your cybersecurity tasks ('add task', 'view tasks', 'complete task 1')\n" +
                   " • Take a cybersecurity quiz ('start quiz')\n" +
                   " • View your activity log ('show activity log')\n" +
                   " • Type 'hello' for a general greeting.\n" +
                   " • Type 'exit' to quit.";
        }
    }
}