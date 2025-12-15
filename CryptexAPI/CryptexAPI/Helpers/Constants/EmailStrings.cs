using CryptexAPI.Enums;

namespace CryptexAPI.Helpers.Constants;

public static class EmailStrings
{
    public static string WelcomeSubject => "Welcome to Cryptex!";

    public static string GetWelcomeBody(string name, string surname)
    {
        return $@"
            <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Hello, {name} {surname} 👋</h2>
                    <p>Welcome to <strong>Cryptex</strong> — your new home for secure and efficient cryptocurrency trading.</p>
                    <p>We're excited to have you on board!</p>
                    <ul>
                        <li>Buy, sell, and trade crypto in real-time</li>
                        <li>Access advanced trading tools</li>
                        <li>Track your portfolio with ease</li>
                    </ul>
                    <p>If you have any questions, feel free to contact our support team at any time.</p>
                    <p>Happy trading! 🚀</p>
                    <br />
                    <p>Best regards,<br />Cryptex Team</p>
                </body>
            </html>
        ";
    }

    public static string BalanceSubject => "Your Current Wallet Balance";

    public static string GetBalanceBody(string name, string surname, double? balance)
    {
        return $@"
            <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Hello, {name} {surname} 💰</h2>
                    <p>Here is a quick update on your crypto wallet:</p>
                    <p><strong>Total Balance:</strong> {balance:C}</p>
                    <p>Keep track of your assets and trade with confidence.</p>
                    <p>Log in to your account to view detailed analytics and portfolio history.</p>
                    <br />
                    <p>Stay secure,<br />Cryptex Team</p>
                </body>
            </html>
        ";
    }

    public static string BuyCoinSubject => "Your Crypto Purchase Confirmation";

    public static string GetBuyCoinBody(string name, string surname, NameOfCoin coin, double amount,
        double? remainingBalance)
    {
        return $@"
            <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Hello, {name} {surname} 🪙</h2>
                    <p>Your recent purchase on <strong>Cryptex</strong> was successful!</p>
                    <p><strong>Coin Purchased:</strong> {coin}</p>
                    <p><strong>Amount:</strong> {amount}</p>
                    <p><strong>Remaining Balance:</strong> {remainingBalance:C}</p>
                    <p>Thank you for trading with us. You can always view your transaction history in your dashboard.</p>
                    <br />
                    <p>Happy trading!<br />Cryptex Team</p>
                </body>
            </html>
        ";
    }

    public static string SellCoinSubject => "Your Crypto Sale Confirmation";

    public static string GetSellCoinBody(string name, string surname, NameOfCoin coin, double amount,
        double? remainingBalance)
    {
        return $@"
            <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Hello, {name} {surname} 🪙</h2>
                    <p>Your recent sale on <strong>Cryptex</strong> was successful!</p>
                    <p><strong>Coin Sold:</strong> {coin}</p>
                    <p><strong>Amount:</strong> {amount}</p>
                    <p><strong>Updated Balance:</strong> {remainingBalance:C}</p>
                    <p>Thank you for trading with us. Check your dashboard for full transaction details.</p>
                    <br />
                    <p>Best regards,<br />Cryptex Team</p>
                </body>
            </html>
        ";
    }

    public static string ConvertCurrencySubject => "Your Crypto Conversion Confirmation";

    public static string GetConvertCurrencyBody(
        string name,
        string surname,
        NameOfCoin fromCoin,
        double fromCoinBalance,
        NameOfCoin toCoin,
        double toCoinBalance
    )
    {
        return $@"
            <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Hello, {name} {surname} 🔄</h2>
                    <p>Your recent currency conversion on <strong>Cryptex</strong> was successful!</p>
                    <p><strong>From:</strong> {fromCoin} (Balance: {fromCoinBalance})</p>
                    <p><strong>To:</strong> {toCoin} (Balance: {toCoinBalance})</p>
                    <p>Thank you for using Cryptex for your trading needs!</p>
                    <br />
                    <p>Best regards,<br />Cryptex Team</p>
                </body>
            </html>
        ";
    }

    public static string TicketCreatedSubject => "Your Support Ticket Has Been Created";

    public static string GetTicketCreatedBody(string name, string surname, int ticketId, Status status)
    {
        return $@"
            <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Hello, {name} {surname} 📨</h2>
                    <p>Your support ticket has been successfully created.</p>
                    <p><strong>Ticket ID:</strong> {ticketId}</p>
                    <p><strong>Status:</strong> {status}</p>
                    <p>You can continue to use the platform while waiting for our response.</p>
                    <br />
                    <p>We appreciate your patience.<br />Cryptex Support Team</p>
                </body>
            </html>
        ";
    }
}