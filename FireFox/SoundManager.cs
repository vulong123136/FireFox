using System;
using System.IO;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMPLib; // Cần Add Reference: Windows Media Player (wmp.dll)

namespace FireFox
{
    public static class SoundManager
    {
        private static WindowsMediaPlayer bgmPlayer = new WindowsMediaPlayer();
        private static string bgmTempPath = Path.Combine(Path.GetTempPath(), "bgm_temp.wav");

        // --- 1. PHÁT NHẠC NỀN (LẶP LẠI VĨNH VIỄN) ---
        public static void PlayBGM()
        {
            try
            {
                // Trích xuất file từ Resource ra ổ cứng để WMP đọc được
                if (!File.Exists(bgmTempPath))
                {
                    using (FileStream fs = new FileStream(bgmTempPath, FileMode.Create))
                    {
                        Properties.Resources.bgm.CopyTo(fs);
                    }
                }

                bgmPlayer.URL = bgmTempPath;
                bgmPlayer.settings.setMode("loop", true); // Bật chế độ lặp lại
                bgmPlayer.settings.volume = 50; // Âm lượng 50%
                bgmPlayer.controls.play();
            }
            catch { /* Bỏ qua nếu lỗi */ }
        }

        public static void StopBGM()
        {
            try { bgmPlayer.controls.stop(); } catch { }
        }

        // --- 2. PHÁT HIỆU ỨNG ÂM THANH (SFX) ---
        // Dùng SoundPlayer để phát đè lên nhạc nền mà không làm ngắt nhạc
        public static void PlaySound(UnmanagedMemoryStream soundResource)
        {
            if (soundResource != null)
            {
                Task.Run(() =>
                {
                    using (SoundPlayer sfx = new SoundPlayer(soundResource))
                    {
                        sfx.Play();
                    }
                });
            }
        }

        // --- 3. TỰ ĐỘNG GẮN TIẾNG CLICK CHO MỌI NÚT ---
        public static void AttachClickSound(Control parent)
        {
            // Gán cho chính container đó (nếu nó click được)
            parent.Click += (s, e) => PlaySound(Properties.Resources.click);

            // Duyệt đệ quy tất cả nút con
            foreach (Control c in parent.Controls)
            {
                if (c is Button || c is RadioButton || c is PictureBox || c is Label)
                {
                    c.Click += (s, e) => PlaySound(Properties.Resources.click);
                }
                if (c.HasChildren) AttachClickSound(c);
            }
        }
    }
}