using System;
using System.Configuration;              // the classic .NET Framework way to read App.config
using System.Drawing;
using System.Runtime.InteropServices;    // RuntimeInformation: tells us which .NET we are running on
using System.Windows.Forms;

namespace ClickCounter
{
    // A tiny window: a greeting read from App.config, a line saying which .NET
    // it is running on, and a click counter.
    // The screen is built in code (no Designer file) so you can see everything
    // in one place.
    public class MainForm : Form
    {
        private readonly Label _greeting = new Label { Name = "greetingLabel" };
        private readonly Label _runtime = new Label { Name = "runtimeLabel" };
        private readonly Label _count = new Label { Name = "countLabel" };
        private readonly Button _button = new Button { Name = "clickButton" };
        private int _clicks;

        public MainForm()
        {
            Text = "Click Counter";
            ClientSize = new Size(360, 180);
            StartPosition = FormStartPosition.CenterScreen;

            _greeting.Text = GetGreeting();
            _runtime.Text = "Running on: " + RuntimeInformation.FrameworkDescription;
            _count.Text = "Clicks: 0";
            _button.Text = "Click me";
            _button.AutoSize = true;
            _button.Click += OnButtonClick;

            _greeting.AutoSize = true;
            _runtime.AutoSize = true;
            _count.AutoSize = true;

            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(16),
            };
            panel.Controls.AddRange(new Control[] { _greeting, _runtime, _count, _button });
            Controls.Add(panel);
        }

        // TODO(human): read the "Greeting" setting from App.config and return it.
        // Decide what should happen when the setting is missing (see LESSON.md, Step 2).
        private static string GetGreeting()
        {
            return "(greeting not loaded yet)";
        }

        private void OnButtonClick(object sender, EventArgs e)
        {
            _clicks++;
            _count.Text = "Clicks: " + _clicks;
        }
    }
}
