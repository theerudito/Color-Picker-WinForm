using Gma.System.MouseKeyHook;
using Timer = System.Windows.Forms.Timer;

namespace Color_Picker
{
    public partial class Formulario : Form
    {
        private Timer updateTimer;

        private Color currentColor;

        private IKeyboardMouseEvents globalMouseEvents;

        private int separation = 20;

        public Formulario()
        {
            InitializeComponent();

            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;

            updateTimer = new Timer
            {
                Interval = 50
            };
            updateTimer.Tick += UpdateColor;
            updateTimer.Start();

            globalMouseEvents = Hook.GlobalEvents();
            globalMouseEvents.MouseMove += HandleGlobalMouseMove;
            globalMouseEvents.MouseClick += HandleGlobalMouseClick;
        }

        private void UpdateColor(object sender, EventArgs e)
        {
            var cursorPosition = Cursor.Position;
            currentColor = GetColorAtCursor(cursorPosition);

            this.BackColor = currentColor;

            if (currentColor.GetBrightness() > 0.5)
            {
                label_Color.ForeColor = Color.Black;
            }
            else
            {
                label_Color.ForeColor = Color.White;
            }

            label_Color.Text = $"#{currentColor.R:X2}{currentColor.G:X2}{currentColor.B:X2}";
        }

        private Color GetColorAtCursor(Point cursorPosition)
        {
            using (var bmp = new Bitmap(1, 1))
            {
                using (var g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(cursorPosition, Point.Empty, new Size(1, 1));
                }
                return bmp.GetPixel(0, 0);
            }
        }

        private void HandleGlobalMouseMove(object sender, MouseEventArgs e)
        {
            var cursorPosition = Cursor.Position;
            this.Location = new Point(cursorPosition.X + separation, cursorPosition.Y);
        }

        private void HandleGlobalMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                try
                {
                    Clipboard.SetText(label_Color.Text);

                    this.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error while copying to clipboard: " + ex.Message);
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (globalMouseEvents != null)
            {
                globalMouseEvents.MouseMove -= HandleGlobalMouseMove;
                globalMouseEvents.MouseClick -= HandleGlobalMouseClick;
                globalMouseEvents.Dispose();
            }
            base.OnFormClosing(e);
        }
    }
}