using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace cw {
    internal static class Program {
        [STAThread]
        private static void Main(string[] args) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Application.Run(new Form1());

            var url = string.Empty;
            var file = string.Empty;

            var addToUrl = false;
            var addToFile = false;

            foreach (var arg in args) {
                switch (arg) {
                    case "-u":
                        addToUrl = true;
                        addToFile = false;
                        break;

                    case "-f":
                        addToFile = true;
                        addToUrl = false;
                        break;

                    default:
                        if (addToUrl) {
                            url += arg + " ";
                        }

                        if (addToFile) {
                            file += arg + " ";
                        }

                        break;
                }
            }

            url = url.Trim();
            file = file.Trim();

            if (string.IsNullOrWhiteSpace(url) &&
                string.IsNullOrWhiteSpace(file)) {
                const string help = "Usage:\r\n" +
                                    "\r\n" +
                                    "cw.exe [-u <url>] [-f <file>]\r\n" +
                                    "\r\n" +
                                    "-u <url> - URL to open.\r\n" +
                                    "-f <file> - JSON config file to use.";

                MessageBox.Show(
                    help,
                    Application.ProductName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            var configs = new List<Config>();

            if (!string.IsNullOrWhiteSpace(file) &&
                File.Exists(file)) {
                try {
                    var json = File.ReadAllText(file);

                    if (json.StartsWith("[") &&
                        json.EndsWith("]")) {
                        configs = JsonConvert.DeserializeObject<List<Config>>(json);
                    }
                    else if (json.StartsWith("{") &&
                             json.EndsWith("}")) {
                        configs = new List<Config> {
                            JsonConvert.DeserializeObject<Config>(json)
                        };
                    }
                    else {
                        throw new Exception("Not valid JSON file");
                    }
                }
                catch {
                    MessageBox.Show(
                        "Unable to load config from JSON!",
                        Application.ProductName,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return;
                }
            }

            if (configs == null) {
                MessageBox.Show(
                    "Something went wrong...",
                    Application.ProductName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            var runningWindows = 0;
            Form lastWindow = null;

            foreach (var config in configs) {
                var wurl = config.url;

                if (string.IsNullOrWhiteSpace(wurl)) {
                    wurl = url;
                }

                if (string.IsNullOrWhiteSpace(wurl)) {
                    MessageBox.Show(
                        "No URL specified",
                        Application.ProductName,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return;
                }

                var fmb = new fmBrowser(wurl, config.saveCache.HasValue && config.saveCache.Value);

                // Set position and state.
                if (config.top.HasValue) {
                    fmb.Top = config.top.Value;
                }
                if (config.left.HasValue) {
                    fmb.Left = config.left.Value;
                }
                if (config.width.HasValue) {
                    fmb.Width = config.width.Value;
                }
                if (config.height.HasValue) {
                    fmb.Height = config.height.Value;
                }
                if (config.windowState.HasValue) {
                    fmb.WindowState = (FormWindowState) config.windowState.Value;
                }

                // Show form.
                fmb.Show();

                runningWindows++;

                lastWindow = fmb;
            }

            if (runningWindows == 0 &&
                !string.IsNullOrWhiteSpace(url)) {
                var fmb = new fmBrowser(url, false);

                // Show form.
                fmb.Show();

                lastWindow = fmb;
            }

            if (lastWindow == null) {
                MessageBox.Show(
                    "No windows have been spun up. Aborting.",
                    Application.ProductName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            Application.Run(lastWindow);
        }
    }
}