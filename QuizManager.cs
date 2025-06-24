// QuizManager.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CybersecurityChatbot
{
    // Manages the quiz functionality, including questions, answers, and scoring.
    public class QuizManager
    {
        private List<QuizQuestion> quizQuestions;
        private int currentQuestionIndex = 0;
        private int correctAnswers = 0;
        private bool inQuiz = false;
        private Action<string> addSystemMessageCallback; // Delegate to send messages back to ChatEngine/ChatHistoryManager

        // Nested helper class for QuizQuestion. Made public so its properties are accessible if needed.
        public class QuizQuestion
        {
            public string Question { get; set; }
            public List<string> Options { get; set; } // For multiple choice, will be ["True", "False"] for T/F
            public int CorrectAnswerIndex { get; set; } // 0-indexed for options, 0 for True, 1 for False
            public string Explanation { get; set; }
            public bool IsTrueFalse { get; set; } = false; // New property to indicate T/F question

            // Formats the question and options for display.
            public string GetFormattedQuestion()
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(Question);
                if (IsTrueFalse)
                {
                    sb.AppendLine("True or False?");
                }
                else
                {
                    for (int i = 0; i < Options.Count; i++)
                    {
                        sb.AppendLine($"{(char)('A' + i)}. {Options[i]}");
                    }
                }
                return sb.ToString();
            }
        }

        // Constructor requires a callback to report messages back to the chat history.
        public QuizManager(Action<string> addSystemMessageCallback)
        {
            this.addSystemMessageCallback = addSystemMessageCallback ?? throw new ArgumentNullException(nameof(addSystemMessageCallback));
            InitializeQuizQuestions();
        }

        // Initializes the set of quiz questions.
        private void InitializeQuizQuestions()
        {
            quizQuestions = new List<QuizQuestion>
            {
                // Existing Multiple Choice Questions
                new QuizQuestion
                {
                    Question = "What is phishing?",
                    Options = new List<string> { "A type of malware", "A technique to trick users into revealing info", "A strong password", "A network security protocol" },
                    CorrectAnswerIndex = 1,
                    Explanation = "Phishing is a fraudulent attempt to obtain sensitive information by disguising as a trustworthy entity through deceptive communications."
                },
                new QuizQuestion
                {
                    Question = "Which of the following is an example of a strong password?",
                    Options = new List<string> { "password123", "12345678", "P@ssw0rd!", "MyName1" },
                    CorrectAnswerIndex = 2,
                    Explanation = "A strong password includes a mix of uppercase, lowercase, numbers, and symbols, and is not easily guessable. 'P@ssw0rd!' fits these criteria."
                },
                new QuizQuestion
                {
                    Question = "What does MFA stand for in cybersecurity?",
                    Options = new List<string> { "Malicious File Analyzer", "Multi-Factor Authentication", "Managed Firewall Access", "Mobile Fraud Alert" },
                    CorrectAnswerIndex = 1,
                    Explanation = "MFA, or Multi-Factor Authentication, requires more than one method of verification to grant access, significantly increasing security."
                },
                new QuizQuestion
                {
                    Question = "What is the primary purpose of antivirus software?",
                    Options = new List<string> { "To speed up your computer", "To protect against malicious software", "To manage network connections", "To encrypt data" },
                    CorrectAnswerIndex = 1,
                    Explanation = "Antivirus software is primarily designed to detect, prevent, and remove malicious software like viruses, worms, and Trojans."
                },
                new QuizQuestion
                {
                    Question = "Which common threat involves attackers trying to guess your password repeatedly?",
                    Options = new List<string> { "Phishing", "Malware", "Brute-force attack", "DDoS attack" },
                    CorrectAnswerIndex = 2,
                    Explanation = "A brute-force attack is a trial-and-error method used to guess login information by systematically trying many combinations until the correct one is found."
                },
                new QuizQuestion
                {
                    Question = "What is encryption?",
                    Options = new List<string> { "A method to speed up internet", "Converting data to a secret code", "Blocking unwanted emails", "Scanning for viruses" },
                    CorrectAnswerIndex = 1,
                    Explanation = "Encryption is the process of transforming information into a secret code (ciphertext) to protect it from unauthorized access."
                },
                new QuizQuestion
                {
                    Question = "What is a firewall used for?",
                    Options = new List<string> { "To block pop-up ads", "To manage internet speed", "To monitor and control network traffic", "To backup data" },
                    CorrectAnswerIndex = 2,
                    Explanation = "A firewall is a network security system that monitors and controls incoming and outgoing network traffic based on predetermined security rules."
                },

                // New True/False Questions
                new QuizQuestion
                {
                    Question = "It is safe to click on any link in an email, as long as it looks legitimate.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswerIndex = 1, // False
                    Explanation = "It's crucial to be cautious with email links. Phishing emails often look legitimate but contain malicious links. Always verify the sender and hover over links to see their true destination before clicking."
                    , IsTrueFalse = true
                },
                new QuizQuestion
                {
                    Question = "Using the same strong password for all your online accounts is a good security practice.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswerIndex = 1, // False
                    Explanation = "Reusing passwords, even strong ones, is a major security risk. If one account is compromised, all other accounts using that same password become vulnerable. Use unique, strong passwords for each service, ideally with a password manager."
                    , IsTrueFalse = true
                },
                new QuizQuestion
                {
                    Question = "Public Wi-Fi networks are always secure for sensitive activities like online banking.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswerIndex = 1, // False
                    Explanation = "Public Wi-Fi networks are generally unencrypted and insecure, making your data vulnerable to interception by malicious actors. Avoid sensitive activities like banking or online shopping on public Wi-Fi. Use a VPN for added security if you must."
                    , IsTrueFalse = true
                },
                 new QuizQuestion
                {
                    Question = "Updating your software and operating system regularly helps protect against security vulnerabilities.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswerIndex = 0, // True
                    Explanation = "Software updates often include patches for newly discovered security vulnerabilities. Keeping your systems updated is a fundamental cybersecurity practice to stay protected against known threats.",
                    IsTrueFalse = true
                },
                new QuizQuestion
                {
                    Question = "Ransomware encrypts your files and demands payment to decrypt them.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswerIndex = 0, // True
                    Explanation = "Ransomware is a type of malicious software that encrypts a victim's files and demands a ransom payment (usually in cryptocurrency) for the decryption key.",
                    IsTrueFalse = true
                }
            };
            ShuffleQuestions(); // Shuffle questions when initialized
        }

        // Shuffles the quiz questions randomly.
        private void ShuffleQuestions()
        {
            Random rng = new Random();
            int n = quizQuestions.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                QuizQuestion value = quizQuestions[k];
                quizQuestions[k] = quizQuestions[n];
                quizQuestions[n] = value;
            }
        }

        // Starts a new quiz session, resetting state and shuffling questions.
        public void StartNewQuiz()
        {
            if (inQuiz)
            {
                addSystemMessageCallback("A quiz is already in progress. Please answer the current question or clear the chat to start a new one.");
                return;
            }

            inQuiz = true;
            currentQuestionIndex = 0;
            correctAnswers = 0;
            ShuffleQuestions(); // Shuffle again for a fresh quiz experience
            addSystemMessageCallback("Quiz started! Choose the correct option (A, B, C, D for multiple choice, or True/False).");
            PresentQuizQuestion(); // Display the first question
        }

        // Presents the current quiz question to the user via the system message callback.
        private void PresentQuizQuestion()
        {
            if (currentQuestionIndex < quizQuestions.Count)
            {
                QuizQuestion q = quizQuestions[currentQuestionIndex];
                addSystemMessageCallback(q.GetFormattedQuestion());
            }
            else
            {
                EndQuiz(); // All questions have been presented
            }
        }

        // Processes the user's answer to the current quiz question.
        public void ProcessQuizAnswer(string answer)
        {
            if (!inQuiz || currentQuestionIndex >= quizQuestions.Count)
            {
                addSystemMessageCallback("The quiz is not active or has ended. Please start a new one if you wish to play again.");
                inQuiz = false; // Ensure state is reset
                return;
            }

            QuizQuestion q = quizQuestions[currentQuestionIndex];
            int userAnswerIndex = -1;
            string upperAnswer = answer.Trim().ToUpper();

            if (q.IsTrueFalse)
            {
                // Handle True/False questions
                if (upperAnswer == "TRUE")
                {
                    userAnswerIndex = 0; // Assuming "True" is the first option
                }
                else if (upperAnswer == "FALSE")
                {
                    userAnswerIndex = 1; // Assuming "False" is the second option
                }
            }
            else
            {
                // Handle Multiple Choice questions (A, B, C, D or 1, 2, 3, 4)
                // Attempt to parse answer as A, B, C, D
                if (upperAnswer.Length == 1 && upperAnswer[0] >= 'A' && upperAnswer[0] < 'A' + q.Options.Count)
                {
                    userAnswerIndex = upperAnswer[0] - 'A';
                }
                // Attempt to parse answer as 1, 2, 3, 4
                else if (int.TryParse(upperAnswer, out int num) && num >= 1 && num <= q.Options.Count)
                {
                    userAnswerIndex = num - 1;
                }
            }


            if (userAnswerIndex == q.CorrectAnswerIndex)
            {
                addSystemMessageCallback("Correct! 🎉");
                correctAnswers++;
            }
            else if (userAnswerIndex != -1) // Valid input but incorrect answer
            {
                string correctAnswerText = q.IsTrueFalse ? q.Options[q.CorrectAnswerIndex] : $"{(char)('A' + q.CorrectAnswerIndex)}. {q.Options[q.CorrectAnswerIndex]}";
                addSystemMessageCallback($"Incorrect. The correct answer was {correctAnswerText}");
            }
            else // Invalid input format
            {
                string errorMessage = q.IsTrueFalse ? "Invalid answer. Please type 'True' or 'False'." : "Invalid answer. Please choose A, B, C, or D (or 1, 2, 3, 4).";
                addSystemMessageCallback(errorMessage);
                return; // Do not advance question on invalid input
            }

            addSystemMessageCallback($"Explanation: {q.Explanation}");
            currentQuestionIndex++;
            PresentQuizQuestion(); // Move to the next question or end the quiz
        }

        // Ends the current quiz session and reports the final score.
        private void EndQuiz()
        {
            inQuiz = false;
            // Handle division by zero if quizQuestions.Count is 0, though it shouldn't be with initialized questions
            int scorePercentage = (quizQuestions.Count > 0) ? (int)((double)correctAnswers / quizQuestions.Count * 100) : 0;
            addSystemMessageCallback($"Quiz complete! You scored {scorePercentage}% ({correctAnswers} out of {quizQuestions.Count} correct).");
            // Optionally, log this score or save it to a user profile
        }

        // Gets the score of the last completed quiz. Returns -1 if no quiz is completed or is in progress.
        public int GetQuizScore()
        {
            if (inQuiz) return -1; // Quiz is still in progress
            if (quizQuestions == null || quizQuestions.Count == 0) return 0; // No questions means 0 score

            return (int)((double)correctAnswers / quizQuestions.Count * 100);
        }

        // Checks if a quiz is currently active.
        public bool IsQuizActive()
        {
            return inQuiz;
        }

        // Resets the quiz state to idle, allowing a new quiz to be started.
        public void ResetQuizState()
        {
            inQuiz = false;
            currentQuestionIndex = 0;
            correctAnswers = 0;
            // Questions remain shuffled until a new quiz starts
        }
    }
}