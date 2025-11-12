using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Linq;

namespace FireFox
{
    // --- 1. CLASS FORM CHÍNH (BẮT BUỘC PHẢI ĐỂ ĐẦU TIÊN) ---
    public partial class Form3 : Form
    {
        int totalPlayers;
        List<Player> players = new List<Player>();
        List<Square> boardData = new List<Square>();
        List<Card> chanceCards = new List<Card>();
        List<Card> communityCards = new List<Card>();

        int currentPlayerIndex = 0;
        bool isProcessingTurn = false;
        int stepX, stepY;
        Label lblInfoPanel;

        public Form3(int soNguoi)
        {
            InitializeComponent();
            this.totalPlayers = soNguoi;

            // Nạp ảnh bàn cờ an toàn
            Image boardImg = GetImageFromResource("banco"); // Đổi tên 'banco' nếu ảnh của bạn tên khác
            if (boardImg != null)
            {
                this.BackgroundImage = boardImg;
                this.BackgroundImageLayout = ImageLayout.Stretch;
            }

            stepX = this.ClientSize.Width / 13;
            stepY = this.ClientSize.Height / 13;

            CreateInfoPanel();
            InitBoardData();
            InitCards();
            SetupGame();

            if (pbDice != null) pbDice.Image = GetImageFromResource("dice1");
        }

        private void btnRoll_Click(object sender, EventArgs e)
        {
            if (isProcessingTurn) return;
            isProcessingTurn = true;

            if (players.Count == 0) return;

            Player current = players[currentPlayerIndex];
            Random rnd = new Random();
            int dice = rnd.Next(1, 7);

            if (pbDice != null) pbDice.Image = GetImageFromResource("dice" + dice);
            AddLog($"{current.Name} gieo được {dice} điểm.");

            current.Position += dice;
            if (current.Position >= 40)
            {
                current.Position %= 40;
                current.Money += 200;
                AddLog("Qua vòng Start +200$");
            }
            MovePlayerToken(current);

            HandleSquareLogic(current);
            UpdateInfoPanel();

            // Kiểm tra phá sản
            if (CheckBankruptcy(current))
            {
                // Nếu phá sản, không tăng index vì danh sách đã thụt lại
            }
            else
            {
                currentPlayerIndex++;
            }

            if (currentPlayerIndex >= players.Count) currentPlayerIndex = 0;

            if (players.Count == 1)
            {
                MessageBox.Show($"CHÚC MỪNG {players[0].Name} ĐÃ VÔ ĐỊCH!", "GAME OVER");
                this.Close();
            }
            else
            {
                UpdateTurnUI();
            }

            isProcessingTurn = false;
        }

        private bool CheckBankruptcy(Player p)
        {
            if (p.Money < 0)
            {
                MessageBox.Show($"{p.Name} đã phá sản!", "Phá sản");
                AddLog($"❌ {p.Name} PHÁ SẢN!");

                this.Controls.Remove(p.Token);
                p.Token.Dispose();

                foreach (var sq in p.OwnedProperties)
                {
                    sq.Owner = null;
                    sq.HouseLevel = 0;
                    if (sq.HouseVisual != null)
                    {
                        this.Controls.Remove(sq.HouseVisual);
                        sq.HouseVisual = null;
                    }
                }

                players.Remove(p);

                if (currentPlayerIndex >= players.Count) currentPlayerIndex = 0;
                else currentPlayerIndex--;

                return true;
            }
            return false;
        }

        // Hàm cập nhật lượt đi (Phiên bản chống lỗi Crash)
        private void UpdateTurnUI()
        {
            if (btnRoll == null || players.Count == 0) return;

            if (currentPlayerIndex >= players.Count) currentPlayerIndex = 0;
            if (currentPlayerIndex < 0) currentPlayerIndex = 0;

            try
            {
                Player p = players[currentPlayerIndex];
                btnRoll.Text = $"Lượt: {p.Name}";
                btnRoll.BackColor = p.Color;

                if (p.Color == Color.Yellow || p.Color == Color.Green)
                    btnRoll.ForeColor = Color.Black;
                else
                    btnRoll.ForeColor = Color.White;
            }
            catch { currentPlayerIndex = 0; }
        }

