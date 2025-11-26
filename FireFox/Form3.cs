using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Linq;

namespace FireFox
{
    public partial class Form3 : Form
    {
        // --- KHAI BÁO BIẾN TOÀN CỤC ---
        int totalPlayers;
        List<Player> players = new List<Player>();
        List<Square> boardData = new List<Square>();
        List<Card> chanceCards = new List<Card>();
        List<Card> communityCards = new List<Card>();

        int currentPlayerIndex = 0;
        bool isProcessingTurn = false; // Chặn bấm nút khi đang xử lý
        int stepX, stepY; // Kích thước bước nhảy quân cờ
        Label lblInfoPanel; // Bảng hiển thị tiền

        public bool WantsToExit = false; // Biến cờ báo thoát game

        // --- KHỞI TẠO FORM ---
        public Form3(int soNguoi)
        {
            InitializeComponent();
            this.totalPlayers = soNguoi;
            this.KeyPreview = true; // Để bắt phím ESC

            // Nạp ảnh bàn cờ (Nếu có trong Resources)
            Image boardImg = GetImageFromResource("banco");
            if (boardImg != null)
            {
                this.BackgroundImage = boardImg;
                this.BackgroundImageLayout = ImageLayout.Stretch;
            }

            // Tính toán tọa độ bàn cờ dựa trên kích thước Form
            stepX = this.ClientSize.Width / 13;
            stepY = this.ClientSize.Height / 13;

            // Khởi tạo dữ liệu
            CreateInfoPanel();
            InitBoardData();
            InitCards();
            SetupGame();

            // Hiển thị xúc xắc mặc định
            if (pbDice != null) pbDice.Image = GetImageFromResource("dice1");
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            // Tự động gắn tiếng click cho tất cả nút trên bàn cờ
            SoundManager.AttachClickSound(this);
        }

        // --- SỰ KIỆN NÚT GIEO XÚC XẮC (VÒNG LẶP CHÍNH) ---
        private void btnRoll_Click(object sender, EventArgs e)
        {
            if (isProcessingTurn) return;
            isProcessingTurn = true;

            if (players.Count == 0) return;

            Player current = players[currentPlayerIndex];
            Random rnd = new Random();
            int dice = rnd.Next(1, 7);

            // Hiển thị hình ảnh xúc xắc
            if (pbDice != null) pbDice.Image = GetImageFromResource("dice" + dice);
            AddLog($"{current.Name} gieo được {dice} điểm.");

            // Di chuyển người chơi
            current.Position += dice;

            // Xử lý khi đi qua ô Start
            if (current.Position >= 40)
            {
                current.Position %= 40;
                current.Money += 200;

                // Âm thanh nhận tiền
                SoundManager.PlaySound(Properties.Resources.cash);
                AddLog("Qua vòng Start +200$");
            }

            // Cập nhật vị trí hình ảnh
            MovePlayerToken(current);

            // Xử lý logic tại ô vừa đến (Mua đất, trả tiền...)
            HandleSquareLogic(current);

            // Cập nhật bảng thông tin tiền nong
            UpdateInfoPanel();

            // KIỂM TRA PHÁ SẢN VÀ CHIẾN THẮNG
            if (CheckBankruptcy(current))
            {
                // Nếu có người vừa phá sản, kiểm tra xem còn lại 1 người duy nhất không?
                if (players.Count == 1)
                {
                    ShowWinEffect(players[0].Name);
                    this.Close(); // Đóng Form game sau khi hiệu ứng kết thúc
                    return;
                }
                // Nếu vẫn còn > 1 người, game tiếp tục (index đã được xử lý trong CheckBankruptcy)
            }
            else
            {
                // Nếu không phá sản, chuyển lượt cho người tiếp theo
                currentPlayerIndex++;
            }

            // Quay vòng index nếu vượt quá số người chơi
            if (currentPlayerIndex >= players.Count) currentPlayerIndex = 0;

            UpdateTurnUI();
            isProcessingTurn = false;
        }

