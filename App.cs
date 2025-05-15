using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XDAutoClicker
{
    public partial class App : Form
    {
        private readonly string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private readonly int version = 16;
        private int clickInterval;
        private bool hasRun;
        private Keys hotkey1 = Keys.F6;
        private Keys hotkey2 = Keys.F7;
        private int hotkeyToChange;
        private bool isClickerRunning;
        private bool isLeftClick = true;
        private bool isWaitingForKey;

        public App()
        {
            InitializeComponent();

            appData = Path.Combine(appData, "XD's AutoClicker");

            CreateFiles();
            Update();

            timer1.Tick += timer1_Tick;
            timer1.Interval = 1000;

            RegisterHotKey(Handle, 1, 0x0000, (uint)hotkey1);
            RegisterHotKey(Handle, 2, 0x0000, (uint)hotkey2);

            var hotkey1File = Path.Combine(appData, "hotkey1");
            if (File.Exists(hotkey1File))
            {
                var content1 = File.ReadAllText(hotkey1File);
                hotkey1 = (Keys)Enum.Parse(typeof(Keys), content1, true);
                UnregisterHotKey(Handle, 1);
                RegisterHotKey(Handle, 1, 0x0000, (uint)hotkey1);
                button1.Text = $"Start ({hotkey1})";
            }

            var hotkey2File = Path.Combine(appData, "hotkey2");
            if (File.Exists(hotkey2File))
            {
                var content2 = File.ReadAllText(hotkey2File);
                hotkey2 = (Keys)Enum.Parse(typeof(Keys), content2, true);
                UnregisterHotKey(Handle, 2);
                RegisterHotKey(Handle, 2, 0x0000, (uint)hotkey2);
                button2.Text = $"Left Click ({hotkey2})";
            }

            var time1File = Path.Combine(appData, "time1");
            if (File.Exists(time1File))
            {
                var content = File.ReadAllText(time1File);
                textBox1.Text = $"{content}";
            }

            var time2File = Path.Combine(appData, "time2");
            if (File.Exists(time2File))
            {
                var content = File.ReadAllText(time2File);
                textBox2.Text = $"{content}";
            }

            var time3File = Path.Combine(appData, "time3");
            if (File.Exists(time1File))
            {
                var content = File.ReadAllText(time3File);
                textBox3.Text = $"{content}";
            }

            var time4File = Path.Combine(appData, "time4");
            if (File.Exists(time4File))
            {
                var content = File.ReadAllText(time4File);
                textBox4.Text = $"{content}";
            }

            progressBar2.Hide();
            label1.Hide();
            label2.Hide();
            label3.Hide();
            label4.Hide();
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);


        private bool HasTextButNoNumbers()
        {
            if (!string.IsNullOrWhiteSpace(textBox1.Text) && Regex.IsMatch(textBox1.Text, @"[^\d]"))
                if (!string.IsNullOrWhiteSpace(textBox2.Text) && Regex.IsMatch(textBox2.Text, @"[^\d]"))
                    if (!string.IsNullOrWhiteSpace(textBox3.Text) && Regex.IsMatch(textBox3.Text, @"[^\d]"))
                        if (!string.IsNullOrWhiteSpace(textBox4.Text) && Regex.IsMatch(textBox4.Text, @"[^\d]"))
                            return true;

            return false;
        }

        private new async void Update()
        {
            try
            {
                using (var client2 = new HttpClient())
                {
                    var resultInt =
                        await client2.GetStringAsync(
                            "https://raw.githubusercontent.com/XDPXI/XDAutoClicker/refs/heads/main/latest");
                    var latestVersionInt = int.Parse(resultInt.Trim());
                    var resultString = await client2.GetStringAsync(
                        "https://raw.githubusercontent.com/XDPXI/XDAutoClicker/refs/heads/main/latestString");
                    var latestVersionString = resultString.Trim();
                    {
                        if (latestVersionInt > version)
                        {
                            progressBar2.Show();
                            label1.Show();
                            label2.Show();
                            textBox1.Hide();
                            textBox2.Hide();
                            textBox3.Hide();
                            textBox4.Hide();
                            button1.Hide();
                            button2.Hide();
                            button3.Hide();
                            button4.Hide();
                            UnregisterHotKey(Handle, 1);
                            UnregisterHotKey(Handle, 2);

                            Application.EnableVisualStyles();
                            using (var folderDialog = new FolderBrowserDialog())
                            {
                                folderDialog.Description = "Select a folder to install to.";
                                if (folderDialog.ShowDialog() == DialogResult.OK)
                                {
                                    var selectedFolder = folderDialog.SelectedPath;
                                    var destinationPath =
                                        $@"{selectedFolder}\XD's AutoClicker V{latestVersionString}.exe";
                                    try
                                    {
                                        using (var client = new HttpClient())
                                        {
                                            var fileBytes = await client.GetByteArrayAsync(
                                                "https://github.com/XDPXI/XDAutoClicker/releases/latest/download/XD.s.AutoClicker.exe");
                                            File.WriteAllBytes(destinationPath, fileBytes);
                                        }
                                    }
                                    catch (Exception)
                                    {
                                    }

                                    Process.Start("explorer.exe", $"/select,\"{destinationPath}\"");
                                    Close();
                                }
                                else
                                {
                                    Hide();
                                    MessageBox.Show("Error Updating!", "Error!", MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                                    Close();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error Updating!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private new void Hide()
        {
            ShowInTaskbar = false;
            WindowState = FormWindowState.Minimized;
        }

        private void CreateFiles()
        {
            hasRun = false;
            if (!hasRun)
            {
                hasRun = true;
                Directory.CreateDirectory(appData);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        protected override void WndProc(ref Message m)
        {
            const int wmHotkey = 0x0312;

            if (m.Msg == wmHotkey)
            {
                if (m.WParam.ToInt32() == 1)
                    ToggleAutoClicker();
                else if (m.WParam.ToInt32() == 2) ToggleClickType();
            }

            base.WndProc(ref m);
        }

        private async void ChangeHotkey(int hotkeyId)
        {
            hotkeyToChange = hotkeyId;
            isWaitingForKey = true;

            progressBar2.Show();
            label3.Show();
            label4.Show();
            textBox1.Hide();
            textBox2.Hide();
            textBox3.Hide();
            textBox4.Hide();
            button1.Hide();
            button2.Hide();
            button3.Hide();
            button4.Hide();

            await Task.Run(() =>
            {
                while (isWaitingForKey)
                {
                }
            });

            if (hotkeyToChange == 1)
            {
                UnregisterHotKey(Handle, 1);
                RegisterHotKey(Handle, 1, 0x0000, (uint)hotkey1);
                button1.Text = $"Start ({hotkey1})";
                progressBar2.Hide();
                label3.Hide();
                label4.Hide();
                textBox1.Show();
                textBox2.Show();
                textBox3.Show();
                textBox4.Show();
                button1.Show();
                button2.Show();
                button3.Show();
                button4.Show();

                var hotkey1File = Path.Combine(appData, "hotkey1");
                if (File.Exists(hotkey1File)) File.Delete(hotkey1File);
                File.WriteAllText(hotkey1File, hotkey1.ToString());
            }
            else if (hotkeyToChange == 2)
            {
                UnregisterHotKey(Handle, 2);
                RegisterHotKey(Handle, 2, 0x0000, (uint)hotkey2);
                button2.Text = $"Left Click ({hotkey2})";
                progressBar2.Hide();
                label3.Hide();
                label4.Hide();
                textBox1.Show();
                textBox2.Show();
                textBox3.Show();
                textBox4.Show();
                button1.Show();
                button2.Show();
                button3.Show();
                button4.Show();

                var hotkey2File = Path.Combine(appData, "hotkey2");
                if (File.Exists(hotkey2File)) File.Delete(hotkey2File);
                File.WriteAllText(hotkey2File, hotkey2.ToString());
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (isWaitingForKey)
            {
                if (hotkeyToChange == 1)
                    hotkey1 = keyData;
                else if (hotkeyToChange == 2) hotkey2 = keyData;
                isWaitingForKey = false;
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ToggleClickType()
        {
            isLeftClick = !isLeftClick;
            if (isLeftClick)
                button2.Text = $"Left Click ({hotkey2})";
            else
                button2.Text = $"Right Click ({hotkey2})";
        }

        public void ToggleAutoClicker()
        {
            if (isClickerRunning)
            {
                StopClicker();
                button1.Text = $"Start ({hotkey1})";
            }
            else
            {
                if (!HasTextButNoNumbers())
                {
                    StartClicker();
                    button1.Text = $"Stop ({hotkey1})";
                }
                else
                {
                    MessageBox.Show("Set the input boxes to numbers!", "Error!", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void StartClicker()
        {
            if (!HasTextButNoNumbers())
            {
                try
                {
                    var hr = Convert.ToInt32(textBox1.Text);
                    var min = Convert.ToInt32(textBox2.Text);
                    var sec = Convert.ToInt32(textBox3.Text);
                    var mili = Convert.ToInt32(textBox4.Text);

                    var f1 = Path.Combine(appData, "time1");
                    if (File.Exists(f1)) File.Delete(f1);
                    File.WriteAllText(f1, textBox1.Text);
                    var f2 = Path.Combine(appData, "time2");
                    if (File.Exists(f2)) File.Delete(f2);
                    File.WriteAllText(f2, textBox2.Text);
                    var f3 = Path.Combine(appData, "time3");
                    if (File.Exists(f3)) File.Delete(f3);
                    File.WriteAllText(f3, textBox3.Text);
                    var f4 = Path.Combine(appData, "time4");
                    if (File.Exists(f4)) File.Delete(f4);
                    File.WriteAllText(f4, textBox4.Text);

                    sec = sec * 1000;
                    min = min * 60000;
                    hr = hr * 3600000;

                    clickInterval = mili + sec + min + hr;
                    timer1.Interval = clickInterval;
                    timer1.Start();
                    isClickerRunning = true;
                }
                catch
                {
                    StopClicker();
                    button1.Text = $"Start ({hotkey1})";
                    MessageBox.Show("Set the input boxes to numbers!", "Error!", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            else
            {
                StopClicker();
                button1.Text = $"Start ({hotkey1})";
                MessageBox.Show("Set the input boxes to numbers!", "Error!", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void StopClicker()
        {
            timer1.Stop();
            isClickerRunning = false;
        }

        private void PerformClick()
        {
            if (isLeftClick)
                mouse_event(0x02 | 0x04, 0, 0, 0, 0);
            else
                mouse_event(0x08 | 0x10, 0, 0, 0, 0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ToggleAutoClicker();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ToggleClickType();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ChangeHotkey(1);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ChangeHotkey(2);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            PerformClick();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            UnregisterHotKey(Handle, 1);
            UnregisterHotKey(Handle, 2);
            base.OnFormClosing(e);
        }
    }
}