        private void HandleSquareLogic(Player p)
        {
            if (p.Position >= boardData.Count) return;
            Square sq = boardData[p.Position];
            AddLog($"-> Đến: {sq.Name}");

            if (sq.Type == SquareType.Land || sq.Type == SquareType.Company)
            {
                if (sq.Owner == null)
                {
                    if (p.Money >= sq.Price)
                    {
                        DialogResult result = MessageBox.Show($"Mua '{sq.Name}' ({sq.Price}$)?", "Mua đất", MessageBoxButtons.YesNo);
                        if (result == DialogResult.Yes)
                        {
                            p.Money -= sq.Price;
                            sq.Owner = p;
                            sq.HouseLevel = 0;
                            p.OwnedProperties.Add(sq);
                            DrawHouseOnBoard(sq, p.Color);
                            AddLog($"Đã mua {sq.Name}");
                        }
                    }
                    else AddLog("Không đủ tiền mua.");
                }
                else if (sq.Owner == p)
                {
                    if (sq.Type == SquareType.Land && sq.HouseLevel < 3)
                    {
                        int cost = sq.UpgradeCost();
                        if (p.Money >= cost)
                        {
                            DialogResult r = MessageBox.Show($"Nâng cấp nhà (Cấp {sq.HouseLevel + 1}) - Giá {cost}$?", "Nâng cấp", MessageBoxButtons.YesNo);
                            if (r == DialogResult.Yes)
                            {
                                p.Money -= cost;
                                sq.HouseLevel++;
                                UpdateHouseVisual(sq);
                                AddLog($"Nâng cấp {sq.Name} -> Lv{sq.HouseLevel}");
                            }
                        }
                    }
                }
                else
                {
                    int rent = sq.CalculateRent();
                    p.Money -= rent;
                    sq.Owner.Money += rent;
                    MessageBox.Show($"Đất của {sq.Owner.Name}!\nPhạt: {rent}$");
                    AddLog($"Trả {rent}$ cho {sq.Owner.Name}");
                }
            }
            else if (sq.Type == SquareType.Chance) DrawCard(p, chanceCards, "CƠ HỘI");
            else if (sq.Name.Contains("Khí Vận")) DrawCard(p, communityCards, "KHÍ VẬN");
            else if (sq.Type == SquareType.Tax) { p.Money -= sq.Price; AddLog($"Đóng thuế {sq.Price}$"); }
            else if (sq.Type == SquareType.GoToJail) { p.Position = 10; MovePlayerToken(p); AddLog("Vào Tù!"); }
        }

        private void SetupGame()
        {
            Color[] colors = { Color.Red, Color.Blue, Color.Green, Color.Yellow };

            for (int i = 0; i < totalPlayers; i++)
            {
                Player p = new Player($"Người {i + 1}", colors[i], 1500);

                PictureBox pb = new PictureBox();
                pb.Size = new Size(40, 40);
                pb.SizeMode = PictureBoxSizeMode.Zoom;
                pb.BackColor = Color.Transparent;

                // Nạp ảnh nhân vật: player1, player2...
                Image playerImg = GetImageFromResource("player" + (i + 1));
                if (playerImg != null) pb.Image = playerImg;
                else pb.BackColor = p.Color;

                this.Controls.Add(pb);
                pb.BringToFront();

                p.Token = pb;
                players.Add(p);
                MovePlayerToken(p);
            }

            AddLog("Bắt đầu game! (1500$)");
            UpdateTurnUI();
            UpdateInfoPanel();
        }

        private Image GetImageFromResource(string resourceName)
        {
            try
            {
                object obj = Properties.Resources.ResourceManager.GetObject(resourceName);
                if (obj is Image) return (Image)obj;
                if (obj is byte[])
                {
                    using (MemoryStream ms = new MemoryStream((byte[])obj)) return Image.FromStream(ms);
                }
            }
            catch { }
            return null;
        }

        // --- CÁC HÀM HỖ TRỢ KHÁC (GIỮ NGUYÊN) ---
        private void DrawHouseOnBoard(Square sq, Color playerColor)
        {
            Point pos = CalculatePosition(boardData.IndexOf(sq));
            Label house = new Label();
            house.Size = new Size(15, 15);
            house.BackColor = playerColor;
            house.BorderStyle = BorderStyle.FixedSingle;
            house.Text = "0";
            house.ForeColor = (playerColor == Color.Yellow) ? Color.Black : Color.White;
            house.TextAlign = ContentAlignment.MiddleCenter;
            house.Font = new Font("Arial", 6, FontStyle.Bold);
            house.Location = new Point(pos.X + 10, pos.Y - 10);
            this.Controls.Add(house);
            house.BringToFront();
            sq.HouseVisual = house;
        }

        private void UpdateHouseVisual(Square sq)
        {
            if (sq.HouseVisual != null)
            {
                sq.HouseVisual.Text = sq.HouseLevel.ToString();
                if (sq.HouseLevel == 3) { sq.HouseVisual.Size = new Size(20, 20); sq.HouseVisual.BorderStyle = BorderStyle.Fixed3D; }
            }
        }