        // --- XỬ LÝ LOGIC Ô CỜ ---
        private void HandleSquareLogic(Player p)
        {
            if (p.Position >= boardData.Count) return;
            Square sq = boardData[p.Position];
            AddLog($"-> Đến: {sq.Name}");

            if (sq.Type == SquareType.Land || sq.Type == SquareType.Company)
            {
                // 1. Đất chưa có chủ -> Mua
                if (sq.Owner == null)
                {
                    if (p.Money >= sq.Price)
                    {
                        DialogResult result = MessageBox.Show($"Mua '{sq.Name}' giá {sq.Price}$?", "Mua đất", MessageBoxButtons.YesNo);
                        if (result == DialogResult.Yes)
                        {
                            p.Money -= sq.Price;
                            sq.Owner = p;
                            sq.HouseLevel = 0;
                            p.OwnedProperties.Add(sq);

                            DrawHouseOnBoard(sq, p.Color);
                            SoundManager.PlaySound(Properties.Resources.cash); // Tiếng chi tiền
                            AddLog($"Đã mua {sq.Name}");
                        }
                    }
                    else AddLog("Không đủ tiền mua.");
                }
                // 2. Đất của chính mình -> Nâng cấp nhà
                else if (sq.Owner == p)
                {
                    if (sq.Type == SquareType.Land && sq.HouseLevel < 3)
                    {
                        int cost = sq.UpgradeCost();
                        if (p.Money >= cost)
                        {
                            DialogResult r = MessageBox.Show($"Nâng cấp nhà lên cấp {sq.HouseLevel + 1} (Giá {cost}$)?", "Nâng cấp", MessageBoxButtons.YesNo);
                            if (r == DialogResult.Yes)
                            {
                                p.Money -= cost;
                                sq.HouseLevel++;
                                UpdateHouseVisual(sq);

                                SoundManager.PlaySound(Properties.Resources.cash); // Tiếng chi tiền
                                AddLog($"Nâng cấp {sq.Name} -> Lv{sq.HouseLevel}");
                            }
                        }
                    }
                }
                // 3. Đất người khác -> Trả tiền thuê
                else
                {
                    int rent = sq.CalculateRent();
                    p.Money -= rent;
                    sq.Owner.Money += rent;

                    SoundManager.PlaySound(Properties.Resources.cash); // Tiếng mất tiền
                    MessageBox.Show($"Đây là đất của {sq.Owner.Name}!\nBạn bị phạt: {rent}$");
                    AddLog($"Trả {rent}$ cho {sq.Owner.Name}");
                }
            }
            // Ô Cơ Hội / Khí Vận
            else if (sq.Type == SquareType.Chance) DrawCard(p, chanceCards, "CƠ HỘI");
            else if (sq.Name.Contains("Khí Vận")) DrawCard(p, communityCards, "KHÍ VẬN");

            // Ô Thuế
            else if (sq.Type == SquareType.Tax)
            {
                p.Money -= sq.Price;
                SoundManager.PlaySound(Properties.Resources.cash);
                AddLog($"Đóng thuế {sq.Price}$");
            }

            // Ô Vào Tù
            else if (sq.Type == SquareType.GoToJail)
            {
                p.Position = 10; // Vị trí ô tù
                MovePlayerToken(p);
                AddLog("Vào Tù!");
            }
        }

