namespace karty {
    internal class Program {
        static void Main(string[] args) {
            Random random = new();

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            //0 - ♥
            //1 - 🍀
            //2 - 🌰
            //3 - 🎱

            /*
            ┌───────────┐┐┐┐
            │░░░░░░░░░░░││││
            │░░░░░░░░░░░││││
            │░░░░░░░░░░░││││
            │░░░░░░░░░░░││││
            │░░░░░░░░░░░││││
            │░░░░░░░░░░░││││
            │░░░░░░░░░░░││││
            └───────────┘┘┘┘
            */

            // Welcome screen
            Console.WriteLine(@"Welcome to prsi-in-terminal!

Controls:
    Left / Right arrow - select card
    Down arrow - draw a card
    Enter - discard the selected card


Press any key to continue ...");

            Console.ReadKey(true);
            Console.Clear();

            string[] symbols = { "❤️", "🍀", "🌰", "🎱" };
            int width = 10;   //10,8,4
            int height = 7;   //7 ,5,3
            int playingSpeed = 500; //ms


            int[] cardDefinitions = new int[(4 * 8) * 2]; // [n*2] = cardSymbolId, [n*2+1] = cardNumber

            for (int n = 0; n < cardDefinitions.Length / 2; n++) {
                cardDefinitions[n * 2] = (n / 8);
                cardDefinitions[n * 2 + 1] = (n % 8) + 7;
            }
            int[] drawingStack = Enumerable.Repeat(1, 32).ToArray();
            int[] discardPile = Enumerable.Repeat(0, 32).ToArray();
            int[] player1Hand = Enumerable.Repeat(0, 32).ToArray();
            int[] player2Hand = Enumerable.Repeat(0, 32).ToArray();
            int topmostCardOnDiscardPile;
            int cursorPos = 0;
            int randomCardPos;
            randomCardPos = random.Next(0, drawingStack.Length);
            while (drawingStack[randomCardPos] == 0) { randomCardPos = random.Next(0, drawingStack.Length); }
            drawingStack[randomCardPos] = 0;
            topmostCardOnDiscardPile = randomCardPos;
            discardPile[randomCardPos] = 1;

            DrawCard(drawingStack, player1Hand, 4);
            DrawCard(drawingStack, player2Hand, 4);


            int[] cursorAndCard = new int[2]; // [0] = selected card pos in hand, [1] = placed card (from cardDefinitions)
            int lastCardCount;
            bool canPlayer1Play = true;
            bool canPlayer2Play = true;


            while (CardCountInHand(player1Hand) != 0 && CardCountInHand(player2Hand) != 0) {
                if (CardCountInHand(player1Hand) > 10 || CardCountInHand(player2Hand) > 10) { width = 4; height = 3; } else if (CardCountInHand(player1Hand) > 5 || CardCountInHand(player2Hand) > 5) { width = 8; height = 5; } else { width = 10; height = 7; }



                lastCardCount = CardCountInHand(player2Hand);

                Console.Clear();
                WriteCanvas(topmostCardOnDiscardPile, cardDefinitions, player1Hand, player2Hand, drawingStack, height, width, symbols, cursorPos, discardPile);
                while (CardCountInHand(player2Hand) == lastCardCount && canPlayer1Play) {
                    ShuffleIfNeeded(drawingStack, discardPile, topmostCardOnDiscardPile);
                    lastCardCount = CardCountInHand(player2Hand);
                    cursorAndCard = ReadKey(cursorPos, CardCountInHand(player2Hand), topmostCardOnDiscardPile, cardDefinitions, player2Hand, drawingStack, discardPile);
                    cursorPos = cursorAndCard[0];
                    topmostCardOnDiscardPile = cursorAndCard[1];
                    Console.Clear();
                    WriteCanvas(topmostCardOnDiscardPile, cardDefinitions, player1Hand, player2Hand, drawingStack, height, width, symbols, cursorPos, discardPile);

                }
                ShuffleIfNeeded(drawingStack, discardPile, topmostCardOnDiscardPile);
                if (CardCountInHand(player2Hand) < lastCardCount) {
                    if (topmostCardOnDiscardPile % 8 == 0) { DrawCard(drawingStack, player1Hand, 2); canPlayer2Play = false; } else if (topmostCardOnDiscardPile % 8 == 7) { canPlayer2Play = false; } else if (topmostCardOnDiscardPile % 8 == 5) { topmostCardOnDiscardPile = (UserJokerSelection(symbols)); }
                }
                Console.Clear();
                WriteCanvas(topmostCardOnDiscardPile, cardDefinitions, player1Hand, player2Hand, drawingStack, height, width, symbols, cursorPos, discardPile);
                canPlayer1Play = true;



                if (CardCountInHand(player2Hand) != 0 && canPlayer2Play) {
                    Thread.Sleep(playingSpeed);

                    lastCardCount = CardCountInHand(player1Hand);
                    for (int i = 0; i < CardCountInHand(player1Hand) * 2; i++) {
                        randomCardPos = random.Next(0, CardCountInHand(player1Hand));
                        if (CanDiscardCard(topmostCardOnDiscardPile, WhichCardIsSelected(player1Hand, randomCardPos))) {
                            ShuffleIfNeeded(drawingStack, discardPile, topmostCardOnDiscardPile);
                            topmostCardOnDiscardPile = WhichCardIsSelected(player1Hand, randomCardPos);
                            discardPile[WhichCardIsSelected(player1Hand, randomCardPos)] = 1;
                            player1Hand[WhichCardIsSelected(player1Hand, randomCardPos)] = 0;
                            if (topmostCardOnDiscardPile % 8 == 0) { DrawCard(drawingStack, player2Hand, 2); canPlayer1Play = false; } else if (topmostCardOnDiscardPile % 8 == 7) { canPlayer1Play = false; } else if (topmostCardOnDiscardPile % 8 == 5) { topmostCardOnDiscardPile = PcJokerSelection(player1Hand); }
                            break;
                        }
                    }
                    if (lastCardCount == CardCountInHand(player1Hand)) {
                        ShuffleIfNeeded(drawingStack, discardPile, topmostCardOnDiscardPile);
                        DrawCard(drawingStack, player1Hand, 1);
                    }
                    Console.Clear();
                    WriteCanvas(topmostCardOnDiscardPile, cardDefinitions, player1Hand, player2Hand, drawingStack, height, width, symbols, cursorPos, discardPile);
                }
                canPlayer2Play = true;
            }

            EndingScreen(player2Hand);
        }

        /// <summary>
        /// Automatically prints the game result when no <paramref name="message"/> specified.
        /// Prints the <paramref name="message"/> if specified.
        /// Waits for space to be pressed and ends the program.
        /// </summary>
        static void EndingScreen(int[] player2Hand, string? message = null) {
            Console.WriteLine();
            if (message == null) {
                if (player2Hand.Sum() == 0) {
                    Console.WriteLine("You win!");
                } else {
                    Console.WriteLine("You lose!");
                }
            } else {
                Console.WriteLine(message);
            }
            Console.WriteLine("\nPress SPACE to exit");
            while (Console.ReadKey(true).Key != ConsoleKey.Spacebar) { }
            Environment.Exit(0);
        }

        static string CreateCard(int cardSymbolId, int cardNumber, string[] cardSymbols, int width, int height, bool selectedCard) {
            Dictionary<int, string> cardNumberSymbolDict = new() {
                {10, "X" },
                {11, "J" },
                {12, "Q" },
                {13, "K" },
                {14, "A" }
            };
            string cardSymbol = cardNumberSymbolDict.TryGetValue(cardNumber, out var symbol) ? symbol : cardNumber.ToString();


            string card = "";
            if (selectedCard) {
                card += ("▄");
                for (int x = 0; x < width; x++) { card += ("▄"); }
                card += ("▄" + "\n");


                card += ("█ " + cardSymbol);
                for (int x = 0; x < width - 2; x++) {
                    card += (" ");
                }
                card += ("█" + "\n");


                for (int y = 0; y < height - 2; y++) {
                    card += ("█");
                    for (int x = 0; x < width; x++) {
                        if ((height - 2) / 2 == y && (width - 2) / 2 == x) { card += cardSymbols[cardSymbolId]; x += 2; }
                        card += (" ");
                    }
                    card += ("█" + "\n");
                }
                card += ("█");



                for (int x = 0; x < width - 2; x++) {
                    card += (" ");
                }
                card += (cardSymbol + " █" + "\n");



                card += ("▀");
                for (int x = 0; x < width; x++) { card += ("▀"); }
                card += ("▀");
            } else {
                card += ("╔");
                for (int x = 0; x < width; x++) { card += ("═"); }
                card += ("╗" + "\n");


                card += ("║ " + cardSymbol);
                for (int x = 0; x < width - 2; x++) {
                    card += (" ");
                }
                card += ("║" + "\n");


                for (int y = 0; y < height - 2; y++) {
                    card += ("║");
                    for (int x = 0; x < width; x++) {
                        if ((height - 2) / 2 == y && (width - 2) / 2 == x) { card += cardSymbols[cardSymbolId]; x += 2; }
                        card += (" ");
                    }
                    card += ("║" + "\n");
                }
                card += ("║");



                for (int x = 0; x < width - 2; x++) {
                    card += (" ");
                }
                card += (cardSymbol + " ║" + "\n");



                card += ("╚");
                for (int x = 0; x < width; x++) { card += ("═"); }
                card += ("╝");
            }

            return (card);

        }
        static int CardCountInHand(int[] cardsInHand) {
            return cardsInHand.Count(i => i == 1);
        }

        static string JoinMultilineStringsHorizontally(string firstString, string secondString) {

            string[] partsA = firstString.Split('\n');
            string[] partsB = secondString.Split('\n');
            string joined = "";
            for (int n = 0; n < partsA.Length; n++) {
                joined += partsA[n] + partsB[n] + "\n";
            }

            return joined.Substring(0, joined.Length - 1);
        }

        static void DrawCard(int[] drawingStack, int[] playerCards, int howManyToDraw) {
            Random random = new();
            int randomCard = random.Next(0, drawingStack.Length);
            for (int n = 0; n < howManyToDraw && drawingStack.Sum() != 0; n++) {
                while (drawingStack[randomCard] == 0) { randomCard = random.Next(0, drawingStack.Length); }
                drawingStack[randomCard] = 0;
                playerCards[randomCard] = 1;
            }
        }

        static void ShuffleIfNeeded(int[] drawingStack, int[] discardPile, int topmostCardOnDiscardPile) {
            if (drawingStack.Sum() == 0) {
                Array.Copy(discardPile, drawingStack, discardPile.Length);
                Array.Fill(discardPile, 0);
                discardPile[topmostCardOnDiscardPile] = 1;
                drawingStack[topmostCardOnDiscardPile] = 0;
            }
        }

        static void WriteCanvas(int topmostCardOnDiscardPile, int[] cardDefinitions, int[] player1Hand, int[] player2Hand, int[] drawingStack, int height, int width, string[] symbols, int cursorPos, int[] discardPile) {
            string singleCard;
            string cardsInHandString = "";
            for (int n = 0; n <= height; n++) {
                cardsInHandString += "\n";
            }
            for (int n = 0; n < player1Hand.Length; n++) {
                if (player1Hand[n] == 1) {
                    //singleCard = CreateCard(cardDefinitions[n * 2], cardDefinitions[n * 2 + 1], symbols, width, height, false);
                    singleCard = CreateCardFromBehind(height, width);
                    cardsInHandString = JoinMultilineStringsHorizontally(cardsInHandString, singleCard);
                }
            }
            Console.WriteLine(cardsInHandString);


            string stackRightBorder = "┐\n│\n│\n│\n│\n│\n│\n│\n┘";
            string stackString = CreateCardFromBehind(7, 10);

            int cardStackHeightRatio = 4;

            for (int n = 0; n < CardCountInHand(drawingStack) / cardStackHeightRatio; n++) { stackString = JoinMultilineStringsHorizontally(stackString, stackRightBorder); }
            stackString = JoinMultilineStringsHorizontally(stackString, "  \n  \n  \n  \n  \n  \n  \n  \n  ");
            string discardPileString = CreateCard(cardDefinitions[topmostCardOnDiscardPile * 2], cardDefinitions[topmostCardOnDiscardPile * 2 + 1], symbols, 10, stackRightBorder.Length / 2 - 1, false);
            for (int n = 0; n < CardCountInHand(discardPile) / cardStackHeightRatio - 1; n++) { discardPileString = JoinMultilineStringsHorizontally(discardPileString, stackRightBorder); }
            Console.WriteLine(JoinMultilineStringsHorizontally(stackString, discardPileString));


            cardsInHandString = "";
            for (int n = 0; cardsInHandString.Length <= height; n++) {
                cardsInHandString += "\n";
            }
            int ownedCardIndex = 0;
            for (int n = 0; n < player2Hand.Length; n++) {
                if (player2Hand[n] == 1) {
                    cardsInHandString = JoinMultilineStringsHorizontally(cardsInHandString, CreateCard(cardDefinitions[n * 2], cardDefinitions[n * 2 + 1], symbols, width, height, cursorPos == ownedCardIndex));
                    ownedCardIndex++;
                }
            }
            Console.WriteLine(cardsInHandString);

        }

        static int[] ReadKey(int pozicekurzoru, int pocetKvruce, int kartanastole, int[] balicek_karet, int[] kartyhrac, int[] lizaci_balicek, int[] odhazovaci_balicek) {
            ConsoleKey sipka = Console.ReadKey().Key;
            while (!((sipka == ConsoleKey.RightArrow && pozicekurzoru != pocetKvruce - 1) || (sipka == ConsoleKey.LeftArrow && pozicekurzoru != 0) || sipka == ConsoleKey.Enter || sipka == ConsoleKey.DownArrow)) {
                Console.Write("\b \b");
                sipka = Console.ReadKey().Key;

            }
            if (sipka == ConsoleKey.LeftArrow) { return new int[] { pozicekurzoru - 1, kartanastole }; } else if (sipka == ConsoleKey.RightArrow) { return new int[] { pozicekurzoru + 1, kartanastole }; } else if (sipka == ConsoleKey.Enter) {
                if (CanDiscardCard(kartanastole, WhichCardIsSelected(kartyhrac, pozicekurzoru))) {
                    kartanastole = WhichCardIsSelected(kartyhrac, pozicekurzoru);
                    odhazovaci_balicek[(WhichCardIsSelected(kartyhrac, pozicekurzoru))] = 1;
                    kartyhrac[WhichCardIsSelected(kartyhrac, pozicekurzoru)] = 0;
                    if (pozicekurzoru != 0) {
                        pozicekurzoru--;
                    }
                }
                return new int[] { pozicekurzoru, kartanastole };
            } else if (sipka == ConsoleKey.DownArrow) {
                ShuffleIfNeeded(lizaci_balicek, odhazovaci_balicek, kartanastole);
                DrawCard(lizaci_balicek, kartyhrac, 1);
                return new int[] { pozicekurzoru, kartanastole };
            } else { return new int[] { pozicekurzoru, kartanastole }; }

        }

        static bool CanDiscardCard(int topmostCardOnDiscardPile, int cardToTryToPlace) {
            if (cardToTryToPlace % 8 == 5) { return true; }
            if (cardToTryToPlace % 8 == topmostCardOnDiscardPile % 8) { return true; }
            if (cardToTryToPlace / 8 == topmostCardOnDiscardPile / 8) { return true; } else { return false; }

        }

        static int WhichCardIsSelected(int[] playersCards, int cursorPosition) {
            int result = 0;
            cursorPosition++;
            for (int n = 0; cursorPosition != 0; n++) {
                result = n;
                if (playersCards[n] == 1) { cursorPosition--; }
            }
            return result;
        }

        /// <summary>
        /// Joker / converter - the card that changes the card "color"
        /// </summary>
        static int UserJokerSelection(string[] znaky) {
            int jokerHeight = 3;
            int jokerWidth = 4;
            int cursorPos = 0;
            string jokerCardsString = "";
            for (int n = 0; n < jokerHeight; n++) {
                Console.WriteLine("\n");
            }
            ConsoleKey arrow = new();
            while (arrow != ConsoleKey.Enter) {
                for (int n = 0; jokerCardsString.Length <= jokerHeight; n++) {
                    jokerCardsString += "\n";
                }
                for (int n = 0; n < znaky.Length; n++) {
                    jokerCardsString = JoinMultilineStringsHorizontally(jokerCardsString, CreateCard(n, 5 + 7, znaky, jokerWidth, jokerHeight, n == cursorPos));
                }
                Console.CursorTop -= jokerWidth + 1;
                Console.CursorLeft = 0;
                Console.WriteLine(jokerCardsString);
                jokerCardsString = "";
                arrow = Console.ReadKey().Key;
                while (!((arrow == ConsoleKey.RightArrow && cursorPos != 4 - 1) || (arrow == ConsoleKey.LeftArrow && cursorPos != 0) || arrow == ConsoleKey.Enter)) {
                    Console.Write("\b \b");
                    arrow = Console.ReadKey().Key;

                }
                if (arrow == ConsoleKey.LeftArrow) { cursorPos--; } else if (arrow == ConsoleKey.RightArrow) { cursorPos++; }
            }


            return (cursorPos * 8 + 5);
        }

        static int PcJokerSelection(int[] cardsInHand) {
            int biggest = 0;
            int whichColorHasTheMostCards = 0;
            int howManyCardsOfSpecificColor = 0;
            for (int symbol = 0; symbol < 4; symbol++) {
                for (int i = 0; i < 8; i++) {
                    if (cardsInHand[symbol * 8 + i] == 1 && i != 5) { howManyCardsOfSpecificColor++; }
                }
                if (whichColorHasTheMostCards < howManyCardsOfSpecificColor) {
                    whichColorHasTheMostCards = howManyCardsOfSpecificColor;
                    biggest = symbol;
                }

            }
            return (biggest * 8 + 5);
        }

        static string CreateCardFromBehind(int height, int width) {
            string cardString = "";
            cardString += "┌";
            for (int i = 0; i < width; i++) {
                cardString += "─";
            }
            cardString += "┐\n";

            for (int i = 0; i < height; i++) {
                cardString += "│";
                for (int n = 0; n < width; n++) {
                    cardString += "╳";
                }
                cardString += "│\n";
            }
            cardString += "└";
            for (int i = 0; i < width; i++) {
                cardString += "─";
            }
            cardString += "┘";

            return (cardString);
        }
    }
}