        private void CreateInfoPanel()
        {
            lblInfoPanel = new Label();
            lblInfoPanel.Size = new Size(300, 400);
            lblInfoPanel.Location = new Point(this.ClientSize.Width / 2 - 150, 150);
            lblInfoPanel.BackColor = Color.White;
            lblInfoPanel.BorderStyle = BorderStyle.FixedSingle;
            lblInfoPanel.Font = new Font("Arial", 10);
            this.Controls.Add(lblInfoPanel);
            lblInfoPanel.BringToFront();
        }

        private void UpdateInfoPanel()
        {
            string info = "=== TÀI CHÍNH ===\n\n";
            foreach (var p in players)
            {
                info += $"👤 {p.Name}: {p.Money}$\n";
                if (p.OwnedProperties.Count > 0)
                    info += "   Đất: " + string.Join(", ", p.OwnedProperties.Select(s => $"{s.Name}(Lv{s.HouseLevel})"));
                else info += "   (Chưa có đất)";
                info += "\n-----------------\n";
            }
            lblInfoPanel.Text = info;
        }

        private void MovePlayerToken(Player p)
        {
            Point basePos = CalculatePosition(p.Position);
            int offset = players.IndexOf(p) * 5;
            p.Token.Location = new Point(basePos.X + offset, basePos.Y + offset);
        }

        private Point CalculatePosition(int pos)
        {
            int x = 0, y = 0;
            int boardW = this.ClientSize.Width;
            int boardH = this.ClientSize.Height;
            int rightEdge = boardW - (int)(stepX * 1.5);
            int bottomEdge = boardH - (int)(stepY * 1.5);
            int leftEdge = (int)(stepX * 0.5);
            int topEdge = (int)(stepY * 0.5);

            if (pos >= 0 && pos < 10) { x = rightEdge - (pos * stepX); y = bottomEdge; }
            else if (pos >= 10 && pos < 20) { x = leftEdge; y = bottomEdge - ((pos - 10) * stepY); }
            else if (pos >= 20 && pos < 30) { x = leftEdge + ((pos - 20) * stepX); y = topEdge; }
            else if (pos >= 30 && pos < 40) { x = rightEdge; y = topEdge + ((pos - 30) * stepY); }
            return new Point(x, y);
        }

        private void AddLog(string msg)
        {
            if (rtbLog != null) { rtbLog.AppendText(msg + "\n"); rtbLog.ScrollToCaret(); }
        }

        private void DrawCard(Player p, List<Card> deck, string deckName)
        {
            Random rnd = new Random();
            Card card = deck[rnd.Next(deck.Count)];
            MessageBox.Show($"{deckName}: {card.Description}");
            AddLog($"{deckName}: {card.Description}");

            if (card.Action == CardAction.Money) p.Money += card.Value;
            else if (card.Action == CardAction.GoJail) { p.Position = 10; MovePlayerToken(p); }
            else if (card.Action == CardAction.Move)
            {
                if (card.Value == 0) { p.Position = 0; p.Money += 200; }
                else p.Position = (p.Position + card.Value + 40) % 40;
                MovePlayerToken(p);
            }
        }