        // --- XỬ LÝ RÚT THẺ ---
        private void DrawCard(Player p, List<Card> deck, string deckName)
        {
            Random rnd = new Random();
            Card card = deck[rnd.Next(deck.Count)];

            MessageBox.Show($"{deckName}: {card.Description}");
            AddLog($"{deckName}: {card.Description}");

            if (card.Action == CardAction.Money)
            {
                p.Money += card.Value;
                if (card.Value > 0) SoundManager.PlaySound(Properties.Resources.cash); // Nhận tiền
                else SoundManager.PlaySound(Properties.Resources.cash); // Mất tiền
            }
            else if (card.Action == CardAction.GoJail)
            {
                p.Position = 10;
                MovePlayerToken(p);
            }
            else if (card.Action == CardAction.Move)
            {
                if (card.Value == 0) // Về Start
                {
                    p.Position = 0;
                    p.Money += 200;
                    SoundManager.PlaySound(Properties.Resources.cash);
                }
                else
                {
                    p.Position = (p.Position + card.Value + 40) % 40;
                }
                MovePlayerToken(p);
            }
        }

        // --- XỬ LÝ PHÁ SẢN ---
        private bool CheckBankruptcy(Player p)
        {
            if (p.Money < 0)
            {
                // 1. Phát âm thanh thất bại
                SoundManager.PlaySound(Properties.Resources.fail);

                // 2. Hiện Form hiệu ứng bị loại
                EliminatePlayer(p.Name);
                AddLog($"❌ {p.Name} PHÁ SẢN!");

                // 3. Xóa quân cờ
                if (p.Token != null)
                {
                    this.Controls.Remove(p.Token);
                    p.Token.Dispose();
                }

                // 4. Reset đất đai
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

                // 5. Xóa người chơi khỏi danh sách
                players.Remove(p);

                // 6. Điều chỉnh index
                if (currentPlayerIndex >= players.Count) currentPlayerIndex = 0;
                else currentPlayerIndex--;

                return true; // Có người phá sản
            }
            return false;
        }

        // --- HIỆN CÁC FORM HIỆU ỨNG ---
        private void ShowWinEffect(string winnerName)
        {
            SoundManager.StopBGM(); // Tắt nhạc nền game
            FormWinEffect f = new FormWinEffect(winnerName);
            f.ShowDialog();
        }

        private void EliminatePlayer(string name)
        {
            FormPlayerEliminated f = new FormPlayerEliminated(name);
            f.ShowDialog();
        }

        private void ShowPauseMenu()
        {
            FormPauseMenu f = new FormPauseMenu();
            f.ShowDialog();

            if (f.Option == 1) { /* Tiếp tục */ }
            else if (f.Option == 2)
            {
                this.Hide();
                Form1 f1 = new Form1();
                f1.ShowDialog();
                this.Close();
            }
            else if (f.Option == 3)
            {
                WantsToExit = true;
                this.Close();
            }
        }

        // Phím tắt ESC
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                ShowPauseMenu();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        // --- HÀM HỖ TRỢ UI & DỮ LIỆU ---
        private void UpdateTurnUI()
        {
            if (btnRoll == null || players.Count == 0) return;

            // Bảo vệ index
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

            // Tinh chỉnh vị trí nhà cho đẹp
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
                if (sq.HouseLevel == 3)
                {
                    sq.HouseVisual.Size = new Size(20, 20);
                    sq.HouseVisual.BorderStyle = BorderStyle.Fixed3D;
                }
            }
        }

        private void MovePlayerToken(Player p)
        {
            Point basePos = CalculatePosition(p.Position);
            // Xếp các quân cờ lệch nhau một chút để không đè lên nhau
            int offset = players.IndexOf(p) * 5;
            p.Token.Location = new Point(basePos.X + offset, basePos.Y + offset);
        }

        private Point CalculatePosition(int pos)
        {
            int x = 0, y = 0;
            int boardW = this.ClientSize.Width;
            int boardH = this.ClientSize.Height;

            // Canh chỉnh tọa độ dựa trên kích thước bàn cờ
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
            if (rtbLog != null)
            {
                rtbLog.AppendText(msg + "\n");
                rtbLog.ScrollToCaret();
            }
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

        // --- DỮ LIỆU CỐ ĐỊNH (KHỞI TẠO BAN ĐẦU) ---
        private void InitBoardData()
        {
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

    // --- 2. CÁC CLASS DỮ LIỆU PHỤ ---
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