        private void InitBoardData()
        {
            // --- Nạp dữ liệu (Đã rút gọn, hãy copy lại phần đầy đủ từ bài trước) ---
            boardData.Add(new Square("Bắt đầu (GO)", SquareType.Start, 0));
            boardData.Add(new Square("Phú Lâm", SquareType.Land, 60));
            boardData.Add(new Square("Khí Vận", SquareType.Chance, 0));
            boardData.Add(new Square("Nhà Bè Phú Xuân", SquareType.Land, 60));
            boardData.Add(new Square("Thuế Lợi Tức", SquareType.Tax, 200));
            boardData.Add(new Square("Bến Xe Lục Tỉnh", SquareType.Company, 200));
            boardData.Add(new Square("Thị Nghè", SquareType.Land, 100));
            boardData.Add(new Square("Cơ Hội", SquareType.Chance, 0));
            boardData.Add(new Square("Tân Định", SquareType.Land, 100));
            boardData.Add(new Square("Bến Chương Dương", SquareType.Land, 120));

            boardData.Add(new Square("Thăm Tù", SquareType.Jail, 0));
            boardData.Add(new Square("Phan Đình Phùng", SquareType.Land, 140));
            boardData.Add(new Square("Cty Điện Lực", SquareType.Company, 150));
            boardData.Add(new Square("Trịnh Minh Thế", SquareType.Land, 140));
            boardData.Add(new Square("Lý Thái Tổ", SquareType.Land, 160));
            boardData.Add(new Square("Bến Xe Lam Chợ Lớn", SquareType.Company, 200));
            boardData.Add(new Square("Đại Lộ Hùng Vương", SquareType.Land, 180));
            boardData.Add(new Square("Khí Vận", SquareType.Chance, 0));
            boardData.Add(new Square("Gia Long", SquareType.Land, 180));
            boardData.Add(new Square("Bến Bạch Đằng", SquareType.Land, 200));

            boardData.Add(new Square("Bãi Đậu Xe", SquareType.Parking, 0));
            boardData.Add(new Square("Sân Bay", SquareType.Land, 220));
            boardData.Add(new Square("Đường Công Lý", SquareType.Land, 220));
            boardData.Add(new Square("Cơ Hội", SquareType.Chance, 0));
            boardData.Add(new Square("Đại Lộ Thống Nhất", SquareType.Land, 240));
            boardData.Add(new Square("Đại Lộ Cộng Hòa", SquareType.Land, 240));
            boardData.Add(new Square("Bến Xe An Đông", SquareType.Company, 200));
            boardData.Add(new Square("Đại Lộ Hồng Bàng", SquareType.Land, 260));
            boardData.Add(new Square("Đại Lộ Hai Bà Trưng", SquareType.Land, 260));
            boardData.Add(new Square("Cty Thủy Cục", SquareType.Company, 150));

            boardData.Add(new Square("Vô Tù", SquareType.GoToJail, 0));
            boardData.Add(new Square("Xa Lộ Biên Hòa", SquareType.Land, 280));
            boardData.Add(new Square("Khí Vận", SquareType.Chance, 0));
            boardData.Add(new Square("Lê Văn Duyệt", SquareType.Land, 300));
            boardData.Add(new Square("Phan Thanh Giản", SquareType.Land, 300));
            boardData.Add(new Square("Bến Xe Lam Sài Gòn", SquareType.Company, 200));
            boardData.Add(new Square("Cơ Hội", SquareType.Chance, 0));
            boardData.Add(new Square("Nguyễn Thái Học", SquareType.Land, 320));
            boardData.Add(new Square("Nha Trang", SquareType.Land, 350));
            boardData.Add(new Square("Thuế Lương Bổng", SquareType.Tax, 100));
        }

        private void InitCards()
        {
            chanceCards.Add(new Card("Tiến thẳng tới ô Bắt Đầu", CardAction.Move, 0));
            chanceCards.Add(new Card("Vi phạm luật, phạt 150$", CardAction.Money, -150));
            chanceCards.Add(new Card("VÀO TÙ NGAY!", CardAction.GoJail, 0));
            chanceCards.Add(new Card("Trúng số, nhận 200$", CardAction.Money, 200));

            communityCards.Add(new Card("Tiền bác sĩ, trả 50$", CardAction.Money, -50));
            communityCards.Add(new Card("Nhận tiền thừa kế 100$", CardAction.Money, 100));
            communityCards.Add(new Card("VÀO TÙ NGAY!", CardAction.GoJail, 0));
        }
    }

    // --- 2. CÁC CLASS DỮ LIỆU PHỤ (ĐẶT Ở DƯỚI CÙNG) ---

    public enum SquareType { Start, Land, Chance, Jail, GoToJail, Tax, Parking, Company }
    public enum CardAction { Money, Move, GoJail, GetOutOfJail }

    public class Card
    {
        public string Description { get; set; }
        public CardAction Action { get; set; }
        public int Value { get; set; }
        public Card(string desc, CardAction action, int val) { Description = desc; Action = action; Value = val; }
    }

    public class Square
    {
        public string Name { get; set; }
        public int Price { get; set; }
        public int BaseRent { get; set; }
        public int HouseLevel { get; set; }
        public SquareType Type { get; set; }
        public Player Owner { get; set; }
        public Label HouseVisual { get; set; }

        public Square(string name, SquareType type, int price)
        {
            Name = name; Type = type; Price = price;
            BaseRent = price / 10; HouseLevel = 0; Owner = null; HouseVisual = null;
        }

        public int CalculateRent()
        {
            if (Type != SquareType.Land) return BaseRent;
            return BaseRent * (1 + HouseLevel);
        }
        public int UpgradeCost() { return Price / 2; }
    }

    public class Player
    {
        public string Name { get; set; }
        public int Money { get; set; }
        public int Position { get; set; }
        public PictureBox Token { get; set; }
        public Color Color { get; set; }
        public bool HasJailCard { get; set; }
        public List<Square> OwnedProperties { get; set; }

        public Player(string name, Color color, int startMoney)
        {
            Name = name; Money = startMoney; Position = 0; Color = color;
            HasJailCard = false;
            OwnedProperties = new List<Square>();
        }
    }
}